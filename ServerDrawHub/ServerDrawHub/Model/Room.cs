using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerDrawHub.Model
{
    public class Room
    {
        public string RoomId { get; set; }
        public List<Socket> clients;  // Danh sách các client trong phòng
        private readonly object clientLock = new object();
        private List<string> drawingData; // Dữ liệu vẽ trong phòng (danh sách lệnh vẽ)
        public Room(string roomId)
        {
            RoomId = roomId;
            clients = new List<Socket>();
            drawingData = new List<string>();
        }

        // Thêm client vào phòng
        public async Task AddClient(Socket clientSocket)
        {
            if (!clients.Contains(clientSocket))
            {
                clients.Add(clientSocket);
            }

            // Gửi trạng thái bản vẽ hiện tại cho client mới
            await SendExistingDrawingDataAsync(clientSocket);
        }

        // Xóa client khỏi phòng
        public void RemoveClient(Socket clientSocket)
        {
            clients.Remove(clientSocket);

        }

        // Gửi tin nhắn đến tất cả các client trong phòng
        public async Task BroadcastMessageAsync(string message, Socket senderSocket)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            List<Socket> clientsSnapshot;

            // Tạo một bản sao của danh sách client để tránh giữ khóa lâu
            lock (clientLock)
            {
                clientsSnapshot = new List<Socket>(clients);
            }

            // Gửi tin nhắn tới các client
            foreach (var client in clientsSnapshot)
            {
                try
                {
                    if (client.Connected && client != senderSocket)
                    {
                        await client.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send message to client in room {RoomId}: {ex.Message}");
                }
            }

            // Lưu dữ liệu vẽ nếu là một thông điệp vẽ
            if (message.StartsWith("draw:") || message.StartsWith("erase:"))
            {
                lock (drawingData)
                {
                    drawingData.Add(message);
                }
            }
        }

        public async Task RemoveClientOutOfRoom(Socket senderSocket, string username)
        {
          
            string ThongBaoToiClientKhac = $"chat:{username} Đã thoát khỏi phòng!\n";

           await BroadcastMessageAsync(ThongBaoToiClientKhac, senderSocket);

            lock (clientLock)
            {
                //Xoa nguoi dung
                clients.Remove(senderSocket);

            }    
           

        }

        // Lấy danh sách các client trong phòng
        public List<Socket> GetClients()
        {
            return new List<Socket>(clients);
        }

        // Gửi dữ liệu bản vẽ hiện tại tới client mới
        public async Task SendExistingDrawingDataAsync(Socket clientSocket)
        {
            List<string> snapshotDrawingData;

            lock (drawingData)
            {
                snapshotDrawingData = new List<string>(drawingData); // Tạo bản sao để giải phóng khóa ngay
            }

            foreach (var data in snapshotDrawingData)
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(data);
                await clientSocket.SendAsync(new ArraySegment<byte>(messageBytes), SocketFlags.None);
            }
        }


        // Thêm dữ liệu vẽ mới vào danh sách
        public void AddDrawingData(string data)
        {
            lock (drawingData)
            {
                drawingData.Add(data);
            }
        }
    }

}
