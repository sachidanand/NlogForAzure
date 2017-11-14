using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.ComponentModel.DataAnnotations;
using NLog.Layouts;
using NLog.Targets;
using System.Configuration;

namespace NLog.Target.Custom
{
    [Target("AzureAppendBlob")]
    public sealed class NLogAppendBlobStorage : TargetWithLayout
    {
        [Required]
        public string ConnectionString { get; set; }

        [Required]
        public string Container { get; set; }

        [Required]
        public string BlobName { get; set; }

        protected override void InitializeTarget()
        {
            base.InitializeTarget();
        }
        protected override void Write(LogEventInfo logEvent)
        {
            var layoutMessage = Layout.Render(logEvent);
            Common common = new Common(logEvent, layoutMessage);
            string con = ConfigurationManager.AppSettings[ConnectionString].ToString();
            CloudBlobClient _client = CloudStorageAccount.Parse(con).CreateCloudBlobClient();
            string containerName = Container;
            string blobName = BlobName;
            CloudBlobContainer _container = _client.GetContainerReference(containerName);
            _container.CreateIfNotExists();
            CloudAppendBlob _blob = _container.GetAppendBlobReference(blobName);
           // _blob.CreateOrReplace();
            _blob.AppendText(common.MachineName + "|" + common.LogTimeStamp + "|" + common.Level + "|" + common.LoggerName + "|" + common.Message + "|" + common.MessageWithLayout + "|" + common.CustomField1 + "|" + common.CustomField2 + "|" + common.CustomField3+ "|" + common.CustomField4+ "|" + common.CustomField5);

        }
    }
}



