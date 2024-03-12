using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace FRIWOCenter.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            //await Clients.All.SendAsync("ReceiveMessage", user, message);
            await Clients.User(user).SendAsync(message);
        }

        public async Task ReceiveMessage(string user, string message)
        {
            await Clients.User(user).SendAsync(message);
        }
    }
}
