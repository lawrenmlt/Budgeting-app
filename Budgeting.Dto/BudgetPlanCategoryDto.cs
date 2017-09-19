using Budgeting.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budgeting.Dto
{
    public class BudgetPlanCategoryDto
    {
        // comment test
        public int BudgetPlanCategoryId { get; set; }
        public int BudgetPlanId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool UsePercent { get; set; }

        public string AllocatedAmount { get; set; }
        public string AllocatedPercentage { get; set; }

        public string SpentAmount { get; set; }
        public string SpentPercentage { get; set; }

        public string RemainingAmount { get; set; }
        public string RemainingPercentage { get; set; }

        public CategorySpendingStatus SpendingStatus { get; set; }

        public List<LookupDto> CategoryOptions { get; set; }
        public List<ActivityDto> Activities { get; set; }
    }
}
