using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Core.Business.Settings;
using Core.Business.Websites;
using Core.Models.Account;

namespace Core.Business.Ping
{
    /// <summary>
    /// Async pinging manager
    /// </summary>
    public class PingService
    {
        private readonly WebsiteService _websiteService;

        // todo: Make this a database setting
        private const int MAX_AMOUNT_PARALLEL = 5;

        public PingService()
        {
            _websiteService = new WebsiteService();
        }

        /// <summary>
        /// Ping user servers
        /// Maximum of 5 servers at once
        /// Uses the tcp protocol
        /// </summary>
        public void Start()
        {
            Parallel.ForEach(_websiteService.GetAllWebsites(includeBanned: false).ToList(),
                new ParallelOptions { MaxDegreeOfParallelism = MAX_AMOUNT_PARALLEL },
                PingServer);
        }

        /// <summary>
        /// Ping a specific server
        /// </summary>
        /// <param name="website"></param>
        public void PingServer(Database.Entities.Websites website)
        {
            if (!string.IsNullOrEmpty(website.ServerIP) && website.ServerPort.HasValue)
            {
                PingServer(website.Id, website.ServerIP, (int)website.ServerPort);
            }
        }

        private void PingServer(int websiteId, string serverIp, int serverPort)
        {
            var ipAddress = IPAddress.Parse(serverIp);
            var remoteEp = new IPEndPoint(ipAddress, serverPort);

            var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            client.BeginConnect(remoteEp, PingCompletedCallback, new OnlineModel(websiteId, client));
        }

        private void PingCompletedCallback(IAsyncResult ar)
        {
            var onlineModel = (OnlineModel)ar.AsyncState;
            bool isOnline = true;

            try
            {
                onlineModel.ClientSocket.EndConnect(ar);
            }
            catch
            {
                isOnline = false;
            }
            finally
            {
                onlineModel.ClientSocket.Dispose();
            }

            _websiteService.UpdateMonitor(onlineModel.WebsiteId, isOnline);
        }

    }
}
