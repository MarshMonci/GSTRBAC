using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileClient
{
    public class TimeTracker
    {

        public TimeTracker()
        {

        }

        public DateTime getCurrentTimeInterval()
        {
            return DateTime.Now;
        }

        public DateTime getCurrentTimeInterval(int timeChoice)
        {
            DateTime timeStamp;
            switch (timeChoice)
            {
                case 0:
                    timeStamp = DateTime.Now;
                    break;
                case 1:
                    timeStamp = new DateTime(1, 1, 1, 5, 0, 0);
                    break;
                case 2:
                    timeStamp = new DateTime(1, 1, 1, 8, 0, 0);
                    break;
                case 3:
                    timeStamp = new DateTime(1, 1, 1, 12, 0, 0);
                    break;
                case 4:
                    timeStamp = new DateTime(1, 1, 1, 15, 0, 0);
                    break;
                case 5:
                    timeStamp = new DateTime(1, 1, 1, 18, 0, 0);
                    break;
                case 6:
                    timeStamp = new DateTime(1, 1, 1, 22, 0, 0);
                    break;
                default:
                    timeStamp = DateTime.Now;
                    break;
            }

            return timeStamp;
        }
    }
}
