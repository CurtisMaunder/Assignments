using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminWebAPI.Models;

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


    [Required, ForeignKey("Account")]
    public int AccountNumber { get; set; }
    public virtual Account Account { get; set; }

    [ForeignKey("DestinationAccount")]
    public int? DestinationAccountNumber { get; set; }
    public virtual Account DestinationAccount { get; set; }

    [Required, Column(TypeName = "money")]
    public decimal Amount { get; set; }

    public string Comment { get; set; }

    [Required]
    public DateTime TransactionTimeUTC { get; set; }
}