using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Bluepay
{
    public class BluePayTransactionFactory
    {
        private const string _releaseVersion = "3.0.1";
        private static readonly Encoding _encoding = Encoding.ASCII;
        private const string _baseUrl = "https://secure.bluepay.com/interfaces/";
        private const string _apiUrlPart = "bp10emu";
        private const string _tpsHashType = "HMAC_SHA512";
        private const string _contentType = "application/x-www-form-urlencoded";
        private const string _userAgent = "BluePay C# Library/" + _releaseVersion;
        private const string _masterId = null;
        
        private readonly string _accountId;
        private readonly string _secretKey;
        private readonly string _mode;

        public BluePayTransactionFactory(string accountId, string secretKey, string mode)
        {
            _accountId = accountId;
            _secretKey = secretKey;
            _mode = mode;
        }

        public BluepayCreditCardTransaction BuildCreditCardDebit(
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
            string ccNumber,
            string ccExpiration,
            string cvv2,
            decimal amount)
        {
            return new BluepayCreditCardTransaction(
                accountId: _accountId,
                secretKey: _secretKey,
                mode: _mode,
                encoding: _encoding,
                baseUrl: _baseUrl,
                apiUrlPart: _apiUrlPart,
                tpsHashType: _tpsHashType,
                contentType: _contentType,
                userAgent: _userAgent,
                paymentType: "CREDIT",
                masterId: _masterId,
                transactionType: "SALE",
                firstName: firstName,
                lastName: lastName,
                address1: address1,
                address2: address2,
                city: city,
                state: state,
                zip: zip,
                country: country,
                phone: phone,
                email: email,
                ccNumber: ccNumber,
                ccExpiration: ccExpiration,
                cvv2: cvv2,
                amount: amount);
        }
    }
    
    public class BluepayCreditCardTransaction
    {
        private readonly string _apiUrl;
        private readonly Encoding _encoding;
        private readonly string _baseUrl;
        private readonly string _contentType;
        private readonly string _userAgent;
        
        public string PostData { get; }

        public BluepayCreditCardTransaction(
            string accountId,
            string secretKey,
            string mode,
            Encoding encoding,
            string baseUrl,
            string apiUrlPart,
            string tpsHashType,
            string contentType,
            string userAgent,
            string paymentType,
            string masterId,
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
            string ccNumber,
            string ccExpiration,
            string cvv2,
            decimal amount)
        {
            _encoding = encoding;
            _baseUrl = baseUrl;
            _apiUrl = baseUrl + apiUrlPart;
            _contentType = contentType;
            _userAgent = userAgent;
            
            var tps = accountId + transactionType + amount + masterId + mode;

            var secretKeyBytes = _encoding.GetBytes(secretKey);
            var messageBytes = _encoding.GetBytes(tps);

            string tamperProofSeal;
            using (var hmac = new HMACSHA512(secretKeyBytes))
            {
                var hashBytes = hmac.ComputeHash(messageBytes);
                tamperProofSeal = ByteArrayToString(hashBytes);
            }

            PostData =
                "MERCHANT=" + HttpUtility.UrlEncode(accountId) +
                "&MODE=" + HttpUtility.UrlEncode(mode) +
                "&TRANSACTION_TYPE=" + HttpUtility.UrlEncode(transactionType) +
                "&TAMPER_PROOF_SEAL=" + HttpUtility.UrlEncode(tamperProofSeal) +
                "&NAME1=" + HttpUtility.UrlEncode(firstName) +
                "&NAME2=" + HttpUtility.UrlEncode(lastName) +
                "&COMPANY_NAME=" +
                "&AMOUNT=" + HttpUtility.UrlEncode(amount.ToString(CultureInfo.InvariantCulture)) +
                "&ADDR1=" + HttpUtility.UrlEncode(address1) +
                "&ADDR2=" + HttpUtility.UrlEncode(address2) +
                "&CITY=" + HttpUtility.UrlEncode(city) +
                "&STATE=" + HttpUtility.UrlEncode(state) +
                "&ZIPCODE=" + HttpUtility.UrlEncode(zip) +
                "&COMMENT=" +
                "&PHONE=" + HttpUtility.UrlEncode(phone) +
                "&EMAIL=" + HttpUtility.UrlEncode(email) +
                "&REBILLING=" +
                "&REB_FIRST_DATE=" +
                "&REB_EXPR=" +
                "&REB_CYCLES=" +
                "&REB_AMOUNT=" +
                "&RRNO=" + HttpUtility.UrlEncode(masterId) +
                "&PAYMENT_TYPE=" + HttpUtility.UrlEncode(paymentType) +
                "&INVOICE_ID=" +
                "&ORDER_ID=" +
                "&CUSTOM_ID=" +
                "&CUSTOM_ID2=" +
                "&AMOUNT_TIP=" +
                "&AMOUNT_TAX=" +
                "&AMOUNT_FOOD=" +
                "&AMOUNT_MISC=" +
                "&CUSTOMER_IP=" + System.Net.Dns.GetHostEntry("").AddressList.FirstOrDefault() +
                "&TPS_HASH_TYPE=" + HttpUtility.UrlEncode(tpsHashType) +
                "&RESPONSEVERSION=5" +
                "&CC_NUM=" + HttpUtility.UrlEncode(ccNumber) +
                "&CC_EXPIRES=" + HttpUtility.UrlEncode(ccExpiration) +
                "&CVCVV2=" + HttpUtility.UrlEncode(cvv2);
        }
        
        private static string ByteArrayToString(byte[] bytes)
        {
            var sOutput = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                sOutput.Append(b.ToString("X2"));
            }

            return sOutput.ToString();
        }

        public HttpRequestMessage GetRequest()
        {
            var postBody = _encoding.GetBytes(PostData);
            
            var requestContent = new ByteArrayContent(postBody);
            requestContent.Headers.ContentLength = postBody.Length;
            requestContent.Headers.ContentType = new MediaTypeHeaderValue(_contentType);

            var request = new HttpRequestMessage(HttpMethod.Post, _apiUrl);
            request.Headers.Add("User-Agent", _userAgent);
            request.Content = requestContent;

            return request;
        } 
    }
}
