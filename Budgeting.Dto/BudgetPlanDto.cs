using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budgeting.Dto
{
    public class BudgetPlanDto
    {
        public int BudgetPlanId { get; set; }
        public string Description { get; set; }
        public List<BudgetPlanCategoryDto> Categories { get; set; }
        public List<LookupDto> CategoryOptions { get; set; }
        //public CategoryDto NewBudgetPlanCategory { get; set; }
    }
}
