using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Bluepay
{
    public abstract class BluepayTransaction
    {
        protected readonly string AccountId;
        protected readonly string SecretKey;
        protected readonly string Mode;
        protected readonly string ApiUrl;
        protected readonly Encoding Encoding;
        protected readonly string BaseUrl;
        protected readonly string ContentType;
        protected readonly string UserAgent;
        protected readonly string TamperProofSeal;
        protected readonly string TpsHashType;
        protected readonly string PaymentType;
        protected readonly string TransactionType;
        protected readonly string MasterId;
        protected readonly string FirstName;
        protected readonly string LastName;
        protected readonly string Address1;
        protected readonly string Address2;
        protected readonly string City;
        protected readonly string State;
        protected readonly string Zip;
        protected readonly string Country;
        protected readonly string Phone;
        protected readonly string Email;
        protected readonly decimal Amount;

        public string PostData { get; protected set; }

        public BluepayTransaction(
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
            string paymentType,
            string transactionType,
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
            decimal amount)
        {
            AccountId = accountId;
            SecretKey = secretKey;
            Mode = mode;
            Encoding = encoding;
            BaseUrl = baseUrl;
            ApiUrl = baseUrl + apiUrlPart;
            ContentType = contentType;
            UserAgent = userAgent;
            TpsHashType = tpsHashType;
            PaymentType = paymentType;
            MasterId = masterId;
            PaymentType = paymentType;
            TransactionType = transactionType;
            FirstName = firstName;
            LastName = lastName;
            Address1 = address1;
            Address2 = address2;
            City = city;
            State = state;
            Zip = zip;
            Country = country;
            Phone = phone;
            Email = email;
            Amount = amount;
            
            var tps = AccountId + TransactionType + Amount + MasterId + Mode;

            var secretKeyBytes = Encoding.GetBytes(secretKey);
            var messageBytes = Encoding.GetBytes(tps);

            using (var hmac = new HMACSHA512(secretKeyBytes))
            {
                var hashBytes = hmac.ComputeHash(messageBytes);
                TamperProofSeal = ByteArrayToString(hashBytes);
            }
        }
        
        protected static string ByteArrayToString(byte[] bytes)
        {
            var sOutput = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                sOutput.Append(b.ToString("X2"));
            }

            return sOutput.ToString();
        }

        protected string GetDefaultPostData()
        {
            return
                "MERCHANT=" + HttpUtility.UrlEncode(AccountId) +
                "&MODE=" + HttpUtility.UrlEncode(Mode) +
                "&TRANSACTION_TYPE=" + HttpUtility.UrlEncode(TransactionType) +
                "&TAMPER_PROOF_SEAL=" + HttpUtility.UrlEncode(TamperProofSeal) +
                "&NAME1=" + HttpUtility.UrlEncode(FirstName) +
                "&NAME2=" + HttpUtility.UrlEncode(LastName) +
                "&COMPANY_NAME=" +
                "&AMOUNT=" + HttpUtility.UrlEncode(Amount.ToString(CultureInfo.InvariantCulture)) +
                "&ADDR1=" + HttpUtility.UrlEncode(Address1) +
                "&ADDR2=" + HttpUtility.UrlEncode(Address2) +
                "&CITY=" + HttpUtility.UrlEncode(City) +
                "&STATE=" + HttpUtility.UrlEncode(State) +
                "&ZIPCODE=" + HttpUtility.UrlEncode(Zip) +
                "&COMMENT=" +
                "&PHONE=" + HttpUtility.UrlEncode(Phone) +
                "&EMAIL=" + HttpUtility.UrlEncode(Email) +
                "&REBILLING=" +
                "&REB_FIRST_DATE=" +
                "&REB_EXPR=" +
                "&REB_CYCLES=" +
                "&REB_AMOUNT=" +
                "&RRNO=" + HttpUtility.UrlEncode(MasterId) +
                "&PAYMENT_TYPE=" + HttpUtility.UrlEncode(PaymentType) +
                "&INVOICE_ID=" +
                "&ORDER_ID=" +
                "&CUSTOM_ID=" +
                "&CUSTOM_ID2=" +
                "&AMOUNT_TIP=" +
                "&AMOUNT_TAX=" +
                "&AMOUNT_FOOD=" +
                "&AMOUNT_MISC=" +
                "&CUSTOMER_IP=" + System.Net.Dns.GetHostEntry("").AddressList.FirstOrDefault() +
                "&TPS_HASH_TYPE=" + HttpUtility.UrlEncode(TpsHashType) +
                "&RESPONSEVERSION=5";
        }
        
        public HttpRequestMessage GetRequest()
        {
            var postBody = Encoding.GetBytes(PostData);
            
            var requestContent = new ByteArrayContent(postBody);
            requestContent.Headers.ContentLength = postBody.Length;
            requestContent.Headers.ContentType = new MediaTypeHeaderValue(ContentType);

            var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl);
            request.Headers.Add("User-Agent", UserAgent);
            request.Content = requestContent;

            return request;
        }
    }
}