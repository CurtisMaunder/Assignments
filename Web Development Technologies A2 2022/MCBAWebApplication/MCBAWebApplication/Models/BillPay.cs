using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCBAWebApplication.Models;

public enum Period {
    [Display(Name = "One off")]
    OneOff = 0,
    Monthly = 1
}

public enum Status {
    Awaiting = 0,
    Success = 1,
    Failure = 2,
    Frozen = 3
}

public class BillPay {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BillPayId { get; set; }

    [Required]
    public int AccountNumber { get; set; }
    //public virtual Account Account { get; set; }

    [Required]
    public int PayeeID { get; set; }
    public virtual Payee Payee { get; set; }

    [Required, Column(TypeName = "money")]
    public decimal Amount { get; set; }

    [Required, Column(TypeName = "datetime2")]
    public DateTime ScheduleTimeUtc { get; set; }

    [Required]
    public Period Period { get; set; }

    [Required]
    public Status Status { get; set; }
}