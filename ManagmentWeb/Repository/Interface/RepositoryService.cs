using Dapper;
using ManagmentWeb.Models;
using ManagmentWeb.Models.Chat;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ManagmentWeb.Repository.Interface
{
    public class RepositoryService: IRepository
    {
        private SqlConnection GetConnection()
        {
            return new SqlConnection(AppSetting.ConnectionString);
        }
        public async Task<List<RoleModel>> GetRolesAsync()
        {
            using (SqlConnection conn = GetConnection())
            {
                var list = new List<RoleModel>();
                var reader = await conn.QueryAsync<RoleModel>("sp_GetRoles", commandType: CommandType.StoredProcedure);
                
                if (reader != null)
                    list = reader.ToList();
                return list;
            }
        }
        public async Task<List<UserModel>> GetUsersAsync(int? RoleId)
        {
            using (SqlConnection conn = GetConnection())
            {
                DynamicParameters param = new DynamicParameters();
                if (RoleId > 0)
                    param.Add("@RoleId", RoleId);
                var list = new List<UserModel>();
                var reader = await conn.QueryAsync<UserModel>("sp_GetUsers", param, commandType: CommandType.StoredProcedure);

                if (reader != null)
                    list = reader.ToList();
                return list;
            }
        }
        #region Projects
        public async Task<bool> AddProjectAsync(AddProjectModel addProject)
        {

            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                var transaction = conn.BeginTransaction();
                try
                {
                    DynamicParameters param = new DynamicParameters();
                    if (addProject.ProjectId > 0)
                        param.Add("@ProjectId", addProject.ProjectId);
                    param.Add("@Name", addProject.Name);
                    param.Add("@Description", addProject.Description);
                    param.Add("@StartDate", addProject.StartDate);
                    param.Add("@DueDate", addProject.DueDate);
                    param.Add("@CreatedBy", addProject.CreatedBy);
                    param.Add("@Priority", addProject.Priority);
                    int ProjectId = await transaction.Connection.ExecuteScalarAsync<int>("sp_AddProject", param, transaction: transaction, commandType: CommandType.StoredProcedure);
                    if (addProject.Filelist.Count > 0)
                    {
                        foreach (var item in addProject.Filelist)
                        {
                            param = new DynamicParameters();
                            param.Add("@ProjectId", ProjectId);
                            param.Add("@DisplayFileName", item.DisplayFileName);
                            param.Add("@FileName", item.FileName);
                            param.Add("@Path", item.Path);
                            await transaction.Connection.ExecuteScalarAsync<int>("sp_AddProjectDocument", param, transaction: transaction, commandType: CommandType.StoredProcedure);
                        }

                    }
                    if (addProject.UsersId!=null && addProject.UsersId.Count()>0)
                    {
                        param = new DynamicParameters();
                        param.Add("@ProjectId", ProjectId);
                        await transaction.Connection.ExecuteScalarAsync<int>("sp_DeleteProjectUser", param, transaction: transaction, commandType: CommandType.StoredProcedure);
                        
                        foreach (var item in addProject.UsersId)
                        {
                            param = new DynamicParameters();
                            param.Add("@ProjectId", ProjectId);
                            param.Add("@UserId", item);
                            await transaction.Connection.ExecuteScalarAsync<int>("sp_AddProjectUser", param, transaction: transaction, commandType: CommandType.StoredProcedure);
                        }
                    }
                    transaction.Commit();
                    
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }
           
            
        }
        public async Task<List<ProjectsModel>> GetProjectsAsync(int? ProjectId=null, int? Status=null, string? Name=null)
        {
            using (SqlConnection conn = GetConnection())
            {
                DynamicParameters param = new DynamicParameters();
                var list = new List<ProjectsModel>();
                if (ProjectId!=null && ProjectId>0)
                    param.Add("@ProjectId", ProjectId);
                if (Status != null && Status > 0)
                    param.Add("@Status", Status);
                if (!string.IsNullOrEmpty(Name))
                    param.Add("@Name", Name);
                var reader = await conn.QueryAsync<ProjectsModel>("sp_GetProjects", param, commandType: CommandType.StoredProcedure);

                if (reader != null)
                    list = reader.ToList();
                return list;
            }
        }
        #endregion
        #region Payment
        public async Task<bool> AddPaymentAsync(AddPaymentModel addPayment)
        {
            try
            {
                decimal total = addPayment.PaymentType==4? addPayment.Total:(addPayment.Rate*addPayment.Hours);
                using (SqlConnection conn = GetConnection())
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@Hours", addPayment.Hours);
                    param.Add("@PaymentMethod", addPayment.PaymentMethod);
                    param.Add("@PaymentType", addPayment.PaymentType);
                    param.Add("@PayTo", addPayment.PayTo);
                    param.Add("@CreatedBy", addPayment.CreatedBy);
                    param.Add("@Total", addPayment.Total);
                    
                    var reader = await conn.ExecuteScalarAsync<long>("sp_AddPayment", param, commandType: CommandType.StoredProcedure);
                    return true;
                }
            }
            catch (Exception ex)
            {

                return false;
            }

        }
        public async Task<List<PaymentInfoModel>> GetPaymentAsync(int? PaymentType, int? PaymentMethod, int? PayTo)
        {
            using (SqlConnection conn = GetConnection())
            {
                var list = new List<PaymentInfoModel>();
                DynamicParameters param = new DynamicParameters();
                if (PayTo!=null && PayTo>0)
                    param.Add("@PayTo", PayTo);
                if (PaymentType != null)
                    param.Add("@PaymentType", PaymentType);
                if (PaymentMethod != null)
                    param.Add("@PaymentMethod", PaymentMethod);
                var reader = await conn.QueryAsync<PaymentInfoModel>("sp_GetPayment", param, commandType: CommandType.StoredProcedure);

                if (reader != null)
                    list = reader.ToList();
                return list;
            }
        }
        public async Task<List<ProjectTaskModel>> GetProjectTaskAsync(int ProjectId)
        {
            using (SqlConnection conn = GetConnection())
            {
                var list = new List<ProjectTaskModel>();
                DynamicParameters param = new DynamicParameters();
                if (ProjectId > 0)
                    param.Add("@ProjectId", ProjectId);
                var reader = await conn.QueryAsync<ProjectTaskModel>("sp_GetProjectTask", param, commandType: CommandType.StoredProcedure);

                if (reader != null)
                    list = reader.ToList();
                return list;
            }
        }
        public async Task<bool> AddProjectTaskAsync(ProjectTaskModel taskModel)
        {
            
                using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                var transaction = conn.BeginTransaction();
                try
                {
                   
                    var list = new List<ProjectTaskModel>();
                    DynamicParameters param = new DynamicParameters();
                    if (taskModel.TaskId > 0)
                        param.Add("@TaskId", taskModel.TaskId);

                    param.Add("@ProjectId", taskModel.ProjectId);
                    param.Add("@Name", taskModel.Name);
                    param.Add("@StartDate", taskModel.StartDate);
                    param.Add("@EndDate", taskModel.EndDate);
                    param.Add("@CreatedBy", taskModel.CreatedBy);
                    int TaskId = await transaction.Connection.ExecuteScalarAsync<int>("sp_AddTask", param, transaction: transaction, commandType: CommandType.StoredProcedure);
                    if (taskModel.UsersId != null && taskModel.UsersId.Count() > 0)
                    {
                        param = new DynamicParameters();
                        param.Add("@TaskId", TaskId);
                        await transaction.Connection.ExecuteScalarAsync<int>("sp_DeleteTaskUser", param, transaction: transaction, commandType: CommandType.StoredProcedure);

                        foreach (var item in taskModel.UsersId)
                        {
                            param = new DynamicParameters();
                            param.Add("@TaskId", TaskId);
                            param.Add("@UserId", item);
                            await transaction.Connection.ExecuteScalarAsync<int>("sp_AddTaskUser", param, transaction: transaction, commandType: CommandType.StoredProcedure);
                        }
                    }
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }
            
            
        }

        public async Task<List<MilestoneModel>> GetMilestoneAsync(int? MilestoneId,int? ResponsiblePersonId)
        {
            using (SqlConnection conn = GetConnection())
            {
                var list = new List<MilestoneModel>();
                DynamicParameters param = new DynamicParameters();
                if (MilestoneId > 0)
                    param.Add("@MilestoneId", MilestoneId);
                if (ResponsiblePersonId > 0)
                    param.Add("@ResponsiblePersonId", ResponsiblePersonId);
                var reader = await conn.QueryAsync<MilestoneModel>("sp_GetMilestone", param, commandType: CommandType.StoredProcedure);

                if (reader != null)
                    list = reader.ToList();
                return list;
            }
        }
        public async Task<bool> AddMilestoneAsync(MilestoneModel milestone)
        {

            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                var transaction = conn.BeginTransaction();
                try
                {

                    var list = new List<ProjectTaskModel>();
                    DynamicParameters param = new DynamicParameters();
                    if (milestone.MileStoneId > 0)
                        param.Add("@MileStoneId", milestone.MileStoneId);

                    param.Add("@Name", milestone.Name);
                    param.Add("@DueDate", milestone.DueDate);
                    param.Add("@StartDate", milestone.StartDate);
                    param.Add("@ExpectedDate", milestone.ExpectedDate);
                    param.Add("@ResponsiblePersonId", milestone.ResponsiblePersonId);
                    param.Add("@CreatedBy", milestone.CreatedBy);
                    int TaskId = await transaction.Connection.ExecuteScalarAsync<int>("sp_AddMilestone", param, transaction: transaction, commandType: CommandType.StoredProcedure);
                 
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }


        }

        public async Task<List<NoteBookModel>> GetNoteBookAsync(int? NoteBookId)
        {
            using (SqlConnection conn = GetConnection())
            {
                var list = new List<NoteBookModel>();
                DynamicParameters param = new DynamicParameters();
                if (NoteBookId > 0)
                    param.Add("@NoteBookId", NoteBookId);
                var reader = await conn.QueryAsync<NoteBookModel>("sp_GetNoteBook", param, commandType: CommandType.StoredProcedure);

                if (reader != null)
                    list = reader.ToList();
                return list;
            }
        }
        public async Task<bool> AddNoteBookAsync(NoteBookModel noteBook)
        {

            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                var transaction = conn.BeginTransaction();
                try
                {
                    var list = new List<ProjectTaskModel>();
                    DynamicParameters param = new DynamicParameters();
                    if (noteBook.NoteBookId > 0)
                        param.Add("@NoteBookId", noteBook.NoteBookId);

                    param.Add("@Title", noteBook.Title);
                    param.Add("@Description", noteBook.Description);
                    param.Add("@CreatedBy", noteBook.CreatedBy);
                    int TaskId = await transaction.Connection.ExecuteScalarAsync<int>("sp_AddNoteBook", param, transaction: transaction, commandType: CommandType.StoredProcedure);

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }


        }
        public async Task<List<LogTimeModel>> GetLogTimeAsync()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    var list = new List<LogTimeModel>();

                    var reader = await conn.QueryAsync<LogTimeModel>("sp_GetLogTime", commandType: CommandType.StoredProcedure);

                    if (reader != null)
                        list = reader.ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        
        }
        public async Task<bool> AddLogTimeAsync(LogTimeModel LogTime)
        {

            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                var transaction = conn.BeginTransaction();
                try
                {
                    var list = new List<ProjectTaskModel>();
                    DynamicParameters param = new DynamicParameters();
                    if (LogTime.LogId > 0)
                        param.Add("@LogId", LogTime.LogId);

                    param.Add("@UserId", LogTime.UserId);
                    param.Add("@LogDate", LogTime.LogDate);
                    param.Add("@StartTime", LogTime.StartTime);
                    param.Add("@EndTime", LogTime.EndTime);
                    param.Add("@CreatedBy", LogTime.CreatedBy);
                    int TaskId = await transaction.Connection.ExecuteScalarAsync<int>("sp_AddLogTime", param, transaction: transaction, commandType: CommandType.StoredProcedure);

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }


        }
        #endregion
        #region Chat
        public async Task<bool> AddUserChatAsync(int UserId, string ConnectionId)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                var transaction = conn.BeginTransaction();
                try
                {
                    DynamicParameters param = new DynamicParameters();
                     
                    param.Add("@UserId", UserId);
                    param.Add("@ConnectionId", ConnectionId);
                    int TaskId = await transaction.Connection.ExecuteScalarAsync<int>("sp_AddUserChat", param, transaction: transaction, commandType: CommandType.StoredProcedure);

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }


        }
        public async Task<List<UserChatModel>> GetUserChatAsync(int? UserId)
        {
            using (SqlConnection conn = GetConnection())
            {
                var list = new List<UserChatModel>();
                DynamicParameters param = new DynamicParameters();
                if (UserId != null && UserId > 0)
                    param.Add("@UserId", UserId);
               
                var reader = await conn.QueryAsync<UserChatModel>("sp_GetUserChat", param, commandType: CommandType.StoredProcedure);

                if (reader != null)
                    list = reader.ToList();
                return list;
            }
        }
        #endregion
    }
}
