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
        public Room(string roomId)
        {
            RoomId = roomId;
            clients = new List<Socket>();
        }

        // Thêm client vào phòng
        public void AddClient(Socket clientSocket)
        {
            if (!clients.Contains(clientSocket))
            {
                clients.Add(clientSocket);
            }
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

            foreach (var client in clients)
            {
                try
                {
                    if (client.Connected && client != senderSocket && senderSocket != null)
                    {
                        await client.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send message to client in room {RoomId}: {ex.Message}");
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
    }

}
