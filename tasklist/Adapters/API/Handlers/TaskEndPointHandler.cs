﻿#region Licence
/* The MIT License (MIT)
Copyright © 2014 Ian Cooper <ian_hammond_cooper@yahoo.co.uk>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the “Software”), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE. */

#endregion

using System;
using Common.Logging;
using OpenRasta.Web;
using Tasklist.Adapters.API.Resources;
using Tasklist.Ports.ViewModelRetrievers;
using Tasks.Adapters.DataAccess;
using Tasks.Ports.Commands;

namespace Tasklist.Adapters.API.Handlers
{
    public class TaskEndPointHandler
    {
        private readonly ITaskRetriever taskRetriever;
        private readonly ICommunicationContext communicationContext;
        private readonly ITasksDAO tasksDao;
        private readonly ILog logger;
        private readonly ITaskListRetriever taskListRetriever;

        public TaskEndPointHandler(ITaskRetriever taskRetriever, ITaskListRetriever taskListRetriever, ICommunicationContext communicationContext, ITasksDAO tasksDAO, ILog logger)
        {
            this.taskRetriever = taskRetriever;
            this.taskListRetriever = taskListRetriever;
            this.communicationContext = communicationContext;
            tasksDao = tasksDAO;
            this.logger = logger;
        }

        [HttpOperation(HttpMethod.GET)]
        public OperationResult Get()
        {
            TaskListModel responseResource = taskListRetriever.RetrieveTasks();
            return new OperationResult.OK {ResponseResource = responseResource};
        }

        [HttpOperation(HttpMethod.GET)]
        public OperationResult Get(int taskId)
        {
            return new OperationResult.OK {ResponseResource = taskRetriever.Get(taskId)};
        }

        [HttpOperation(HttpMethod.POST)]
        public OperationResult Post(TaskModel newTask)
        {
            var addTaskCommand = new AddTaskCommand(
                taskName: newTask.TaskName,
                taskDescription: newTask.TaskDescription,
                dueDate: DateTime.Parse(newTask.DueDate),
                tasksDAO:tasksDao,
                logger: logger
                );

            if (addTaskCommand.IsValid())
            {
                addTaskCommand.Execute();
            }

            return new OperationResult.Created
                {
                    ResponseResource = taskRetriever.Get(addTaskCommand.TaskId),
                    CreatedResourceUrl = new Uri(string.Format("{0}/tasks/{1}", communicationContext.ApplicationBaseUri, addTaskCommand.TaskId))
                };
        }
    }
}