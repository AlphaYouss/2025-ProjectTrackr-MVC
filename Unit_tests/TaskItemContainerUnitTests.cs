using ProjectTrackr.Containers;
using ProjectTrackr.Models;
using System.Data;
using Unit_tests.Stubs;

namespace Unit_tests
{
    public class TaskItemContainerTests
    {
        private readonly TaskItemContainerStubs tics;
        private readonly TaskItemContainer tic;

        public TaskItemContainerTests()
        {
            tics = new TaskItemContainerStubs();
            tic = new TaskItemContainer(tics);
        }

        [Fact]
        public void CreateTaskItem_Positive_ReturnsGuid()
        {
            tics.successReturnValue = true;
            TaskItem item = new() { id = Guid.NewGuid() };

            Guid result = tic.CreateTaskItem(item);

            Assert.NotEqual(Guid.Empty, result);
        }

        [Fact]
        public void CreateTaskItem_Negative_ThrowsException()
        {
            tics.successReturnValue = null;
            TaskItem item = null;

            Assert.Throws<NullReferenceException>(() => tic.CreateTaskItem(item));
        }

        [Fact]
        public void EditTask_Positive_Succeeds()
        {
            tics.existReturnValue = true;
            TaskItem item = new() { id = Guid.NewGuid() };

            Exception ex = Record.Exception(() => tic.EditTask(item));

            Assert.Null(ex);
        }

        [Fact]
        public void EditTask_Negative_ThrowsException()
        {
            tics.existReturnValue = null;
            TaskItem item = null;

            Assert.Throws<NullReferenceException>(() => tic.EditTask(item));
        }

        [Fact]
        public void GetTaskDetails_Positive_ReturnsDataTable()
        {
            tics.existReturnValue = true;
            Guid taskId = Guid.NewGuid();

            DataTable table = tic.GetTaskDetails(taskId);

            Assert.NotNull(table);
        }

        [Fact]
        public void GetTaskDetails_Negative_ThrowsException()
        {
            tics.existReturnValue = null;
            Guid taskId = Guid.Empty;

            Assert.Throws<NullReferenceException>(() => tic.GetTaskDetails(taskId));
        }

        [Fact]
        public void GetTasks_Positive_ReturnsDataTable()
        {
            tics.existReturnValue = true;
            Guid projectId = Guid.NewGuid();

            DataTable table = tic.GetTasks(projectId);

            Assert.NotNull(table);
        }

        [Fact]
        public void GetTasks_Negative_ThrowsException()
        {
            tics.existReturnValue = null;
            Guid projectId = Guid.Empty;

            Assert.Throws<NullReferenceException>(() => tic.GetTasks(projectId));
        }

        [Fact]
        public void TaskExists_Positive_ReturnsTrue()
        {
            tics.existReturnValue = true;
            string taskName = "Test Task";

            bool exists = tic.TaskExists(taskName);

            Assert.True(exists);
        }

        [Fact]
        public void TaskExists_Negative_ThrowsException()
        {
            tics.existReturnValue = null;
            string taskName = "";

            Assert.Throws<NullReferenceException>(() => tic.TaskExists(taskName));
        }
    }
}