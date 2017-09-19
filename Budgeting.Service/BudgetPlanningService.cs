using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Budgeting.Dto;
using Budgeting.Data;

namespace Budgeting.Service
{
    public class BudgetPlanningService : BaseService
    {
        public BudgetPlanDto FillBudgetPlanDto(int budgetPlanId)
        {
            BudgetPlanDto dto = new BudgetPlanDto();
            dto.BudgetPlanId = budgetPlanId;
            dto.Description = null;
            if (budgetPlanId != 0)
            {
                dto.Categories = GetBudgetCategoryList(budgetPlanId);
                using (BudgetingEntities db = new BudgetingEntities())
                {
                    dto.Description = (from bp in db.BudgetPlans
                                        where bp.BudgetPlanId == budgetPlanId
                                        select bp.Name)
                                        .Single();
                }
            }
            return dto;
        }

        public int GetSelectedBudgetPlan()
        {
            int budgetPlanId;
            using (BudgetingEntities db = new BudgetingEntities())
            {
                try
                {
                    budgetPlanId = (from b in db.BudgetPlans
                                    where b.UserId == UserId
                                        && b.Selected == true
                                    select b.BudgetPlanId)
                                        .Single();
                }
                catch (Exception)
                {
                    budgetPlanId = 0;
                }
            }
            return budgetPlanId;
        }

        public List<BudgetPlanCategoryDto> GetBudgetCategoryList(int budgetPlanId)
        {
            List<BudgetPlanCategoryDto> list = new List<BudgetPlanCategoryDto>();
            using (BudgetingEntities db = new BudgetingEntities())
            {
                var cats = (from bpc in db.BudgetPlanCategories
                                  join cat in db.Categories on bpc.CategoryId equals cat.CategoryId
                                  join u in db.Users on cat.UserId equals u.UserId
                                  where bpc.BudgetPlanId == budgetPlanId
                                  select new 
                                  {
                                      BudgetPlanId = budgetPlanId,
                                      Tax = u.ExpectedTaxPerc,
                                      BaseSalary = u.BaseSalary/12,
                                      BudgetPlanCategoryId = bpc.BudgetPlanCategoryId,
                                      CategoryId = cat.CategoryId,
                                      CategoryName = cat.Name,
                                      AllocatedAmount = bpc.AllocatedAmount,
                                      AllocatedPercentage = bpc.AllocatedPercentage,
                                      UsePercent = bpc.UsePercent
                                  })
                                  .ToList();
                if (cats.Any())
                {
                    decimal salaryAfterTaxes = (cats[0].BaseSalary ?? 0) * (100 - (cats[0].Tax ?? 0)) / 100;
                    foreach (var cat in cats)
                    {
                        decimal? amount = cat.AllocatedAmount;
                        decimal? perc = cat.AllocatedPercentage;
                        if (cat.UsePercent)
                        {
                            if (cat.BaseSalary == 0)
                                amount = null;
                            else
                                amount = ((cat.AllocatedPercentage ?? 0) / 100) * salaryAfterTaxes;
                        }
                        else
                        {
                            if (cat.BaseSalary == 0)
                                perc = null;
                            else
                                perc = ((cat.AllocatedAmount ?? 0) / salaryAfterTaxes) * 100;
                        }
                        list.Add(new BudgetPlanCategoryDto
                        {
                            BudgetPlanId = cat.BudgetPlanId,
                            BudgetPlanCategoryId = cat.BudgetPlanCategoryId,
                            CategoryId = cat.CategoryId,
                            CategoryName = cat.CategoryName,
                            AllocatedAmount = Decimal.Round(amount ?? 0, 2).ToString(),
                            AllocatedPercentage = Decimal.Round(perc ?? 0, 2).ToString(),
                            UsePercent = cat.UsePercent
                        });
                    }
                }
            }
            list = list.OrderBy(x => x.CategoryName).ToList();
            return list;
        }

        public int CreateNewBudgetPlan(string name)
        {
            BudgetPlan bp = new BudgetPlan();
            bp.Name = name;
            bp.UserId = UserId;
            using (BudgetingEntities db = new BudgetingEntities())
            {
                db.BudgetPlans.Add(bp);
                db.Entry(bp).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
            }
            return bp.BudgetPlanId;
        }

        public int AddCategoryToBudgetPlan(BudgetPlanCategoryDto dto)
        {
            BudgetPlanCategory bc = new BudgetPlanCategory();
            using (BudgetingEntities db = new BudgetingEntities())
            {
                //if (dto.CategoryId == 0)
                //{
                //    // add to category table
                //    Category c = new Category();
                //    c.UserId = UserId;
                //    c.Name = dto.Name;
                //    db.Categories.Add(c);
                //    db.Entry(c).State = System.Data.Entity.EntityState.Added;
                //    db.SaveChanges();
                //    dto.CategoryId = c.CategoryId;
                //}

                // add to budget table
                //decimal baseSalary = (from u in db.Users
                //                      where u.UserId == UserId
                //                      select u.BaseSalary)
                //                      .Single() ?? 0;
                //baseSalary = baseSalary / 12; // get month salary
                bc.CategoryId = dto.CategoryId;
                bc.BudgetPlanId = dto.BudgetPlanId;
                bc.UsePercent = dto.UsePercent;
                decimal tempVal;
                if (dto.UsePercent)
                {
                    if (Decimal.TryParse(dto.AllocatedPercentage, out tempVal))
                    {
                        bc.AllocatedPercentage = tempVal;
                        //bc.AllocatedAmount = (baseSalary == 0 ? null : (bc.AllocatedPercentage * baseSalary) / 100);
                    }
                }
                else
                {
                    if (Decimal.TryParse(dto.AllocatedAmount, out tempVal))
                    {
                        bc.AllocatedAmount = tempVal;
                        //bc.AllocatedPercentage = (baseSalary == 0 ? null : (bc.AllocatedAmount / baseSalary) * 100);
                    }
                }
                db.BudgetPlanCategories.Add(bc);
                db.Entry(bc).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
            }
            return bc.BudgetPlanCategoryId;
        }

        public void SaveBudgetPlanCategory(BudgetPlanCategoryDto dto)
        {
            using (BudgetingEntities db = new BudgetingEntities())
            {
                BudgetPlanCategory bpc = GetBudgetPlanCategory(dto.BudgetPlanCategoryId);
                if (!dto.UsePercent)
                {
                    bpc.AllocatedAmount = Decimal.Parse(dto.AllocatedAmount);
                }
                else
                {
                    bpc.AllocatedPercentage = Decimal.Parse(dto.AllocatedPercentage);
                }
                db.Entry(bpc).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        public int DeleteBudgetPlanCategory(int budgetPlanCategoryId)
        {
            int budgetPlanId = 0;
            using (BudgetingEntities db = new BudgetingEntities())
            {
                BudgetPlanCategory bpc = db.BudgetPlanCategories.Find(budgetPlanCategoryId);
                budgetPlanId = bpc.BudgetPlanId;
                db.BudgetPlanCategories.Remove(bpc);
                db.SaveChanges();
            }
            return budgetPlanId;
        }

        private BudgetPlanCategory GetBudgetPlanCategory(int budgetPlanCategoryId)
        {
            BudgetPlanCategory bpc;
            using (BudgetingEntities db = new BudgetingEntities())
            {
                bpc = (from b in db.BudgetPlanCategories
                     where b.BudgetPlanCategoryId == budgetPlanCategoryId
                     select b)
                     .Single();
            }
            return bpc;
        }
    }
}
