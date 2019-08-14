using Bogus;
using System;
using CustomerGraph.Models;
using System.Collections.Generic;
using static Bogus.DataSets.Name;
using System.Linq;
using System.IO;
using System.Text;

namespace CustomerGraph.DataGenerator
{
    public class Program
    {
        private static readonly string locale = "en_CA";
        private static readonly int customerCount = 5;
        private static readonly int addressCount = 15;
        private static readonly int contactCount = 30;
        private static readonly string filePath = @"C:\temp\queries.txt";
        private static readonly string customerInsert = "INSERT INTO Customer Values ({0}, {1}, '{2}', '{3}', '{4}')";
        private static readonly string addressInsert = "INSERT INTO [Address] VALUES ({0}, '{1}', '{2}', '{3}' ,'{4}', '{5}', '{6}', {7})";
        private static readonly string contactInsert = "INSERT INTO [Contact] VALUES ({0}, {1}, '{2}', '{3}', '{4}')";
        private static readonly string contactMethodInsert = "INSERT INTO [ContactMethod] VALUES ({0}, {1}, '{2}', '{3}', {4}, {5})";

        static void Main()
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            List<Customer> customers = GetCustomers();
            string queries = BuildQueries(customers);
            WriteQueriesToFile(queries, "Customer");

            List<int> customerNumbers = customers.Select(c => c.CustomerNumber).ToList();
            List<Address> addresses = GetAddresses();
            queries = BuildQueries(addresses, customerNumbers);
            WriteQueriesToFile(queries, "Address");

            List<int> addressIds = addresses.Select(a => a.AddressId).ToList();
            List<Contact> contacts = GetContacts();
            queries = BuildQueries(contacts, addressIds);
            WriteQueriesToFile(queries, "Contact");

            List<int> contactIds = contacts.Select(a => a.ContactId).ToList();
            List<ContactMethod> contactMethods = GetContactMethods();
            queries = BuildQueries(contactMethods, contactIds);
            WriteQueriesToFile(queries, "Contact Methods");
        }

        private static string BuildQueries(List<ContactMethod> contactMethods, List<int> contactIds)
        {
            //INSERT INTO [ContactMethod] VALUES (1, 2, 'type', '<Email, nvarchar(100),>', 123, 13434)
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < contactMethods.Count; i++)
            {
                int contactIndex = i / 2;

                stringBuilder.AppendFormat(contactMethodInsert, contactMethods[i].ContactMethodId, contactIds[contactIndex], contactMethods[i].Type,
                    contactMethods[i].Email, contactMethods[i].AreaCode, contactMethods[i].Number);
                stringBuilder.AppendLine();
            }
            return stringBuilder.ToString();
        }

        private static string BuildQueries(List<Contact> contacts, List<int> addressIds)
        {
            // INSERT INTO [Contact] VALUES (1, 2, 'first', 'last', 'mr')
            StringBuilder stringBuilder = new StringBuilder();
            Faker faker = new Faker();
            string[] titles = new string[] { "Mr.", "Ms." };
            for (int i = 0; i < contacts.Count; i++)
            {
                stringBuilder.AppendFormat(contactInsert, contacts[i].ContactId, faker.PickRandom(addressIds), contacts[i].FirstName,
                    contacts[i].LastName, faker.PickRandom(titles));
                stringBuilder.AppendLine();
            }
            return stringBuilder.ToString();
        }

        private static string BuildQueries(List<Address> addresses, List<int> customerNumbers)
        {
            // INSERT INTO [Address] VALUES (1, 'line 1', 'line 2', 'city' ,'state', 'zip', 'CA', 12)
            StringBuilder stringBuilder = new StringBuilder();
            Faker faker = new Faker();
            for (int i = 0; i < addresses.Count; i++)
            {
                stringBuilder.AppendFormat(addressInsert, addresses[i].AddressId, addresses[i].AddressLineOne, addresses[i].AddressLineTwo, 
                    addresses[i].City, addresses[i].State, addresses[i].ZipCode, addresses[i].Country, faker.PickRandom(customerNumbers));
                stringBuilder.AppendLine();
            }
            return stringBuilder.ToString();
        }

        private static string BuildQueries(List<Customer> customers)
        {
            // insert into Customer Values (11, 707, 'CA_02', 'CAD', 'Y')
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < customers.Count; i++)
            {
                stringBuilder.AppendFormat(customerInsert, customers[i].CustomerNumber, customers[i].BusinessUnitId
                    , customers[i].SalesChannel, customers[i].Currency, customers[i].Status);
                stringBuilder.AppendLine();
            }
            return stringBuilder.ToString();
        }

        private static void WriteQueriesToFile(string queries, string name)
        {
            if (!File.Exists(filePath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    sw.WriteLine(string.Format("-- Start {0}", name));
                    sw.Write(queries);
                    sw.WriteLine(string.Format("-- End {0}", name));
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLine(string.Format("-- Start {0}", name));
                    sw.Write(queries);
                    sw.WriteLine(string.Format("-- End {0}", name));
                }
            }
        }

        private static List<Contact> GetContacts()
        {
            List<Contact> contacts = new List<Contact>();
            int counter = 1111;
            for (int i = 0; i < contactCount; i++)
            {
                Contact contact = GetPerson();
                contact.ContactId = counter++;
                contacts.Add(contact);
            }
            return contacts;
        }

        private static List<Address> GetAddresses()
        {
            List<Address> addresses = new List<Address>();
            int counter = 111;
            for (int i = 0; i < addressCount; i++)
            {
                addresses.Add(GetAddress(counter++));
            }
            
            return addresses;
        }

        private static List<Customer> GetCustomers()
        {
            int counter = 11;
            string[] salesChannels = new string[] { "CA_01", "CA_02", "CA_03", "CA_04" };
            List<Customer> customers = new List<Customer>();
            for (int i = 0; i < customerCount; i++)
            {
                var fakeCustomer = new Faker<Customer>()
                    .RuleFor(p => p.CustomerNumber, f => counter++)
                    .RuleFor(p => p.BusinessUnitId, f => 707)
                    .RuleFor(p => p.Currency, f => "CAD")
                    .RuleFor(p => p.SalesChannel, f => f.PickRandom(salesChannels))
                    .RuleFor(p => p.Status, f => "Y");
                Customer customer = fakeCustomer.Generate();
                customers.Add(customer);
            }

            return customers;
        }

        private static Address GetAddress(int counter)
        {
            var fakeAddress = new Faker<Address>(locale)
                .RuleFor(p => p.AddressId, f => counter++)
                .RuleFor(p => p.AddressLineOne, f => f.Address.StreetAddress())
                .RuleFor(p => p.City, f => f.Address.City())
                .RuleFor(p => p.State, f => f.Address.StateAbbr())
                .RuleFor(p => p.Country, f => "CA")
                .RuleFor(p => p.ZipCode, f => f.Address.ZipCode());
            var address = fakeAddress.Generate();
            return address;
        }

        private static List<ContactMethod> GetContactMethods()
        {
            int counter = 11111;
            List<ContactMethod> contactMethods = new List<ContactMethod>();
            for (int i = 0; i < contactCount; i++)
            {
                Contact contact = GetPerson();
                string fakeEmail = GetFakeEmail(contact);
                Random random = new Random();
                List<ContactMethod> contactMethodsPerContact = new List<ContactMethod>
                {
                    new ContactMethod
                    {
                        ContactMethodId = counter++,
                        Email = fakeEmail,
                        Type = "Email Address"

                    },
                    new ContactMethod
                    {
                        ContactMethodId = counter++,
                        Type = "Phone Number",
                        AreaCode = random.Next(200, 800),
                        Number = random.Next(2000000, 8000000)
                    }
                };
                contactMethods.AddRange(contactMethodsPerContact);
            }
            return contactMethods;
        }


        private static string GetFakeEmail(Contact contact)
        {
            return string.Format("{0}@geemail.com", new Faker().Internet.UserName(contact.FirstName, contact.LastName));
        }

        private static Contact GetPerson()
        {
            var fakeName = new Faker<Contact>(locale)
                            .RuleFor(p => p.FirstName, f => f.Name.FirstName(f.PickRandom<Gender>()))
                            .RuleFor(p => p.LastName, f => f.Name.LastName());
            return fakeName.Generate();
        }
    }
}
