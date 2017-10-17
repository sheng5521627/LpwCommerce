using Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Tasks
{
    /// <summary>
    /// 任务管理器
    /// </summary>
    public partial class TaskManager
    {
        private static readonly TaskManager _taskManager = new TaskManager();
        private readonly IList<TaskThread> _taskThreads = new List<TaskThread>();
        private const int _notRunTasksInterval = 60 * 30;//30 minites

        private TaskManager()
        {

        }

        public void Initialze()
        {
            this._taskThreads.Clear();
            var taskService = EngineContext.Current.Resolve<IScheduleTaskService>();
            var scheduleTasks = taskService.GetAllTasks().OrderBy(m => m.Seconds).ToList();

            foreach (var scheduleTaskGrouped in scheduleTasks.GroupBy(m => m.Seconds))
            {
                var taskThread = new TaskThread()
                {
                    Seconds = scheduleTaskGrouped.Key
                };
                foreach (var scheduleTask in scheduleTaskGrouped)
                {
                    var task = new Task(scheduleTask);
                    taskThread.AddTask(task);
                }
                this._taskThreads.Add(taskThread);
            }

            //sometimes a task period could be set to several hours (or even days).
            //in this case a probability that it'll be run is quite small (an application could be restarted)
            //we should manually run the tasks which weren't run for a long time
            var notRunTask = scheduleTasks
                .Where(x => x.Seconds >= _notRunTasksInterval)
                .Where(x => !x.LastStartUtc.HasValue || x.LastStartUtc.Value.AddSeconds(_notRunTasksInterval) < DateTime.UtcNow).ToList();
            if (notRunTask.Count > 0)
            {
                var taskThread = new TaskThread()
                {
                    RunOnlyOnce = true,
                    Seconds = 60 * 5
                };
                foreach (var scheduleTask in notRunTask)
                {
                    var task = new Task(scheduleTask);
                    taskThread.AddTask(task);
                }
                this._taskThreads.Add(taskThread);
            }
        }

        public void Start()
        {
            foreach (var taskThread in this._taskThreads)
            {
                taskThread.InitTimer();
            }
        }

        public void Stop()
        {
            foreach (var taskThread in this._taskThreads)
            {
                taskThread.Dispose();
            }
        }

        public static TaskManager Instance
        {
            get { return _taskManager; }
        }

        public IList<TaskThread> TaskThreads
        {
            get { return new ReadOnlyCollection<TaskThread>(this._taskThreads); }
        }
    }
}
