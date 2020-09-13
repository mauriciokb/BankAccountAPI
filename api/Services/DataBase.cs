using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BankAccountWebAPI
{    
    /// <summary>
    /// Class responsible for persisting SingleAccOperation, DoubleAccOperation 
    /// and BankAccount objects to database.
    /// It uses EF with SQLite
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DatabasePersistence : IPersist<object> 
    {
        /// <summary>
        /// EF Context
        /// </summary>
        private Context context;

        public DatabasePersistence(Context context)
        {
            this.context = context;
        }

        public bool Persist(object data)
        {            
            context.Add(data);

            // The SaveChanges method ensures that not only the operation (single or double acc op) is saved, 
            // but also the bank account, since it is encapsulated by the former
            int res = context.SaveChanges();

            if(res < 1)
                return false;
            else 
                return true;
        }

    }

    /// <summary>
    /// Class responsible for reading info from the database.
    /// It uses EF with SQLite.
    /// </summary>
    public class DatabaseReader : IReadData
    {
        /// <summary>
        /// EF Context
        /// </summary>
        private Context context;

        public DatabaseReader(Context context)
        {
            this.context = context;
        }
        
        public bool GetAccountById(int id, out Account acc)
        {
            acc = null;
            
            var result = context.BankAccounts.Where(a => a.AccountId == id);

            if(result.Count() < 1)
            {
                return false;
            }        
            
            acc = result.First();

            return true;
        }

        public bool GetOperationsByAccountId(int id, out List<SingleAccountOperation> opList)
        {
            opList = new List<SingleAccountOperation>();
            
            // Selects all single acc operations associated with the given acc ID
            var singleAccOperations = context.SingleAccOperations.Include(a => a.PrimaryAcc).Where(a => a.PrimaryAccId == id &&  !(a is DoubleAccountOperation));
            // Selects all double acc operations associated with the given acc ID
            var doubleAccOperations = context.DoubleAccOperations.Include(a => a.PrimaryAcc).Include(a => a.SecondaryAcc).Where(a => a.PrimaryAccId == id || a.SecondaryAccId == id);
            
            List<int> teste = new List<int>();
            
            // Adds the single acc operations to the output list
            opList.AddRange(singleAccOperations);
            // Adds the double acc operations to the output list (ps. double inherits from single)
            opList.AddRange(doubleAccOperations);
   
            return true;        
        }
    }
}