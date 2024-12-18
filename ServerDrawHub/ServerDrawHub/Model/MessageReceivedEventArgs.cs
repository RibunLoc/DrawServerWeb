using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDrawHub.Model
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public string message { get; set; }
        public MessageReceivedEventArgs(string message)
        {
            this.message = message;
        }   

    }
}
