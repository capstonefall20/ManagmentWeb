using ManagmentWeb.Models.Entity;
using ManagmentWeb.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Threading.Tasks;
namespace ManagmentWeb
{
    public class SignalRChatHub : Hub
    {
        public static int CurrentUserID;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public int UserId
        {
            get
            {
                return _httpContextAccessor.HttpContext.User.GetUserId(); 
            }
        }
        public SignalRChatHub(IRepository repository
            , IHttpContextAccessor httpContextAccessor
            , UserManager<ApplicationUser> userManager
            )
        {
            this._userManager = userManager;
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }
        public override async Task OnConnectedAsync()
        {
            var name = Context.User.Identity.Name;
            var user = await _userManager.GetUserAsync(Context.User);
            CurrentUserID = UserId;
            var id = Context.ConnectionId + "_" + UserId.ToString();
            await this._repository.AddUserChatAsync(CurrentUserID, id);
            string jsonUsers = JsonConvert.SerializeObject(await this._repository.GetUserChatAsync(null));
            await Clients.All.SendAsync("GetAllUsers", jsonUsers);
            await base.OnConnectedAsync();
        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public async Task SendPrivateMessage(string fromUserId, string message)
        {
            int usertoconectid = 1;
            string name = "";
            string FromUserId = "";
            string ToUserId = "";
            string status = "";
          await  Clients.Client(usertoconectid.ToString()).SendAsync(fromUserId, name, message, FromUserId, ToUserId, status, fromUserId, false);
        }
    }
    public class PrivateChatMessage
    {
        public string userName { get; set; }
        public string message { get; set; }
        public string FilePath { get; set; }
        public bool IsMe { get; set; }
        public string Date { get; set; }

    }
}
