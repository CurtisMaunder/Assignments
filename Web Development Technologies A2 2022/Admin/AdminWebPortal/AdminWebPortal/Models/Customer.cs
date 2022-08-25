using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AdminWebPortal.Models;

public class Customer {
    public int CustomerID { get; set; }

    [Required, StringLength(50)]
    public string Name { get; set; }

    [StringLength(11)]
    public string TFN { get; set; }

    [StringLength(50)]
    public string Address { get; set; }

    [StringLength(40)]
    public string Suburb { get; set; }

    [StringLength(3, MinimumLength = 2)]
    public string State { get; set; }

    [StringLength(4, MinimumLength = 4)]
    [RegularExpression("^[0-9]+$")]
    public string Postcode { get; set; }

    //The regular expression should conform to an Australian phone number
    [StringLength(12)]
    [RegularExpression("^(?:\\((?=.*\\)))?(0?[4])\\)? (\\d\\d([ ](?=\\d{3})|(?!\\d\\d[- ]?\\d[- ]))\\d\\d[- ]?\\d[- ]?\\d{3})$")]
    public string Mobile { get; set; }

    public bool Locked { get; set; }

    [JsonProperty("accounts")]
    public List<Account> Accounts { get; set; }
}