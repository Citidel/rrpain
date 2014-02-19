using System;
using System.Collections.Generic;
using ChatSharp;
using RRpain.Classes.Common;
using RRpain.Classes.Core;

namespace RRpain.Classes.Commands
{
    [CommandAttribute("RRpain", "")]
    public class RRpain : CommandHandler
    {
        public RRpain()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            if (paramList.Count <= 1) return;
            switch (paramList[1])
            {
                case "shutdown":
                    if (Utils.IsDev(user.Nick) || Utils.IsAdmin(user.Nick))
                    {
                        Utils.SendNotice("Shutting down bot.", user.Nick);
                        Environment.Exit(0);
                    }
                    else
                    {
                        Utils.SendNotice("This is a restricted command.", user.Nick);
                    }
                    break;
            }
        }
    }
}
