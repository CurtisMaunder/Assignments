using System.ComponentModel.DataAnnotations;

namespace AdminWebPortal.Models;

public enum AccountType {
    Checking = 1,
    Saving = 2
}

public class Account {

    [Display(Name = "Account Number")]
    [StringLength(4, MinimumLength = 4, ErrorMessage = "An account number is 4 digits in length")]
    public int AccountNumber { get; set; }

    [Display(Name = "Type")]
    public AccountType AccountType { get; set; }

    [Required]
    public int CustomerID { get; set; }
    public Customer Customer { get; set; }

    [DataType(DataType.Currency)]
    public decimal Balance { get; set; }

    public List<Transaction> Transactions { get; set; }
}
