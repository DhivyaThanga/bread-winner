using System;
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
        private readonly string _destPath;

        public ReadFromBlobWorkItem(string blobUri, string destPath, WorkBatch workBatch, CancellationToken cancellationToken) : base(blobUri, workBatch, cancellationToken)
        {
            _destPath = destPath;
        }

        protected override void DoAlways(CancellationToken cancellationToken)
        {
            try
            {
                CloudConsole.WriteLine($"Work Item {Id} of {BatchId} starting");
                var storageAccount = CloudStorageAccount.Parse(
                    ConfigurationManager.AppSettings["Azure.Storage.ConnectionString"]);
                var blockBlob = GetBlobReference(storageAccount, Id);
                DownloadBlobFile(_destPath, blockBlob);
                CloudConsole.WriteLine($"Work Item {Id} of {BatchId} done");
            }
            catch (Exception)
            {
                CloudConsole.WriteLine($"Work Item {Id} of {BatchId} failed");
                WorkItemStatus = WorkItemStatus.Failed;
            }

        }

        protected override void DoFinally(CancellationToken cancellationToken)
        {
            CloudConsole.WriteLine($"Batch {BatchId} done");
        }

        private static ICloudBlob GetBlobReference(CloudStorageAccount storageAccount, string uri)
        {
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blockBlob = blobClient.GetBlobReferenceFromServer(new Uri(uri));

            return blockBlob;
        }

        private static string DownloadBlobFile(string destPath, ICloudBlob blockBlob)
        {
            var path = GetAndCreateFullPath(destPath, Path.GetDirectoryName(blockBlob.Name));

            var fileName = Path.GetFileName(blockBlob.Name);
            var target = new MemoryStream();
            if (fileName != null)
                blockBlob.DownloadToStream(target);

            return fileName;
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