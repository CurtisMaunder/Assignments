using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminWebAPI.Models;

public class Customer {
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int CustomerID { get; set; }

    [Required]
    public string Name { get; set; }

    public string TFN { get; set; }

    public string Address { get; set; }

    public string Suburb { get; set; }

    public string State { get; set; }

    public string Postcode { get; set; }

    public string Mobile { get; set; }

    [Required]
    public bool Locked { get; set; }

    public virtual List<Account> Accounts { get; set; }
}