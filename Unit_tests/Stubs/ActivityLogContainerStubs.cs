using ProjectTrackr.Interfaces;
using ProjectTrackr.Models;
using System.Data;

namespace Unit_tests.Stubs
{
    public class ActivityLogContainerStubs : IActivityLog
    {
        public ActivityLog acivityLog;
        public bool? existReturnValue = null;
        public bool? successReturnValue = null;

        public void CreateActivityLog(ActivityLog activityLog)
        {
            acivityLog = new();

            if (existReturnValue == null || activityLog.id == Guid.Empty)
            {
                throw new NullReferenceException("Invalid use of stub code. First set field existReturnValue and pass a activitylog.");
            }
        }

        public void DeleteActivityLogByProjectId(Guid projectId)
        {
            if (existReturnValue == null || projectId == Guid.Empty)
            {
                throw new NullReferenceException("Invalid use of stub code. First set field existReturnValue and pass a projectId.");
            }
        }

        public DataTable GetActivityLogsByProjectId(Guid projectId)
        {
            acivityLog = new();

            if (existReturnValue == null || projectId == Guid.Empty)
            {
                throw new NullReferenceException("Invalid use of stub code. First set field existReturnValue and pass a projectId.");
            }
            return new DataTable();
        }
    }
}