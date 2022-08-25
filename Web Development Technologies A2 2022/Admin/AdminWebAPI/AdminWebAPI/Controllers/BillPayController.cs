using Microsoft.AspNetCore.Mvc;
using AdminWebAPI.Models;
using AdminWebAPI.Models.DataManager;

namespace AdminWebAPI.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class BillPayController : ControllerBase {
        private readonly BillPayManager _billPayManager;

        public BillPayController(BillPayManager billPayManager) => _billPayManager = billPayManager;

        //GET: api/BillPay/{id}
        //{id} is an Account.AccountNumber
        //Used to get all the BillPays associated with {id}
        //Full example URL: https://localhost:7291/api/BillPay/4100
        [HttpGet("{id}")]
        public async Task<IEnumerable<BillPay>> GetAll(int id) {
            return await _billPayManager.GetAll(id);
        }

        //GET: api/BIllPay/Single/{id}
        //{id} is a BillPay.BillPayId
        //Used to get a single BillPay associated with {id}
        //Full example URL: https://localhost:7291/api/BillPay/Single/1
        [HttpGet("single/{id}")]
        public async Task<BillPay> Get(int id) {
            return await _billPayManager.Get(id);
        }

        //PUT: api/BillPay/
        //Used to update a BillPay passed through as JSON object
        //Full example URL: https://localhost:7291/api/BillPay/4100
        [HttpPut]
        public async Task<int> UpdatePayment(BillPay billPay) {
            return await _billPayManager.Update(billPay.BillPayId, billPay);
        }
    }
}