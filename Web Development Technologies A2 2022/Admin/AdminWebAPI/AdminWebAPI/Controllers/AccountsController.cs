using Microsoft.AspNetCore.Mvc;
using AdminWebAPI.Models;
using AdminWebAPI.Models.DataManager;

namespace AdminWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase {
        private readonly AccountManager _accountManager;

        public AccountsController(AccountManager accountManager) => _accountManager = accountManager;

        //GET: api/Accounts/all/{id}
        //{id} is a Customer.CustomerID
        //Used to get all accounts associated with that CustomerID
        //Full example URL: https://localhost:7291/api/Accounts/all/2100
        [HttpGet("all/{id}")]
        public async Task<IEnumerable<Account>> GetAll(int id) {
            return await _accountManager.GetAll(id);
        }

        //GET: api/Accounts/{id}
        //{id} is an Account.AccountNumber
        //Used to get the single account associated with this AccountNumber
        //Full Example URL: https://localhost:7291/api/Accounts/4100
        [HttpGet("{id}")]
        public async Task<Account> GetByID(int id) {
            return await _accountManager.Get(id);
        }

        //GET: api/Accounts/{id}/Transactions
        //{id} is an Account.AccountNumber
        //Used to get all transactions associated with tis AccountNumber
        //Full Example URL: https://localhost:7291/api/4100/Transactions
        [HttpGet("{id}/Transactions")]
        public async Task<List<Transaction>> GetTransactionByID(int id) {
            Account account = await _accountManager.Get(id);
            var transaction = from t in account.Transactions select t;
            List < Transaction > transactions = transaction.ToList();
            return transactions;
        }
    }
}
