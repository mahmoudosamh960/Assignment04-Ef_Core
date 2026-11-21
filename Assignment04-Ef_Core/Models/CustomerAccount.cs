namespace NationalBankManagementSystem.Models
{
    public class CustomerAccount
    {
        public int CustomerId { get; set; }

        public string AccountNumber { get; set; }

        public DateTime OwnershipStartDate { get; set; }

        public string OwnershipType { get; set; }

        public string AccountStatus { get; set; }

        public Customer Customer { get; set; }

        public Account Account { get; set; }
    }
}
