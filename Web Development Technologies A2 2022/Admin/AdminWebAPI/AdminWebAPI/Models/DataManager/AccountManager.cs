using AdminWebAPI.Data;
using AdminWebAPI.Models.Repository;

using Microsoft.EntityFrameworkCore;

namespace AdminWebAPI.Models.DataManager;

public class AccountManager : IDataRepository<Account, int> {
    private readonly McbaWebContext _context;

    public AccountManager(McbaWebContext context) => _context = context;

    public async Task<IEnumerable<Account>> GetAll(int id) {
        return await _context.Accounts.Where(x => x.CustomerID == id).ToListAsync();
    }

    public async Task<Account> Get(int id) {
        var account = await _context.Accounts.Include(account => account.Transactions).SingleOrDefaultAsync(account => account.AccountNumber == id);

        if (account == null) {
            return null;
        }

        return account;
    }

    public async Task<int> Update(int id, Account account) {
        _context.Update(account);

        await _context.SaveChangesAsync();

        return 1;
    }

    public Task<IEnumerable<Account>> GetAll() {
        throw new NotImplementedException();
    }
}