using System.Text;
using System.Web;

namespace Bluepay
{
    public class BluepayCreditTransaction :
        BluepayTransaction
    {
        public BluepayCreditTransaction(
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
            string ccNumber,
            string ccExpiration,
            string cvv2)
            : base(accountId, secretKey, mode, encoding, baseUrl, apiUrlPart, tpsHashType, contentType, userAgent, masterId, paymentType, transactionType,
                firstName, lastName, address1, address2, city, state, zip, country, phone, email, amount)
        {
            PostData = GetDefaultPostData()
                + "&CC_NUM=" + HttpUtility.UrlEncode(ccNumber)
                + "&CC_EXPIRES=" + HttpUtility.UrlEncode(ccExpiration)
                + "&CVCVV2=" + HttpUtility.UrlEncode(cvv2);
        }
    }
}