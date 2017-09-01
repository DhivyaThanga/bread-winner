using System;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace WebApi.BlobExample
{
    public static class CloudConsole
    {
        public static void WriteLine(string line)
        {
            var storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.AppSettings["Azure.Storage.ConnectionString"]);

            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("CloudConsole");
            table.CreateIfNotExists();

            var operation = TableOperation.Insert(
                new TableEntity((long.MaxValue - DateTime.Now.Ticks).ToString(), line));

            table.Execute(operation);
        }
    }
}