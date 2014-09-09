#region Licence
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
using Tasks.Adapters.DataAccess;
using Tasks.Model;

namespace Tasks.Ports.Commands
{
    public class AddTaskCommand : Command, ICanBeValidated
    {
        private readonly ITasksDAO tasksDAO;
        private readonly ILog logger;
        private readonly string taskDescription;
        private readonly DateTime? taskDueDate;
        private readonly string taskName; 

        public int TaskId { get; set; }

        public AddTaskCommand(string taskName, string taskDescription, DateTime? dueDate, ITasksDAO tasksDAO, ILog logger)
            :base(Guid.NewGuid())
        {
            this.tasksDAO = tasksDAO;
            this.logger = logger;
            this.taskName = taskName;
            this.taskDescription = taskDescription;
            taskDueDate = dueDate;
        }

        public bool IsValid()
        {
            if ((taskDescription == null) || (taskName == null))
            {
                return false;
            }

            return true;
        }

        public override void Execute()
        {
            logger.DebugFormat("Executing Add Task command with Name: {0}, Description: {1}, DueDate: {2}", taskName, taskDescription, taskDueDate);
            if (IsValid())
            {
                var inserted = tasksDAO.Add(
                    new Task(
                        taskName: taskName,
                        taskDecription: taskDescription,
                        dueDate: taskDueDate
                        )
                    );
                TaskId = inserted.Id;
                logger.DebugFormat("Finished executing Add Task, added task with ID: {0} ", TaskId);
            }
            else
            {
                logger.ErrorFormat("Aborted executing Add Task, command was not valid");
                throw new ArgumentException("The commmand was not valid");
            }
        }
    }
}