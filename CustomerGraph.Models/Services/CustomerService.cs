using GraphQL.Language.AST;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CustomerGraph.Models.Services
{
    public interface ICustomerService
    {
        IEnumerable<Customer> GetCustomers(IDictionary<string, Field> subFields, int customerNumber);
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
            string query = BuildSqlQuery(subFields, customerNumber);
            List<Customer> customers = Read(query);
            return customers;
        }

        private string BuildSqlQuery(IDictionary<string, Field> subFields, int id)
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

        private List<Customer> Read(string query)
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
                        }
                    }
                }
            }

            return customers;
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
