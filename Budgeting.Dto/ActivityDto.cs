using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budgeting.Dto
{
    public class ActivityDto
    {
        public int ActivityId { get; set; }

        [DisplayName("Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yy}")]
        public DateTime DateOfActivity { get; set; }

        public string Description { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter an amount.")]
        public decimal? Amount { get; set; }

        [DisplayName("Category")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select a category")]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public bool Recurring { get; set; }

        public int RecurringDay { get; set; }

        public bool Expenditure { get; set; }

        public List<LookupDto> CategoryOptions { get; set; }

        public bool Pretax { get; set; }
    }
}
