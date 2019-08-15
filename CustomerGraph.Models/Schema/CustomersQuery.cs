using CustomerGraph.Models.Services;
using GraphQL.Types;

namespace CustomerGraph.Models.Schema
{
    public class CustomersQuery : ObjectGraphType<object>
    {
        public CustomersQuery(ICustomerService customerService)
        {
            Field<ListGraphType<CustomerType>>(
                    "customers",
                    arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "customerNumber" }),
                    resolve: context => customerService.GetCustomers(context.SubFields, context.GetArgument<int>("customerNumber", defaultValue: -1)));
        }
    }
}
