using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminWebAPI.Models;

public class Payee {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PayeeID { get; set; }

    [Required, StringLength(50)]
    public string Name { get; set; }

    [Required, StringLength(50)]
    public string Address { get; set; }

    [Required, StringLength(40)]
    public string Suburb { get; set; }

    [Required, StringLength(4, MinimumLength = 4)]
    public string Postcode { get; set; }

    //The regular expression should conform to an Australian phone number
    [StringLength(14)]
    [RegularExpression("^(?:\\((?=.*\\)))?(0?[4])\\)? (\\d\\d([ ](?=\\d{3})|(?!\\d\\d[- ]?\\d[- ]))\\d\\d[- ]?\\d[- ]?\\d{3})$")]
    public string Phone { get; set; }
}