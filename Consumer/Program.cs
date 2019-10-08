using CustomerGraph.Models;
using GraphQL.Client;
using GraphQL.Common.Request;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Consumer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var query = @"query { customers { customerNumber businessUnitId salesChannel } }";
            var request = new GraphQLRequest()
            {
                Query = query
            };

            var graphQLClient = new GraphQLClient("http://localhost:54068/graph/");

            var graphQLResponse = await graphQLClient.PostAsync(request);

            string json = graphQLResponse.Data.ToString(Formatting.None);

            Output output = JsonConvert.DeserializeObject<Output>(json);
        }
    }

    public class Output
    {
        [JsonProperty(PropertyName = "customers")]
        public List<Customer> Customers { get; set; }
    }
}
