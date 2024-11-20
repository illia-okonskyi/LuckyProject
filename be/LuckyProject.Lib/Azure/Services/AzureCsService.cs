using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Communication.Email;
using LuckyProject.Lib.Basics.Extensions;

namespace LuckyProject.Lib.Azure.Services
{
    public class AzureCsServiceOptions
    {
        public string ConnectionString { get; set; }
        public string EmailSenderAddress { get; set; }
    }

    public class AzureCsService : IAzureCsService
    {
        #region Internals & ctor
        private readonly AzureCsServiceOptions options;

        public AzureCsService(IOptions<AzureCsServiceOptions> options)
        {
            this.options = options.Value;
        }
        #endregion

        #region Public interface
        #region SendEmailAsync
        public async Task<EmailSendStatus> SendEmailAsync(
            List<EmailAddress> to,
            EmailContent content,
            List<EmailAddress> cc = null,
            List<EmailAddress> bcc = null,
            CancellationToken cancellationToken = default)
        {
            var client = new EmailClient(options.ConnectionString);
            var message = new EmailMessage(
                options.EmailSenderAddress,
                new EmailRecipients(to, cc.EmptyIfNull(), bcc.EmptyIfNull()),
                content);
            var op = await client.SendAsync(WaitUntil.Completed, message, cancellationToken);
            return op.Value.Status;
        }

        public Task<EmailSendStatus> SendEmailAsync(
            EmailAddress to,
            EmailContent content,
            CancellationToken cancellationToken = default) => 
            SendEmailAsync(new() { to }, content, null, null, cancellationToken);

        public Task<EmailSendStatus> SendEmailAsync(
            EmailAddress to,
            string subject,
            string htmlContent,
            CancellationToken cancellationToken = default) =>
            SendEmailAsync(
                new() { to },
                new EmailContent(subject) { Html = htmlContent},
                null,
                null,
                cancellationToken);
        #endregion
        #endregion
    }
}
