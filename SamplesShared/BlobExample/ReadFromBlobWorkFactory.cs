﻿using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using BreadWinner;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SamplesShared.BlobExample
{
    public class ReadFromBlobWorkFactory
    {
        private readonly Func<bool> _checkBoundedBufferStatusFunc;
        private readonly WorkAvailableRepo _workAvailableRepo;

        public ReadFromBlobWorkFactory(Func<bool> checkBoundedBufferStatusFunc, WorkAvailableRepo workAvailableRepo)
        {
            _checkBoundedBufferStatusFunc = checkBoundedBufferStatusFunc;
            _workAvailableRepo = workAvailableRepo;
        }

        public IWorkItem[] Startup(CancellationToken cancellationToken)
        {
            Debug.WriteLine("Bounded buffer starting");
            var path = ConfigurationManager.AppSettings["Azure.Storage.Path"];
            return GetWorkItems(path, "tmp/startup", cancellationToken);
        }

        public IWorkItem[] Create(CancellationToken cancellationToken)
        {
            if (_checkBoundedBufferStatusFunc()) Debug.WriteLine("Bounded buffer healthy");
            var path = ConfigurationManager.AppSettings["Azure.Storage.Path"];
            if (!_workAvailableRepo.IsWorkAvailable())
            {
                return null;
            }

            return GetWorkItems(path, "tmp/any", cancellationToken);
        }

        private IWorkItem[] GetWorkItems(string sourcePath, string destPath,  CancellationToken cancellationToken)
        {
            var fileLocations = GetAllFilesWithPatternInBlob(sourcePath, ".*part.*");
            var batch = new WorkBatch(fileLocations.Length);

            if (Directory.Exists(destPath))
                Directory.Delete(destPath, true);

            CloudConsole.WriteLine($"Created batch {batch.Id} with {fileLocations.Length} blobs");

            return fileLocations
                .Select(x => new ReadFromBlobWorkItem(x.AbsoluteUri, destPath, batch, cancellationToken))
                .Cast<IWorkItem>()
                .ToArray();
        }

        private static Uri[] GetAllFilesWithPatternInBlob(string path, string pattern)
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(
                    ConfigurationManager.AppSettings["Azure.Storage.ConnectionString"]);
                var containerName = ConfigurationManager.AppSettings["Azure.Storage.Container"];
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(containerName);
                var blobs = container.ListBlobs(path, true, options: new BlobRequestOptions
                {
                    MaximumExecutionTime = new TimeSpan(0, 0, 0, 20)
                }).ToArray();
                var regx = new Regex($"{path}{pattern}");

                var blobsLocation = blobs.OfType<CloudBlockBlob>()
                    .Where(x => regx.IsMatch(x.Name))
                    .Select(x => x.Uri)
                    .ToArray();

                return blobsLocation;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}