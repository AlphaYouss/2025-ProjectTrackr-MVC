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
        private Validator validator = new Validator();

        private ProjectContainer projectContainer { get; set; }
        private UserContainer userContainer { get; set; }
        private ActivityLogContainer activityLogContainer { get; set; }
        private TaskItemContainer taskItemContainer { get; set; }

        private Guid userGuid { get; set; }

        public TaskController(IConfiguration configuration)
        {
            projectContainer = new ProjectContainer(new ProjectDAL(configuration));
            userContainer = new UserContainer(new UserDAL(configuration));
            activityLogContainer = new ActivityLogContainer(new ActivityLogDAL(configuration));
            taskItemContainer = new TaskItemContainer(new TaskItemDAL(configuration));
        }

        // GET: TaskController
        public ActionResult View(Guid id)
        {
            TaskItem taskItem = new TaskItem();
            DataTable tableTasks = taskItemContainer.GetTaskDetails(id);
            
            for (int i = 0; i < tableTasks.Rows.Count; i++)
            {
                taskItem.id = Guid.Parse(tableTasks.Rows[i]["ID"].ToString());
                taskItem.title = tableTasks.Rows[i]["Title"].ToString();
                taskItem.description = tableTasks.Rows[i]["Description"].ToString();
                taskItem.status = (Models.TaskStatus)tableTasks.Rows[i]["Status"];
                taskItem.createdAt = (DateTime)tableTasks.Rows[i]["CreatedAt"];
                taskItem.projectId = Guid.Parse(tableTasks.Rows[i]["ID"].ToString());
            }

            return View(taskItem);
        }

        // GET: TaskController/Create
        [HttpGet]
        public ActionResult Create(Guid id)
        {
            TaskItemViewModel taskItemViewModel = new TaskItemViewModel
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
            userGuid = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (!checkInputs(model))
                return RegisterFailed(model);
            else if (taskItemContainer.TaskExists(model.title) == true)
            {
                ModelState.AddModelError("ProjectExists", "Task already exists.");
                return RegisterFailed(model);
            }
            else
            {
                CreateTask(model);

                return RedirectToAction("Details", "Project", new {id = model.projectId});
            }
        }

        private bool checkInputs(TaskItemViewModel model)
        {
            if (!validator.ValidateStrings(model.title))
            {
                ModelState.AddModelError("InvalidString", "Task title format is invalid.");
                return false;
            }
            else
                return true;
        }

        private ActionResult RegisterFailed(TaskItemViewModel model)
        {
            TempData["Errors"] = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

            return View(model);
        }

        private void CreateTask(TaskItemViewModel model)
        {
            userGuid = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            TaskItem item = new TaskItem();
            item.id = Guid.NewGuid();
            item.title = model.title;
            item.projectId = model.projectId;
            item.status = (Models.TaskStatus)model.status;
            item.description = model.description;   
            item.createdAt = DateTime.Now;

            taskItemContainer.CreateTaskItem(item);

            ActivityLog activityLog = new ActivityLog();
            activityLog.id = Guid.NewGuid();
            activityLog.createdAt = DateTime.Now;
            activityLog.userId = userGuid;
            activityLog.user = null;
            activityLog.projectId = model.projectId;
            activityLog.taskId = item.id;
            activityLog.task = item;
            activityLog.action = $"Created task '{item.title}'.";

            activityLogContainer.CreateActivityLog(activityLog);
        }


        // GET: TaskController/Edit/(ID)
        public ActionResult Edit(Guid id)
        {
            DataTable tableTask = taskItemContainer.GetTaskDetails(id);

            TaskItemViewModel taskItemViewModel = new TaskItemViewModel();

            for (int i = 0; i < tableTask.Rows.Count; i++)
            {
                taskItemViewModel.id = Guid.Parse(tableTask.Rows[i]["ID"].ToString());
                taskItemViewModel.title = tableTask.Rows[i]["Title"].ToString();
                taskItemViewModel.description = tableTask.Rows[i]["Description"].ToString();
                taskItemViewModel.status = (TaskItemViewModel.TaskStatus)(Models.TaskStatus)tableTask.Rows[i]["Status"];
                taskItemViewModel.createdAt = (DateTime)tableTask.Rows[i]["CreatedAt"];
                taskItemViewModel.projectId = Guid.Parse(tableTask.Rows[i]["ProjectId"].ToString());
            }

            return View(taskItemViewModel);
        }

        // POST: TaskController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TaskItemViewModel model)
        {
            userGuid = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (!validator.ValidateStrings(model.title))
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
                TaskItem taskItem = new TaskItem();
                taskItem.id = model.id;
                taskItem.title = model.title;
                taskItem.projectId = model.projectId;
                taskItem.status = (Models.TaskStatus)model.status;
                taskItem.description = model.description;
                taskItem.createdAt = DateTime.Now;

                ActivityLog log = new ActivityLog();
                log.id = Guid.NewGuid();
                log.action = $"Updated task contents, name and/or description.";
                log.createdAt = DateTime.Now;
                log.userId = userGuid;
                log.project = null;
                log.projectId = taskItem.projectId;
                log.task = null;
                log.taskId = taskItem.id;

                taskItemContainer.EditTask(taskItem);
                activityLogContainer.CreateActivityLog(log);

                return RedirectToAction("Details", "Project", new { id = model.projectId });
            }
        }
    }
}