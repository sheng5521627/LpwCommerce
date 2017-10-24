using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Messages;

namespace Services.Messages
{
    /// <summary>
    /// Email sender
    /// </summary>
    public partial class EmailSender : IEmailSender
    {
        private readonly IDownloadService _downloadService;

        public EmailSender(IDownloadService downloadService)
        {
            this._downloadService = downloadService;
        }

        public void SendEmail(EmailAccount emailAccount, string subject, string body, 
            string fromAddress, string fromName, string toAddress, string toName, string replyToAddress = null, string replyToName = null, 
            IEnumerable<string> bcc = null, IEnumerable<string> cc = null, string attachmentFilePath = null, 
            string attachmentFileName = null, int attachedDownloadId = 0)
        {
            throw new NotImplementedException();
        }
    }
}
