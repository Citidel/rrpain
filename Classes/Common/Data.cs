using System.Collections.Generic;

namespace RRpain.Classes.Common
{
    /// <summary>
    /// RRpain data
    /// </summary>
    public static class Data
    {
        // Messages
        public const string MessageJoinChannel = "{0}, Welcome to The Resonant Rise IRC";
        public const string MessageIdentified = ":NickServ!NickServ@services.esper.net NOTICE RRpain :You are now identified for RRpain.";

        public const string MessageRestricted = "This command is restricted to ops only.";
        public const string MessageRestrictedIrc = "This command is restricted to the IRC channel only.";

        public const string MessageSlap = "{0} {1} {2}'s {3} with {4}!";
        public const string MessageUpdate = "Server Version: {0}, Update information: {1}";

        public const string McBansApiKey = "83855ea895268ec47f2e7ea0e8a25323f11e189c";

        // Devs
        public static readonly List<string> Developers = new List<string>
        {
            "Helkarakse",
            "Citidel",
            "Citidel_",
        };

        // Server Admin
        public static readonly List<string> Admin = new List<string>
        {
            "Ryahn"
        };
        public static readonly List<string> GamerPoop = new List<string>
        {
            "umY9aO2dXsQ", "ZU2vIAryZU4", "Q4SXtMmcdUo", "6xtjLnyMyo4",
            "BCtsFtPs1fY", "LG46DboDfhk", "yOpdnWYZ_ic", "nR9gMg4hKfY",
            "ZPllpzxmM5k", "9qMdd96Dqko", "CpeRk1YFn8s", "jpJ7NihviVU",
            "DM8krWnU0uQ", "crgEIhI3y_o", "pSawGT5bgdM", "gawughIGjK0",
            "Rs_ty7I3X8w", "azI_RlDHTBE", "BUOw3JyPRlQ", "661dbFQYbgU",
            "60sn_8qXsTA", "qsIS17jJH7A", "voc9TrSVVOA", "7vo5xhRTOjU",
            "k5FRbZ1zZm4", "8WzT2fnkfPk", "V-tgSGYU9fw", "mM-bfn8vZ_w",
            "GQ1ISnkS5oY", "TTUrDkBUtfk", "EM1RXZGWiqo", "SjIeo5cOGQs",
            "YmqlrhCNiMU", "HycTcUN6H0M", "HqQMS4vYSkM", "4xQLjIDjcIs",
        };
    }
}