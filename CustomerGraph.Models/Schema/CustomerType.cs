using GraphQL.Types;

namespace CustomerGraph.Models.Schema
{
    public class CustomerType : ObjectGraphType<Customer>
    {
        public CustomerType()
        {
            Field(f => f.CustomerNumber);
            Field(f => f.BusinessUnitId);
            Field(f => f.Currency);
            Field(f => f.SalesChannel);
            Field(f => f.Status);
        }
    }
}
