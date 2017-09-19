using Budgeting.Dto;
using Budgeting.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Budgeting.Web.Controllers
{
    public class BudgetPlaningController : Controller
    {
        // GET: BudgetPlaning
        public ActionResult Index()
        {
            BudgetPlanningService s = new BudgetPlanningService();
            return View(s.GetSelectedBudgetPlan());
        }

        public ActionResult Create(string Name)
        {
            BudgetPlanningService s = new BudgetPlanningService();
            int newBudgetPlanId = s.CreateNewBudgetPlan(Name);
            return BudgetPlanCategoryListing(newBudgetPlanId);
        }

        public ActionResult BudgetPlanSelection()
        {
            LookupService s = new LookupService();
            List<LookupDto> list = s.GetBudgetPlanList();
            return PartialView(list);
        }

        public ActionResult BudgetPlanCategoryListing(int budgetPlanId, string ErrMessage = null)
        {
            BudgetPlanDto plan;
            try
            {
                BudgetPlanningService s = new BudgetPlanningService();
                plan = s.FillBudgetPlanDto(budgetPlanId);
                LookupService ls = new LookupService();
                plan.CategoryOptions = ls.GetCategoryList(budgetPlanId, false);
            }
            catch(Exception ex)
            {
                return PartialView(ex.Message);
            }
            if (ErrMessage != null)
                ViewBag.ErrMessage = ErrMessage;
            return PartialView("BudgetPlanCategoryListing", plan);
        }
        
        public ActionResult AddCategoryToBudgetPlan_Blank(int budgetPlanId)
        {
            BudgetPlanCategoryDto b = new BudgetPlanCategoryDto();
            LookupService s = new LookupService();
            b.CategoryOptions = s.GetCategoryList(budgetPlanId);
            return PartialView("AddCategoryToBudgetPlan", b);
        }

        public ActionResult AddCategoryToBudgetPlan(BudgetPlanCategoryDto bpc)
        //public ActionResult AddCategoryToBudgetPlan(int BudgetPlanId, int CategoryId, bool UsePercent, decimal? AllocatedAmount, decimal? AllocatedPercentage)
        {
            string errMessage = null;
            if (bpc.AllocatedAmount == null && bpc.AllocatedPercentage == null)
            {
                ModelState.AddModelError("Amount unset", "Either a dollar amount or a percentage must set for each budget category.");
            }
            if (ModelState.IsValid)
            {
                BudgetPlanningService s = new BudgetPlanningService();
                s.AddCategoryToBudgetPlan(bpc);
            }
            else
            {
                errMessage = "";
                foreach (var val in ModelState.Values)
                {
                    foreach (var err in val.Errors)
                    {
                        errMessage += "\n" + err.ErrorMessage;
                    }
                }
            }
            return BudgetPlanCategoryListing(bpc.BudgetPlanId, errMessage);
        }

        public ActionResult Delete(int budgetPlanCategoryId)
        {
            BudgetPlanningService s = new BudgetPlanningService();
            int budgetPlanId = s.DeleteBudgetPlanCategory(budgetPlanCategoryId);
            return BudgetPlanCategoryListing(budgetPlanId);
        }
        
        public ActionResult SaveAmount(int budgetPlanCategoryId, string allocatedAmount)
        {
            BudgetPlanningService s = new BudgetPlanningService();
            int budgetPlanId = s.GetSelectedBudgetPlan();
            allocatedAmount = allocatedAmount.TrimStart('$'); // get rid of the dollar signs
            decimal temp = 0;
            if (Decimal.TryParse(allocatedAmount, out temp))
            {
                BudgetPlanCategoryDto b = new BudgetPlanCategoryDto();
                b.BudgetPlanCategoryId = budgetPlanCategoryId;
                b.AllocatedAmount = allocatedAmount;
                b.UsePercent = false;
                s.SaveBudgetPlanCategory(b);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Allocated amount cannot be parsed as a decimal number. Please make sure Amount is a number.");
            }
            return BudgetPlanCategoryListing(budgetPlanId);
        }

        public ActionResult SavePercent(int budgetPlanCategoryId, string allocatedPercent)
        {
            BudgetPlanningService s = new BudgetPlanningService();
            int budgetPlanId = s.GetSelectedBudgetPlan();
            SummaryService ss = new SummaryService();
            decimal temp = 0;
            allocatedPercent = allocatedPercent.Replace('%', (char)0);
            if (Decimal.TryParse(allocatedPercent, out temp))
            {
                BudgetPlanCategoryDto b = new BudgetPlanCategoryDto();
                b.BudgetPlanCategoryId = budgetPlanCategoryId;
                b.AllocatedPercentage = allocatedPercent;
                b.UsePercent = true;
                s.SaveBudgetPlanCategory(b);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Allocated percentage cannot be parsed as a decimal number. Please make sure Percent is a number.");
            }
            return BudgetPlanCategoryListing(budgetPlanId);
        }
    }
}