using ProjectTrackr.Interfaces;
using ProjectTrackr.Models;
using System.Data;

namespace Unit_tests.Stubs
{
    public class TaskItemContainerStubs : ITaskItem
    {
        public TaskItem? item;
        public bool? existReturnValue = null;
        public bool? successReturnValue = null;
        public Guid id = Guid.Empty;

        public Guid CreateTaskItem(TaskItem item)
        {
            if (successReturnValue == null || item == null)
            {
                throw new NullReferenceException("Invalid use of stub code. First set field successReturnValue and the taskitem.");
            }
            id = item.id;
            item = new();

            return id;
        }

        public void EditTask(TaskItem item)
        {
            this.item = item;

            if (existReturnValue == null)
            {
                throw new NullReferenceException("Invalid use of stub code. First set field existReturnValue and pass a item.");
            }
        }

        public DataTable GetTaskDetails(Guid itemId)
        {
            if (existReturnValue == null || itemId == Guid.Empty)
            {
                throw new NullReferenceException("Invalid use of stub code. First set field existReturnValue and itemId.");
            }
            return new DataTable();
        }

        public DataTable GetTasks(Guid projectId)
        {
            if (existReturnValue == null || projectId == Guid.Empty)
            {
                throw new NullReferenceException("Invalid use of stub code. First set field existReturnValue and projectId.");
            }
            return new DataTable();
        }

        public bool TaskExists(string taskName)
        {
            if (existReturnValue == null || taskName == "")
            {
                throw new NullReferenceException("Invalid use of stub code. First set field existReturnValue and taskName.");
            }
            return existReturnValue.Value;
        }
    }
}