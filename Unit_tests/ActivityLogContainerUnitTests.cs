using System.Data;
using ProjectTrackr.Models;
using ProjectTrackr.Containers;
using Unit_tests.Stubs;

namespace Unit_tests
{
    public class ActivityLogContainerTests
    {
        private readonly ActivityLogContainerStubs alcs;
        private readonly ActivityLogContainer alc;

        public ActivityLogContainerTests()
        {
            alcs = new ActivityLogContainerStubs();
            alc = new ActivityLogContainer(alcs);
        }


        [Fact]
        public void CreateActivityLog_Positive()
        {
            alcs.existReturnValue = true;

            ActivityLog log = new() { id = Guid.NewGuid() };

            Exception ex = Record.Exception(() => alc.CreateActivityLog(log));

            Assert.Null(ex);
        }

        [Fact]
        public void CreateActivityLog_Negative()
        {
            alcs.existReturnValue = null;

            ActivityLog log = new() { id = Guid.Empty };

            Assert.Throws<NullReferenceException>(() => alc.CreateActivityLog(log));
        }

        [Fact]
        public void DeleteActivityLogByProjectId_Positive()
        {
            alcs.existReturnValue = true;

            Guid projectId = Guid.NewGuid();

            Exception ex = Record.Exception(() => alc.DeleteActivityLogByProjectId(projectId));

            Assert.Null(ex);
        }

        [Fact]
        public void DeleteActivityLogByProjectId_Negative()
        {
            alcs.existReturnValue = null;

            Guid projectId = Guid.Empty;

            Assert.Throws<NullReferenceException>(() => alc.DeleteActivityLogByProjectId(projectId));
        }

        [Fact]
        public void GetActivityLogsByProjectId_Positive()
        {
            alcs.existReturnValue = true;

            Guid projectId = Guid.NewGuid();

            DataTable result = alc.GetActivityLogsByProjectId(projectId);

            Assert.NotNull(result);
            Assert.IsType<DataTable>(result);
        }

        [Fact]
        public void GetActivityLogsByProjectId_Negative()
        {
            alcs.existReturnValue = null;

            Guid projectId = Guid.Empty;

            Assert.Throws<NullReferenceException>(() => alc.GetActivityLogsByProjectId(projectId));
        }
    }
}