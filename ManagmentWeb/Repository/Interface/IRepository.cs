using ManagmentWeb.Models;
using ManagmentWeb.Models.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagmentWeb.Repository.Interface
{
  public  interface IRepository
    {
        Task<List<RoleModel>> GetRolesAsync();
        Task<List<UserModel>> GetUsersAsync(int? RoleId);
        #region Projects
        Task<bool> AddProjectAsync(AddProjectModel addProject);
        Task<List<ProjectsModel>> GetProjectsAsync(int?ProjectId, int? Status, string? Name);
        #endregion
        #region Payment
        Task<bool> AddPaymentAsync(AddPaymentModel addPayment);
        Task<List<PaymentInfoModel>> GetPaymentAsync(int? PaymentType, int? PaymentMethod, int? PayTo);
        #endregion
        #region Tasks
        Task<List<ProjectTaskModel>> GetProjectTaskAsync(int ProjectId);
        Task<bool> AddProjectTaskAsync(ProjectTaskModel addTask);
        #endregion
        #region Milestone
        Task<List<MilestoneModel>> GetMilestoneAsync(int? MilestoneId, int? ResponsiblePersonId);
        Task<bool> AddMilestoneAsync(MilestoneModel model);
        #endregion

        #region NoteBook
        Task<List<NoteBookModel>> GetNoteBookAsync(int? NoteBookId);
        Task<bool> AddNoteBookAsync(NoteBookModel model);
        #endregion
        #region LogTime
        Task<List<LogTimeModel>> GetLogTimeAsync();
        Task<bool> AddLogTimeAsync(LogTimeModel model);
        #endregion

        #region Chat
        Task<bool> AddUserChatAsync(int UserId, string ConnectionId);
        Task<List<UserChatModel>> GetUserChatAsync(int? UserId);
        #endregion
    }
}
