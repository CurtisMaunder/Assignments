using System.ComponentModel.DataAnnotations;

namespace AdminWebPortal.Models;

public enum TransactionType {
    Deposit = 1,
    Withdraw = 2,
    TransferDebit = 3,
    TransferCredit = 4,
    ServiceCharge = 5
}
public class Transaction {
    public int TransactionID { get; set; }

    [Required]
    public TransactionType TransactionType { get; set; }

    [Required]
    public int AccountNumber { get; set; }

    public int? DestinationAccountNumber { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [StringLength(30, ErrorMessage = ("Maximum of 30 characters in a comment"))]
    public string Comment { get; set; }

    [Required]
    public DateTime TransactionTimeUTC { get; set; }
}