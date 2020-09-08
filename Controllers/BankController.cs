using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BankAccountWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BankController : ControllerBase
    {
        #region Objects used to persist/read data (bank account and operations)
        private IPersist<BankAccount> accPersist;
        private IRead<BankAccount> bankAccountReader;
        private IPersist<SingleAccountOperation> singleAccOpPersist;
        private IPersist<DoubleAccountOperation> doubleAccOpPersist;
     
        #endregion

        public BankController(IPersist<BankAccount> accPersist,
                                         IRead<BankAccount> bankAccountReader,
                                         IPersist<SingleAccountOperation> singleAccOpPersist,
                                         IPersist<DoubleAccountOperation> doubleAccOpPersist)
        {
            this.accPersist = accPersist;
            this.bankAccountReader = bankAccountReader;
            this.singleAccOpPersist = singleAccOpPersist;
            this.doubleAccOpPersist = doubleAccOpPersist;           
        }

       
        [HttpPost]
        [Route("CreateBankAccount")]
        [Route("CreateBankAccount/{ownerName}")]
        public ActionResult CreateBankAccount(string ownerName)
        {
            try
            {
                BankAccount acc = new BankAccount(ownerName);

                if(accPersist.Persist(acc))
                {
                    return Ok(acc);
                }
                else
                {               
                    return StatusCode((int)HttpStatusCode.InternalServerError, "Unknown error while persisting data.");
                }
            }
            catch(Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Exception msg: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("Withdraw")]
        [Route("Withdraw/{bankAccId}/{amount}")]
        public ActionResult Withdraw(int bankAccId, decimal amount)
        {
            BankAccount acc;

            // Retrieve bank account
            if(!bankAccountReader.Read(bankAccId, out acc))
            {
                return NotFound("Unable to find bank account with the given ID (" + bankAccId + ")");
            }

            // Creates the object with all the data necessary to perform a Withdraw
            SingleAccountOperation withdrawOp = new SingleAccountOperation(acc);
            withdrawOp.Amount = amount;
            withdrawOp.OperationType = OperationType.WIDTHDRAW;
       
            // Creates the object responsible for carrying out the withdraw operation
            WidthdrawHandler opHandler = new WidthdrawHandler();

            // Creates the object responsible for calculating the tax associate with the operation
            SingleAccountOpTaxHandler taxHandler = new SingleAccountOpTaxHandler(opHandler);

            // Creates the object responsible for persisting the operation 
            SingleAccountOpPersistHandler persistHandler = new SingleAccountOpPersistHandler(taxHandler, singleAccOpPersist);

            // Executes the widhtraw
            if(!persistHandler.Execute(withdrawOp))
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Unable to execute transfer.");
            }

            return Ok(acc);
        }

        [HttpPost]
        [Route("Deposit")]
        [Route("Deposit/{bankAccId}/{amount}")]
        public ActionResult Deposit(int bankAccId, decimal amount)
        {
            BankAccount acc;

            // Retrieve bank account
            if(!bankAccountReader.Read(bankAccId, out acc))
            {
                return NotFound("Unable to find bank account with the given ID (" + bankAccId + ")");
            }

            // Creates the object with all the data necessary to perform a deposit
            SingleAccountOperation depositOp = new SingleAccountOperation(acc);
            depositOp.Amount = amount;
            depositOp.OperationType = OperationType.DEPOSIT;

            // Creates the object responsible for carrying out the deposit operation
            DepositHandler opHandler = new DepositHandler();

            // Creates the object responsible for calculating the tax associate with the operation
            SingleAccountOpTaxHandler taxHandler = new SingleAccountOpTaxHandler(opHandler);

            // Creates the object responsible for persisting the operation 
            SingleAccountOpPersistHandler persistHandler = new SingleAccountOpPersistHandler(taxHandler, singleAccOpPersist);

            // Executes the widhtraw
            if(!persistHandler.Execute(depositOp))
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Unable to execute transfer.");
            }

            return Ok(acc);
        }

        [HttpPost]
        [Route("Transfer")]
        [Route("Transfer/{sourceAccId}/{destAccId}/{amount}")]
        public ActionResult Transfer(int sourceAccId, int destAccId, decimal amount)
        {
            BankAccount sourceAcc, destAcc;

            // Retrieve source bank account
            if(!bankAccountReader.Read(sourceAccId, out sourceAcc))
            {
                return NotFound("Unable to find source bank account with the given ID (" + sourceAccId + ")");
            }

            // Retrieve dest bank account
            if(!bankAccountReader.Read(destAccId, out destAcc))
            {
                return NotFound("Unable to find dest bank account with the given ID (" + destAccId + ")");
            }

            // Creates the object with all the data necessary to perform a deposit
            DoubleAccountOperation transferOp = new DoubleAccountOperation(sourceAcc, destAcc);
            transferOp.Amount = amount;
            transferOp.OperationType = OperationType.TRANSFERENCE;

            // Creates the object responsible for carrying out the deposit operation
            TransferHandler opHandler = new TransferHandler();

            // Creates the object responsible for calculating the tax associate with the operation
            DoubleAccountOpTaxHandler taxHandler = new DoubleAccountOpTaxHandler(opHandler);

            // Creates the object responsible for persisting the operation 
            DoubleAccountOpPersistHandler persistHandler = new DoubleAccountOpPersistHandler(taxHandler, doubleAccOpPersist);

            // Executes the widhtraw
            if(!persistHandler.Execute(transferOp))
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Unable to execute transfer.");
            }

            return Ok(destAcc);
        }

        [HttpGet]
        [Route("GetBankStatement")]
        [Route("GetBankStatement/{sourceAccId}/{destAccId}/{amount}")]
        public ActionResult GetBankStatement()
        {
            return Ok();
        }
    }
}
