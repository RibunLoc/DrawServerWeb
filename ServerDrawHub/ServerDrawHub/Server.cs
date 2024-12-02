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

        public Server()
        {
            InitializeComponent();
            
        }

        private void Server_Load(object sender, EventArgs e)
        {
            BufferedGraphicsContext NoiDungHienTai = BufferedGraphicsManager.Current;
            bufferedGraphics = NoiDungHienTai.Allocate(panel_draw.CreateGraphics(), panel_draw.ClientRectangle);
            Task.Run(() => {
                ProcessDrawQueue();
                });

            panel_draw.Paint += panel_draw_Paint;
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
            while(isRunning)
            {
                (Point start, Point end)? line = null;

                lock (drawQueue)
                {
                    if(drawQueue.Count > 0)
                    {
                        line = drawQueue.Dequeue();
                    }
                }

                if(line.HasValue)
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
            using (Pen pen = new Pen(BackColor, 2))
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
/*
            using (Graphics g = this.panel_draw.CreateGraphics())
            {*/
                bufferedGraphics.Graphics.DrawLine(Pens.Black, start, end);

            panel_draw.Invalidate();
            /* }*/
        }



        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e) // button Listen
        {
            if (isRun)
            {
                isRun = false;
                button1.Text = "Stop!";
                button1.BackColor = Color.White;
                server = new TcpListener(IPAddress.Any, 5000);
                tb_input_port.Text = 5000.ToString();
                server.Start();
                ListenForClient();
            }
            else
            {
                isRunning = true;
                isRun = true;
                button1.Text =  "Listen";
                button1.BackColor = SystemColors.ActiveBorder;
                // cần ngắt hệ thống server
                if (server != null)
                {
                    StopServer();
                    server = null;
                }
            }
            
        }

        
    }
}
