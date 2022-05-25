using Soccer.Common.Models;

namespace Soccer.Web.Helpers
{
    public interface IMailHelper
    {
        Response SendMail(string toName, string toEmail, string subject, string body);
    }
}