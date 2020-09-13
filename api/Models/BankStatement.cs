using System.Collections.Generic;
using System.Linq;

namespace BankAccountWebAPI
{
    class BankStatement
    {
        const int FirstColPadding = 25;
        const int SecondColPadding = 20;

        private Account account;

        private List<SingleAccountOperation> operations;

        public BankStatement(Account account, List<SingleAccountOperation> operations)
        {
            this.account = account;
            this.operations = operations;
        }

        // Formats the bank statement in a user friendly way
        // Example:
        // 20/10/2020 12:00:00   DEPOSIT   +50
        //                       TAX       -0,5
        //                                 45,5
        public override string ToString()
        {           
            // Order the operations by its execution timestamp
            operations = operations.OrderBy(a => a.ExecutionTimeStamp).ToList();
  
            List<string> opStrList = new List<string>();

            foreach(var op in operations)
            {   
                string opString = string.Empty;
                
                string datetimeStr = op.ExecutionTimeStamp.ToString("dd/MM/yyyy HH:mm:ss").PadRight(FirstColPadding);
                string operationStr = op.OperationType.ToString().PadRight(SecondColPadding);
                string amountStr = op.Amount.ToString();

                decimal taxAmount = 0;

             
                if(op is DoubleAccountOperation doubleAccOp)
                {
                    string opSignal = (account.AccountId == doubleAccOp.PrimaryAcc.AccountId) ? "-" : "+";
                    string extraInfo = (account.AccountId == doubleAccOp.PrimaryAcc.AccountId) ? "To: " + doubleAccOp.SecondaryAcc.OwnerName : "From: " + doubleAccOp.PrimaryAcc.OwnerName; 
                    taxAmount = (account.AccountId == doubleAccOp.PrimaryAcc.AccountId) ? doubleAccOp.TaxAmount : doubleAccOp.ExtraTaxAmount;
                    
                    string opInfo = opString = string.Format("{0}{1}{2}", datetimeStr, operationStr, opSignal + amountStr);
                    string opExtraInfo = string.Format("{0}{1}", "".PadRight(FirstColPadding), extraInfo);

                    opString = string.Join(System.Environment.NewLine, opInfo, opExtraInfo);                   
                }
                else if(op is SingleAccountOperation singleAccOp)
                {
                    string opSignal = (singleAccOp.OperationType == OperationType.DEPOSIT) ? "+" : "-";
                    taxAmount = singleAccOp.TaxAmount;

                    opString = string.Format("{0}{1}{2}", datetimeStr, operationStr, opSignal + amountStr);
                }

                // Adds tax info (if necessary)
                string taxInfo = string.Format("{0}{1}{2}", "".PadRight(FirstColPadding), "Tax".PadRight(SecondColPadding), "-" + taxAmount.ToString());
                if(taxAmount > 0)
                {
                    opString = string.Join(System.Environment.NewLine, opString, taxInfo);
                }

                // Adds a blank line at the end of the operation
                opString = string.Join(System.Environment.NewLine, opString, "");
            
                opStrList.Add(opString);                
            }

            // Adds the final balance
            opStrList.Add(string.Format("{0,-40}{1}", "", account.Balance));

            return string.Join(System.Environment.NewLine, opStrList);
        }


    }
}