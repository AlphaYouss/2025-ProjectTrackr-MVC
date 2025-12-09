using ProjectTrackr.Models;
using System.Data;

namespace ProjectTrackr.Interfaces
{
    public interface ITaskItem
    {
        Guid CreateTaskItem(TaskItem item);
        bool TaskExists(string taskName);
        void EditTask(TaskItem item);
        DataTable GetTaskDetails(Guid itemId);
        DataTable GetTasks(Guid projectId);
    }
}