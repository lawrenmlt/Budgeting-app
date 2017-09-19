using Budgeting.Common;
using Budgeting.Data;
using Budgeting.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budgeting.Service
{
    public class SummaryService : BaseService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateOfInterest"></param>
        /// <returns></returns>
        public List<BudgetPlanCategoryDto> GetMonthActivitiesByCategory(DateTime dateOfInterest)
        {
            DateTime firstDayOfMonth = new DateTime(dateOfInterest.Year, dateOfInterest.Month, 1);
            BudgetPlanningService s = new BudgetPlanningService();
            List<BudgetPlanCategoryDto> list = new List<BudgetPlanCategoryDto>();
            using (BudgetingEntities db = new BudgetingEntities())
            {
                int budgetPlanId = s.GetSelectedBudgetPlan();

                // initialize list
                list = s.GetBudgetCategoryList(budgetPlanId);
                foreach (BudgetPlanCategoryDto temp in list)
                {
                    temp.Activities = new List<ActivityDto>();
                }
                list.Add(new BudgetPlanCategoryDto {
                    CategoryName = "Spending outside of budget",
                    Activities = new List<ActivityDto>()
                });

                // get all activities from this month
                List<ActivityDto> thisMonthsActivities = GetThisMonthsActivities(dateOfInterest);

                // sort this month's activities into the correct categories
                foreach (ActivityDto dto in thisMonthsActivities)
                {
                    if (dto.Pretax)
                        dto.Description = dto.Description + " (Pre-tax)";
                    BudgetPlanCategoryDto b = list.Find(x => x.CategoryId == dto.CategoryId);
                    if (b == null)
                    {
                        b = list.Find(x => x.CategoryId == 0);
                        if (b == null)
                            throw new Exception("Unable to add spending activity outside of budget plan.");
                    }
                    b.Activities.Add(dto);
                }

                // calculate spend information for each category
                foreach (BudgetPlanCategoryDto c in list)
                {
                    CalculateSpentAndRemaining(c);

                    // sort activities
                    c.Activities = c.Activities.OrderByDescending(x => x.DateOfActivity.Day).ToList();
                }

                // list sort
                list = list.OrderByDescending(x => x.SpendingStatus).ToList();

            }
            return list;
        }

        private List<ActivityDto> GetThisMonthsActivities(DateTime dateOfInterest)
        {
            List<ActivityDto> thisMonthsActivities;
            DateTime firstDayOfMonth = new DateTime(dateOfInterest.Year, dateOfInterest.Month, 1);
            using (BudgetingEntities db = new BudgetingEntities())
            {
                thisMonthsActivities = (from a in db.Activities
                                        join c in db.Categories on a.CategoryId equals c.CategoryId
                                        where (a.DateOfActivity >= firstDayOfMonth
                                                 || (a.Recurring == true
                                                     && a.RecurringDay <= dateOfInterest.Day
                                                     && a.DateOfActivity <= dateOfInterest))
                                             && a.Active == true
                                             && c.UserId == UserId
                                        select new ActivityDto
                                        {
                                            ActivityId = a.ActivityId,
                                            CategoryId = a.CategoryId,
                                            CategoryName = c.Name,
                                            Amount = a.Amount,
                                            Description = a.Description,
                                            Recurring = a.Recurring ?? false,
                                            RecurringDay = a.RecurringDay ?? 1,
                                            DateOfActivity = a.DateOfActivity,
                                            Pretax = a.Pretax ?? false,
                                            Expenditure = a.Expenditure
                                        })
                                        .ToList();
            }
            return thisMonthsActivities;
        }

        public UserDto GetUserOverallSummary()
        {
            using (BudgetingEntities db = new BudgetingEntities())
            {
                UserDto dto;

                // get base user information
                var dtos = (from u in db.Users
                               where u.UserId == UserId
                               select new UserDto
                               {
                                   UserId = u.UserId,
                                   BaseSalary = u.BaseSalary ?? 0,
                                   ExpectedTaxPercentage = u.ExpectedTaxPerc ?? 0
                               })
                               .ToList();
                if (!dtos.Any())
                {
                    throw new Exception("Cannot find User with UserId = " + UserId);
                }
                if (dtos.Count > 1)
                {
                    // TODO database shenanighans
                }
                dto = dtos.First();
                dto.BaseSalary = dto.BaseSalary / 12;

                // get activity information
                var activities = GetThisMonthsActivities(DateTime.Now);
                var pretaxActivities = activities
                                        .Where(x => x.Pretax == true)
                                        .Select(x => x.Amount)
                                        .ToList();
                decimal temp = 0;
                foreach (int amount in pretaxActivities)
                {
                    temp += amount;
                }
                dto.TotalPretaxSpending = temp;
                decimal salaryAfterPretaxSpending = dto.BaseSalary - dto.TotalPretaxSpending;
                dto.ExpectedTaxAmount = salaryAfterPretaxSpending * (dto.ExpectedTaxPercentage / 100);
                dto.SalaryAfterTax = dto.BaseSalary - dto.TotalPretaxSpending - dto.ExpectedTaxAmount;
                var totalExpenditures = activities
                                          .Where(x => x.Expenditure == true)
                                          .Select(x => x.Amount)
                                          .ToList();
                temp = 0;
                foreach (int amount in totalExpenditures)
                {
                    temp += amount;
                }
                dto.CurrentMonthSpendingTotal = temp;
                var rev = activities
                          .Where(x => x.Pretax == false && x.Expenditure == false)
                          .Select(x => x.Amount)
                          .ToList();
                temp = 0;
                foreach(int amount in rev)
                {
                    temp += amount;
                }
                dto.OtherRevenue = temp;

                // get budget information
                var budgetCategoryAllocation = (from b in db.BudgetPlanCategories
                                                join p in db.BudgetPlans on b.BudgetPlanId equals p.BudgetPlanId
                                                join c in db.Categories on b.CategoryId equals c.CategoryId
                                                where c.UserId == UserId && p.Selected == true
                                                select new
                                                {
                                                    AllocatedAmount = b.AllocatedAmount,
                                                    AllocatedPercentage = b.AllocatedPercentage,
                                                    UsePercent = b.UsePercent
                                                }
                                                )
                                                .ToList();
                temp = 0;
                foreach (var bca in budgetCategoryAllocation)
                {
                    if (bca.UsePercent)
                    {
                        // budgeted amounts do NOT take pretax spending into account
                        temp += ((bca.AllocatedPercentage ?? 0) / 100) * (dto.BaseSalary - dto.ExpectedTaxAmount); 
                    }
                    else
                    {
                        temp += bca.AllocatedAmount ?? 0;
                    }
                }
                dto.CurrentMonthBudgetedAmount = temp;
                if (dto.CurrentMonthBudgetedAmount > 0)
                {
                    dto.CurrentMonthSpendingPercentage = (dto.CurrentMonthSpendingTotal / dto.CurrentMonthBudgetedAmount) * 100;
                    dto.CurrentMonthRemainingAmount = dto.CurrentMonthBudgetedAmount - dto.CurrentMonthSpendingTotal + dto.OtherRevenue;
                }
                return RoundToTwoDecimals(dto);
            }
        }

        private UserDto RoundToTwoDecimals(UserDto u)
        {
            u.BaseSalary = Decimal.Round(u.BaseSalary, 2);
            u.ExpectedTaxPercentage = Decimal.Round(u.ExpectedTaxPercentage, 2);
            u.ExpectedTaxAmount = Decimal.Round(u.ExpectedTaxAmount, 2);
            u.TotalPretaxSpending = Decimal.Round(u.TotalPretaxSpending, 2);
            u.CurrentMonthSpendingTotal = Decimal.Round(u.CurrentMonthSpendingTotal, 2);
            u.CurrentMonthBudgetedAmount = Decimal.Round(u.CurrentMonthBudgetedAmount, 2);
            u.CurrentMonthSpendingPercentage = Decimal.Round(u.CurrentMonthSpendingPercentage, 2);
            u.CurrentMonthRemainingAmount = Decimal.Round(u.CurrentMonthRemainingAmount, 2);
            u.SalaryAfterTax = Decimal.Round(u.SalaryAfterTax, 2);
            u.OtherRevenue = decimal.Round(u.OtherRevenue, 2);
            return u;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        private void CalculateSpentAndRemaining(BudgetPlanCategoryDto c)
        {
            decimal runningSpentSum = 0;
            foreach (ActivityDto act in c.Activities)
            {
                //act.DateOfActivity = GetDateOfActivity(act, dateOfInterest);
                runningSpentSum += (act.Amount ?? 0);
            }
            c.SpentAmount = runningSpentSum.ToString();
            c.SpendingStatus = CategorySpendingStatus.Neutral;
            decimal allocatedAmount;
            if (Decimal.TryParse(c.AllocatedAmount, out allocatedAmount))
            {
                if (allocatedAmount > 0)
                {
                    decimal percentage = (runningSpentSum / allocatedAmount) * 100;
                    percentage = Decimal.Round(percentage, 2);
                    c.SpentPercentage = percentage.ToString();
                    if (percentage < 95)
                        c.SpendingStatus = CategorySpendingStatus.Green;
                    else if (percentage > 100)
                        c.SpendingStatus = CategorySpendingStatus.Red;
                    else
                        c.SpendingStatus = CategorySpendingStatus.Yellow;

                    decimal remainingAmount = allocatedAmount - runningSpentSum;
                    c.RemainingAmount = Decimal.Round(remainingAmount, 2).ToString();
                    c.RemainingPercentage = Decimal.Round(((remainingAmount / allocatedAmount) * 100), 2).ToString();
                }
            }
            //return c;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="budgetPlanCategoryId"></param>
        /// <param name="dateOfInterest"></param>
        /// <returns></returns>
        public BudgetPlanCategoryDto GetMonthCategoryActivities(int budgetPlanCategoryId, DateTime dateOfInterest)
        {
            BudgetPlanCategoryDto dto = new BudgetPlanCategoryDto();
            DateTime firstDayOfMonth = new DateTime(dateOfInterest.Year, dateOfInterest.Month, 1);

            var summary = GetMonthActivitiesByCategory(DateTime.Now);
            dto = summary.Where(x => x.BudgetPlanCategoryId == budgetPlanCategoryId).Single();
            return dto;
            //if (budgetPlanCategoryId == 0)
            //{
            //}
            //else
            //{
            //    dto.BudgetPlanCategoryId = budgetPlanCategoryId;
            //    using (BudgetingEntities db = new BudgetingEntities())
            //    {
            //        var searchingForBPC = (from bpc in db.BudgetPlanCategories
            //                               join cat in db.Categories on bpc.CategoryId equals cat.CategoryId
            //                               where bpc.BudgetPlanCategoryId == budgetPlanCategoryId
            //                               select new BudgetPlanCategoryDto
            //                               {
            //                                   BudgetPlanId = bpc.BudgetPlanId,
            //                                   BudgetPlanCategoryId = bpc.BudgetPlanCategoryId,
            //                                   CategoryId = cat.CategoryId,
            //                                   CategoryName = cat.Name,
            //                                   AllocatedAmount = (bpc.AllocatedAmount ?? 0).ToString(),
            //                                   AllocatedPercentage = (bpc.AllocatedPercentage ?? 0).ToString(),
            //                                   UsePercent = bpc.UsePercent
            //                               })
            //                               .ToList();
            //        if (!searchingForBPC.Any())
            //        {
            //            throw new Exception("Unable to find the budget plan category to delete from.");
            //        }
            //        if (searchingForBPC.Count > 1)
            //        {
            //            // TODO database shenanigans
            //        }
            //        dto = searchingForBPC.First();
            //        dto.Activities = (from a in db.Activities
            //                          where a.CategoryId == dto.CategoryId
            //                                && (a.DateOfActivity >= firstDayOfMonth
            //                                    || (a.Recurring == true
            //                                        && a.RecurringDay <= dateOfInterest.Day
            //                                        && a.DateOfActivity <= dateOfInterest))
            //                                && a.Active == true
            //                          select new ActivityDto
            //                          {
            //                              ActivityId = a.ActivityId,
            //                              CategoryId = a.CategoryId,
            //                              Amount = a.Amount,
            //                              Description = a.Description,
            //                              Recurring = a.Recurring ?? false,
            //                              RecurringDay = a.RecurringDay ?? 1,
            //                              DateOfActivity = a.DateOfActivity
            //                          })
            //                          .ToList();
            //    }
            //    dto.Activities = dto.Activities.OrderBy(x => x.DateOfActivity.Day).ToList();
            //}
            //CalculateSpentAndRemaining(dto);
            //return dto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        private DateTime GetDateOfActivity(ActivityDto a, DateTime d)
        {
            if (a.Recurring == true)
            {
                return new DateTime(d.Year, d.Month, a.RecurringDay);
            }
            else
            {
                return a.DateOfActivity;
            }
        }
        
    }
}
