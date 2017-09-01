using System;
using System.Configuration;
using System.IO;
using System.Threading;
using BreadWinner;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SamplesShared.BlobExample
{
    public class ReadFromBlobWorkItem : BatchWorkItem
    {
        private readonly string _destPath;

        public ReadFromBlobWorkItem(string blobUri, string destPath, WorkBatch workBatch, CancellationToken cancellationToken) : base(blobUri, workBatch, cancellationToken)
        {
            _destPath = destPath;
        }

        protected override void DoAlways(CancellationToken cancellationToken)
        {
            var storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.AppSettings["Azure.Storage.ConnectionString"]);
            var blockBlob = GetBlobReference(storageAccount, Id);
            DownloadBlobFile(_destPath, blockBlob);
        }


        protected override void DoAlwaysErrorCallback(Exception exception, CancellationToken cancellationToken)
        {
            throw exception;
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
            if (fileName != null)
                blockBlob.DownloadToFile(Path.Combine(path, fileName), FileMode.OpenOrCreate);

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