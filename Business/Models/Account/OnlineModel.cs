using System.Net.Sockets;

namespace Core.Models.Account
{
    public class OnlineModel
    {
        public OnlineModel(int websiteId, Socket socket)
        {
            WebsiteId = websiteId;
            ClientSocket = socket;
        }

        public Socket ClientSocket { get; set; }
        public int WebsiteId { get; private set; }
    }
}
