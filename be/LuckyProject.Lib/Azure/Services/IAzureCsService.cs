using Azure.Communication.Email;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Azure.Services
{
    public interface IAzureCsService
    {
        #region SendEmailAsync
        Task<EmailSendStatus> SendEmailAsync(
            List<EmailAddress> to,
            EmailContent content,
            List<EmailAddress> cc = null,
            List<EmailAddress> bcc = null,
            CancellationToken cancellationToken = default);
        Task<EmailSendStatus> SendEmailAsync(
            EmailAddress to,
            EmailContent content,
            CancellationToken cancellationToken = default);
        Task<EmailSendStatus> SendEmailAsync(
            EmailAddress to,
            string subject,
            string htmlContent,
            CancellationToken cancellationToken = default);
        #endregion
    }
}
