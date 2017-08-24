using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using PoorManWork;

namespace Api.BlobExample
{
    public class ReadFromBlobWorkItem : PoorManSyncedWorkItem
    {
        private readonly Uri _blobUri;

        public ReadFromBlobWorkItem(Uri blobUri, PoorManWorkBatchSynchronizer workBatchSynchronizer, CancellationToken cancellationToken) : base(workBatchSynchronizer, cancellationToken)
        {
            _blobUri = blobUri;
        }

        protected override void DoAlways(CancellationToken cancellationToken)
        {
            var storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            var blobClient = storageAccount.CreateCloudBlobClient();
            var blockBlob = blobClient.GetBlobReferenceFromServer(_blobUri);

            using (var fileStream = System.IO.File.OpenWrite(@"path\myfile"))
            {
                blockBlob.DownloadToStream(fileStream);
            }
        }

        protected override void DoFinally(CancellationToken cancellationToken)
        {
            Debug.WriteLine("Job Done!");
        }
    }
}