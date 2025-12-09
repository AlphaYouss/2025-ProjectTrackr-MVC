using ProjectTrackr.Interfaces;
using ProjectTrackr.Models;
using System.Data;
using Project = ProjectTrackr.Models.Project;

namespace Unit_tests.Stubs
{
    public class ProjectContainerStubs : IProject
    {
        public Project? project;
        public bool? existReturnValue = null;
        public bool? successReturnValue = null;
        public Guid id = Guid.Empty;

        public Guid CreateProject(Project project)
        {
            if (successReturnValue == null || project == null)
            {
                throw new NullReferenceException("Invalid use of stub code. First set field successReturnValue and the project.");
            }
            id = project.id;
            project = new();

            return id;
        }

        public void DeleteProject(Guid projectId)
        {
            if (existReturnValue == null || projectId == Guid.Empty)
            {
                throw new NullReferenceException("Invalid use of stub code. First set field existReturnValue and pass a project.");
            }

            project = null;

            if (project != null) {
                throw new Exception("Project still exists.");
            }
        }

        public void EditProject(Project project)
        {
            this.project = project;

            if (existReturnValue == null)
            {
                throw new NullReferenceException("Invalid use of stub code. First set field existReturnValue and pass a project.");
            }
        }

        public DataTable GetProjectDetails(Guid projectId)
        {
            if (existReturnValue == null || projectId == Guid.Empty)
            {
                throw new NullReferenceException("Invalid use of stub code. First set field existReturnValue and user.");
            }
            return new DataTable();
        }

        public DataTable GetProjects(User user)
        {
            user = new();

            if (existReturnValue == null || user.id == Guid.Empty)
            {
                throw new NullReferenceException("Invalid use of stub code. First set field existReturnValue and user.");
            }
            return new DataTable();
        }

        public bool ProjectExists(string projectName, Guid projectId, bool editProject)
        {
            if (existReturnValue == null || projectName == "" || projectId == Guid.Empty)
            {
                throw new NullReferenceException("Invalid use of stub code. First set field existReturnValue and email.");
            }
            return existReturnValue.Value;
        }
    }
}