using AdminWebAPI.Data;
using AdminWebAPI.Models.Repository;
using AdminWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminWebAPI.Models.DataManager;

public class CustomerManager : IDataRepository<Customer, int> {
    private readonly McbaWebContext _context;

    public CustomerManager(McbaWebContext context) => _context = context;

    public async Task<IEnumerable<Customer>> GetAll() {
        return  await _context.Customers.ToListAsync();
    }

    public async Task<Customer> Get(int id) {
        var customer = await _context.Customers.Include(customer => customer.Accounts).ThenInclude(account => account.Transactions).SingleOrDefaultAsync(customer => customer.CustomerID == id);

        if (customer == null) {
            return null;
        }

        return customer;
    }

    public async Task<int> Update(int id, Customer customer) {
        _context.Update(customer);
        
        await _context.SaveChangesAsync();

        return 1;
    } 
}