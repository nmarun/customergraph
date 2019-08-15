using System.Collections.Generic;

namespace CustomerGraph.Models
{
    public class Contact
    {
        public int ContactId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public List<ContactMethod> ContactMethods { get; set; }
    }
}
