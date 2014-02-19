using System;
using System.Collections.Generic;
using System.Linq;
using ChatSharp;
using RRpain.Classes.Common;
using RRpain.Classes.Instances;
//using RRpain.Classes.JSON;
using Newtonsoft.Json;

namespace RRpain.Classes
{
    public static class Handler
    {
        public static void CommandAnnounce(IList<string> paramList, string nick)
        {
            if (paramList.Count < 3)
            {
                Utils.SendNotice("Usage: !announce <time in seconds> <repeats> <message>", nick);
            }
            else
            {
                var msg = "";
                var timeTick = Convert.ToInt32(paramList[1]) * 1000;
                var timeCount = Convert.ToInt32(paramList[2]);
                if (timeTick == 0) return;
                Program.AnnounceTimer.Interval = timeTick;
                GC.KeepAlive(Program.AnnounceTimer);
                for (var i = 3; i < paramList.Count; i++)
                {
                    msg = msg + paramList[i] + " ";
                }
                Announcement.AnnounceMsg = msg;
                Announcement.AnnounceTimes = timeCount;
                Program.AnnounceTimer.Enabled = true;
            }
        }


        public static void CommandDev()
        {
            // Placeholder method for any future dev related commands
        }
    }
       
}
