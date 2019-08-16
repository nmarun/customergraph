using GraphQL.Types;

namespace CustomerGraph.Models.Schema
{
    public class ContactType : ObjectGraphType<Contact>
    {
        public ContactType()
        {
            Name = "ContactType";
            Field(f => f.ContactId);
            Field(f => f.FirstName);
            Field(f => f.LastName);
            Field(f => f.Title);
            Field<ListGraphType<ContactMethodType>>("contactmethods");
        }
    }
}
