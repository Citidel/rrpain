﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using ChatSharp;
using RRpain.Classes.Common;

namespace RRpain.Classes.Core
{
    public abstract class CommandHandler
    {
        public abstract void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand);

        protected static void HandleHelp(IList<string> paramList)
        {
            var exists = false;
            foreach (var help in Program.Commands.Where(cmd => cmd.Value.Listener == paramList[1]).Select(cmd => cmd.Value.Help))
            {
                Utils.SendChannel(!string.IsNullOrEmpty(help) ? help : Data.MessageRestricted);
                exists = true;
            }

            if (!exists)
            {
                Utils.SendChannel("Command not found.");
            }
        }

        protected static int GenerateRandom(int min, int max)
        {
            var random = RandomNumberGenerator.Create();
            var b = new byte[4];
            random.GetBytes(b);
            return (int)Math.Round(((double) BitConverter.ToUInt32(b, 0) / UInt32.MaxValue) * (max - min - 1)) + min;
        }
    }
}
