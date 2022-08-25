using Microsoft.AspNetCore.Mvc;
using MCBAWebApplication.Data;
using MCBAWebApplication.Models;
using MCBAWebApplication.Filters;

namespace MCBAWebApplication.Controllers;

[AuthorizeCustomer]
public class WithdrawController : Controller {
    public const decimal SURCHARGE_WITHDRAW = 0.05M;
    public const decimal CHECKING_MINIMUM = 300.0M;

    private readonly McbaWebContext _context;

    private int CustomerId => HttpContext.Session.GetInt32(nameof(CustomerId)).Value;

    public WithdrawController(McbaWebContext context) => _context = context;

    //Called from the Customer index
    //Is given an account number as an id 
    public async Task<IActionResult> Index(int id) => View(await _context.Accounts.FindAsync(id));

    //Called from the Index view
    //Validates the deposit
    [HttpPost]
    public async Task<IActionResult> Index(int id, decimal amount, string comments) {
        var account = await _context.Accounts.FindAsync(id);

        if (amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive");
        if (decimal.Round(amount, 2) != amount)
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places");
        if (!ModelState.IsValid) {
            ViewBag.ammount = amount;
            return View(account);
        }

        //- - Fees logic - -
        //Check if the user has used their 2 free transactions
        //If so apply a $0.05 surcharge
        //Moved after client only checks because we access the database here

        int transactions = 0;
        foreach (var transaction in account.Transactions) {
            if (transaction.TransactionType == TransactionType.Withdraw || transaction.TransactionType == TransactionType.TransferDebit)
                transactions++;
        }

        if (transactions >= 2) {
            if(account.AccountType == AccountType.Checking) {
                if (account.AccountType == AccountType.Checking && amount > (account.Balance - SURCHARGE_WITHDRAW - CHECKING_MINIMUM))
                    ModelState.AddModelError(nameof(amount), "Not enough money to withdraw this amount - Checking Accounts need minimum $300 remaining");
            }
            else {
                if (amount > (account.Balance + SURCHARGE_WITHDRAW))
                    ModelState.AddModelError(nameof(amount), "Not enough money to withdraw this amount");
            }
        }
        else {
            if (account.AccountType == AccountType.Checking) {
                if (account.AccountType == AccountType.Checking && amount > (account.Balance - CHECKING_MINIMUM))
                    ModelState.AddModelError(nameof(amount), "Not enough money to withdraw this amount - Checking Accounts need minimum $300 remaining");
            }
            else {
                if (amount > (account.Balance + SURCHARGE_WITHDRAW))
                    ModelState.AddModelError(nameof(amount), "Not enough money to withdraw this amount");
            }
        }

        if (!ModelState.IsValid) {
            ViewBag.Amount = amount;
            return View(account);
        }

        return RedirectToAction(nameof(Confirmation), new { id, amount, comments, transactions});
    }

    //Called from the Index action
    //Asks the user to confirm their withdrawl
    public async Task<IActionResult> Confirmation(int id, decimal amount, string comments, int transactions) {
        var account = await _context.Accounts.FindAsync(id);

        ViewBag.Amount = amount;
        ViewBag.Comments = comments;
        ViewBag.Transactions = transactions;

        return View(account);
    }

    //Called from Confirmation view
    //Updates the database with the withdrawl then returns to Customer index
    [HttpPost]
    public async Task<IActionResult> Withdraw(int id, decimal amount, string comment, int transactions) {
        var account = await _context.Accounts.FindAsync(id);

        account.Balance -= amount;
        account.Transactions.Add(
            new Transaction {
                TransactionType = TransactionType.Withdraw,
                Amount = amount,
                Comment = comment,
                TransactionTimeUTC = DateTime.UtcNow
            });

        await _context.SaveChangesAsync();

        if (transactions >= 2) {
            account.Balance -= SURCHARGE_WITHDRAW;
            account.Transactions.Add(
                new Transaction {
                    TransactionType = TransactionType.ServiceCharge,
                    Amount = SURCHARGE_WITHDRAW,
                    Comment = "Service Fees - Withdrawl",
                    TransactionTimeUTC = DateTime.UtcNow
                });
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Customer");
    }
}

