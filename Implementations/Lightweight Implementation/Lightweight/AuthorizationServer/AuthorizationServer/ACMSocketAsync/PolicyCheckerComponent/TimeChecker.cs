using SerializablePackages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACMSocketAsync
{
    public class TimeChecker
    {
        public bool CheckTime(DataRow interval, Packages.STZone userZone)
        {
            if (TimeSpan.Compare(interval.Field<TimeSpan>("EndTime"), interval.Field<TimeSpan>("StartTime")) < 1)
            {
                if (TimeSpan.Compare(userZone.StartTime.TimeOfDay, interval.Field<TimeSpan>("StartTime")) < 1 ||
                TimeSpan.Compare(userZone.StopTime.TimeOfDay, interval.Field<TimeSpan>("EndTime")) >= 0)
                {
                    return true;
                }
            }
            else if (TimeSpan.Compare(interval.Field<TimeSpan>("StartTime"), userZone.StartTime.TimeOfDay) >= 0 &&
               TimeSpan.Compare(interval.Field<TimeSpan>("EndTime"), userZone.StopTime.TimeOfDay) < 1)
            {
                return true;
            }

            return false;
        }
    }
}
