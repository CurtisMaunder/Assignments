using AdminWebPortal.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace AdminWebPortal.Helpers;

public static class CustomerFetcher {
    public static async Task<Customer> GetCustomerByIDAsync(string customerID) {
        Customer customer = new Customer();

        using (var client = new HttpClient()) {
            client.BaseAddress = new Uri("https://localhost:7291/api/customers");
            //HTTP GET
            client.DefaultRequestHeaders.Clear();
            //define request data format
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //sending request
            HttpResponseMessage httpResponseMessage = await client.GetAsync("customers/" + customerID);
            //checking if success
            if (httpResponseMessage.IsSuccessStatusCode) {
                var empResponse = httpResponseMessage.Content.ReadAsStringAsync().Result;
                //deserializing
                customer = JsonConvert.DeserializeObject<Customer>(empResponse);

            }
        }

        return customer;
    }

    public static async Task<List<Customer>> GetAllCustomersAsync() {
        List<Customer> customers = new List<Customer>();
        
        using (var client = new HttpClient()) {
            client.BaseAddress = new Uri("https://localhost:7291/api/");
            //HTTP GET
            client.DefaultRequestHeaders.Clear();
            //define request data format
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //sending request
            HttpResponseMessage httpResponseMessage = await client.GetAsync("customers");
            //checking if success
            if (httpResponseMessage.IsSuccessStatusCode) {
                var empResponse = httpResponseMessage.Content.ReadAsStringAsync().Result;
                //deserializing
                customers = JsonConvert.DeserializeObject<List<Customer>>(empResponse);
            }
        }

        return customers;
    }
}