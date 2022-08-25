using AdminWebPortal.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace AdminWebPortal.Helpers;

public static class BillPayFetcher {
    public static async Task<BillPay> GetBillByIDAsync(string id) {
        BillPay billPay = new BillPay();

        using (var client = new HttpClient()) {
            client.BaseAddress = new Uri("https://localhost:7291/api/BillPay/");
            //HTTP GET
            client.DefaultRequestHeaders.Clear();
            //define request data format
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //sending request
            HttpResponseMessage httpResponseMessage = await client.GetAsync("single/" + id);
            //checking if success
            if (httpResponseMessage.IsSuccessStatusCode) {
                var empResponse = httpResponseMessage.Content.ReadAsStringAsync().Result;
                //deserializing
                billPay = JsonConvert.DeserializeObject<BillPay>(empResponse);
            }
        }

        return billPay;
    }

    public static async Task<List<BillPay>> GetAllBillsAsync(string id) {
        List<BillPay> bills = new List<BillPay>(); 
        
        using (var client = new HttpClient()) {
            client.BaseAddress = new Uri("https://localhost:7291/api/BillPay/");
            //HTTP GET
            client.DefaultRequestHeaders.Clear();
            //define request data format
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //sending request
            HttpResponseMessage httpResponseMessage = await client.GetAsync(id);
            //checking if success
            if (httpResponseMessage.IsSuccessStatusCode) {
                var empResponse = httpResponseMessage.Content.ReadAsStringAsync().Result;
                //deserializing
                bills = JsonConvert.DeserializeObject<List<BillPay>>(empResponse);
            }
        }

        return bills;
    }
}