namespace CustomerGraph.Models
{
    public class ContactMethod
    {
        public int ContactMethodId { get; set; }
        public string Type { get; set; }
        public string Email { get; set; }
        public int AreaCode { get; set; }
        public int Number { get; set; }
    }
}
