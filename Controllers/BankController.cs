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
        //private IPersist<BankAccount> accPersist;
        private IReadData dataReader;
        private IPersist<object> genericPersist;
     
        public BankController(IReadData dataReader,
                              IPersist<object> genericPersist)
        {
            this.dataReader = dataReader;
            this.genericPersist = genericPersist;
        }

       
        [HttpPost]
        [Route("CreateBankAccount")]
        [Route("CreateAccount/{ownerName}")]
        public ActionResult CreateBankAccount(string ownerName)
        {
            try
            {
                Account acc = new Account(ownerName);

                //if(accPersist.Persist(acc))
                if(genericPersist.Persist(acc))
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
        [Route("Withdraw/{accId}/{amount}")]
        public ActionResult Withdraw(int accId, decimal amount)
        {
            try
            {
                Account acc;

                // Retrieve bank account
                if(!dataReader.GetAccountById(accId, out acc))
                {
                    return NotFound("Unable to find bank account with the given ID (" + accId + ").");
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
                // SingleAccountOpPersistHandler persistHandler = new SingleAccountOpPersistHandler(taxHandler, singleAccOpPersist);
                GenericPersistenceHandler<SingleAccountOperation> persistHandler = new GenericPersistenceHandler<SingleAccountOperation>(taxHandler, genericPersist);

                // Executes the widhtraw
                if(!persistHandler.Execute(withdrawOp))
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, "Unable to execute transfer.");
                }

                return Ok(acc);
            }
            catch(Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Exception msg: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("Deposit")]
        [Route("Deposit/{accId}/{amount}")]
        public ActionResult Deposit(int accId, decimal amount)
        {
            try
            {
                Account acc;

                // Retrieve bank account
                if(!dataReader.GetAccountById(accId, out acc))
                {
                    return NotFound("Unable to find bank account with the given ID (" + accId + ").");
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
                // SingleAccountOpPersistHandler persistHandler = new SingleAccountOpPersistHandler(taxHandler, singleAccOpPersist);
                GenericPersistenceHandler<SingleAccountOperation> persistHandler = new GenericPersistenceHandler<SingleAccountOperation>(taxHandler, genericPersist);

                // Executes the widhtraw
                if(!persistHandler.Execute(depositOp))
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, "Unable to execute transfer.");
                }

                return Ok(acc);
            }
            catch(Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Exception msg: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("Transfer")]
        [Route("Transfer/{sourceAccId}/{destAccId}/{amount}")]
        public ActionResult Transfer(int sourceAccId, int destAccId, decimal amount)
        {
            try
            {
                Account sourceAcc, destAcc;

                // Retrieve source bank account
                if(!dataReader.GetAccountById(sourceAccId, out sourceAcc))
                {
                    return NotFound("Unable to find source bank account with the given ID (" + sourceAccId + ").");
                }

                // Retrieve dest bank account
                if(!dataReader.GetAccountById(destAccId, out destAcc))
                {
                    return NotFound("Unable to find destination bank account with the given ID (" + destAccId + ").");
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
                GenericPersistenceHandler<DoubleAccountOperation> persistHandler = new GenericPersistenceHandler<DoubleAccountOperation>(taxHandler, genericPersist);

                // Executes the widhtraw
                if(!persistHandler.Execute(transferOp))
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, "Unable to execute transfer.");
                }

                return Ok(destAcc);
            }
            catch(Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Exception msg: " + ex.Message);
            }
        }

        [HttpGet]
        [Route("GetBankStatement")]
        [Route("GetBankStatement/{accId}")]
        public ActionResult GetBankStatement(int accId)
        {
            try
            {
                Account acc;

                // Retrieve bank account
                if (!dataReader.GetAccountById(accId, out acc))
                {
                    return NotFound("Unable to find bank account with the given ID (" + accId + ").");
                }

                // Retrieve associated operations 
                List<SingleAccountOperation> operations;
                if (!dataReader.GetOperationsByAccountId(accId, out operations))
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, "Unable to retrieve the operations associated to the account " + accId + ".");
                }

                BankStatement bankStatement = new BankStatement(acc, operations);

                return Ok(bankStatement.ToString());
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Exception msg: " + ex.Message);
            }
        }
        
    }
}
