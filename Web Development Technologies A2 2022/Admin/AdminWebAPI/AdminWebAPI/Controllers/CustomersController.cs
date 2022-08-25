using Microsoft.AspNetCore.Mvc;
using AdminWebAPI.Models;
using AdminWebAPI.Models.DataManager;

namespace AdminWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase {
    private readonly CustomerManager _customerManager;

    public CustomersController(CustomerManager customerManager) => _customerManager = customerManager;

    //GET: api/customers
    //Get all Customers in the database 
    //Full Example URL: https://localhost:7291/api/Customers
    [HttpGet]
    public async Task<IEnumerable<Customer>> Get() {
        return await _customerManager.GetAll();
    }

    //GET: api/Customers/{id}
    //{id} is a Customer.CustomerID
    //Get the Customer associated with this CustomerID
    //Full Example URL: https://localhost:7291/api/Customers/2100
    [HttpGet("{id}")]
    public async Task<Customer> GetByID(int id) {
        return await _customerManager.Get(id);
    }

    //PUT: api/Customers/
    //Used to update a Customer passed through as JSON object
    //Full example URL: https://localhost:7291/api/Customers/
    [HttpPut]
    public async Task<int> UpdateCustomer(Customer customer) {
        return await _customerManager.Update(customer.CustomerID, customer);
    }
}