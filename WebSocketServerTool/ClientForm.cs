﻿using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebSocketServerTool
{
    public partial class ClientForm : Form
    {
        private IClientContext context;
        private bool newMessage = true;

        public ClientForm(IClientContext context)
        {
            InitializeComponent();
            this.context = context;
            context.Message += Context_Message;
            context.Closed += Context_Closed;
        }

        private void Context_Closed()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate {
                    Context_Closed();
                });
                return;
            }

            status.Text = "WebSocket closed.";
        }

        private void Context_Message(byte[] message, int length, WebSocketMessageType type, bool lastMessage)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate {
                    Context_Message(message, length, type, lastMessage);
                });
                return;
            }

            if (newMessage)
            {
                outputText.AppendText(string.Format("Message Received on {0}:{1}",
                    DateTime.Now, Environment.NewLine));
            }

            if (type == WebSocketMessageType.Binary)
            {
                outputText.Append(message, length);
            }
            else
            {
                outputText.AppendText(Encoding.UTF8.GetString(message));
            }

            if (lastMessage)
            {
                outputText.AppendText(Environment.NewLine);
                outputText.AppendText(Environment.NewLine);
            }

            newMessage = lastMessage;
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if (input.Binary)
            {
                context.Send(input.Bytes, WebSocketMessageType.Binary);
            }
            else
            {
                context.Send(input.Bytes, WebSocketMessageType.Text);
            }
        }

        private void ClientForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            context.Message -= Context_Message;
            context.Closed -= Context_Closed;
            context.Close();
        }
    }
}
