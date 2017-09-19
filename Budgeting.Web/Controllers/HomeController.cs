using Budgeting.Dto;
using Budgeting.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Budgeting.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            SummaryService s = new SummaryService();
            List<BudgetPlanCategoryDto> budgetCategories = s.GetMonthActivitiesByCategory(DateTime.Now);
            return View("SummaryIndex", budgetCategories);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Delete(int activityId, int budgetPlanCategoryId)
        {
            ActivityService s = new ActivityService();
            s.DeleteActivity(activityId);
            SummaryService ss = new SummaryService();
            return PartialView("_CategorySummary", ss.GetMonthCategoryActivities(budgetPlanCategoryId, DateTime.Now));
        }

        public ActionResult GetCategoryOptions(int budgetPlanId)
        {
            LookupService s = new LookupService();
            return PartialView("_CategoryDropDown", s.GetCategoryList(budgetPlanId, false));
        }

        public ActionResult UpdateCategory(int activityId, int categoryId)
        {
            // need to submit the whole page
            ActivityService s = new ActivityService();
            SummaryService ss = new SummaryService();
            ActivityDto a = new ActivityDto
            {
                ActivityId = activityId,
                CategoryId = categoryId
            };
            s.SaveActivityUpdate(a);
            return PartialView("SummaryIndex", ss.GetMonthActivitiesByCategory(DateTime.Now));
        }

        public ActionResult OverallSummary()
        {
            SummaryService s = new SummaryService();
            UserDto u = s.GetUserOverallSummary();
            return PartialView(u);
        }
    }
}