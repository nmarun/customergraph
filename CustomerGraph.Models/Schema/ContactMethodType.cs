using GraphQL.Types;

namespace CustomerGraph.Models.Schema
{
    public class ContactMethodType : ObjectGraphType<ContactMethod>
    {
        public ContactMethodType()
        {
            Name = "ContactMethodType";
            Field(f => f.ContactMethodId);
            Field(f => f.AreaCode);
            Field(f => f.Email);
            Field(f => f.Number);
            Field(f => f.Type);
        }
    }
}
