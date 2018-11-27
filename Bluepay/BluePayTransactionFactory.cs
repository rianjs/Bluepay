using System.Text;

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

        public BluepayCreditTransaction BuildCreditCardDebit(
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
            return new BluepayCreditTransaction(
                _accountId,
                _secretKey,
                _mode,
                _encoding,
                _baseUrl,
                _apiUrlPart,
                _tpsHashType,
                _contentType,
                _userAgent,
                _masterId,
                firstName,
                lastName,
                address1,
                address2,
                city,
                state,
                zip,
                country,
                phone,
                email,
                amount,
                paymentType: "CREDIT",
                transactionType: "SALE",
                ccNumber: ccNumber,
                ccExpiration: ccExpiration,
                cvv2: cvv2);
        }
        
        public BluepayTransaction BuildAchDebit(
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
            string routingNumber,
            string accountNumber)
        {
            return new BluepayAchTransaction(
                _accountId,
                _secretKey,
                _mode,
                _encoding,
                _baseUrl,
                _apiUrlPart,
                _tpsHashType,
                _contentType,
                _userAgent,
                _masterId,
                firstName,
                lastName,
                address1,
                address2,
                city,
                state,
                zip,
                country,
                phone,
                email,
                amount,
                paymentType: "ACH",
                transactionType: "SALE",
                routingNumber: routingNumber,
                accountNumber: accountNumber);
        }
    }
}
