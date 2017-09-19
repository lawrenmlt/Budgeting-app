using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Budgeting.Dto;
using Budgeting.Data;

namespace Budgeting.Service
{
    public class LookupService : BaseService
    {
        public List<LookupDto> GetBudgetPlanList()
        {
            List<LookupDto> list = new List<LookupDto>();
            using (BudgetingEntities db = new BudgetingEntities())
            {
                list = (from b in db.BudgetPlans
                        where b.UserId == UserId
                        select new LookupDto
                        {
                            Id = b.BudgetPlanId,
                            Name = b.Name
                        })
                        .ToList();
            }
            return list;
        }

        public List<LookupDto> GetCategoryList(int budgetPlanId, bool GetOnlyUnused = true)
        {
            List<LookupDto> list = new List<LookupDto>();
            using (BudgetingEntities db = new BudgetingEntities())
            {
                var categoriesInBudgetPlan = (from bc in db.BudgetPlanCategories
                                             where bc.BudgetPlanId == budgetPlanId
                                             select bc)
                                             .ToList();
                var allUserCategories = (from c in db.Categories
                                        where c.UserId == UserId
                                        select new LookupDto
                                        {
                                            Id = c.CategoryId,
                                            Name = c.Name
                                        })
                                        .ToList();
                foreach (LookupDto l in allUserCategories)
                {
                    // get all user categories that are not already a part of the budget plan
                    if (GetOnlyUnused)
                    {
                        if (!categoriesInBudgetPlan.Where(x => x.CategoryId == l.Id).Any())
                            list.Add(l);
                    }
                    else
                    {
                        list.Add(l);
                    }
                }
                list = list.OrderBy(x => x.Name).ToList();
            }
            return list;
        }
    }
}
