﻿namespace Avalon.Common.Colors
{
    public class DarkBlue : AnsiColor
    {
        public override string ToString()
        {
            return "\x1B[0;34m";
        }

        public override string MudColorCode => "{b";

        public override string Name => "Dark Blue";
    }
}