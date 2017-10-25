using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Messages;
using Services.Media;
using System.Net.Mail;
using System.IO;
using System.Net;

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
            var message = new MailMessage();
            message.From = new MailAddress(fromAddress, fromName);
            message.To.Add(new MailAddress(toAddress, toName));
            if (!string.IsNullOrEmpty(replyToAddress))
            {
                message.ReplyToList.Add(new MailAddress(replyToAddress, replyToName));
            }
            if (bcc != null)
            {
                foreach(var address in bcc.Where(m => !string.IsNullOrWhiteSpace(m)))
                {
                    message.Bcc.Add(address.Trim());
                }
            }
            if (cc != null)
            {
                foreach(var address in cc.Where(m => !string.IsNullOrEmpty(m)))
                {
                    message.CC.Add(address.Trim());
                }
            }

            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            if(!string.IsNullOrEmpty(attachmentFilePath) && File.Exists(attachmentFilePath))
            {
                var attachment = new Attachment(attachmentFilePath);
                attachment.ContentDisposition.CreationDate = File.GetCreationTime(attachmentFilePath);
                attachment.ContentDisposition.ModificationDate = File.GetLastWriteTime(attachmentFilePath);
                attachment.ContentDisposition.ReadDate = File.GetLastAccessTime(attachmentFilePath);
                if (!string.IsNullOrEmpty(attachmentFileName))
                {
                    attachment.Name = attachmentFileName;
                }
                message.Attachments.Add(attachment);
            }

            if (attachedDownloadId > 0)
            {
                var download = _downloadService.GetDownloadById(attachedDownloadId);
                if(download != null)
                {
                    if (!download.UseDownloadUrl)
                    {
                        string fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : download.Id.ToString();
                        fileName += download.Extension;

                        var ms = new MemoryStream(download.DownloadBinary);
                        var attachment = new Attachment(ms, fileName);
                        attachment.ContentDisposition.CreationDate = DateTime.UtcNow;
                        attachment.ContentDisposition.ModificationDate = DateTime.UtcNow;
                        attachment.ContentDisposition.ReadDate = DateTime.UtcNow;
                        message.Attachments.Add(attachment);
                    }
                }
            }

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.UseDefaultCredentials = emailAccount.UseDefaultCredentials;
                smtpClient.Host = emailAccount.Host;
                smtpClient.Port = emailAccount.Port;
                smtpClient.EnableSsl = emailAccount.EnableSsl;
                smtpClient.Credentials = emailAccount.UseDefaultCredentials ? CredentialCache.DefaultNetworkCredentials : new NetworkCredential(emailAccount.Username, emailAccount.Password);
                smtpClient.Send(message);
            }
        }
    }
}
