using System.Linq;
using System.Collections.Generic;

namespace BankAccountWebAPI
{
    /// <summary>
    /// Class responsible for persisting single account operations on database
    /// </summary>
    public class EntitySingleAccOp : IPersist<SingleAccountOperation>
    {
        private Context context;
        public EntitySingleAccOp(Context context)
        {
            this.context = context;
        }

        public bool Persist(SingleAccountOperation op)
        {
            using (var transaction = context.Database.BeginTransaction())
            {  
                int res;
                                
                context.SingleAccOperations.Add(op);
                res = context.SaveChanges();

                if(res < 1)
                {
                    return false;
                }

                context.BankAccounts.Update(op.PrimaryBankAcc);
                res = context.SaveChanges();

                if(res < 1)
                    return false;
                
                transaction.Commit();
            }           

            return true;
        }
    }

    /// <summary>
    /// Class responsible for persisting double account operations on database
    /// </summary>
    public class EntityDoubleAccOp : IPersist<DoubleAccountOperation>
    {
        private Context context;

        public EntityDoubleAccOp(Context context)
        {
            this.context = context;
        }

        public bool Persist(DoubleAccountOperation op)
        {
            using (var transaction = context.Database.BeginTransaction())
            {  
                int res;
                                
                context.DoubleAccOperations.Add(op);
                res = context.SaveChanges();

                if(res < 1)
                {
                    return false;
                }

                context.BankAccounts.Update(op.PrimaryBankAcc);
                res = context.SaveChanges();

                if(res < 1)
                    return false;
                
                transaction.Commit();
            }
            

            return true;
        }
    }

    /// <summary>
    /// Class responsible for persisting and retrieving accounts from database
    /// </summary>
    public class EntityBankAccount : IPersist<BankAccount>, IRead<BankAccount>
    {
        private Context context;

        public EntityBankAccount(Context context)
        {
            this.context = context;
        }
        
        public bool Persist(BankAccount acc)
        {
            int res = 0;

            context.BankAccounts.Add(acc);
            res = context.SaveChanges();        

            return res > 0;
        }

        public bool Read(int id, out BankAccount acc)
        {
            acc = null;
            
            var result = context.BankAccounts.Where(a => a.BankAccountId == id);

            if(result.Count() < 1)
            {
                return false;
            }        
            
            acc = result.First();

            return true;
        }
    }

    public class EntityStatement : IRead<List<string>>
    {
        private Context context;

        public EntityStatement(Context context)
        {
            this.context = context;
        }

        public bool Read(int id, out List<string> opList)
        {
            opList = null;

            var singleAccOperations = context.SingleAccOperations.Where(a => a.PrimaryBankAccId == id);
            var doubleAccOperations = context.DoubleAccOperations.Where(a => a.PrimaryBankAccId == id || a.SecondaryBankAccId == id);
                        
            return true;        
        }
    }
}