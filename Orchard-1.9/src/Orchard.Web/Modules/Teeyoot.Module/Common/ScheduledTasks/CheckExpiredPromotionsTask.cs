using Orchard.Logging;
using Orchard.Tasks.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Services;

namespace Teeyoot.Module.Common.ScheduledTasks
{
    public class CheckExpiredPromotionsTaskHandler : IScheduledTaskHandler
    {
        private const string TASK_TYPE = "CheckExpiredPromotionsTask";

        private readonly IScheduledTaskManager _taskManager;
        private readonly IPromotionService _promotionService;

        public ILogger Logger { get; set; }

        public CheckExpiredPromotionsTaskHandler(IScheduledTaskManager taskManager, IPromotionService promotionService)
        {
            _taskManager = taskManager;
            _promotionService = promotionService;

            Logger = NullLogger.Instance;

            try
            {
                var firstDate = DateTime.Today.Date.AddDays(1).AddMinutes(1);
                firstDate = TimeZoneInfo.ConvertTimeToUtc(firstDate, TimeZoneInfo.Local);
                ScheduleNextTask(firstDate);
            }
            catch (Exception e)
            {
                this.Logger.Error(e, e.Message);
            }
        }

        public void Process(ScheduledTaskContext context)
        {
            if (context.Task.TaskType == TASK_TYPE)
            {
                try
                {
                    Logger.Information("----------------------------- Check Expired promotions task started --------------------------------");
                    _promotionService.CheckExpiredPromotions();
                }
                catch (Exception e)
                {
                    this.Logger.Error("Error occured when running Check Expired promotions task ---------------- >" + e.ToString(), e.Message);
                }
                finally
                {
                    Logger.Information("----------------------------- Check Expired promotions task finished --------------------------------");
                    var nextTaskDate = DateTime.Today.Date.AddDays(1).AddMinutes(1);
                    ScheduleNextTask(TimeZoneInfo.ConvertTimeToUtc(nextTaskDate, TimeZoneInfo.Local));
                }
            }
        }

        private void ScheduleNextTask(DateTime date)
        {
            if (date > DateTime.UtcNow)
            {
                var tasks = this._taskManager.GetTasks(TASK_TYPE);
                if (tasks == null || tasks.Count() == 0)
                    this._taskManager.CreateTask(TASK_TYPE, date, null);
            }
        }
    }
}