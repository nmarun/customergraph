using CustomerGraph.Models.Services;
using GraphQL;
using GraphQL.Types;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CustomerGraph.Models.Schema
{
    public class CustomersQuery : ObjectGraphType<object>
    {
        public CustomersQuery(ICustomerService customerService)
        {
            Field<ListGraphType<CustomerType>>(
                    "customers",
                    arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "customerNumber" }),
                    resolve: context =>
                    {
                        List<Customer> customers = customerService.GetCustomers(context.SubFields, context.GetArgument<int>("customerNumber", defaultValue: -1)).ToList();

                        if(customers == null || !customers.Any())
                        {
                            IDictionary data = new Dictionary<string, string>
                            {
                                { "key", "value" }
                            };

                            ExecutionError customError = new ExecutionError("Customer not found", data)
                            {
                                Code = "1000001",
                                
                            };
                            context.Errors.Add(customError);
                            return null;
                        }

                        return customers;
                    });

            Field<ListGraphType<AddressType>>(
                "addresses",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "addressId" }),
                resolve: context => customerService.GetAddresses(context.SubFields, context.GetArgument<int>("addressId", defaultValue: -1)));
        }
    }
}
