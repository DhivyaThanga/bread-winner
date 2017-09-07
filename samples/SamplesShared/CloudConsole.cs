using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace SamplesShared
{
    public static class CloudConsole
    {
        public static void WriteLine(string line)
        {
#if CONSOLE
            Console.WriteLine(
                    $"[{DateTime.Now.ToString("hh:mm:ss.fff", CultureInfo.InvariantCulture)}] - {line}");
                return;
#endif

#if DEBUG
            Debug.WriteLine($"{line}");
            return;
#endif

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