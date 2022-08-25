using Microsoft.EntityFrameworkCore;
using MCBAWebApplication.Models;

namespace MCBAWebApplication.Data;

public class McbaWebContext : DbContext {
    public McbaWebContext(DbContextOptions<McbaWebContext> options) : base(options) { }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Login> Logins { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<BillPay> BillPays { get; set; }
    public DbSet<Payee> Payees { get; set; }

    //Fluent API
    protected override void OnModelCreating(ModelBuilder builder) {
        base.OnModelCreating(builder);

        //Set check constraints (cannot be expressed with data annotations)
        builder.Entity<Login>().HasCheckConstraint("CH_Login_LoginID", "len(LoginID) = 8").
            HasCheckConstraint("CH_Login_PasswordHash", "len(PasswordHash) = 64");
        builder.Entity<Account>().HasCheckConstraint("CH_Account_Balance", "Balance >= 0");
        builder.Entity<Transaction>().HasCheckConstraint("CH_Transaction_Amount", "Amount > 0");

        //Configure ambiguous account.transactions navigation property relationship
        builder.Entity<Transaction>().
            HasOne(x => x.Account).WithMany(x => x.Transactions).HasForeignKey(x => x.AccountNumber);
    }

}
