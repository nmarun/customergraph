namespace CustomerGraph.Models
{
    public class Customer
    {
        public int CustomerNumber { get; set; }
        public int BusinessUnitId { get; set; }
        public string SalesChannel { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
    }
}
