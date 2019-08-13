using System.Collections.Generic;

namespace CustomerGraph.Models
{
    public class Address
    {
        public int AddressId { get; set; }
        public string AddressLineOne { get; set; }
        public string AddressLineTwo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public List<Contact> Contacts { get; set; }
    }
}
