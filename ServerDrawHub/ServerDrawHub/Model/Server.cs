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

        public Server()
        {
            
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
                    string name = message.Split(':')[2].Trim();
                    

                    // Gọi phương thức CreateRoom để xử lý yêu cầu tạo phòng
                    CreateRoom(clientHandler, roomId);
                    // Thông báo lại tới người dùng khác trong phòng
                    string msg = $"chat: {name} đã tham gia phòng vẽ \n"; //cú pháp: lenh:noidung \n
                    if (rooms.ContainsKey(clientHandler.RoomIdCurrent))
                    {
                        rooms[clientHandler.RoomIdCurrent].BroadcastMessageAsync(msg, clientHandler.ClientSocket);
                    }    
                }
                else if (message.StartsWith("EXIT"))
                {
                    string username = message.Split(':')[1].Trim();
                    // đóng kết nối và xóa người dùng
                    if(rooms.ContainsKey(clientHandler.RoomIdCurrent))
                    {
                        rooms[clientHandler.RoomIdCurrent].RemoveClientOutOfRoom(clientHandler.ClientSocket, username);
                    }
                }
                else 
                {
                    // Chỉ phát tin nhắn tới phòng mà client thuộc về
                    if (rooms.ContainsKey(clientHandler.RoomIdCurrent))
                    {
                        rooms[clientHandler.RoomIdCurrent].BroadcastMessageAsync(message, clientHandler.ClientSocket);
                    }
                }
                
            }
           
        }

        private void UpdteRichtextbox(string messeage)
        {
           
        }

        public void CreateRoom(ClientHandler clientHandler, string roomId)
        {
            if (!rooms.ContainsKey(roomId))
            {
                rooms[roomId] = new Room(roomId);
                CreatedRoom?.Invoke($"Room {roomId} created successfully!");
            }
            JoinRoom(roomId, clientHandler);


        }
       
        

        public async Task BroadcastMessageToRoomAsync(string roomId, string message, Socket socketSender)
        {
            if (rooms.ContainsKey(roomId))
            {
                await rooms[roomId].BroadcastMessageAsync(message, socketSender);
            }
        }


        public void JoinRoom(string roomId, ClientHandler clientHandler)
        {
            if (rooms.ContainsKey(roomId))
            {
                rooms[roomId].AddClient(clientHandler.ClientSocket);
                clientHandler.RoomIdCurrent = roomId; // Cập nhật phòng hiện tại cho client
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
