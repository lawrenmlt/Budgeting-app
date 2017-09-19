using Budgeting.Data;
using Budgeting.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budgeting.Service
{
    public class ActivityService : BaseService
    {
        public void SaveActivity(ActivityDto dto)
        {
            Activity a = new Activity();
            a.CategoryId = dto.CategoryId;
            a.Amount = (dto.Amount ?? 0);
            a.Expenditure = dto.Expenditure;
            a.Recurring = dto.Recurring;
            a.RecurringDay = dto.RecurringDay;
            a.DateOfActivity = dto.DateOfActivity;
            a.Description = dto.Description;
            a.Active = true;
            a.Pretax = dto.Pretax;
            using (BudgetingEntities db = new BudgetingEntities())
            {
                db.Activities.Add(a);
                db.Entry(a).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
            }
        }

        public void SaveActivityUpdate(ActivityDto dto)
        {
            using (BudgetingEntities db = new BudgetingEntities())
            {
                Activity a = db.Activities.Find(dto.ActivityId);
                a.CategoryId = dto.CategoryId;
                db.Entry(a).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        public void DeleteActivity(int activityId)
        {
            using (BudgetingEntities db = new BudgetingEntities())
            {
                Activity a = db.Activities.Find(activityId);
                db.Activities.Remove(a);
                db.Entry(a).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }

        }
    }
}
