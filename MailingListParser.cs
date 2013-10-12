using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace MailingListReader
{
    class MailingListParser
    {
        //bool _isFromHandled = false;
        //bool _isFromColonHandled = false;
        bool _isDateHandled = false;
        //bool _isSubjectHandled = false;
        //bool _isInReplyToHandled = false;
        //bool _isReferencesHandled = false;
        //bool _isMessageIDHandled = false;

        public static List<String> GetMessagesFromList(List<String> i_List)
        {
            List<String> messagesList = new List<string>();

            int i = 0;
            int fromCounter = 0; //1 "From" to each message
            string message = string.Empty;
            while (i < i_List.Count)
            {
                bool isStartsWithFrom = i_List[i].StartsWith("From") && !i_List[i].StartsWith("From:");
                switch (isStartsWithFrom)
                {
                    case true:
                        {
                            if (fromCounter < 1)
                            {
                                message += i_List[i];
                                i++;
                                fromCounter++;
                            }
                            else
                            {
                                messagesList.Add(message);
                                message = string.Empty;
                                fromCounter = 0;
                            }

                            break;
                        }
                    case false:
                        {
                            if (message != string.Empty)
                            {
                                message += "\n" + i_List[i]; 
                            }

                            if (i + 1 >= i_List.Count)
                            {
                                messagesList.Add(message);
                            }
                            

                            i++;
                            break;
                        }
                }
            }

            return messagesList;
        }

        public List<MailMessage> ParseMailMessage(List<String> i_MailingList)
        {
            List<MailMessage> mailMessages = new List<MailMessage>();

            foreach (string Message in i_MailingList)
            {
                string[] messageParts = Message.Split('\n');
                MailMessage mailMessage = new MailMessage();

                foreach (string part in messageParts)
                {
                    if (part.StartsWith("From") && !part.Contains("From:"))
                    {
                        mailMessage.From = part.Substring("From".Length);
                    }
                    else if (part.StartsWith("From:"))
                    {
                        mailMessage.FromColon = part.Substring("From:".Length);
                    }
                    else if (part.StartsWith("Date:") && !_isDateHandled)
                    {
                        string s = part.Substring("Date: ".Length);
                        DateTimeOffset offset;

                        bool isParsed = DateTimeOffset.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out offset);

                        if (!isParsed)
                        {
                            string[] timeParts = part.Split(' ');
                            offset = DateTimeOffset.Parse(s.Substring(0, s.Length - timeParts[timeParts.Length - 1].Length), CultureInfo.InvariantCulture);
                        }

                        mailMessage.Date = offset.UtcDateTime;
                        _isDateHandled = true;
                    }
                    else if (part.StartsWith("Subject:"))
                    {
                        mailMessage.Subject = part.Substring("Subject:".Length);
                    }
                    else if (part.StartsWith("In-Reply-To:"))
                    {
                        mailMessage.In_Reply_To = part.Substring("In-Reply-To:".Length);
                    }
                    else if (part.StartsWith("References:"))
                    {
                        mailMessage.References = part.Substring("References:".Length);
                    }
                    else if (part.StartsWith("Message-ID:"))
                    {
                        mailMessage.MessageID = part.Substring("Message-ID:".Length);
                    }
                    else
                    {
                        mailMessage.MessageData += part;

                    }

                }

                _isDateHandled = false;
                mailMessages.Add(mailMessage);
            }

            return mailMessages;
        }

        public static int[] GetMailCountPerDay(List<MailMessage> mailMessages)
        {
            int daysInMonth = DateTime.DaysInMonth(mailMessages[0].Date.Year, mailMessages[0].Date.Month);
            int[] mailCountPerDay = new int[daysInMonth];

            foreach (var item in mailMessages)
            {
                mailCountPerDay[item.Date.Day - 1]++;
            }

            return mailCountPerDay;
        }
    }
}
