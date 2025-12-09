using Microsoft.AspNetCore.Mvc;
using ProjectTrackr.Containers;
using ProjectTrackr.DALs;
using ProjectTrackr.Interfaces;
using ProjectTrackr.Models;
using ProjectTrackr.Models.ViewModels;
using ProjectTrackr.Tools;
using System.Data;
using System.Security.Claims;

namespace ProjectTrackr.Controllers
{
    public class ProjectController : Controller
    {
        private Validator validator = new Validator();

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
            userGuid = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            User user = new User();
            user.id = userGuid;
            user.username = "";
            user.email = "";

            DataTable tableUser = userContainer.GetUserDetails(user);

            user.id = Guid.Parse(tableUser.Rows[0]["ID"].ToString());
            user.email = tableUser.Rows[0]["Email"].ToString();
            user.username = tableUser.Rows[0]["Username"].ToString();
            user.createdAt = (DateTime)tableUser.Rows[0]["CreatedAt"];

            DataTable tableProjects = projectContainer.GetProjects(user);
            List<Project> projectList = new List<Project>();

            for (int i = 0; i < tableProjects.Rows.Count; i++)
            {
                Project project = new Project();
                project.id = Guid.Parse(tableProjects.Rows[i]["ID"].ToString());
                project.name = tableProjects.Rows[i]["Name"].ToString();
                project.description = tableProjects.Rows[i]["Description"].ToString();
                project.createdAt = (DateTime)tableProjects.Rows[i]["CreatedAt"];
                project.ownerId = Guid.Parse(tableProjects.Rows[i]["OwnerID"].ToString());
                project.owner = user;
                projectList.Add(project);
            }

            return View("ViewAll", projectList);
        }

        // GET: ProjectController/Details/(ID)
        public ActionResult Details(Guid id)
        {
            DataTable tableProject = projectContainer.GetProjectDetails(id);

            Project project = new Project();
            List<ActivityLog> activityLogs = new List<ActivityLog>();

            foreach (DataRow row in tableProject.Rows)
            {
                project.id = Guid.Parse(row["ID"].ToString());
                project.name = row["Name"].ToString();
                project.description = row["Description"].ToString();
                project.createdAt = (DateTime)row["CreatedAt"];
                project.ownerId = Guid.Parse(row["OwnerID"].ToString());
            }

            DataTable tableActivities = activityLogContainer.GetActivityLogsByProjectId(project.id);

            foreach (DataRow row in tableActivities.Rows)
            {
                activityLogs.Add(new ActivityLog
                {
                    id = Guid.Parse(row["ID"].ToString()),
                    action = row["Action"].ToString(),
                    createdAt = (DateTime)row["CreatedAt"],
                    userId = Guid.Parse(row["UserID"].ToString()),
                    projectId = row["ProjectID"] != DBNull.Value ? Guid.Parse(row["ProjectID"].ToString()) : null,
                    taskId = row["TaskID"] != DBNull.Value ? Guid.Parse(row["TaskID"].ToString()) : null
                });
            }

            DataTable tableTasks = taskItemContainer.GetTasks(project.id);
            List<TaskItem> tasks = new List<TaskItem>();

            foreach (DataRow row in tableTasks.Rows) {
                tasks.Add(new TaskItem
                {
                    id = Guid.Parse((string)row["ID"].ToString()),
                    title = row["title"].ToString(),
                    description = row["description"].ToString(),
                    status = (Models.TaskStatus)row["status"],
                    createdAt = (DateTime)row["CreatedAt"],
                    projectId = (Guid)row["ProjectID"]
                });
            }
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
            userGuid = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (!checkInputs(model))
                return RegisterFailed(model);
            else if (projectContainer.ProjectExists(model.name, userGuid, false) == true)
            {
                ModelState.AddModelError("ProjectExists", "Project name is already taken.");
                return RegisterFailed(model);
            }
            else
            {
                CreateProject(model);

                return RedirectToAction("All", "Project");
            }
        }

        private bool checkInputs(ProjectViewModel model)
        {
            if (!validator.ValidateStrings(model.name))
            {
                ModelState.AddModelError("InvalidString", "Project name format is invalid.");
                return false;
            }
            else
                return true;
        }

        private ActionResult RegisterFailed(ProjectViewModel model)
        {
            TempData["Errors"] = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

            return View(model);
        }

        private void CreateProject(ProjectViewModel model)
        {
            userGuid = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            User user = new User();
            user.id = userGuid;
            user.username = "";
            user.email = "";

            DataTable table = userContainer.GetUserDetails(user);

            user.id = Guid.Parse(table.Rows[0]["ID"].ToString());
            user.email = table.Rows[0]["Email"].ToString();
            user.username = table.Rows[0]["Username"].ToString();
            user.createdAt = (DateTime)table.Rows[0]["CreatedAt"];

            Project project = new Project();
            project.id = Guid.NewGuid();
            project.name = model.name;
            project.description = model.description;
            project.createdAt = DateTime.Now;
            project.ownerId = userGuid;
            project.owner = user;

            projectContainer.CreateProject(project);

            ActivityLog log = new ActivityLog();
            log.id = Guid.NewGuid();
            log.action = $"Created project '{project.name}'.";
            log.createdAt = DateTime.Now;
            log.userId = userGuid;
            log.user = user;
            log.project = project;
            log.projectId = project.id;
            log.task = null;
            log.taskId = null;

            activityLogContainer.CreateActivityLog(log);
        }


        // GET: ProjectController/Edit/(ID)
        public ActionResult Edit(Guid id)
        {
            DataTable tableProject = projectContainer.GetProjectDetails(id);

            ProjectViewModel project = new ProjectViewModel();
            User user = new User();

            foreach (DataRow row in tableProject.Rows)
            {
                project.id = Guid.Parse(row["ID"].ToString());
                project.name = row["Name"].ToString();
                project.description = row["Description"].ToString();

                user.id = Guid.Parse(row["OwnerID"].ToString());
                user.username = "";
                user.email = "";
            }

            DataTable tableUser = userContainer.GetUserDetails(user);

            return View(project);
        }

        // POST: ProjectController/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProjectViewModel model)
        {
            userGuid = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (!validator.ValidateStrings(model.name))
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
                Project project = new Project();
                project.name = model.name;
                project.description = model.description;
                project.id = model.id;

                ActivityLog log = new ActivityLog();
                log.id = Guid.NewGuid();
                log.action = $"Updated project contents, name and/or description.";
                log.createdAt = DateTime.Now;
                log.userId = userGuid;
                log.project = project;
                log.projectId = project.id;
                log.task = null;
                log.taskId = null;

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