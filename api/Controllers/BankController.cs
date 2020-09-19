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
        private IDataReader dataReader;
        private IPersistor<Account> accountPersistor;

        private Withdraw withdrawOp;
        private Deposit depositOp;
        private Transference transferenceOp;

        public BankController(IDataReader dataReader,
                              IPersistor<Account> accountPersistor,
                              Withdraw withdrawOp,
                              Deposit depositOp,
                              Transference transferenceOp)
        {
            this.dataReader = dataReader;
            this.accountPersistor = accountPersistor;
            this.withdrawOp = withdrawOp;
            this.depositOp = depositOp;
            this.transferenceOp = transferenceOp;
        }


        [HttpPost]
        [Route("CreateBankAccount")]
        [Route("CreateAccount/{ownerName}")]
        public ActionResult CreateBankAccount(string ownerName)
        {           
            if(String.IsNullOrWhiteSpace(ownerName))
            {
                throw new ArgumentNullException("Onwer's name can't be empty.");                
            }

            Account acc = new Account(ownerName);

            accountPersistor.Persist(acc);
            
            return Ok(acc);
        }

        [HttpPost]
        [Route("Withdraw")]
        [Route("Withdraw/{accId}/{amount}")]
        public ActionResult Withdraw(int accId, decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException("Amount parameter must be more than 0.");
            
            List<Account> accounts = withdrawOp.Execute(new List<int>() { accId }, amount);

            return Ok(accounts.First());
        }

        [HttpPost]
        [Route("Deposit")]
        [Route("Deposit/{accId}/{amount}")]
        public ActionResult Deposit(int accId, decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException("Amount parameter must be more than 0.");

            List<Account> accounts = depositOp.Execute(new List<int>() { accId }, amount);

            return Ok(accounts.First());    
        }

        [HttpPost]
        [Route("Transfer")]
        [Route("Transfer/{sourceAccId}/{destAccId}/{amount}")]
        public ActionResult Transfer(int sourceAccId, int destAccId, decimal amount)
        {
            if(sourceAccId == destAccId)
                throw new InvalidOperationException("Destination account must be different from source account.");

            if (amount <= 0)
                throw new ArgumentOutOfRangeException("Amount parameter must be more than 0.");

            List<Account> accounts = transferenceOp.Execute(new List<int>() { sourceAccId, destAccId }, amount);

            // Returns ok status with both accounts
            return Ok(accounts);
        }

        [HttpGet]
        [Route("GetBankStatement")]
        [Route("GetBankStatement/{accId}")]
        public ActionResult GetBankStatement(int accId)
        {
            Account acc = dataReader.GetAccountById(accId);

            if (acc == null)
            {
                throw new System.Collections.Generic.KeyNotFoundException("Account (" + accId + ") not found.");
            }

            // Retrieve associated operations 
            List<SingleAccountOperation> operations = dataReader.GetOperationsByAccountId(accId);   

            BankStatement bankStatement = new BankStatement(acc, operations);

            return Ok(bankStatement.ToString());     
   
        }
        
    }
}
