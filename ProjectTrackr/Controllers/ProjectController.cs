using Microsoft.AspNetCore.Mvc;
using ProjectTrackr.Containers;
using ProjectTrackr.DALs;
using ProjectTrackr.Models;
using ProjectTrackr.Models.ViewModels;
using ProjectTrackr.Tools;
using System.Data;

namespace ProjectTrackr.Controllers
{
    public class ProjectController : Controller
    {
        private Validator validator = new();

        private ProjectContainer projectContainer { get; set; }
        private UserContainer userContainer { get; set; }
        private ActivityLogContainer activityLogContainer { get; set; }

        private TaskItemContainer taskItemContainer { get; set; }

        private Guid userGuid { get; set; }

        public ProjectController(IConfiguration configuration)
        {
            projectContainer = new ProjectContainer(new ProjectDAL(configuration));
            userContainer = new UserContainer(new UserDAL(configuration));
            activityLogContainer = new ActivityLogContainer(new ActivityLogDAL(configuration));
            taskItemContainer = new TaskItemContainer(new TaskItemDAL(configuration));
        }

        // GET: ProjectController/All
        public ActionResult All()
        {
            userGuid = User.GetUserId();

            if (userGuid == Guid.Empty)
                return RedirectToAction("Index", "Login");

            DataTable tableUser = userContainer.GetUserDetails(new User { id = userGuid });
            if (tableUser.Rows.Count == 0)
                return NotFound();

            DataRow userRow = tableUser.Rows[0];
            User user = new()
            {
                id = userGuid,
                username = userRow["Username"]?.ToString() ?? string.Empty,
                email = userRow["Email"]?.ToString() ?? string.Empty,
                createdAt = userRow["CreatedAt"] != DBNull.Value ? (DateTime)userRow["CreatedAt"] : DateTime.MinValue
            };

            DataTable tableProjects = projectContainer.GetProjects(user);

            List<Project> projectList = [.. tableProjects.AsEnumerable().Select(row => new Project
            {
                id = Guid.Parse(row["ID"].ToString() ?? string.Empty),
                name = row["Name"]?.ToString() ?? string.Empty,
                description = row["Description"]?.ToString() ?? string.Empty,
                createdAt = row["CreatedAt"] != DBNull.Value ? (DateTime)row["CreatedAt"] : DateTime.MinValue,
                ownerId = Guid.Parse(row["OwnerID"].ToString() ?? string.Empty),
                owner = user
            })];

            return View("ViewAll", projectList);
        }

        // GET: ProjectController/Details/(ID)
        public ActionResult Details(Guid id)
        {
            DataTable tableProject = projectContainer.GetProjectDetails(id);
            if (tableProject.Rows.Count == 0)
                return NotFound();

            DataRow projectRow = tableProject.Rows[0];

            Project project = new()
            {
                id = Guid.Parse(projectRow["ID"].ToString() ?? String.Empty),
                name = projectRow["Name"]?.ToString() ?? String.Empty,
                description = projectRow["Description"]?.ToString() ?? String.Empty,
                createdAt = projectRow["CreatedAt"] != DBNull.Value ? (DateTime)projectRow["CreatedAt"] : DateTime.MinValue,
                ownerId = Guid.Parse(projectRow["OwnerID"].ToString() ?? String.Empty)
            };

            DataTable tableActivities = activityLogContainer.GetActivityLogsByProjectId(project.id);
            List<ActivityLog> activityLogs = [.. tableActivities.AsEnumerable().Select(row => new ActivityLog
            {
                id = Guid.Parse(row["ID"].ToString() ?? String.Empty),
                action = row["Action"]?.ToString() ?? String.Empty,
                createdAt = row["CreatedAt"] != DBNull.Value ? (DateTime)row["CreatedAt"] : DateTime.MinValue,
                userId = Guid.Parse(row["UserID"].ToString() ?? String.Empty),
                projectId = row["ProjectID"] != DBNull.Value ? Guid.Parse(row["ProjectID"].ToString() ?? String.Empty) : Guid.Empty,
                taskId = row["TaskID"] != DBNull.Value ? Guid.Parse(row["TaskID"].ToString() ?? String.Empty) : (Guid?)null
            })];

            DataTable tableTasks = taskItemContainer.GetTasks(project.id);
            List<TaskItem> tasks = [.. tableTasks.AsEnumerable().Select(row => new TaskItem
            {
                id = Guid.Parse(row["ID"].ToString() ?? String.Empty),
                title = row["title"]?.ToString() ?? String.Empty,
                description = row["description"]?.ToString() ?? String.Empty,
                status = (Models.TaskStatus)row["status"],
                createdAt = row["CreatedAt"] != DBNull.Value ? (DateTime)row["CreatedAt"] : DateTime.MinValue,
                projectId = row["ProjectID"] != DBNull.Value ? Guid.Parse(row["ProjectID"].ToString() ?? String.Empty) : Guid.Empty
            })];

            project.activityLogs = activityLogs;
            project.tasks = tasks;

            return View("View", project);
        }

        // GET: ProjectController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProjectController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProjectViewModel model)
        {
            userGuid = User.GetUserId();

            if (userGuid == Guid.Empty)
                return RedirectToAction("Index", "Login");

            if (!CheckInputs(model))
                return RegisterFailed(model);
            else if (projectContainer.ProjectExists(model.name, userGuid, false) == true)
            {
                ModelState.AddModelError("ProjectExists", "Project name is already taken.");
                return RegisterFailed(model);
            }
            else
            {
                CreateProject(model, userGuid);

                return RedirectToAction("All", "Project");
            }
        }

        private bool CheckInputs(ProjectViewModel model)
        {
            if (!validator.ValidateString(model.name))
            {
                ModelState.AddModelError("InvalidString", "Project name format is invalid.");
                return false;
            }
            else
                return true;
        }

        private ViewResult RegisterFailed(ProjectViewModel model)
        {
            TempData["Errors"] = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

            return View(model);
        }

        private void CreateProject(ProjectViewModel model, Guid userGuid)
        {
            DataTable userTable = userContainer.GetUserDetails(new User { id = userGuid });

            User user = new()
            {
                id = Guid.Parse(userTable.Rows[0]["ID"].ToString() ?? String.Empty),
                email = userTable.Rows[0]["Email"].ToString() ?? String.Empty,
                username = userTable.Rows[0]["Username"].ToString() ?? String.Empty,
                createdAt = (DateTime)userTable.Rows[0]["CreatedAt"]
            };

            Project project = new()
            {
                id = Guid.NewGuid(),
                name = model.name,
                description = model.description ?? string.Empty,
                createdAt = DateTime.Now,
                ownerId = userGuid,
                owner = user
            };
            ActivityLog log = new()
            {
                id = Guid.NewGuid(),
                action = $"Created project '{project.name}'.",
                createdAt = DateTime.Now,
                userId = userGuid,
                user = user,
                project = project,
                projectId = project.id,
                task = null,
                taskId = null
            };

            projectContainer.CreateProject(project);
            activityLogContainer.CreateActivityLog(log);
        }


        // GET: ProjectController/Edit/(ID)
        public ActionResult Edit(Guid id)
        {
            DataTable tableProject = projectContainer.GetProjectDetails(id);

            if (tableProject.Rows.Count == 0)
                return NoContent();

            DataRow row = tableProject.Rows[0];

            ProjectViewModel project = new()
            {
                id = Guid.Parse(row["ID"].ToString() ?? string.Empty),
                name = row["Name"].ToString() ?? string.Empty,
                description = row["Description"].ToString() ?? string.Empty
            };

            User user = new()
            {
                id = Guid.Parse(row["OwnerID"].ToString() ?? string.Empty),
                username = "",
                email = ""
            };

            return View(project);
        }

        // POST: ProjectController/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProjectViewModel model)
        {
            userGuid = User.GetUserId();

            if (userGuid == Guid.Empty)
                return RedirectToAction("Index", "Login");

            if (!validator.ValidateString(model.name))
            {
                ModelState.AddModelError("InvalidString", "Project name format is invalid.");
                return RegisterFailed(model);
            }
            else if (projectContainer.ProjectExists(model.name, userGuid, true) != false)
            {
                ModelState.AddModelError("ProjectExists", "Project already exists.");
                return RegisterFailed(model);
            }
            else
            {
                Project project = new()
                {
                    name = model.name,
                    description = model.description ?? string.Empty,
                    id = model.id
                };

                ActivityLog log = new()
                {
                    id = Guid.NewGuid(),
                    action = $"Updated project contents, name and/or description.",
                    createdAt = DateTime.Now,
                    userId = userGuid,
                    project = project,
                    projectId = project.id,
                    task = null,
                    taskId = null
                };

                projectContainer.EditProject(project);
                activityLogContainer.CreateActivityLog(log);

                return RedirectToAction("All", "Project");
            }
        }

        // POST: ProjectController/Delete/ID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid id)
        {
            activityLogContainer.DeleteActivityLogByProjectId(id);
            projectContainer.DeleteProject(id);

            return RedirectToAction("All");
        }
    }
}