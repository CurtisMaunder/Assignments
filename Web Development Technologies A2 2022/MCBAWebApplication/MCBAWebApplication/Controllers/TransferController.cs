using Microsoft.AspNetCore.Mvc;
using MCBAWebApplication.Data;
using MCBAWebApplication.Models;
using MCBAWebApplication.Filters;

namespace MCBAWebApplication.Controllers;

[AuthorizeCustomer]
public class TransferController : Controller {
    public const decimal SURCHARGE_TRANSFER = 0.10M;
    public const decimal CHECKING_MINIMUM = 300.0M;

    private readonly McbaWebContext _context;

    private int CustomerId => HttpContext.Session.GetInt32(nameof(CustomerId)).Value;

    public TransferController(McbaWebContext context) => _context = context;

    //Called from the Customer index
    //Is given an account number as an id 
    public async Task<IActionResult> Index(int id) => View(await _context.Accounts.FindAsync(id));

    //Called from the Index view
    //Validates the transfer
    [HttpPost]
    public async Task<IActionResult> Index(int id, int destAccountNumber, decimal amount, string comments) {
        var account = await _context.Accounts.FindAsync(id);
        var destinationAccount = await _context.Accounts.FindAsync(destAccountNumber);

        //Using a null reference exception block because FindAsync does not simply return null on a failed search
        try {
            if (destinationAccount.AccountNumber == account.AccountNumber)
                ModelState.AddModelError(nameof(destinationAccount), "Cannot transfer to the same account");
        }
        catch (NullReferenceException) {
            ModelState.AddModelError(nameof(destinationAccount), "The account was not found");
        }

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
            if (account.AccountType == AccountType.Checking) {
                if (account.AccountType == AccountType.Checking && amount > (account.Balance - SURCHARGE_TRANSFER - CHECKING_MINIMUM))
                    ModelState.AddModelError(nameof(amount), "Not enough money to transfer this amount - Checking Accounts need minimum $300 remaining");
            }
            else {
                if (amount > (account.Balance + SURCHARGE_TRANSFER))
                    ModelState.AddModelError(nameof(amount), "Not enough money to transfer this amount");
            }
        }
        else {
            if (account.AccountType == AccountType.Checking) {
                if (account.AccountType == AccountType.Checking && amount > (account.Balance - CHECKING_MINIMUM))
                    ModelState.AddModelError(nameof(amount), "Not enough money to transfer this amount - Checking Accounts need minimum $300 remaining");
            }
            else {
                if (amount > (account.Balance + SURCHARGE_TRANSFER))
                    ModelState.AddModelError(nameof(amount), "Not enough money to transfer this amount");
            }
        }

        if (!ModelState.IsValid) {
            ViewBag.Amount = amount;
            return View(account);
        }

        return RedirectToAction(nameof(Confirmation), new { id, destAccountNumber, amount, comments, transactions });
    }

    //Called from the Index action
    //Asks the user to confirm their transfer
    public async Task<IActionResult> Confirmation(int id, int destAccountNumber, decimal amount, string comments, int transactions) {
        var account = await _context.Accounts.FindAsync(id);
        var destinationAccount = await _context.Accounts.FindAsync(destAccountNumber);

        ViewBag.Amount = amount;
        ViewBag.Comments = comments;
        ViewBag.Transactions = transactions;
        ViewBag.DestAccountNumber = destinationAccount.AccountNumber;

        return View(account);
    }

    //Called from Confirmation view
    //Updates the database with the transfer then returns to Customer index
    [HttpPost]
    public async Task<IActionResult> Transfer(int id, int destAccountNumber, decimal amount, string comments, int transactions) {
        var account = await _context.Accounts.FindAsync(id);
        var destinationAccount = await _context.Accounts.FindAsync(destAccountNumber);

        account.Balance -= amount;
        account.Transactions.Add(
            new Transaction {
                TransactionType = TransactionType.TransferDebit,
                DestinationAccountNumber = destinationAccount.AccountNumber,
                Amount = amount,
                Comment = comments,
                TransactionTimeUTC = DateTime.UtcNow
            });

        await _context.SaveChangesAsync();

        destinationAccount.Balance += amount;
        destinationAccount.Transactions.Add(
            new Transaction {
                TransactionType = TransactionType.TransferCredit,
                Amount = amount,
                Comment = comments,
                TransactionTimeUTC = DateTime.UtcNow
            });

        await _context.SaveChangesAsync();

        if (transactions >= 2) {
            account.Balance -= SURCHARGE_TRANSFER;
            account.Transactions.Add(
                new Transaction {
                    TransactionType = TransactionType.ServiceCharge,
                    Amount = SURCHARGE_TRANSFER,
                    Comment = "Service Fees - Transfer",
                    TransactionTimeUTC = DateTime.UtcNow
                });
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Customer");
    }
}
