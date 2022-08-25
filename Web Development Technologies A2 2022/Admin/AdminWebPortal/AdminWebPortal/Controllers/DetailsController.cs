using AdminWebPortal.Helpers;
using AdminWebPortal.Models;
using AdminWebPortal.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;

namespace AdminWebPortal.Controllers;

[AuthorizeAdmin]
public class DetailsController : Controller {
    public async Task<IActionResult> Index() {
        string customerID = HttpContext.Session.GetString("customerID");
        //GET:Customers
        Customer customer = await CustomerFetcher.GetCustomerByIDAsync(customerID);

        return View(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Index(string name, string tfn, string address, string suburb, string state, string postcode, string mobile) {
        string customerID = HttpContext.Session.GetString("customerID");
        //GET:Customers
        Customer customer = await CustomerFetcher.GetCustomerByIDAsync(customerID);

        return View(customer);
    }

    
    public async Task<IActionResult> Lock() {
        string customerID = HttpContext.Session.GetString("customerID");

        Customer customer = await CustomerFetcher.GetCustomerByIDAsync(customerID);

        customer.Locked = true;

        var content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");

        using (var client = new HttpClient()) {
            client.BaseAddress = new Uri("https://localhost:7291/api/");
            var response = client.PutAsync("Customers", content).Result;
        }

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Unlock() {
        string customerID = HttpContext.Session.GetString("customerID");

        Customer customer = await CustomerFetcher.GetCustomerByIDAsync(customerID);

        customer.Locked = false;

        var content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");

        using (var client = new HttpClient()) {
            client.BaseAddress = new Uri("https://localhost:7291/api/");
            var response = client.PutAsync("Customers", content).Result;
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> EditDetails(string name, string tfn, string address, string suburb, string state, string postcode, string mobile) {
        string customerID = HttpContext.Session.GetString("customerID");
        Customer customer = await CustomerFetcher.GetCustomerByIDAsync(customerID);

        customer.Name = name;
        customer.TFN = tfn;
        customer.Address = address;
        customer.Suburb = suburb;
        customer.State = state;
        customer.Postcode = postcode;
        customer.Mobile = mobile;

        var content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");

        using (var client = new HttpClient()) {
            client.BaseAddress = new Uri("https://localhost:7291/api/");
            var response = client.PutAsync("Customers", content).Result;
        }

        return RedirectToAction("Index");
    }
}