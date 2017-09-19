using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budgeting.Dto
{
    public class UserDto
    {
        public int UserId { get; set; }

        public decimal BaseSalary { get; set; }

        public decimal ExpectedTaxPercentage { get; set; }

        public decimal ExpectedTaxAmount { get; set; }
        

        public decimal SalaryAfterTax { get; set; }

        public decimal TotalPretaxSpending { get; set; }


        public decimal OtherRevenue { get; set; }

        public decimal CurrentMonthBudgetedAmount { get; set; }
        
        public decimal CurrentMonthSpendingTotal { get; set; }

        public decimal CurrentMonthSpendingPercentage { get; set; } // percentage of budgeted amount, not of month salary

        public decimal CurrentMonthRemainingAmount { get; set; }


        public decimal YearSpendingTotal { get; set; }

        public decimal YearSpendingPercentage { get; set; }

        public decimal YearRemainingAmount { get; set; }
    }
}
