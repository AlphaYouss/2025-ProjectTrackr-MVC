using ProjectTrackr.Models;
using System.Data;

namespace ProjectTrackr.Interfaces
{
    public interface IProject
    {
        Guid CreateProject(Project project);
        bool ProjectExists(string projectName, Guid projectId, bool editProject);
        void EditProject(Project project);
        void DeleteProject(Guid projectId);
        DataTable GetProjectDetails(Guid projectId);
        DataTable GetProjects(User user);
    }
}