using Microsoft.AspNetCore.Mvc;
using ProjectTrackr.Containers;
using ProjectTrackr.DALs;
using ProjectTrackr.Models;
using ProjectTrackr.Models.ViewModels;
using ProjectTrackr.Tools;
using System.Data;
using System.Security.Claims;
using TaskItem = ProjectTrackr.Models.TaskItem;

namespace ProjectTrackr.Controllers
{
    public class TaskController : Controller
    {
        private Validator validator = new();

        private ProjectContainer projectContainer { get; set; }
        private ActivityLogContainer activityLogContainer { get; set; }
        private TaskItemContainer taskItemContainer { get; set; }

        private Guid userGuid { get; set; }

        public TaskController(IConfiguration configuration)
        {
            projectContainer = new ProjectContainer(new ProjectDAL(configuration));
            activityLogContainer = new ActivityLogContainer(new ActivityLogDAL(configuration));
            taskItemContainer = new TaskItemContainer(new TaskItemDAL(configuration));
        }

        // GET: TaskController
        public ActionResult View(Guid id)
        {
            TaskItem taskItem = new();
            DataTable tableTasks = taskItemContainer.GetTaskDetails(id);

            if (tableTasks.Rows.Count == 0)
                return NoContent();

            DataRow row = tableTasks.Rows[0];

            taskItem.id = Guid.Parse(row["ID"].ToString() ?? string.Empty);
            taskItem.title = row["Title"].ToString() ?? string.Empty;
            taskItem.description = row["Description"].ToString() ?? string.Empty;
            taskItem.status = (Models.TaskStatus)row["Status"];
            taskItem.createdAt = (DateTime)row["CreatedAt"];
            taskItem.projectId = Guid.Parse(row["ProjectId"].ToString() ?? string.Empty);

            return View(taskItem);
        }

        // GET: TaskController/Create
        [HttpGet]
        public ActionResult Create(Guid id)
        {
            TaskItemViewModel taskItemViewModel = new()
            {
                projectId = id,
            };

            return View(taskItemViewModel);
        }

        // POST: TaskController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaskItemViewModel model)
        {
            userGuid = User.GetUserId();

            if (userGuid == Guid.Empty)
                return RedirectToAction("Index", "Login");

            if (!checkInputs(model))
                return RegisterFailed(model);
            else if (taskItemContainer.TaskExists(model.title) == true)
            {
                ModelState.AddModelError("ProjectExists", "Task already exists.");
                return RegisterFailed(model);
            }
            else
            {
                if (userGuid == Guid.Empty)
                    return RedirectToAction("Index", "Login");

                CreateTask(model, userGuid);
                return RedirectToAction("Details", "Project", new {id = model.projectId});
            }
        }

        private bool checkInputs(TaskItemViewModel model)
        {
            if (!validator.ValidateString(model.title))
            {
                ModelState.AddModelError("InvalidString", "Task title format is invalid.");
                return false;
            }
            else
                return true;
        }

        private ViewResult RegisterFailed(TaskItemViewModel model)
        {
            TempData["Errors"] = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

            return View(model);
        }

        private void CreateTask(TaskItemViewModel model, Guid userGuid)
        {
            TaskItem item = new()
            {
                id = Guid.NewGuid(),
                title = model.title,
                projectId = model.projectId,
                status = (Models.TaskStatus)model.status,
                description = model.description,
                createdAt = DateTime.Now
            };

            ActivityLog activityLog = new()
            {
                id = Guid.NewGuid(),
                createdAt = DateTime.Now,
                userId = userGuid,
                user = null,
                projectId = model.projectId,
                taskId = item.id,
                task = item,
                action = $"Created task '{item.title}'."
            };

            taskItemContainer.CreateTaskItem(item);
            activityLogContainer.CreateActivityLog(activityLog);
        }

        // GET: TaskController/Edit/(ID)
        public ActionResult Edit(Guid id)
        {
            DataRow row = taskItemContainer.GetTaskDetails(id).Rows[0];

            TaskItemViewModel taskItemViewModel = new()
            {
                id = Guid.Parse(Convert.ToString(row["ID"]) ?? String.Empty),
                title = Convert.ToString(row["Title"]) ?? String.Empty,
                description = Convert.ToString(row["Description"]) ?? String.Empty,
                status = (TaskItemViewModel.TaskStatus)(Models.TaskStatus)row["Status"],
                createdAt = row.Field<DateTime>("CreatedAt"),
                projectId = Guid.Parse(Convert.ToString(row["ProjectId"]) ?? String.Empty)
            };

            return View(taskItemViewModel);
        }

        // POST: TaskController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TaskItemViewModel model)
        {
            userGuid = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? String.Empty);

            if (!validator.ValidateString(model.title))
            {
                ModelState.AddModelError("InvalidString", "Task name format is invalid.");
                return RegisterFailed(model);
            }
            else if (projectContainer.ProjectExists(model.title, userGuid, true) != false)
            {
                ModelState.AddModelError("ProjectExists", "Task already exists.");
                return RegisterFailed(model);
            }
            else
            {
                TaskItem taskItem = new()
                {
                    id = model.id,
                    title = model.title,
                    projectId = model.projectId,
                    status = (Models.TaskStatus)model.status,
                    description = model.description,
                    createdAt = DateTime.Now
                };

                ActivityLog log = new()
                {
                    id = Guid.NewGuid(),
                    action = $"Updated task contents, name and/or description.",
                    createdAt = DateTime.Now,
                    userId = userGuid,
                    project = null,
                    projectId = taskItem.projectId,
                    task = null,
                    taskId = taskItem.id
                };

                taskItemContainer.EditTask(taskItem);
                activityLogContainer.CreateActivityLog(log);

                return RedirectToAction("Details", "Project", new { id = model.projectId });
            }
        }
    }
}