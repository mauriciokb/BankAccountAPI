using NUnit.Framework;
using BankAccountWebAPI;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace TestProject
{
    public class Test
    {
        BankAccountWebAPI.Controllers.BankController controller;
        Mock<IDataReader> dataReaderMock = new Mock<IDataReader>();
        Mock<IPersistor<SingleAccountOperation>> singleAccPersistorMock = new Mock<IPersistor<SingleAccountOperation>>();
        Mock<IPersistor<DoubleAccountOperation>> doubleAccPersistorMock = new Mock<IPersistor<DoubleAccountOperation>>();
        Mock<IPersistor<Account>> accountPersistorMock = new Mock<IPersistor<Account>>();

        [SetUp]
        public void Setup()
        {
            controller = new BankAccountWebAPI.Controllers.BankController(dataReaderMock.Object,
                                                                          accountPersistorMock.Object,
                                                                          new Withdraw(dataReaderMock.Object, singleAccPersistorMock.Object),
                                                                          new Deposit(dataReaderMock.Object, singleAccPersistorMock.Object),
                                                                          new Transference(dataReaderMock.Object, doubleAccPersistorMock.Object));

        }

        [Test]
        public void CreateAccount_EmptyName_ThrowsException()
        {
            try
            {
                controller.CreateBankAccount("");
            }
            catch (System.ArgumentNullException)
            {
                Assert.Pass();
            }
        }

        [Test]
        public void Deposit_NegativeAmount_ThrowsOutOfRangeException()
        {
            try
            {
                controller.Deposit(1,-1m);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                Assert.Pass();
            }          
        }

        [Test]
        public void Withdraw_NegativeAmount_ThrowsOutOfRangeException()
        {
            try
            {
                controller.Withdraw(1, -1m);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                Assert.Pass();
            }          
        }

        [Test]
        public void Transference_NegativeAmount_ThrowsOutOfRangeException()
        {
            try
            {
                controller.Transfer(1, 2, -1m);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                Assert.Pass();
            }           
        }


        [Test]
        public void Deposit_InvalidAccount_ThrowsKeyNotFoundException()
        {
            try
            {
                // Mocks the reader to returns null, indicating no account was found
                dataReaderMock.Setup(reader => reader.GetAccountById(1)).Returns(() => null);
                controller.Deposit(1, 0);
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                Assert.Pass();
            }           
        }

        [Test]
        public void Withdraw_InvalidAccount_ThrowsKeyNotFoundException()
        {
            try
            {
                // Mocks the reader to returns null, indicating no account was found
                dataReaderMock.Setup(reader => reader.GetAccountById(1)).Returns(() => null);
                controller.Withdraw(1, 0);
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                Assert.Pass();
            }           
        }

        [Test]
        public void Transference_InvalidAccount_ThrowsKeyNotFoundException()
        {
            try
            {
                // Mocks the reader to returns destAcc if passed accId matches
                dataReaderMock.Setup(reader => reader.GetAccountById(It.IsAny<int>())).Returns(() => null );

                controller.Transfer(1, 0, 5);
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                Assert.Pass();
            }          
        }


        [Test]
        public void Withdraw_AccountWithInsufficientBalance_ShouldThrowInvalidOperationException()
        {
            try
            {
                // Mocks the reader to returns a bank account with 50 bucks
                dataReaderMock.Setup(reader => reader.GetAccountById(It.IsAny<int>())).Returns(new Account(1, "Pedro", 50));

                // Then tries to withdraw 100 bucks!
                controller.Withdraw(1, 100);
            }
            catch (System.InvalidOperationException)
            {
                Assert.Pass();
            }          
        }


        [Test]
        public void Transference_AccountWithInsufficientBalance_ShouldThrowInvalidOperationException()
        {
            try
            {
                // Mocks the reader to returns a bank account with 50 bucks
                dataReaderMock.Setup(reader => reader.GetAccountById(It.IsAny<int>())).Returns(new Account(1, "Pedro", 50));

                // Then tries to transfer 100 bucks!
                controller.Transfer(1, 2, 100);
            }
            catch (System.InvalidOperationException)
            {
                Assert.Pass();
            }         
        }

        [Test]
        public void Transference_SourceAccountAndDestinationAccountAreTheSame_ShouldThrowInvalidOperationException()
        {
            try
            {
                // Mocks the reader to returns a bank account with 50 bucks
                dataReaderMock.Setup(reader => reader.GetAccountById(It.IsAny<int>())).Returns(new Account(1, "Pedro", 50));

                // Then tries to transfer 20 bucks to the same account
                controller.Transfer(1, 1, 20);
            }
            catch (System.InvalidOperationException)
            {
                Assert.Pass();
            }         
        }


        [Test]
        public void Withdraw_Withdraws10DollarsFromAccountWith20DollarsBalance_ShouldRemain6Dollars()
        {
            // Mocks the reader to returns a bank account with 20 bucks
            dataReaderMock.Setup(reader => reader.GetAccountById(It.IsAny<int>())).Returns(new Account(1, "Pedro", 20));

            // Then withdraws 10 bucks
            // Since the tax for this kind of operation is 4 dollars, 
            // the returned account must have balance equals to 6 dollars
            Microsoft.AspNetCore.Mvc.OkObjectResult returnedValue = (Microsoft.AspNetCore.Mvc.OkObjectResult)controller.Withdraw(1, 10);

            Assert.AreEqual(6m, ((Account)returnedValue.Value).Balance);                     
        }

        [Test]
        public void Deposit_Deposits10DollarsAtAnAccountWith20DollarsBalance_NewBalanceShouldBe29DollarsAnd90Cents()
        {        
            // Mocks the reader to returns a bank account with 20 bucks
            dataReaderMock.Setup(reader => reader.GetAccountById(It.IsAny<int>())).Returns(new Account(1, "Pedro", 20));

            // Then deposits 10 bucks
            // Since the tax for this kind of operation is 1% of the deposited amount, 
            // the returned account must have balance equals to 29,9 dollars
            Microsoft.AspNetCore.Mvc.OkObjectResult returnedValue = (Microsoft.AspNetCore.Mvc.OkObjectResult)controller.Deposit(1, 10);

            Assert.AreEqual(29.9m, ((Account)returnedValue.Value).Balance);          
        }


        [Test]
        public void Transference_Transfers10DollarsFromAccountWith20DollarsToAccountWith10Dollars_SourceAccountShouldHave9DollarsAndDestinationAccountShouldHave20Dollars()
        {
            // Mocks the reader to returns accounts according to the received ID
            dataReaderMock.Setup(reader => reader.GetAccountById(It.IsAny<int>())).Returns((int accId) => 
                                                                                                { 
                                                                                                    if(accId == 1 )
                                                                                                        return new Account(1, "Pedro", 20);
                                                                                                    else
                                                                                                        return new Account(2, "Joao", 10); 
                                                                                                 });

            // Then transfers 10 bucks from an account with 20 dollars balance
            // to an account with 10 dollars balance.
            // Since the tax for this kind of operation is 1 dollar for the source account, 
            // the returned source account must have 9 dollars and the destination account must have 20 dollars
            Microsoft.AspNetCore.Mvc.OkObjectResult returnedValue = (Microsoft.AspNetCore.Mvc.OkObjectResult)controller.Transfer(1, 2, 10);
            List<Account> accounts = (List<Account>)returnedValue.Value;

            Assert.AreEqual(9m, accounts[0].Balance);
            Assert.AreEqual(20m, accounts[1].Balance);
        }

        [Test]
        public void BankStatement_CreatesAccountWith10DollarsAndWithdraws20_BankStatementShouldBeEmpty()
        {
            Account acc = new Account(1, "Pedro", 10);
            List<SingleAccountOperation> opList = new List<SingleAccountOperation>();

            // Mocks the reader to returns a bank account with 10 bucks
            dataReaderMock.Setup(reader => reader.GetAccountById(It.IsAny<int>())).Returns(acc);
            // Mocks the persistor to adds the operations to opList
            singleAccPersistorMock.Setup(persistor => persistor.Persist(It.IsAny<SingleAccountOperation>())).Returns((SingleAccountOperation op) => { opList.Add(op); return true; });

            // Then withdraws 20 bucks
            try
            {
                controller.Withdraw(1, 20);
            }
            catch (System.Exception) { }

            // Mocks the reader to return the opList on GetOperationsByAccountId
            dataReaderMock.Setup(reader => reader.GetOperationsByAccountId(It.IsAny<int>())).Returns( opList );

            // Now requests for bank statement and compares it with an empty bank statement
            // Since the withdraw was not executed, the returned bank statement should have no operations
            Microsoft.AspNetCore.Mvc.OkObjectResult returnedValue = (Microsoft.AspNetCore.Mvc.OkObjectResult)controller.GetBankStatement(1);
            string bankStatement = (string)returnedValue.Value;

            BankAccountWebAPI.BankStatement dummyStatement = new BankAccountWebAPI.BankStatement(acc, new List<SingleAccountOperation>());
            Assert.AreEqual(dummyStatement.ToString(), bankStatement);
        }

        [Test]
        public void BankStatement_CreatesAccountWith10DollarsAndTransfers20_BankStatementShouldBeEmpty()
        {
            Account acc = new Account(1, "Pedro", 10);
            List<SingleAccountOperation> opList = new List<SingleAccountOperation>();

            // Mocks the reader to returns a bank account with 10 bucks
            dataReaderMock.Setup(reader => reader.GetAccountById(It.IsAny<int>())).Returns(acc);
            // Mocks the persistor to adds the operations to opList
            singleAccPersistorMock.Setup(persistor => persistor.Persist(It.IsAny<SingleAccountOperation>())).Returns((SingleAccountOperation op) => { opList.Add(op); return true; });

            // Then transfers 20 bucks
            try
            {
                controller.Transfer(1, 2, 20);
            }
            catch (System.Exception) { }

            // Mocks the reader to return the opList on GetOperationsByAccountId
            dataReaderMock.Setup(reader => reader.GetOperationsByAccountId(It.IsAny<int>())).Returns(opList);

            // Now requests for bank statement and compares it with an empty bank statement
            // Since the transference was not executed, the returned bank statement should have no operations
            Microsoft.AspNetCore.Mvc.OkObjectResult returnedValue = (Microsoft.AspNetCore.Mvc.OkObjectResult)controller.GetBankStatement(1);
            string bankStatement = (string)returnedValue.Value;

            BankAccountWebAPI.BankStatement dummyStatement = new BankAccountWebAPI.BankStatement(acc, new List<SingleAccountOperation>());
            Assert.AreEqual(dummyStatement.ToString(), bankStatement);
        }

        [Test]
        public void BankStatement_CreatesAccountWith10DollarsAndDeposits10AndWithdraws5_BankStatementShouldHaveOneDepositAndOneWithdraw()
        {
            Account acc = new Account(1, "Pedro", 10);
            List<SingleAccountOperation> opList = new List<SingleAccountOperation>();

             // Mocks the reader to returns a bank account with 10 bucks
            dataReaderMock.Setup(reader => reader.GetAccountById(It.IsAny<int>())).Returns(acc);
            // Mocks the persistor to adds the operations to opList and updates the account!
            singleAccPersistorMock.Setup(persistor => persistor.Persist(It.IsAny<SingleAccountOperation>())).Returns((SingleAccountOperation op) => { opList.Add(op); return true; });

            controller.Deposit(1, 20);
            controller.Withdraw(1, 5);

            // Mocks the reader to return the opList on GetOperationsByAccountId
            dataReaderMock.Setup(reader => reader.GetOperationsByAccountId(It.IsAny<int>())).Returns(opList);

            // Now requests for bank statement 
            Microsoft.AspNetCore.Mvc.OkObjectResult returnedValue = (Microsoft.AspNetCore.Mvc.OkObjectResult)controller.GetBankStatement(1);
            string bankStatement = (string)returnedValue.Value;

            BankAccountWebAPI.BankStatement dummyStatement;

            #region Creates dummy bank statement
            DepositTaxApplier depositTaxApplier = new DepositTaxApplier();
            SingleAccountOperation depositOp = new SingleAccountOperation(acc);
            depositOp.OperationType = OperationType.DEPOSIT;
            // Unfortunately I can't create a dummy timestamp. I should have used an IGetTime interface!
            depositOp.ExecutionTimeStamp = opList[0].ExecutionTimeStamp;
            depositOp.Amount = 20;
            depositTaxApplier.Apply(depositOp);

            WithdrawTaxApplier withdrawTaxApplier = new WithdrawTaxApplier();
            SingleAccountOperation withdrawOp = new SingleAccountOperation(acc);
            withdrawOp.OperationType = OperationType.WIDTHDRAW;
            // Unfortunately I can't create a dummy timestamp. I should have used an IGetTime interface!
            withdrawOp.ExecutionTimeStamp = opList[1].ExecutionTimeStamp;
            withdrawOp.Amount = 5;
            withdrawTaxApplier.Apply(withdrawOp);

            decimal balance = 10m;
            balance += depositOp.Amount - depositOp.TaxAmount;
            balance += -(withdrawOp.Amount + withdrawOp.TaxAmount);
            Account dummyAccount = new Account(acc.AccountId, acc.OwnerName, balance);

            dummyStatement = new BankAccountWebAPI.BankStatement(dummyAccount, new List<SingleAccountOperation>() { depositOp, withdrawOp });
            #endregion
      
            string dummyStatementStr = dummyStatement.ToString();
            Assert.AreEqual(dummyStatementStr, bankStatement);
        }


        [Test]
        public void BankStatement_Transfers10DollarsFromA30DollarsAccountToA20DollarsAccount_StatementOfBothAccountsShouldHaveOneTransference()
        {
            Account srcAcc = new Account(1, "Pedro", 30);
            Account destAcc = new Account(2, "Joao", 20);
            List<DoubleAccountOperation> opList = new List<DoubleAccountOperation>();

            // Mocks the reader to returns accounts according to the received ID
            dataReaderMock.Setup(reader => reader.GetAccountById(It.IsAny<int>())).Returns((int accId) =>
                                                                                    {
                                                                                        if (accId == srcAcc.AccountId)
                                                                                            return srcAcc;
                                                                                        else
                                                                                            return destAcc;
                                                                                    });
            // Mocks the persistor to adds the operations to opList and updates the account!
            doubleAccPersistorMock.Setup(persistor => persistor.Persist(It.IsAny<DoubleAccountOperation>())).Returns((DoubleAccountOperation op) => { opList.Add(op); return true; });

            controller.Transfer(1, 2, 10);

            // Mocks the reader to return the opList on GetOperationsByAccountId
            dataReaderMock.Setup(reader => reader.GetOperationsByAccountId(It.IsAny<int>())).Returns(opList.Cast<SingleAccountOperation>().ToList());

            // Now requests for bank statement 
            Microsoft.AspNetCore.Mvc.OkObjectResult returnedValue = (Microsoft.AspNetCore.Mvc.OkObjectResult)controller.GetBankStatement(srcAcc.AccountId);
            string srcBankStatement = (string)returnedValue.Value;
            returnedValue = (Microsoft.AspNetCore.Mvc.OkObjectResult)controller.GetBankStatement(destAcc.AccountId);
            string destBankStatement = (string)returnedValue.Value;

            BankAccountWebAPI.BankStatement srcDummyStatement, destDummyStatement;

            #region Creates dummy bank statement
            TransferenceTaxApplier transferenceTaxApplier = new TransferenceTaxApplier();
            DoubleAccountOperation transferenceOp = new DoubleAccountOperation(srcAcc, destAcc);
            transferenceOp.OperationType = OperationType.TRANSFERENCE;
            // Unfortunately I can't create a dummy timestamp. I should have used an IGetTime interface!
            transferenceOp.ExecutionTimeStamp = opList[0].ExecutionTimeStamp;
            transferenceOp.Amount = 10;
            transferenceTaxApplier.Apply(transferenceOp);         

            decimal srcBalance = 30m;
            srcBalance -= transferenceOp.Amount + transferenceOp.TaxAmount;      
            Account srcDummyAccount = new Account(srcAcc.AccountId, srcAcc.OwnerName, srcBalance);

            srcDummyStatement = new BankAccountWebAPI.BankStatement(srcDummyAccount, new List<SingleAccountOperation>() { (SingleAccountOperation)transferenceOp });

            decimal destBalance = 20m;
            destBalance += (transferenceOp.Amount - transferenceOp.ExtraTaxAmount);
            Account destDummyAccount = new Account(destAcc.AccountId, destAcc.OwnerName, destBalance);

            destDummyStatement = new BankAccountWebAPI.BankStatement(destDummyAccount, new List<SingleAccountOperation>() { (SingleAccountOperation)transferenceOp });
            #endregion

            Assert.AreEqual(srcDummyStatement.ToString(), srcBankStatement);
            Assert.AreEqual(destDummyStatement.ToString(), destBankStatement);
        }
    
    }
}