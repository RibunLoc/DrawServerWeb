using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ServerDrawHub.Model
{
    public class ClientHandler
    {
        public Socket ClientSocket;
        public Func<string, Task> broadcastMessage;
        public bool isrunning;
        public string RoomIdCurrent;

        public EventHandler ClientDisconnected;
        public EventHandler<MessageReceivedEventArgs> MessageReceived;

        public ClientHandler(Socket ClientSocket, Func<string, Task> broadcastMessage)
        {
            this.ClientSocket = ClientSocket;
            this.broadcastMessage = broadcastMessage;

        }

        //Xử lý tin nhắn (nhận tin nhắn và gửi trả)
        public async Task HandleClientAsync()
        {
            isrunning = true;
            try
            {
                IPEndPoint remoteIpEndPoint = (IPEndPoint)ClientSocket.RemoteEndPoint;
                int clientPort = remoteIpEndPoint.Port;
                string clientAddress = remoteIpEndPoint.Address.ToString();

                //MessageReceived?.Invoke(this, new MessageReceivedEventArgs($"Kết nối: {clientAddress}:{clientPort}"));

                byte[] buffer = new byte[1024];

                while (isrunning)
                {
                    int byteReceived = await ClientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                    if (byteReceived == 0) // client mất kết nối
                    {
                        OnClientDisconnected();
                        break;
                    }

                    string message = Encoding.ASCII.GetString(buffer, 0, byteReceived);


                     MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
              
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (ClientSocket != null && ClientSocket.Connected)
                {
                    ClientSocket.Close();
                }
            }
        }

        public void JoinRoom(string RoomId)
        {
            RoomIdCurrent = RoomId;
        }

        public void LeaveRoom()
        {
            RoomIdCurrent = null;
        }

        
        private void OnClientDisconnected()
        {
            isrunning = false;
            ClientDisconnected?.Invoke(this, EventArgs.Empty);
        }

    }


}
