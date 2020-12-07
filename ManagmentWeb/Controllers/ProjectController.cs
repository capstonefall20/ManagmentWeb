using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ManagmentWeb.Enum;
using ManagmentWeb.Models;
using ManagmentWeb.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManagmentWeb.Controllers
{
    public class ProjectController : SecureController
    {
        private IHostingEnvironment _env;
        private readonly IRepository _repository;
        public ProjectController(IRepository repository, IHostingEnvironment env)
        {
            this._env = env;
            _repository = repository;
        }
        public async Task<IActionResult> Projects()
        {
            var result = await this._repository.GetProjectsAsync(null,null,null);
            return View(result);
        }
        [Authorize(Roles = "Admin,Project Creator")]
        public async Task<IActionResult> AddProjects()
        {
            AddProjectModel addProject = new AddProjectModel();
           var result= await this._repository.GetUsersAsync(3);
            addProject.Users = result.Select(x => new SelectListItem { Text = x.FirstName + " " + x.LastName, Value = x.Id.ToString() }).ToList();
            return View(addProject);
        }
        [Authorize(Roles = "Admin,Project Creator")]
        [HttpPost]
        public async Task<IActionResult> AddProjects(AddProjectModel addProject)
        {
            DateTime start = DateTime.ParseExact(addProject.StartDate, "MM/dd/yyyy", CultureInfo.InvariantCulture); 
            DateTime due = DateTime.ParseExact(addProject.DueDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            if (start > due)
            {
                ViewBag.Error = "Due Date must be higher than start date";
                var result = await this._repository.GetUsersAsync(3);
                addProject.Users = result.Select(x => new SelectListItem { Text = x.FirstName + " " + x.LastName, Value = x.Id.ToString() }).ToList();
                return View(addProject);
            }
            string path= this._env.WebRootPath + "/Project";
            string filname = "";
            string fullfilepath = "";
            addProject.Filelist = new List<ProjectFileModel>();
            if (addProject.files!=null && addProject.files.Count>0)
            {
            foreach (var formFile in addProject.files)
            {
                if (formFile.Length > 0)
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    filname= Guid.NewGuid().ToString()+"."+ formFile.FileName.Split(".").LastOrDefault();
                    fullfilepath = Path.Combine(path, filname);
                    using (var stream = new FileStream(fullfilepath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                    addProject.Filelist.Add(new ProjectFileModel
                    {
                        Path = fullfilepath,
                        DisplayFileName = formFile.FileName,
                        FileName = filname
                    });
                }
            }
            }
            addProject.CreatedBy = this.UserId;
            
            if (await this._repository.AddProjectAsync(addProject))
            {
                if (addProject.ProjectId > 0)
                    ViewBag.Success = "Project is added";
                else
                    ViewBag.Success = "Project is updated";
            }
            else
                ViewBag.Error = "Some error occured while saving.";
            return View(addProject);
        }
        [Authorize(Roles = "Admin,Project Creator")]
        public async Task<IActionResult> AddPayment()
        {
            AddPaymentModel addPaymentModel = new AddPaymentModel();
            // 3 is used for worker
            ViewBag.User= await this._repository.GetUsersAsync(3);
            return View(addPaymentModel);
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Project Creator")]
        public async Task<IActionResult> AddPayment(AddPaymentModel addPaymentModel)
        {
            if (ModelState.IsValid)
            {
                if (await this._repository.AddPaymentAsync(addPaymentModel))
                {
                    ModelState.Clear();
                    addPaymentModel = new AddPaymentModel();
                    ViewBag.Success = "Payment success!";

                }
                else
                    ViewBag.Error = "Some error occured while saving.";
                ViewBag.User = await this._repository.GetUsersAsync(3);
            }
            else
            {
                ViewBag.User = await this._repository.GetUsersAsync(3);
            }
            return View(addPaymentModel);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentInfo()
        {
            var result = await this._repository.GetPaymentAsync(null,null,null);
            return View(result);
        }
        //[HttpGet]
        //public async Task<IActionResult> PaymentInfo(int? PaymentType, int? PaymentMethod,int?PayTo)
        //{
        //    var result = await this._repository.GetPaymentAsync(PaymentType, PaymentMethod,null);
        //    return PartialView("/Views/Shared/PartialViews/Project/_Payment.cshtml", result);
        //}
        [HttpGet]
        public async Task<IActionResult> PaymentInfoAjax(int? PaymentType, int? PaymentMethod, int? PayTo)
        {
            var result = await this._repository.GetPaymentAsync(PaymentType, PaymentMethod, null);
            return PartialView("/Views/Shared/PartialViews/Project/_Payment.cshtml", result);
        }
        [HttpGet]
        public async Task<IActionResult> ViewStatus()
        {
            ViewBag.Project = await this._repository.GetProjectsAsync(null,null, null);
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetViewStatus(int? ProjectId, int? Status)
        {
            var result = await this._repository.GetProjectsAsync(ProjectId, Status, null);
            return PartialView("/Views/Shared/PartialViews/Project/_Project.cshtml", result);
        }
        [HttpGet]
        public async Task<IActionResult> Tasks(int ProjectId,string Name)
        {
            ViewBag.ProjectId = ProjectId;
            ViewBag.Name = Name;
            var result = await this._repository.GetProjectTaskAsync(ProjectId);
            return View(result);
        }
        [HttpGet]
        [Authorize(Roles = "Admin,Project Creator")]
        public async Task<IActionResult> AddTask(int ProjectId)
        {
            ProjectTaskModel taskModel = new ProjectTaskModel();
            taskModel.ProjectId = ProjectId;
           
            var result = await this._repository.GetUsersAsync(3);
            taskModel.Users = result.Select(x => new SelectListItem { Text = x.FirstName + " " + x.LastName, Value = x.Id.ToString() }).ToList();
            return View(taskModel);
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Project Creator")]
        public async Task<IActionResult> AddTask(ProjectTaskModel taskModel)
        {
            DateTime start = DateTime.ParseExact(taskModel.StartDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            DateTime end = DateTime.ParseExact(taskModel.EndDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            if (start > end)
            {
                ViewBag.Error = "End Date can't be greater than start date";
                var result1 = await this._repository.GetUsersAsync(3);
                taskModel.Users = result1.Select(x => new SelectListItem { Text = x.FirstName + " " + x.LastName, Value = x.Id.ToString() }).ToList();
                return View(taskModel);
            }
            taskModel.CreatedBy = this.UserId;
            if (await this._repository.AddProjectTaskAsync(taskModel))
            {
                int projectid = taskModel.ProjectId;
                taskModel = new ProjectTaskModel();
                taskModel.ProjectId = projectid;
                ModelState.Clear();
                ViewBag.Success = "Task has been added";
            }
            else
            {
                ViewBag.Error = "There are error while saving";
            }
            var result = await this._repository.GetUsersAsync(3);
            taskModel.Users = result.Select(x => new SelectListItem { Text = x.FirstName + " " + x.LastName, Value = x.Id.ToString() }).ToList();
            return View(taskModel);
        }
        [HttpGet]
        public async Task<IActionResult> Milestone()
        {
            var user = await this._repository.GetUsersAsync(3);
            ViewBag.Users = user.Select(x => new SelectListItem { Text = x.FirstName + " " + x.LastName, Value = x.Id.ToString() }).ToList();
            var result = await this._repository.GetMilestoneAsync(null,null);
            return View(result);
        }
        [HttpGet]
        public async Task<IActionResult> MilestoneInfoAjax(int? ResponsiblePersonId)
        {
            var result = await this._repository.GetMilestoneAsync(null,ResponsiblePersonId);
            return PartialView("/Views/Shared/PartialViews/Project/_Milestone.cshtml", result);
        }
        [HttpGet]
        [Authorize(Roles = "Admin,Project Creator")]
        public async Task<IActionResult> AddMilestone()
        {
            MilestoneModel milestoneModel = new MilestoneModel();
           
            var result = await this._repository.GetUsersAsync(3);
            milestoneModel.Users = result.Select(x => new SelectListItem { Text = x.FirstName + " " + x.LastName, Value = x.Id.ToString() }).ToList();
            return View(milestoneModel);
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Project Creator")]
        public async Task<IActionResult> AddMilestone(MilestoneModel milestoneModel)
        {
            //DateTime start = DateTime.ParseExact(milestoneModel.StartDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            //DateTime expected = DateTime.ParseExact(milestoneModel.ExpectedDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            //DateTime due = DateTime.ParseExact(milestoneModel.DueDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

            milestoneModel.CreatedBy = this.UserId;
            if (await this._repository.AddMilestoneAsync(milestoneModel))
            {
                ModelState.Clear();
                milestoneModel = new MilestoneModel();
                ViewBag.Success = "Milestone has been added";
            }
            else
            {
                ViewBag.Error = "There are error while saving";
            }
            var result = await this._repository.GetUsersAsync(3);
            milestoneModel.Users = result.Select(x => new SelectListItem { Text = x.FirstName + " " + x.LastName, Value = x.Id.ToString() }).ToList();
            return View(milestoneModel);
        }
        [HttpGet]
        public async Task<IActionResult> NoteBook()
        {
            var result = await this._repository.GetNoteBookAsync(null);
            return View(result);
        }
        [HttpGet]
        public async Task<IActionResult> AddNoteBook()
        {
            NoteBookModel noteBookModel = new NoteBookModel();

            return View(noteBookModel);
        }
        [HttpPost]
        public async Task<IActionResult> AddNoteBook(NoteBookModel noteBookModel)
        {
            noteBookModel.CreatedBy = this.UserId;
            if (await this._repository.AddNoteBookAsync(noteBookModel))
            {
                ViewBag.Success = "NoteBook has been added";
                ModelState.Clear();
                noteBookModel = new NoteBookModel();
            }
            else
            {
                ViewBag.Error = "There are error while saving";
            }
            return View(noteBookModel);
        }
    }
}