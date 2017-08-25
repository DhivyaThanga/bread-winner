using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using PoorManWork;

namespace Api.BlobExample
{
    public class ReadFromBlobWorkItem : PoorManSyncedWorkItem
    {
        public ReadFromBlobWorkItem(string blobUri, PoorManWorkBatchSynchronizer workBatchSynchronizer, CancellationToken cancellationToken) : base(blobUri, workBatchSynchronizer, cancellationToken)
        {
        }

        protected override void DoAlways(CancellationToken cancellationToken)
        {
            var storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.AppSettings["Azure.Storage.ConnectionString"]);

            var blobClient = storageAccount.CreateCloudBlobClient();
            var blockBlob = blobClient.GetBlobReferenceFromServer(new Uri(Id));

            using (var fileStream = System.IO.File.OpenWrite(@"path\myfile"))
            {
                blockBlob.DownloadToStream(fileStream);
            }
        }

        protected override void DoAlwaysErrorCallback(Exception exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override void DoFinally(CancellationToken cancellationToken)
        {
            Debug.WriteLine("Job Done!");
        }
    }
}