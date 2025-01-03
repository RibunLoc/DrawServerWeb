﻿namespace ServerDrawHub
{
    partial class QuanLyServer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QuanLyServer));
            this.button1 = new System.Windows.Forms.Button();
            this.Name_port = new System.Windows.Forms.Label();
            this.tb_input_port = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btn_stop = new System.Windows.Forms.Button();
            this.panel_draw = new System.Windows.Forms.Panel();
            this.rtb_content = new System.Windows.Forms.RichTextBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel_draw.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(832, 40);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(176, 56);
            this.button1.TabIndex = 0;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Name_port
            // 
            this.Name_port.AutoSize = true;
            this.Name_port.Font = new System.Drawing.Font("Segoe UI", 11.14286F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.Name_port.Location = new System.Drawing.Point(26, 59);
            this.Name_port.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Name_port.Name = "Name_port";
            this.Name_port.Size = new System.Drawing.Size(71, 37);
            this.Name_port.TabIndex = 1;
            this.Name_port.Text = "Port:";
            // 
            // tb_input_port
            // 
            this.tb_input_port.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_input_port.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tb_input_port.Font = new System.Drawing.Font("Segoe UI", 11.14286F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.tb_input_port.Location = new System.Drawing.Point(5, 15);
            this.tb_input_port.Margin = new System.Windows.Forms.Padding(4);
            this.tb_input_port.Name = "tb_input_port";
            this.tb_input_port.Size = new System.Drawing.Size(484, 35);
            this.tb_input_port.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panel1.Controls.Add(this.tb_input_port);
            this.panel1.Location = new System.Drawing.Point(98, 44);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(493, 64);
            this.panel1.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btn_stop);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Controls.Add(this.Name_port);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1412, 139);
            this.panel2.TabIndex = 4;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // btn_stop
            // 
            this.btn_stop.Location = new System.Drawing.Point(1040, 40);
            this.btn_stop.Name = "btn_stop";
            this.btn_stop.Size = new System.Drawing.Size(182, 59);
            this.btn_stop.TabIndex = 4;
            this.btn_stop.Text = "Stop";
            this.btn_stop.UseVisualStyleBackColor = true;
            this.btn_stop.Click += new System.EventHandler(this.btn_stop_Click);
            // 
            // panel_draw
            // 
            this.panel_draw.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panel_draw.Controls.Add(this.rtb_content);
            this.panel_draw.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_draw.Location = new System.Drawing.Point(0, 139);
            this.panel_draw.Name = "panel_draw";
            this.panel_draw.Size = new System.Drawing.Size(1412, 770);
            this.panel_draw.TabIndex = 5;
            // 
            // rtb_content
            // 
            this.rtb_content.Location = new System.Drawing.Point(98, 21);
            this.rtb_content.Name = "rtb_content";
            this.rtb_content.Size = new System.Drawing.Size(1252, 697);
            this.rtb_content.TabIndex = 0;
            this.rtb_content.Text = "";
            this.rtb_content.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // QuanLyServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1412, 909);
            this.Controls.Add(this.panel_draw);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("Segoe UI", 9.857143F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "QuanLyServer";
            this.Text = "Server";
            this.Load += new System.EventHandler(this.Server_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel_draw.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label Name_port;
        private System.Windows.Forms.TextBox tb_input_port;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel_draw;
        private System.Windows.Forms.RichTextBox rtb_content;
        private System.Windows.Forms.Button btn_stop;
    }
}

