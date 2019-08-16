using GraphQL.Language.AST;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CustomerGraph.Models.Services
{
    public interface ICustomerService
    {
        IEnumerable<Customer> GetCustomers(IDictionary<string, Field> subFields, int customerNumber);
        IEnumerable<Address> GetAddresses(IDictionary<string, Field> subFields, int addressID);
    }
    public class CustomerService : ICustomerService
    {
        private readonly IConfiguration _configuration;

        public CustomerService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IEnumerable<Customer> GetCustomers(IDictionary<string, Field> subFields, int customerNumber)
        {
            string query = BuildSqlQueryForCustomer(subFields, customerNumber);
            List<Customer> customers = ReadCustomerData(query);
            return customers;
        }

        public IEnumerable<Address> GetAddresses(IDictionary<string, Field> subFields, int addressID)
        {
            string query = BuildSqlQueryForAddress(subFields, addressID);
            List<Address> addresses = ReadAddressData(query);
            return addresses;
        }

        private string BuildSqlQueryForCustomer(IDictionary<string, Field> subFields, int id)
        {
            StringBuilder queryBuilder = new StringBuilder();
            List<string> projection = new List<string>();
            queryBuilder.Append("select ");
            string whereClause = string.Format(" where customer.[Id] = {0}", id);

            foreach (KeyValuePair<string, Field> keyValuePair in subFields)
            {
                projection.Add(string.Format("customer.{0}", keyValuePair.Key));
            }
            queryBuilder.Append(string.Join(", ", projection));
            queryBuilder.Append(" from Customer as customer ");


            if (id > -1)
            {
                queryBuilder.Append(whereClause);
            }

            return queryBuilder.ToString();
        }

        private string BuildSqlQueryForAddress(IDictionary<string, Field> subFields, int addressID)
        {
            StringBuilder queryBuilder = new StringBuilder();
            List<string> projection = new List<string>
            {
                "address.AddressId"
            };

            queryBuilder.Append("select ");
            string innerJoinForContacts = " inner join Contact as contact on contact.AddressId = address.AddressId";
            string innerJoinForContactMethods = " inner join ContactMethod as contactmethod on contact.ContactId = contactmethod.ContactId";
            string whereClause = string.Format(" where address.AddressId = {0}", addressID);
            bool hasContacts = false;
            bool hasContactMethods = false;
            foreach (KeyValuePair<string, Field> keyValuePair in subFields)
            {
                if (keyValuePair.Key.ToLower() == "addressid") continue;
                if (keyValuePair.Key.ToLower() == "contacts")
                {
                    projection.Add("contact.ContactId");
                    hasContacts = true;
                    Field contactField = keyValuePair.Value;
                    foreach (Field field in contactField.SelectionSet.Selections)
                    {
                        if (field.Name.ToLower() == "contactid") continue;
                        if (field.Name.ToLower() == "contactmethods")
                        {
                            projection.Add("contactmethod.ContactMethodId");
                            hasContactMethods = true;
                            foreach (Field subField in field.SelectionSet.Selections)
                            {
                                if (subField.Name.ToLower() == "contactmethodid") continue;
                                projection.Add(string.Format("contactmethod.{0}", subField.Name));
                            }
                        }
                        else
                        {
                            projection.Add(string.Format("contact.{0}", field.Name));
                        }
                    }
                }
                
                else
                {
                    projection.Add(string.Format("address.{0}", keyValuePair.Key));
                }
            }
            queryBuilder.Append(string.Join(", ", projection));
            queryBuilder.Append(" from Address as address ");
            if (hasContacts)
            {
                queryBuilder.Append(innerJoinForContacts);
            }
            if (hasContactMethods)
            {
                queryBuilder.Append(innerJoinForContactMethods);
            }
            if (addressID > -1)
            {
                queryBuilder.Append(whereClause);
            }

            return queryBuilder.ToString();
        }


        private List<Customer> ReadCustomerData(string query)
        {
            List<Customer> customers = new List<Customer>();
            string connectionString = _configuration.GetConnectionString("CustomerGraphDb");
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        List<string> fieldNames = Enumerable.Range(0, reader.FieldCount).Select(i => reader.GetName(i).ToLower()).ToList();
                        
                        while (reader.Read())
                        {
                            Customer customer = new Customer();
                            string customerNumber = CheckAndReturnValueFromReader(reader, fieldNames, "customernumber");
                            customer.CustomerNumber = customerNumber == null ? -1 : int.Parse(customerNumber);
                            string businessUnitId = CheckAndReturnValueFromReader(reader, fieldNames, "businessunitid");
                            customer.BusinessUnitId = businessUnitId == null ? -1 : int.Parse(businessUnitId);
                            customer.SalesChannel = CheckAndReturnValueFromReader(reader, fieldNames, "saleschannel");
                            customer.Currency = CheckAndReturnValueFromReader(reader, fieldNames, "currency");
                            customer.Status = CheckAndReturnValueFromReader(reader, fieldNames, "status");
                            customers.Add(customer);
                        }
                    }
                }
            }

            return customers;
        }

        private List<Address> ReadAddressData(string query)
        {
            List<Address> addresses = new List<Address>();
            string connectionString = _configuration.GetConnectionString("CustomerGraphDb");
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        List<string> fieldNames = Enumerable.Range(0, reader.FieldCount).Select(i => reader.GetName(i).ToLower()).ToList();
                        List<int> addressIds = new List<int>();
                        List<int> contactIds = new List<int>();
                        Dictionary<int, List<Contact>> contactsForAddressId = new Dictionary<int, List<Contact>>();
                        Dictionary<int, List<ContactMethod>> contactMethodsForContactId = new Dictionary<int, List<ContactMethod>>();
                        List<Contact> contacts = null;
                        List<ContactMethod> contactMethods = null;
                        while (reader.Read())
                        {
                            Address address = new Address();
                            string addressId = CheckAndReturnValueFromReader(reader, fieldNames, "addressid");
                            address.AddressId  = int.Parse(addressId);
                            if (!addressIds.Contains(address.AddressId))
                            {
                                addressIds.Add(address.AddressId);
                                address.AddressLineOne = CheckAndReturnValueFromReader(reader, fieldNames, "addresslineone");
                                address.AddressLineTwo = CheckAndReturnValueFromReader(reader, fieldNames, "addresslinetwo");
                                address.City = CheckAndReturnValueFromReader(reader, fieldNames, "city");
                                address.State = CheckAndReturnValueFromReader(reader, fieldNames, "state");
                                address.ZipCode = CheckAndReturnValueFromReader(reader, fieldNames, "zipcode");
                                address.Country = CheckAndReturnValueFromReader(reader, fieldNames, "country");
                                addresses.Add(address);
                            }

                            // [ContactId], [AddressId], [FirstName], [LastName], [Title]
                            if (!contactsForAddressId.ContainsKey(address.AddressId))
                            {
                                contacts = new List<Contact>();
                                contactsForAddressId.Add(address.AddressId, contacts);
                            }

                            Contact contact = new Contact();
                            string contactId = CheckAndReturnValueFromReader(reader, fieldNames, "contactid");
                            contact.ContactId = contactId == null ? -1 : int.Parse(contactId);
                            if (!contactIds.Contains(contact.ContactId))
                            {
                                contactIds.Add(contact.ContactId);
                                contact.FirstName = CheckAndReturnValueFromReader(reader, fieldNames, "firstname");
                                contact.LastName = CheckAndReturnValueFromReader(reader, fieldNames, "lastname");
                                contact.Title = CheckAndReturnValueFromReader(reader, fieldNames, "title");
                                contacts.Add(contact);
                            }

                            if (!contactMethodsForContactId.ContainsKey(contact.ContactId))
                            {
                                contactMethods = new List<ContactMethod>();
                                contactMethodsForContactId.Add(contact.ContactId, contactMethods);
                            }
                            ContactMethod contactMethod = new ContactMethod();

                            // [ContactMethodId], [ContactId], [Type], [Email], [AreaCode], [Number]
                            string contactMethodId = CheckAndReturnValueFromReader(reader, fieldNames, "contactmethodid");
                            contactMethod.ContactMethodId = contactMethodId == null ? -1 : int.Parse(contactMethodId);
                            contactMethod.Type = CheckAndReturnValueFromReader(reader, fieldNames, "type");
                            contactMethod.Email = CheckAndReturnValueFromReader(reader, fieldNames, "email");
                            string areaCode = CheckAndReturnValueFromReader(reader, fieldNames, "areacode");
                            contactMethod.AreaCode = areaCode == null ? -1 : int.Parse(areaCode);
                            string number = CheckAndReturnValueFromReader(reader, fieldNames, "number");
                            contactMethod.Number = number == null ? -1 : int.Parse(number);

                            contactMethods.Add(contactMethod);
                        }

                        if (contactsForAddressId.Any())
                        {
                            for (int i = 0; i < addresses.Count; i++)
                            {
                                if (contactsForAddressId.ContainsKey(addresses[i].AddressId))
                                {
                                    addresses[i].Contacts = contactsForAddressId[addresses[i].AddressId];
                                    for (int j = 0; j < addresses[i].Contacts.Count; j++)
                                    {
                                        if (contactMethodsForContactId.ContainsKey(addresses[i].Contacts[j].ContactId)){
                                            addresses[i].Contacts[j].ContactMethods = contactMethodsForContactId[addresses[i].Contacts[j].ContactId];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var json = JsonConvert.SerializeObject(addresses);

            return addresses;
        }

        private string CheckAndReturnValueFromReader(SqlDataReader reader, List<string> fieldNames, string fieldName)
        {
            if (fieldNames.Contains(fieldName))
            {
                return reader[fieldName].ToString();
            }
            return null;
        }
    }
}
