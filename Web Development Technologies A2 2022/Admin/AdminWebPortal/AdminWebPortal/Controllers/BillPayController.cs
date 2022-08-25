using AdminWebPortal.Helpers;
using AdminWebPortal.Models;
using AdminWebPortal.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;

namespace AdminWebPortal.Controllers;

[AuthorizeAdmin]
public class BillPayController : Controller {
    public async Task<IActionResult> Index(int id) {
        //GET:BillPays
        List<BillPay> billPays = await BillPayFetcher.GetAllBillsAsync(id.ToString());

        return View(billPays);
    }

    public async Task<IActionResult> Freeze(string id, int accountnumber) {
        BillPay billPay = await BillPayFetcher.GetBillByIDAsync(id);

        billPay.Status = Status.Frozen;

        var content = new StringContent(JsonConvert.SerializeObject(billPay), Encoding.UTF8, "application/json");

        using (var client = new HttpClient()) {
            client.BaseAddress = new Uri("https://localhost:7291/api/");
            var response = client.PutAsync("Billpay", content).Result;
        }

        return RedirectToAction("Index", new {id = accountnumber});
    }

    public async Task<IActionResult> Unfreeze(string id, int accountnumber) {
        BillPay billPay = await BillPayFetcher.GetBillByIDAsync(id);

        billPay.Status = Status.Awaiting;

        var content = new StringContent(JsonConvert.SerializeObject(billPay), Encoding.UTF8, "application/json");

        using (var client = new HttpClient()) {
            client.BaseAddress = new Uri("https://localhost:7291/api/");
            var response = client.PutAsync("Billpay", content).Result;
        }

        return RedirectToAction("Index", new {id = accountnumber});
    }

}
