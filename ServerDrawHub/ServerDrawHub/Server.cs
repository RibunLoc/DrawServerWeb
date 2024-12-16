using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ServerDrawHub
{
    public partial class Server : Form
    {
        private TcpListener server;
        private bool isRunning;
        private List<Socket> connectedClients = new List<Socket>(); // List of connected clients
        private Socket serverSocket;
        private Dictionary<string, Socket[]> clientRoomMapping = new Dictionary<string, Socket[]>();
        private string roomid;

        public class DrawData
        {
            public int StartX { get; set; }
            public int StartY { get; set; }
            public int EndX { get; set; }
            public int EndY { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public string RoomId { get; set; }
        }

        public Server()
        {
            InitializeComponent();
        }

        private async void Server_Load(object sender, EventArgs e)
        {

            // Initialize the server UI or logic here if needed
        }

        private async void button1_Click(object sender, EventArgs e) // Start/Stop button
        {
            if (!isRunning)
            {
                try
                {
                    int port = Convert.ToInt32(tb_input_port.Text);
                    /*server = new TcpListener(IPAddress.Any, port);
                    server.Start();*/

                    isRunning = true;
                    button1.Text = "Stop";
                    button1.BackColor = Color.Red;


                    serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    serverSocket.Bind(new IPEndPoint(IPAddress.Any, port)); // Replace with your port
                    serverSocket.Listen(10);

                    richTextBox1.AppendText("Server started. Waiting for connections..." + Environment.NewLine);

                    await StartServerAsync();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                StopServer();
            }
        }

        private void StopServer()
        {
            isRunning = false;

            foreach (var client in connectedClients)
            {
                try
                {
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
                catch
                {
                    // Ignore errors while closing sockets
                }
            }

            connectedClients.Clear();
            server?.Stop();

            button1.Text = "Listen";
            button1.BackColor = SystemColors.Control;
        }

        private async Task StartServerAsync()
        {
            List<Task> clientTasks = new List<Task>();

            while (true)
            {
                Socket clientSocket = await serverSocket.AcceptAsync();

                // Handle the client in a new task
                Task clientTask = HandleClientAsync(clientSocket);
                clientTasks.Add(clientTask);

                // Remove completed tasks
                clientTasks.RemoveAll(task => task.IsCompleted);
            }
        }

        private async Task HandleClientAsync(Socket clientSocket)
        {
            /*TaskCompletionSource<string> roomIdTaskSource = new TaskCompletionSource<string>();*/
            try
            {
                IPEndPoint remoteEndPoint = (IPEndPoint)clientSocket.RemoteEndPoint;
                string clientIpAddress = remoteEndPoint.Address.ToString();
                int clientPort = remoteEndPoint.Port;
                StringBuilder messageBuilder = new StringBuilder();

                richTextBox1.AppendText($"Connected from {clientIpAddress}:{clientPort}{Environment.NewLine}");

                byte[] buffer = new byte[1024];

                while (true)
                {
                    // Read data from the client
                    int bytesReceived = await clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);

                    if (bytesReceived == 0) // Client disconnected
                    {
                        richTextBox1.AppendText($"Client {clientIpAddress}:{clientPort} disconnected.{Environment.NewLine}");
                        break;
                    }

                    // Append received data to the buffer
                    string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                    messageBuilder.Append(receivedData);

                    // Split messages using the newline delimiter
                    string[] messages = messageBuilder.ToString().Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                    

                    // Process each complete message
                    for (int i = 0; i < messages.Length; i++)
                    {
                        string message = messages[i];

                        if (message.Contains("roomid"))
                        {
                            // Handle room ID messages
                            roomid = message.Split(':')[1];
                            messageBuilder.Clear();
                            MapClientToRoom(clientSocket, roomid);
                            continue;
                        }

                        string[] parts = message.Split(',');
                        if (i == messages.Length - 1 && parts.Length < 7)
                        {
                            // Keep incomplete JSON in the buffer
                            messageBuilder.Clear();
                            messageBuilder.Append(message);
                        }
                        else
                        {
                            try
                            {
                                

                                // Deserialize the JSON message into a DrawData object
                                //DrawData drawData = System.Text.Json.JsonSerializer.Deserialize<DrawData>(message);

                                // Process the DrawData and broadcast it
                                roomid = parts[6];
                                int index = message.LastIndexOf(parts[6]);
                                message = message.Substring(0,index);

                                messageBuilder.Append(message);
                                richTextBox1.AppendText($"{clientIpAddress}:{clientPort} {messageBuilder}{Environment.NewLine}");

                                if (messageBuilder.Length > 500 || i == messages.Length - 1) // You can adjust the size threshold
                                {
                                    // Send the accumulated messages in one go
                                    await BroadcastMessageAsync(messageBuilder.ToString(), roomid, clientSocket);
                                    messageBuilder.Clear(); // Clear the buffer after sending
                                }
                                //await BroadcastMessageAsync(message, roomid, clientSocket);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Failed to process message: {ex.Message}");
                            }
                        }
                            
                    }

                    // Retain incomplete messages in the buffer
                    /*if (!messageBuilder.ToString().EndsWith("\n"))
                    {
                        messageBuilder.Clear();
                    }
                    else
                    {
                        messageBuilder.Clear();
                    }*/
                }
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText($"Error with client: {ex.Message}{Environment.NewLine}");
            }
            finally
            {
                connectedClients.Remove(clientSocket);
                clientSocket.Close();
            }
        }

        private void MapClientToRoom(Socket clientSocket, string roomId)
        {
            // This method maps the client to the roomId (or any logic for client-room mapping)
            if (clientRoomMapping.ContainsKey(roomId))
            {
                var currentClients = clientRoomMapping[roomId].ToList();
                currentClients.Add(clientSocket);
                clientRoomMapping[roomId] = currentClients.ToArray();
            }
            else
            {
                clientRoomMapping[roomId] = new Socket[] { clientSocket };
            }
        }

        private async Task BroadcastMessageAsync(string message, string roomid, Socket clientSocket)
        {
            //byte[] data = Encoding.ASCII.GetBytes(message);

            // Use a snapshot of connected clients to avoid modifying the list during iteration
           // string jsonMessage = System.Text.Json.JsonSerializer.Serialize(message) + "\n";
            byte[] data = Encoding.UTF8.GetBytes(message);
            List<Socket> clientsSnapshot;

            lock (clientRoomMapping[roomid])
            {
                clientsSnapshot = new List<Socket>(clientRoomMapping[roomid]);
            }

            foreach (Socket client in clientsSnapshot)
            {
                if (client == clientSocket)
                {
                    continue;
                }
                try
                {
                    await client.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);
                }
                catch (Exception ex)
                {
                    Invoke((Action)(() =>
                    {
                        richTextBox1.AppendText($"Failed to send to a client: {ex.Message}{Environment.NewLine}");
                    }));
                }
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

/*
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ServerDrawHub
{
    public partial class Server : Form
    {
        private TcpListener server;
        private bool isRun = true;
        private List<(Point start, Point end)> lines = new List<(Point, Point)>(); // tạo danh sách lưu trữ đường vẽ
        private Queue<(Point start, Point end)> drawQueue = new Queue<(Point, Point)>();
        private BufferedGraphics bufferedGraphics;
        private bool isRunning = false;
        private List<Socket> connectedClients = new List<Socket>();

        public Server()
        {
            InitializeComponent();

        }

        private void Server_Load(object sender, EventArgs e)
        {
            BufferedGraphicsContext NoiDungHienTai = BufferedGraphicsManager.Current;
            bufferedGraphics = NoiDungHienTai.Allocate(panel_draw.CreateGraphics(), panel_draw.ClientRectangle);
            Task.Run(() =>
            {
                ProcessDrawQueue();
            });

            panel_draw.Paint += panel_draw_Paint;
            bufferedGraphics.Graphics.Clear(Color.White);
        }

        private void StopServer()
        {
            isRunning = false;
            server.Stop();
        }
        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private async void ProcessDrawQueue()
        {
            while (isRunning)
            {
                (Point start, Point end)? line = null;

                lock (drawQueue)
                {
                    if (drawQueue.Count > 0)
                    {
                        line = drawQueue.Dequeue();
                    }
                }

                if (line.HasValue)
                {
                    DrawLineOnServer(line.Value.start, line.Value.end);

                }

                await Task.Delay(5);
            }
        }


        private async void ListenForClient()
        {
            try
            {
                isRunning = true;
                while (true)
                {
                    TcpClient client = await server.AcceptTcpClientAsync();
                    ProcessClient(client);

                }
            }
            catch (ObjectDisposedException)
            {
                // Xử lý khi server đã bị ngắt
            }
            catch (SocketException)
            {
                // Xử lý lỗi socket
            }

        }

        private async void ProcessClient(TcpClient client)
        {
            try
            {
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] bodem = new byte[1024];
                    int bytesRead;

                    while ((bytesRead = await stream.ReadAsync(bodem, 0, bodem.Length)) > 0)
                    {
                        string data = Encoding.ASCII.GetString(bodem, 0, bytesRead);

                        //tách dữ liệu và xử lý
                        string[] parts = data.Split(',');
                        Point start = new Point(
                            int.Parse(parts[0]) * panel_draw.Width / int.Parse(parts[4]),
                            int.Parse(parts[1]) * panel_draw.Height / int.Parse(parts[5])
                            );
                        Point end = new Point(
                            int.Parse(parts[2]) * panel_draw.Width / int.Parse(parts[4]),
                            int.Parse(parts[3]) * panel_draw.Height / int.Parse(parts[5])
                            );

                        DrawLineOnServer(start, end);
                        lock (drawQueue)
                        {
                            drawQueue.Enqueue((start, end));
                        }


                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void panel_draw_Paint(object sender, PaintEventArgs e)
        {
            //lines.Add((start, end));

            using (Pen pen = new Pen(Color.Black, 2))
            {
                foreach (var line in lines)
                {
                    bufferedGraphics.Graphics.DrawLine(pen, line.start, line.end);
                }

            }

            bufferedGraphics.Render(e.Graphics);
        }

        private void DrawLineOnServer(Point start, Point end)
        {
            lines.Add((start, end));

            using (Graphics g = this.panel_draw.CreateGraphics())
            {
                bufferedGraphics.Graphics.DrawLine(Pens.Black, start, end);

                Rectangle invalidRect = new Rectangle(
                Math.Min(start.X, end.X),
                Math.Min(start.Y, end.Y),
                Math.Abs(start.X - end.X),
                Math.Abs(start.Y - end.Y));

                const int margin = 5; // You can adjust this margin size if needed
                invalidRect.Inflate(margin, margin); // Increase the invalidation area by a small amount


                panel_draw.Invalidate(invalidRect);
            }
        }



        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e) // button Listen
        {
            if (isRun)
            {
                try
                {
                    isRun = false;
                    button1.Text = "Stop!";
                    button1.BackColor = Color.White;
                    server = new TcpListener(IPAddress.Any, 5000);
                    tb_input_port.Text = 5000.ToString();
                    server.Start();
                    ListenForClient();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            }
            else
            {
                isRunning = true;
                isRun = true;
                button1.Text = "Listen";
                button1.BackColor = SystemColors.ActiveBorder;
                // cần ngắt hệ thống server
                if (server != null)
                {
                    StopServer();
                    server = null;
                }
            }

        }

        private async Task BroadcastMessageAsync(string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);

            foreach (Socket client in connectedClients)
            {
                try
                {
                    await client.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to send to a client: {ex.Message}" + Environment.NewLine);
                }
            }
        }


    }
}
*/