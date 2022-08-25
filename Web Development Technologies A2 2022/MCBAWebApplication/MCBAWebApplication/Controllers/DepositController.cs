using Microsoft.AspNetCore.Mvc;
using MCBAWebApplication.Data;
using MCBAWebApplication.Models;
using MCBAWebApplication.Filters;

namespace MCBAWebApplication.Controllers;

[AuthorizeCustomer]
public class DepositController : Controller {
    private readonly McbaWebContext _context;

    private int CustomerId => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public DepositController(McbaWebContext context) => _context = context;

    //Called from the Customer index
    //Is given an account number as an id 
    public async Task<IActionResult> Index(int id) => View(await _context.Accounts.FindAsync(id));

    //Called from the Index view
    //Validates the deposit
    [HttpPost]
    public async Task<IActionResult> Index(int id, decimal amount, string comment) {
        var account = await _context.Accounts.FindAsync(id);

        if (amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive");
        if (decimal.Round(amount, 2) != amount)
            ModelState.AddModelError(nameof(amount), "Amount mcannot have more than 2 decimal places");
        if (!ModelState.IsValid) { 
            ViewBag.Amount = amount;
            return View(account);
        }

        return RedirectToAction(nameof(Confirmation), new {id, amount, comment});
    }

    //Called from the Index action
    //Asks the user to confirm their deposit
    public async Task<IActionResult> Confirmation(int id, decimal amount, string comment) {
        var account = await _context.Accounts.FindAsync(id);

        ViewBag.amount = amount;
        ViewBag.Comment = comment;
        return View(account);
    }

    //Called from Confirmation view
    //Updates the database with the deposit then returns to Customer index
    [HttpPost]
    public async Task<IActionResult> Deposit(int id, decimal amount, string comment) {
        var account = await _context.Accounts.FindAsync(id);

        account.Balance += amount;
        account.Transactions.Add(
            new Transaction {
                TransactionType = TransactionType.Deposit,
                Amount = amount,
                Comment = comment,
                TransactionTimeUTC = DateTime.UtcNow,
            });

        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Customer");
    }
}
