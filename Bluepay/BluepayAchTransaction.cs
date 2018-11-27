using System.Text;
using System.Web;

namespace Bluepay
{
    public class BluepayAchTransaction :
        BluepayTransaction
    {
        public BluepayAchTransaction(
            string accountId,
            string secretKey,
            string mode,
            Encoding encoding,
            string baseUrl,
            string apiUrlPart,
            string tpsHashType,
            string contentType,
            string userAgent,
            string masterId,
            string firstName,
            string lastName,
            string address1,
            string address2,
            string city,
            string state,
            string zip,
            string country,
            string phone,
            string email,
            decimal amount,
            string paymentType,
            string transactionType,
            string routingNumber,
            string accountNumber)
            : base(accountId, secretKey, mode, encoding, baseUrl, apiUrlPart, tpsHashType, contentType, userAgent, masterId, paymentType, transactionType,
                firstName, lastName, address1, address2, city, state, zip, country, phone, email, amount)
        {
            PostData = GetDefaultPostData()
                + "&ACH_ROUTING=" + HttpUtility.UrlEncode(routingNumber)
                + "&ACH_ACCOUNT=" + HttpUtility.UrlEncode(accountNumber)
                // These last two values are BluePay constants
                + "&ACH_ACCOUNT_TYPE=" + "C"
                + "&DOC_TYPE=" + "WEB";
        }
    }
}