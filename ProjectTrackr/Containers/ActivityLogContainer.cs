using ProjectTrackr.Interfaces;
using ProjectTrackr.Models;
using System.Data;

namespace ProjectTrackr.Containers
{
    public class ActivityLogContainer
    {
        IActivityLog activityLogDAL;

        public ActivityLogContainer(IActivityLog activityLogDAL)
        {
            this.activityLogDAL = activityLogDAL;
        }

        public void CreateActivityLog(ActivityLog activityLog)
        {
            activityLogDAL.CreateActivityLog(activityLog);
        }

        public DataTable GetActivityLogsByProjectId(Guid guid)
        {
            return activityLogDAL.GetActivityLogsByProjectId(guid);    
        }

        public void DeleteActivityLogByProjectId(Guid guid)
        {
            activityLogDAL.DeleteActivityLogByProjectId(guid);
        }
    }
}