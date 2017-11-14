using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using NLog.Layouts;
using NLog.Targets;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace NLog.Target.Custom
{
    [Target("AzureTable")]
    public sealed class NLogTableStorage : TargetWithLayout
    {
        [Required]
        public string ConnectionString { get; set; }

        [Required]
        public string TableName { get; set; }


        protected override void InitializeTarget()
        {
            base.InitializeTarget();
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var layoutMessage = Layout.Render(logEvent);
            TableStorage tableStorage = new TableStorage(logEvent, layoutMessage);
            string con = ConfigurationManager.AppSettings[ConnectionString].ToString();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(con);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(TableName);
            table.CreateIfNotExists();
            TableOperation insertOperation = TableOperation.Insert(tableStorage);
            table.Execute(insertOperation);
        }
    }
}