using ProjectTrackr.Models;
using System.Data;

namespace ProjectTrackr.Interfaces
{
    public interface IActivityLog
    {
        void CreateActivityLog(ActivityLog activityLog);
        DataTable GetActivityLogsByProjectId(Guid projectId);
        void DeleteActivityLogByProjectId(Guid projectId);
    }
}