using ProjectTrackr.Interfaces;
using ProjectTrackr.Models;
using System.Data;

namespace ProjectTrackr.Containers
{
    public class TaskItemContainer
    {
        ITaskItem taskItemDAL;

        public TaskItemContainer(ITaskItem taskItemDAL)
        {
            this.taskItemDAL = taskItemDAL;
        }

        public Guid CreateTaskItem(TaskItem item)
        {
            return taskItemDAL.CreateTaskItem(item);
        }

        public void EditTask(TaskItem item)
        {
            taskItemDAL.EditTask(item);
        }

        public DataTable GetTaskDetails(Guid itemId)
        {
            return taskItemDAL.GetTaskDetails(itemId);
        }

        public DataTable GetTasks(Guid projectId)
        {
            return taskItemDAL.GetTasks(projectId);
        }

        public bool TaskExists(string taskName)
        {
            return taskItemDAL.TaskExists(taskName);
        }
    }
}