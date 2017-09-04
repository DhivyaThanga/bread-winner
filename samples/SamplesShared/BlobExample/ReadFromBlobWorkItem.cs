using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using BreadWinner;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SamplesShared.BlobExample
{
    public class ReadFromBlobWorkItem : AbstractBatchedWorkItem
    {
        public ReadFromBlobWorkItem(string blobUri, WorkBatch batch, CancellationToken cancellationToken) : base(blobUri, batch, cancellationToken)
        {
        }

        protected override void DoAlways(CancellationToken cancellationToken)
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(
                    ConfigurationManager.AppSettings["Azure.Storage.ConnectionString"]);
                var blockBlob = GetBlobReference(storageAccount, Id);

                Result = DownloadBlobFile(blockBlob);
                WorkItemStatus = WorkItemStatus.Successful;
            }
            catch (Exception e)
            {
                CloudConsole.WriteLine($"Work Item {Id} of {Batch.Id} failed, exception {e.Message}");
                WorkItemStatus = WorkItemStatus.Failed;
            }

        }

        protected override void DoFinally(CancellationToken cancellationToken)
        {
            var results = new List<string>();
            foreach (var done in Batch.CompletedWorkItems)
            {
                results.Add((string) done.Result);
            }

            CloudConsole.WriteLine($"Batch {Batch.Id} done");
        }

        private static ICloudBlob GetBlobReference(CloudStorageAccount storageAccount, string uri)
        {
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blockBlob = blobClient.GetBlobReferenceFromServer(new Uri(uri));

            return blockBlob;
        }

        private static string DownloadBlobFile(ICloudBlob blockBlob)
        {
            var fileName = Path.GetFileName(blockBlob.Name);
            var target = new MemoryStream();
            if (fileName != null)
                blockBlob.DownloadToStream(target);

            target.Position = 0;
            var streamReader = new StreamReader(target);

            return streamReader.ReadToEnd();
        }

        private static string GetAndCreateFullPath(string basePath, string relPath)
        {
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            var path = string.IsNullOrEmpty(relPath) ? basePath : Path.Combine(basePath, relPath);

            if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
    }
}