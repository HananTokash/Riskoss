using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MailingListReader
{
    class MailMessage
    {
        public string From { get; set; }
        public string FromColon { get; set; }
        public DateTime Date { get; set; }
        public string Subject { get; set; }
        public string In_Reply_To { get; set; }
        public string References { get; set; }
        public string MessageID { get; set; }
        public string MessageData { get; set; }
    }
}
