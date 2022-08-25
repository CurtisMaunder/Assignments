using Microsoft.AspNetCore.Mvc;
using AdminWebPortal.Models;
using AdminWebPortal.Filters;
using AdminWebPortal.Helpers;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace AdminWebPortal.Controllers;

[AuthorizeAdmin]
public class AdminPanelController : Controller {
    public async Task<IActionResult> Index() {
        HttpContext.Session.Remove("customerID");
        //GET:Customers
        List<Customer> customers = await CustomerFetcher.GetAllCustomersAsync();

        return View(customers);
    }

    [HttpPost]
    public async Task<IActionResult> Search(string customerID) {
        //GET:Customers
        Customer customer = await CustomerFetcher.GetCustomerByIDAsync(customerID);

        if (customer != null) {
            HttpContext.Session.SetString("customerID", customerID);
            return RedirectToAction("Customer", "AdminPanel");
        }

        return RedirectToAction("Index", "AdminPanel");
    }

    public async Task<IActionResult> Customer() {
        string customerID = HttpContext.Session.GetString("customerID");

        //GET:Customers
        Customer customer = await CustomerFetcher.GetCustomerByIDAsync(customerID);

        return View(customer);
    }

    public async Task<IActionResult> Transactions(int id) {
        List<Transaction> transactions = new List<Transaction>();

        using (var client = new HttpClient()) {
            client.BaseAddress = new Uri("https://localhost:7291/api/Accounts/");
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage httpResponseMessage = await client.GetAsync(id + "/Transactions");
            if (httpResponseMessage.IsSuccessStatusCode) {
                var empResponse = httpResponseMessage.Content.ReadAsStringAsync().Result;
                transactions = JsonConvert.DeserializeObject<List<Transaction>>(empResponse);
            }
            else {
                Console.Error.WriteLine(id);    
            }

            return View(transactions);
        }
    }
}
