using System.Data;
using ProjectTrackr.Models;
using ProjectTrackr.Containers;
using Unit_tests.Stubs;

namespace Unit_tests
{
    public class ProjectContainerTests
    {
        private readonly ProjectContainerStubs pcs;
        private readonly ProjectContainer pc;

        public ProjectContainerTests()
        {
            pcs = new ProjectContainerStubs();
            pc = new ProjectContainer(pcs);
        }

        [Fact]
        public void CreateProject_Positive_ReturnsGuid()
        {
            pcs.successReturnValue = true;
            Project project = new() { id = Guid.NewGuid() };

            Guid result = pc.CreateProject(project);

            Assert.NotEqual(Guid.Empty, result);
        }

        [Fact]
        public void CreateProject_Negative_ThrowsException()
        {
            pcs.successReturnValue = null;
            Project project = null;

            Assert.Throws<NullReferenceException>(() => pc.CreateProject(project));
        }

        [Fact]
        public void DeleteProject_Positive_Succeeds()
        {
            pcs.existReturnValue = true;
            Guid projectId = Guid.NewGuid();

            var ex = Record.Exception(() => pc.DeleteProject(projectId));

            Assert.Null(ex);
        }

        [Fact]
        public void DeleteProject_Negative_ThrowsException()
        {
            pcs.existReturnValue = null;
            Guid projectId = Guid.Empty;

            Assert.Throws<NullReferenceException>(() => pc.DeleteProject(projectId));
        }

        [Fact]
        public void EditProject_Positive_Succeeds()
        {
            pcs.existReturnValue = true;
            Project project = new() { id = Guid.NewGuid() };

            var ex = Record.Exception(() => pc.EditProject(project));

            Assert.Null(ex);
        }

        [Fact]
        public void EditProject_Negative_ThrowsException()
        {
            pcs.existReturnValue = null;
            Project project = null;

            Assert.Throws<NullReferenceException>(() => pc.EditProject(project));
        }

        [Fact]
        public void GetProjectDetails_Positive_ReturnsDataTable()
        {
            pcs.existReturnValue = true;
            Guid projectId = Guid.NewGuid();

            DataTable table = pc.GetProjectDetails(projectId);

            Assert.NotNull(table);
        }

        [Fact]
        public void GetProjectDetails_Negative_ThrowsException()
        {
            pcs.existReturnValue = null;
            Guid projectId = Guid.Empty;

            Assert.Throws<NullReferenceException>(() => pc.GetProjectDetails(projectId));
        }

        [Fact]
        public void GetProjects_Positive_ReturnsDataTable()
        {
            pcs.existReturnValue = true;
            User user = new() { id = Guid.NewGuid() };

            DataTable table = pc.GetProjects(user);

            Assert.NotNull(table);
        }

        [Fact]
        public void GetProjects_Negative_ThrowsException()
        {
            pcs.existReturnValue = null;
            User user = new() { id = Guid.Empty };

            Assert.Throws<NullReferenceException>(() => pc.GetProjects(user));
        }

        [Fact]
        public void ProjectExists_Positive_ReturnsTrue()
        {
            pcs.existReturnValue = true;
            string projectName = "Test Project";
            Guid projectId = Guid.NewGuid();
            bool editProject = false;

            bool exists = pc.ProjectExists(projectName, projectId, editProject);

            Assert.True(exists);
        }

        [Fact]
        public void ProjectExists_Negative_ThrowsException()
        {
            pcs.existReturnValue = null;
            string projectName = "";
            Guid projectId = Guid.Empty;
            bool editProject = false;

            Assert.Throws<NullReferenceException>(() => pc.ProjectExists(projectName, projectId, editProject));
        }
    }
}