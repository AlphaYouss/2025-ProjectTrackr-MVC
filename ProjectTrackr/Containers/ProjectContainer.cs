using ProjectTrackr.Interfaces;
using ProjectTrackr.Models;
using System.Data;

namespace ProjectTrackr.Containers
{
    public class ProjectContainer
    {
        IProject projectDAL;
        public ProjectContainer(IProject projectDAL)
        {
            this.projectDAL = projectDAL;
        }

        public Guid CreateProject(Project project)
        {
            return projectDAL.CreateProject(project);
        }

        public void EditProject(Project project)
        {
            projectDAL.EditProject(project);
        }

        public DataTable GetProjects(User user)
        {
            return projectDAL.GetProjects(user);
        }

        public bool ProjectExists(string projectName, Guid projectId, bool editProject)
        {
            return projectDAL.ProjectExists(projectName, projectId, editProject);
        }

        public void DeleteProject(Guid projectId)
        {
            projectDAL.DeleteProject(projectId);
        }

        public DataTable GetProjectDetails(Guid projectId)
        {
            return projectDAL.GetProjectDetails(projectId);
        }
    }
}