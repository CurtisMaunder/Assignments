using Microsoft.AspNetCore.Mvc;
using MCBAWebApplication.Data;
using MCBAWebApplication.Models;
using MCBAWebApplication.Filters;
using Microsoft.EntityFrameworkCore;

namespace MCBAWebApplication.Controllers;

[AuthorizeCustomer]
public class BillPayController : Controller {
    private readonly McbaWebContext _context;

    private int CustomerId => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public BillPayController(McbaWebContext context) => _context = context;

    //Billpay landing page
    //Eagerly loads the bills
    //Lazily loads the payees and customers for viewbags
    //Viewbags are used to help populate the tables as the billpay model doesnt contain the relevant data
    public async Task<IActionResult> Index(int id) {
        var bills = await _context.BillPays.Where(bp => bp.AccountNumber == id).ToListAsync();
        var payees = await _context.Payees.ToListAsync();
        var customer = await _context.Customers.FindAsync(CustomerId);

        ViewBag.AccountNumber = id;
        ViewBag.Payees = payees;

        return View(bills);
    }

    //Called from the Index view
    //Gets a bill from the db context matchin the one selected for deletion
    //Removes the bill and then returns the index with the accountnumber
    [HttpPost]
    public async Task<IActionResult> Delete(int id) {
        var bill = await _context.BillPays.FindAsync(id);
        int accountNum = bill.AccountNumber;

        _context.BillPays.Remove(bill);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { accountNum });
    }

    //Called from the Index view
    //Gets a bill from the db context matching the one selected for editing
    //Allows the user to edit the amount and period of their bill
    public async Task<IActionResult> Edit(int id) {
        var bill = await _context.BillPays.FindAsync(id);

        return View(bill);
    }


    //The validation for the Edit action called from the Edit View
    //Submits the changes to the db context
    [HttpPost]
    public async Task<IActionResult> Edit(int id, decimal amount, DateTime scheduleTimeUtc, Period period) {
        var bill = await _context.BillPays.FindAsync(id);

        if (amount < 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive");
        if (decimal.Round(amount, 2) != amount)
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places");

        if (scheduleTimeUtc <= DateTime.Now.AddHours(1))
            ModelState.AddModelError(nameof(scheduleTimeUtc), "Please select a future time");

        if (!ModelState.IsValid) {
            return View();
        }

        bill.Amount = amount;
        bill.ScheduleTimeUtc = scheduleTimeUtc;
        bill.Period = period;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }


    //Called from the Index View
    //Creates a new billpay
    //Takes in an ID to determine account number
    //Gets all payees to populate selection list
    public async Task<IActionResult> Create(int id) {
        ViewBag.AccountNumber = id;
        ViewBag.Payees = await _context.Payees.ToListAsync();

        return View();
    }

    //Validation of Create action
    //Submits the new entry to the db context
    [HttpPost]
    public async Task<IActionResult> Create(int id, int payeeID, decimal amount, DateTime scheduleTimeUtc, Period period) {
        var payees = _context.Payees.ToList();
        var customer = await _context.Customers.FindAsync(CustomerId);

        var accounts = from a in customer.Accounts select a;

        ViewBag.AccountNumber = id;
        ViewBag.Payees = payees;
        ViewBag.Accounts = accounts;


        if (amount < 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive");
        if (decimal.Round(amount, 2) != amount)
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places");

        if (scheduleTimeUtc <= DateTime.Now.AddHours(1))
            ModelState.AddModelError(nameof(scheduleTimeUtc), "Please select a future time");

        if (!ModelState.IsValid) {
            return View();
        }

        _context.BillPays.Add(
            new BillPay {
                AccountNumber = id,
                PayeeID = payeeID,
                Amount = amount,
                ScheduleTimeUtc = scheduleTimeUtc,
                Period = period,
                Status = Status.Awaiting
            });

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new {id});
    }
}

