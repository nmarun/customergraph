using GraphQL.Types;

namespace CustomerGraph.Models.Schema
{
    public class AddressType : ObjectGraphType<Address>
    {
        public AddressType()
        {
            Name = "AddressType";
            Field(f => f.AddressId);
            Field(f => f.AddressLineOne);
            Field(f => f.AddressLineTwo );
            Field(f => f.City);
            Field(f => f.Country);
            Field(f => f.State);
            Field(f => f.ZipCode);
            Field<ListGraphType<ContactType>>("contacts");
        }
    }
}
