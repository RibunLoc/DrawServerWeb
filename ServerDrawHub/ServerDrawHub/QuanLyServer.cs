using ServerDrawHub.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Schema;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ServerDrawHub
{
    public partial class QuanLyServer : Form
    {

        public Server server;
    
        public QuanLyServer()
        {
            InitializeComponent();
            server = new Server();
            server.MessageReceived += Server_MessageReceived;
            server.ClientDisconnected += Server_ClientDisconnected;
            server.CreatedRoom += UpdateMesseage;
            
        }

        private void UpdateMesseage(string message)
        {
            InvokeIfRequired(() =>
             rtb_content.AppendText($"Message received: {message + Environment.NewLine}")    
            );
        }

        private void Server_ClientDisconnected(object sender, EventArgs e)
        {
            InvokeIfRequired(() =>
             rtb_content.AppendText("Client disconnected!\n")
            );
        }

        private void Server_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            InvokeIfRequired(() =>
             rtb_content.AppendText($"Message received: {e.message}\n")
            );
        }

        private void InvokeIfRequired(Action action)
        {
            if (rtb_content.InvokeRequired)
            {
                rtb_content.Invoke(action);
            }
            else
            {
                action();
            }
        }

        private async void Server_Load(object sender, EventArgs e)
        {

            
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if(!server.isRunning)
            {
                int port = int.Parse(tb_input_port.Text.Trim());
                server.Start(port);
                button1.Enabled = false;
                btn_stop.Enabled = true;
            }
            
        }

        //private async Task BroadcastMessageAsync(string message, Socket senderSocket)
        //{
        //    foreach(var room in server.rooms.Values)
        //    {
        //       await room.BroadcastMessageAsync(message, senderSocket);
        //    }    
        //}

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            if (server.isRunning)
            {
                int.TryParse(tb_input_port.Text, out int port);
                server.Stop();
                button1.Enabled = true;
                btn_stop.Enabled = false;
            }
        }
    }
}