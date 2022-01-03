using api.Models.ViewModels.Hubs.Chat;
using System.Threading.Tasks;

namespace api.Hubs.Clients
{
    public interface IChatClient
    {
        Task ReceiveMessage(HubMessageViewModel message);
    }
}
