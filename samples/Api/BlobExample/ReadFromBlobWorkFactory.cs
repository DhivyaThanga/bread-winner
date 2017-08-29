using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
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
        private readonly Func<bool> _checkBoundedBufferStatusFunc;
        private readonly WorkAvailableRepo _workAvailableRepo;

        public ReadFromBlobWorkFactory(Func<bool> checkBoundedBufferStatusFunc, WorkAvailableRepo workAvailableRepo)
        {
            _checkBoundedBufferStatusFunc = checkBoundedBufferStatusFunc;
            _workAvailableRepo = workAvailableRepo;
        }

        public IPoorManWorkItem[] Create(CancellationToken cancellationToken)
        {
            if(_checkBoundedBufferStatusFunc()) Debug.WriteLine("Bounded buffer healthy");
            if (!_workAvailableRepo.IsWorkAvailable())
            {
                return null;
            }

            var fileLocations = GetAllFilesInBlob();
            var batch = new PoorManWorkBatch(fileLocations.Length);

            if(Directory.Exists("tmp"))
                Directory.Delete("tmp", true);

            CloudConsole.WriteLine($"Created batch {batch.Id} with {fileLocations.Length} blobs");

            return fileLocations
                .Select(x => new ReadFromBlobWorkItem(x.AbsoluteUri, batch, cancellationToken))
                .Cast<IPoorManWorkItem>()
                .ToArray();
        }

        private static Uri[] GetAllFilesInBlob()
        {
            var storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.AppSettings["Azure.Storage.ConnectionString"]);
            var containerName = ConfigurationManager.AppSettings["Azure.Storage.Container"];
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            var path = ConfigurationManager.AppSettings["Azure.Storage.Path"];
            var blobs = container.ListBlobs(path, true).ToArray();
            var blobsLocation = blobs.OfType<CloudBlockBlob>().Select(x => x.Uri).ToArray();

            return blobsLocation;
        }
    }
}