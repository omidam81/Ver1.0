using Orchard.Logging;
using Orchard.Tasks.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Services;

namespace Teeyoot.Module.Common.ScheduledTasks
{
    public class SendRelaunchRequestTaskHandler : IScheduledTaskHandler
    {
        private const string TASK_TYPE = "SendRelaunchRequestTask";

        private readonly IScheduledTaskManager _taskManager;
        private readonly ICampaignService _campaignService;

        public ILogger Logger { get; set; }

        public SendRelaunchRequestTaskHandler(IScheduledTaskManager taskManager, ICampaignService campaignService)
        {
            _taskManager = taskManager;
            _campaignService = campaignService;

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
                    Logger.Information("----------------------------- Check Relaunch campaigns task started --------------------------------");
                    _campaignService.CheckCountOfReservedRequestsOfCampaigns();
                }
                catch (Exception e)
                {
                    this.Logger.Error("Error occured when running Check Relaunch campaigns task ---------------- >" + e.ToString(), e.Message);
                }
                finally
                {
                    Logger.Information("----------------------------- Check Relaunch campaigns task finished --------------------------------");
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