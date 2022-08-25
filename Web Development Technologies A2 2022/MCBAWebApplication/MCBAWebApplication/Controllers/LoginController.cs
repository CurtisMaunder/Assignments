using Microsoft.AspNetCore.Mvc;
using MCBAWebApplication.Data;
using MCBAWebApplication.Models;
using SimpleHashing;

namespace MCBAWebApplication.Controllers;

[Route("/Mcba/SecureLogin")]

public class LoginController : Controller {
    private readonly McbaWebContext _context;

    public LoginController(McbaWebContext context) => _context = context;

    public ActionResult Login() => View();

    [HttpPost]
    public async Task<ActionResult> Login(string loginID, string password) {
        var login = await _context.Logins.FindAsync(loginID);

        if (login == null || string.IsNullOrEmpty(password) || !PBKDF2.Verify(login.PasswordHash, password)) {
            ModelState.AddModelError("LoginFailed", "Login failed, please try again");
            return View(new Login { LoginID = loginID});
        }

        //Check if customer is able to log in
        //Jank but I messed it up in the schema and its honestly too late to fix
        Customer customer = await _context.Customers.FindAsync(login.CustomerID);

        if (customer.Locked) {
            ModelState.AddModelError("LoginFailed", "This account is locked please contact your admin");
            return View(new Login { LoginID = loginID });
        }

        //Login customer
        HttpContext.Session.SetInt32(nameof(Customer.CustomerID), login.CustomerID);
        HttpContext.Session.SetString(nameof(Customer.Name), login.Customer.Name);

        return RedirectToAction("Index", "Customer");
    }

    [Route("LogoutNow")]
    public IActionResult Logout() {
        //Logout customer
        HttpContext.Session.Clear();

        return RedirectToAction("Index", "Home");
    }
}