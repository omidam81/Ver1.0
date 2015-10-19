using Orchard.Logging;
using Orchard.Tasks.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Services;
using Teeyoot.Module.Services.Interfaces;

namespace Teeyoot.Module.Common.ScheduledTasks
{
    public class SendShippedOrderAfter3DayToBuyerTaskHandler : IScheduledTaskHandler
    {
        private const string TASK_TYPE = "SendShippedOrderAfter3DayToBuyerTask";

        private readonly IScheduledTaskManager _taskManager;
        private readonly ITeeyootMessagingService _teeyootMessagingService;

        public ILogger Logger { get; set; }

        public SendShippedOrderAfter3DayToBuyerTaskHandler(IScheduledTaskManager taskManager, ICampaignService campaignService, ITeeyootMessagingService teeyootMessagingService)
        {
            _taskManager = taskManager;
            _teeyootMessagingService = teeyootMessagingService;

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
                    Logger.Information("----------------------------- Send Finished Campaign after 1 day to Seller task started --------------------------------");
                    _teeyootMessagingService.SendOrderShipped3DaysToBuyer();
                }
                catch (Exception e)
                {
                    this.Logger.Error("Error occured when running Send Finished Campaign after 1 day to Seller task ---------------- >" + e.ToString(), e.Message);
                }
                finally
                {
                    Logger.Information("----------------------------- Send Finished Campaign after 1 day to Seller task finished --------------------------------");
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