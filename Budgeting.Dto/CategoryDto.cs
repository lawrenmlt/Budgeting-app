using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budgeting.Dto
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public decimal CategoryTotalSpentDollar { get; set; }
        public decimal CategoryTotalSpentPercent { get; set; }
        public decimal CategoryBudgetDollar { get; set; }
        public decimal CategoryBudgetPercent { get; set; }
        public List<ActivityDto> Activities { get; set; }
        public bool UsePercent { get; set; }

        public List<LookupDto> CategoryOptions { get; set; }
    }
}
