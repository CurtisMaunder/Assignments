using Microsoft.EntityFrameworkCore;
using AdminWebAPI.Models;

namespace AdminWebAPI.Data;

public class McbaWebContext : DbContext {
    public McbaWebContext(DbContextOptions<McbaWebContext> options) : base(options) { }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Login> Logins { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<BillPay> BillPays { get; set; }
    public DbSet<Payee> Payees { get; set; }
}
