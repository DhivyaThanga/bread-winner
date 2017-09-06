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
        private readonly Action<byte[]> _storeResults;

        public ReadFromBlobWorkItem(Action<byte[]> storeResults, string blobUri, IWorkBatch batch, CancellationToken cancellationToken) : base(blobUri, batch, cancellationToken)
        {
            _storeResults = storeResults;
        }

        protected override void DoAlways(CancellationToken cancellationToken)
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(
                    ConfigurationManager.AppSettings["Azure.Storage.ConnectionString"]);
                var blockBlob = GetBlobReference(storageAccount, Id);

                Result = DownloadBlobToMemory(blockBlob);
                Status = WorkStatus.Successful;
            }
            catch (Exception e)
            {
                CloudConsole.WriteLine($"Work Item {Id} of {Batch.Id} failed, exception {e.Message}");
                Status = WorkStatus.Failed;
            }

        }

        protected override void DoFinally(CancellationToken cancellationToken)
        {
            try
            {
                var results = new List<byte>();
                foreach (var result in Batch.Results)
                {
                    results.AddRange((byte[]) result);
                }

                _storeResults(results.ToArray());

                CloudConsole.WriteLine($"Batch {Batch.Id} done");
            }
            catch (Exception e)
            {
                
            }

        }

        private static ICloudBlob GetBlobReference(CloudStorageAccount storageAccount, string uri)
        {
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blockBlob = blobClient.GetBlobReferenceFromServer(new Uri(uri));

            return blockBlob;
        }

        private static byte[] DownloadBlobToMemory(ICloudBlob blockBlob)
        {
            var fileName = Path.GetFileName(blockBlob.Name);
            var target = new MemoryStream();
            if (fileName != null)
                blockBlob.DownloadToStream(target);

            target.Position = 0;
            return target.ToArray();
        }
    }
}