using NUnit.Framework;
using BankAccountWebAPI;
using Moq;

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
            catch(System.Exception)
            {
                Assert.Fail();
            }

            Assert.Fail();
        }

        [Test]
        public void Deposit_InvalidAmount_ThrowsOutOfRangeException()
        {
            try
            {
                controller.Deposit(1,-1m);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                Assert.Pass();
            }
            catch (System.Exception)
            {
                Assert.Fail();
            }

            Assert.Fail();
        }

        [Test]
        public void Withdraw_InvalidAmount_ThrowsOutOfRangeException()
        {
            try
            {
                controller.Withdraw(1, -1m);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                Assert.Pass();
            }
            catch (System.Exception)
            {
                Assert.Fail();
            }

            Assert.Fail();
        }

        [Test]
        public void Transference_InvalidAmount_ThrowsOutOfRangeException()
        {
            try
            {
                controller.Transfer(1, 2, -1m);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                Assert.Pass();
            }
            catch (System.Exception)
            {
                Assert.Fail();
            }

            Assert.Fail();
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
            catch (System.Exception)
            {
                Assert.Fail();
            }

            Assert.Fail();
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
            catch (System.Exception)
            {
                Assert.Fail();
            }

            Assert.Fail();
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
            catch (System.Exception)
            {
                Assert.Fail();
            }

            Assert.Fail();
        }
    
    }
}