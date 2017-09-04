using System;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using BreadWinner;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SamplesShared.BlobExample
{
    public class ReadFromBlobSampleWorkFactory : ISampleWorkFactory
    {
        private readonly Func<bool> _checkBoundedBufferStatusFunc;
        private readonly WorkAvailableRepo _workAvailableRepo;
        private string[] _storedData;
        private readonly object _lock = new object();

        public ReadFromBlobSampleWorkFactory(Func<bool> checkBoundedBufferStatusFunc, WorkAvailableRepo workAvailableRepo)
        {
            _checkBoundedBufferStatusFunc = checkBoundedBufferStatusFunc;
            _workAvailableRepo = workAvailableRepo;
        }

        public IWorkItem[] Startup(
            CancellationToken cancellationToken, ManualResetEvent started = null)
        {
            var path = ConfigurationManager.AppSettings["Azure.Storage.Path"];
            return GetWorkItems(
                path,
                (uri, batch) =>
                    new StartupReadFromBlobWorkItem(
                        started, StoreResults, uri.AbsoluteUri, batch, cancellationToken)
                        as IWorkItem,
                cancellationToken);
        }

        public IWorkItem[] Create(CancellationToken cancellationToken)
        {
            GC.Collect();

            if (_checkBoundedBufferStatusFunc()) CloudConsole.WriteLine("Bounded buffer healthy");
            var path = ConfigurationManager.AppSettings["Azure.Storage.Path"];
            if (!_workAvailableRepo.IsWorkAvailable())
            {
                return null;
            }

            return GetWorkItems(
                path,
                (uri, batch) => 
                    new ReadFromBlobWorkItem(StoreResults, uri.AbsoluteUri, batch, cancellationToken) 
                    as IWorkItem, 
                cancellationToken);
        }

        private void StoreResults(string[] data)
        {
            lock (_lock)
            {
                _storedData = data;
            }
        }

        private static IWorkItem[] GetWorkItems(string sourcePath, Func<Uri, WorkBatch, IWorkItem> workItemFactoryMethod, CancellationToken cancellationToken)
        {
            var fileLocations = GetAllFilesWithPatternInBlob(sourcePath, ".*part.*");
            var batch = new WorkBatch(fileLocations.Length);

            CloudConsole.WriteLine($"Created batch {batch.Id} with {fileLocations.Length} blobs");

            return fileLocations
                .Select(uri => workItemFactoryMethod(uri, batch))
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