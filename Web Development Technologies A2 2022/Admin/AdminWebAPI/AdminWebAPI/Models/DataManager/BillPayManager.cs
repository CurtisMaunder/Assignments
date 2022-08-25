using AdminWebAPI.Data;
using AdminWebAPI.Models.Repository;
using AdminWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminWebAPI.Models.DataManager;

public class BillPayManager : IDataRepository<BillPay, int> {
    private readonly McbaWebContext _context;

    public BillPayManager(McbaWebContext context) => _context = context;

    public async Task<BillPay> Get(int id) {
        return await _context.BillPays.Where(x => x.BillPayId == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<BillPay>> GetAll(int id) {
        return await _context.BillPays.Where(x => x.AccountNumber == id).ToListAsync();
    }

    public Task<IEnumerable<BillPay>> GetAll() {
        throw new NotImplementedException();
    }

    public async Task<int> Update(int id, BillPay billPay) {
        _context.Update(billPay);

        await _context.SaveChangesAsync();

        return 1;
    }
}
