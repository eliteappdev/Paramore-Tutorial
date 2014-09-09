using System;
using System.Net;
using Common.Logging;
using OpenRasta.Web;
using paramore.brighter.commandprocessor;
using Tasklist.Adapters.API.Resources;
using Tasks.Adapters.DataAccess;
using Tasks.Ports.Commands;

namespace Tasklist.Adapters.API.Handlers
{
    public class TaskReminderEndpointHandler
    {
        private readonly ITasksDAO tasksDao;
        private readonly ILog logger;

        public TaskReminderEndpointHandler(ITasksDAO tasksDAO, ILog logger)
        {
            tasksDao = tasksDAO;
            this.logger = logger;
        }

        [HttpOperation(HttpMethod.POST)]
        public OperationResult Post(TaskReminderModel reminder)
        {
            var reminderCommand = new TaskReminderCommand(
                taskName: reminder.TaskName,
                dueDate: DateTime.Parse(reminder.DueDate),
                recipient: reminder.Recipient,
                copyTo: reminder.CopyTo,
                tasksDao: tasksDao,
                logger: loger
                );


            return new OperationResult.OK()
            {
                StatusCode = (int) HttpStatusCode.Accepted
            };
        }
    }
}