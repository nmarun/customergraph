using GraphQL;

namespace CustomerGraph.Models.Schema
{
    public class CustomerSchema : GraphQL.Types.Schema
    {
        public CustomerSchema(IDependencyResolver dependencyResolver) : base(dependencyResolver)
        {
            Query = dependencyResolver.Resolve<CustomersQuery>();
            DependencyResolver = dependencyResolver;
        }
    }
}
