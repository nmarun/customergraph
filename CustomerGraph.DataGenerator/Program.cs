using Bogus;
using System;
using CustomerGraph.Models;

namespace CustomerGraph.DataGenerator
{
    public class Program
    {
        private static string locale = "en_CA";
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        private static Address GetFakeAddress()
        {
            var fakeAddress = new Faker<Address>(locale)
                .RuleFor(p => p.AddressLineOne, f => f.Address.StreetAddress())
                .RuleFor(p => p.City, f => f.Address.City())
                .RuleFor(p => p.State, f => f.Address.State())
                .RuleFor(p => p.Country, f => "CA")
                .RuleFor(p => p.ZipCode, f => f.Address.ZipCode())
            var address = fakeAddress.Generate();
            return address;
        }

        private static List<ContactMethodRequest> GetContactMethods(string fakeEmail, PhoneNumber phoneNumber)
        {
            return new List<ContactMethodRequest>
                {
                    new ContactMethodRequest
                    {
                        TypeCode = ContactMethodType.EmailAddress,
                        UsageTypeCode = ContactMethodUsageType.Email,
                        EmailAddress = fakeEmail,
                        Purpose= ContactMethodPurposeType.Invoice
                    }
                    ,
                    new ContactMethodRequest
                    {
                        TypeCode = ContactMethodType.TelephoneNumber,
                        UsageTypeCode = ContactMethodUsageType.HomePhone,
                        PhoneNumber = phoneNumber,
                        Purpose= ContactMethodPurposeType.Phone

                    },
                };
        }

        private static PhoneNumber GetFakePhoneNumber()
        {
            Random random = new Random();
            var fakePhone = new Faker<PhoneNumber>()
                .RuleFor(p => p.AreaCode, f => random.Next(200, 800).ToString())
                .RuleFor(p => p.CountryCode, f => "1")
                .RuleFor(p => p.Number, f => random.Next(2000000, 8000000).ToString());

            var phoneNumber = fakePhone.Generate();
            return phoneNumber;
        }

        private static string GetFakeEmail(PersonName personName)
        {
            return string.Format("{0}@geemail.com", new Faker().Internet.UserName(personName.GivenNameOne, personName.LastName));
        }

        private static PersonName GetFakePersonName(string locale)
        {
            var fakeName = new Faker<PersonName>(locale)
                            .RuleFor(p => p.GivenNameOne, f => f.Name.FirstName(f.PickRandom<Gender>()))
                            .RuleFor(p => p.LastName, f => f.Name.LastName())
                            .RuleFor(p => p.NameUsage, f => "1000001");
            return fakeName.Generate();
        }
    }
}
