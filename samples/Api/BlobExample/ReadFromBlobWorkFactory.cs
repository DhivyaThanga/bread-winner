using System.Configuration;
using System.Linq;
using System.Threading;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using PoorManWork;

namespace Api.BlobExample
{
    public class ReadFromBlobWorkFactory : IPoorManWorkFactory
    {
        public IPoorManWorkItem[] Create(CancellationToken cancellationToken)
        {
            var storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.AppSettings["Azure.Storage.ConnectionString"]);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("mycontainer");
            var blobs = container.ListBlobs("prefix", true);
            var listBlobItems = blobs as IListBlobItem[] ?? blobs.ToArray();
            var synchronizer = new PoorManWorkBatchSynchronizer(listBlobItems.Length);

            return listBlobItems
                .Select(x => new ReadFromBlobWorkItem(x.Uri.AbsoluteUri, synchronizer, cancellationToken))
                .Cast<IPoorManWorkItem>()
                .ToArray();
        }
    }
}