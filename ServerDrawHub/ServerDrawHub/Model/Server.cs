using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ServerDrawHub.Model
{
    public class Server
    {
        private TcpListener serverListener;
        public bool isRunning = false;
        public Dictionary<string, Room> rooms = new Dictionary<string, Room>(); // Sử dụng Dictionary để quản lý phòng theo RoomId
        private Func<string, Task> broadcastMessage;
        
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler ClientDisconnected;
        public event Action<string> CreatedRoom;


        public Server(Func<string, Task> broadcastMessage)
        {
            this.broadcastMessage = broadcastMessage;
            rooms = new Dictionary<string, Room>();
            
        }



        // Khởi động server
        public async Task Start(int port)
        {
            if (isRunning) return;

            try
            {
                serverListener = new TcpListener(IPAddress.Any, port);
                serverListener.Start();
                isRunning = true;

                while (isRunning)
                {
                    Socket clientSocket = await serverListener.AcceptSocketAsync();

                    //khi có một client tới tạo một clientHandler mới
                    ClientHandler clientHandler = new ClientHandler(clientSocket, broadcastMessage);
                    clientHandler.MessageReceived += ClientHandler_MessageReceived;
                    clientHandler.ClientDisconnected += ClientHandler_ClientDisconnected;
                    
                    
                    Task.Run(() => clientHandler.HandleClientAsync());

                }
            }
            catch (Exception ex)
            {

            }
        }

        private void UpdateRichTextBox(string message)
        {

        }

        //xử lý khí client ngắt kết nối
        private void ClientHandler_ClientDisconnected(object sender, EventArgs e)
        {
            
        }

        //xử lý khi có tin nhắn mới 
        private void ClientHandler_MessageReceived(object sender, MessageReceivedEventArgs e)
        {

            var clientHandler = sender as ClientHandler;
            if (clientHandler != null)
            {
                string message = e.message;

                // Kiểm tra loại yêu cầu: tạo phòng
                if (message.StartsWith("roomid:"))
                {
                    // Lấy RoomId từ tin nhắn
                    string roomId = message.Split(':')[1].Trim();
                    

                    // Gọi phương thức CreateRoom để xử lý yêu cầu tạo phòng
                    CreateRoom(clientHandler, roomId);
                }
                else
                {
                    
                    //truyền tin nhắn đến các phòng tương ứng
                    foreach (var room in rooms.Values)
                    {
                        room.BroadcastMessageAsync(e.message);
                    }
                }
            }
           
        }

        private void UpdteRichtextbox(string messeage)
        {
           
        }

        public void CreateRoom(ClientHandler clientHandler ,string RoomId)
        {
            if (!rooms.ContainsKey(RoomId))
            {
                rooms[RoomId] = new Room(RoomId);
                // Nếu phòng chưa tồn tại, tạo mới phòng và thêm client vào phòng
                
                clientHandler.RoomIdCurrent = RoomId;
                rooms[RoomId].AddClient(clientHandler.ClientSocket);

                //// Thông báo client đã tạo phòng thành công
                //string successMessage = $"Room {RoomId} created successfully!";
                //SendMessageToClient(clientHandler, successMessage);
                CreatedRoom?.Invoke($"Room {RoomId} created successfully!");

            }
            else
            {
                JoinRoom(RoomId, clientHandler.ClientSocket);
            } 
                
        }




        // Phương thức này sẽ gửi tin nhắn đến tất cả các phòng trong server
        private async Task BroadcastMessageAsync(string message)
        {
            foreach (var room in rooms.Values)
            {
                // Gửi tin nhắn đến tất cả các client trong mỗi phòng
                await room.BroadcastMessageAsync(message);
            }
        }




        public void JoinRoom(string RooomId, Socket clientsocket)
        {
            if (rooms.ContainsKey(RooomId))
            {
                rooms[RooomId].AddClient(clientsocket);
            }
        }

        public void LeaveRoom(string RoomId, Socket clientsocket)
        {
            if (rooms.ContainsKey(RoomId))
                rooms[RoomId].RemoveClient(clientsocket);
        }

        // Dừng server
        public void Stop()
        {
            isRunning = false;
            foreach (var room in rooms.Values)
            {
                foreach(var client in room.clients)
                {
                    client.Close();
                }
            }
            serverListener.Stop();
        }

    }

}
