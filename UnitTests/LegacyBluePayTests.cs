using Bluepay;
using NUnit.Framework;

namespace UnitTests
{
    public class LegacyBluePayTests
    {
        private const string _firstName = "Bob";
        private const string _lastName = "Tester";
        private const string _address1 = "1234 Test St.";
        private const string _address2 = "Apt #500";
        private const string _city = "Testville";
        private const string _state = "IL";
        private const string _zip = "54321";
        private const string _country = "USA";
        private const string _phone = "123-123-12345";
        private const string _email = "test@bluepay.com";

        private const string _ccNumber = "4111111111111111";
        private const string _ccExpiration = "1225";
        private const string _cvv2 = "123";

        private const decimal _amount = 3.00m;
        
        [Test]
        public void CancelCustomerAchTransaction()
        {
            // Lifted from https://www.bluepay.com/developers/api-documentation/csharp/transactions/cancel-transaction/
            string accountId = "Merchant's Account ID Here";
            string secretKey = "Merchant's Secret Key Here";
            string mode = "TEST";

            var payment = new BluePay
            (
                accountId,
                secretKey,
                mode
            );

            payment.SetCustomerInformation
            (
                firstName: _firstName,
                lastName: _lastName,
                address1: _address1,
                address2: _address2,
                city: _city,
                state: _state,
                zip: _zip,
                country: _country,
                phone: _phone,
                email: _email
            );
            
            payment.SetCCInformation
            (
                ccNumber: _ccNumber,
                ccExpiration: _ccExpiration,
                cvv2: _cvv2
            );

            // Sale Amount: $3.00
            payment.Sale(amount: "3.00");
            
            var legacyPostData = payment.Process();
            
            
            var bpFactory = new BluePayTransactionFactory(accountId, secretKey, mode);
            var foo = bpFactory.BuildCreditCardDebit(
                _firstName,
                _lastName,
                _address1,
                _address2,
                _city,
                _state,
                _zip,
                _country,
                _phone,
                _email,
                _ccNumber,
                _ccExpiration,
                _cvv2,
                _amount);
            
            Assert.AreEqual(foo.PostData, legacyPostData);
        }
    }
}