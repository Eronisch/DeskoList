using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;

namespace Core.Business.Connection
{
    /// <summary>
    /// Manager for testing external services
    /// </summary>
    public static class ConnectionTestService
    {
        /// <summary>
        /// Tries to connect to the database
        /// </summary>
        /// <param name="host"></param>
        /// <param name="database"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool IsValidDatabaseConnection(string host, string database, string username, string password)
        {
            string connectionString =
                string.Format("Server={0};Database={1};User Id={2};Password={3};", host, database, username, password);

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (SqlException ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Tries to connect to the email service
        /// </summary>
        /// <param name="smtpServerAddress"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool IsValidEmailConnection(string smtpServerAddress, string port)
        {
            IPAddress ipOut;
            int portOut;

            if (IPAddress.TryParse(smtpServerAddress, out ipOut) && int.TryParse(port, out portOut))
            {
                IPEndPoint endPoint = new IPEndPoint(ipOut, portOut);

                using (Socket tcpSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    try
                    {
                        tcpSocket.Connect(endPoint);
                        return tcpSocket.Connected;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            return false;
        }
    }
}
