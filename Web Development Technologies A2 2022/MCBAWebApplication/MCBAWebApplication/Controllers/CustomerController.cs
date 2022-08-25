using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCBAWebApplication.Data;
using MCBAWebApplication.Models;
using MCBAWebApplication.Filters;
using X.PagedList;
using SimpleHashing;

namespace MCBAWebApplication.Controllers;

[AuthorizeCustomer]
public class CustomerController : Controller {
    private readonly McbaWebContext _context;

    //ReSharper disable once PossibleInvalidOperationException
    private int CustomerId => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public CustomerController(McbaWebContext context) => _context = context;
    
    //Customer landing page
    //Lazily loads the customer
    public async Task<IActionResult> Index() {
        var customer = await _context.Customers.FindAsync(CustomerId);

        return View(customer);
    }

    //Called from Index page
    //Lazily loads all customer info
    //Allows user to edit profile information
    public async Task<IActionResult> MyProfile() => View(await _context.Customers.FindAsync(CustomerId));

    //Called from MyProfile view
    //Lazily load's the customers info
    //Assigns any changed values to the customer
    //Validates
    //Updates database
    [HttpPost]
    public async Task<IActionResult> MyProfile(string name, string tfn, string address, string suburb, string state, string postcode, string mobile) {
        var customer = await _context.Customers.FindAsync(CustomerId);

        if (name != customer.Name && name != null)
            customer.Name = name;

        if (tfn != customer.TFN && tfn != null)
            customer.TFN = tfn;

        if (address != customer.Address && address != null)
            customer.Address = address;

        if (suburb != customer.Suburb && suburb != null)
            customer.Suburb = suburb;

        if (state != customer.State && state != null)
            customer.State = state;

        if (postcode != customer.Postcode && postcode != null)
            customer.Postcode = postcode;

        if (mobile != customer.Mobile && mobile != null)
            customer.Mobile = mobile;

        await _context.SaveChangesAsync();

        return View(customer);
    }

    //Called from the MyProfile view
    //Changes the users password
    //Submits it to the database hashed
    [HttpPost]
    public async Task<IActionResult> ChangePassword(string password) {
        if(password.Length > 16)
            ModelState.AddModelError(nameof(password), "Password must be less than 16 characters");
        if (!ModelState.IsValid) {
            return RedirectToAction(nameof(MyProfile));
        }

        string hashedPassword = PBKDF2.Hash(password);

        Console.WriteLine(CustomerId.ToString());

        var login = await _context.Logins.Where(x => x.CustomerID == CustomerId).SingleOrDefaultAsync();

        login.PasswordHash = hashedPassword;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(MyProfile));
    }

    //Called from Index view
    //Lazily loads all of the transactions for the account
    public async Task<IActionResult> MyStatements(int id, int? page) {
        var account = await _context.Accounts.FindAsync(id);

        ViewBag.Balance = account.Balance;

        //Get transactions and sort by date
        var transactions = from t in account.Transactions select t;
        transactions = transactions.OrderByDescending(t => t.TransactionTimeUTC);

        if(page == null)
            page = 1;
        int pageSize = 4;
        int pageNumber = (page ?? 1);

        return View(transactions.ToPagedList(pageNumber, pageSize));
    }
}

