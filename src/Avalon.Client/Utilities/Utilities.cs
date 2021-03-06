﻿using Argus.Extensions;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using Avalon.Common.Models;

namespace Avalon.Utilities
{
    /// <summary>
    /// General utilities that don't currently fit other places.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Removes unsupported characters or other sets of sequences we don't want parsed.
        /// </summary>
        /// <param name="sb"></param>
        public static void RemoveUnsupportedCharacters(this StringBuilder sb)
        {
            // Remove single characters we're not supporting, rebuild the string.
            var removeChars = new HashSet<char> { (char)1, (char)249, (char)251, (char)252, (char)255, (char)65533 };

            // Go through the StringBuilder backwards and remove any characters in our HashSet.
            for (int i = sb.Length - 1; i >= 0; i--)
            {
                char temp = sb[i];

                if (removeChars.Contains(sb[i]))
                {
                    sb.Remove(i, 1);
                }
            }

            // Remove the up, down, left, right, blink, reverse and underline.  We're not supporting
            // these at this time although we will support come of them in the future.
            sb.Replace("\x1B[1A", ""); // Up
            sb.Replace("\x1B[1B", ""); // Down
            sb.Replace("\x1B[1C", ""); // Right
            sb.Replace("\x1B[1D", ""); // Left
            sb.Replace("\x1B[5m", ""); // Blink
        }

        /// <summary>
        /// Returns the danger color for a given percent.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static SolidColorBrush ColorPercent(int value, int maxValue)
        {
            if (value < (maxValue / 2))
            {
                return Brushes.Red;
            }
            else if (value < ((maxValue * 3) / 4))
            {
                return Brushes.Yellow;
            }
            else
            {
                return Brushes.White;
            }
        }

        /// <summary>
        /// Processes a speedwalk command into a set of commands.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Speedwalk(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "";
            }

            var sb = new StringBuilder();
            var list = input.Split(' ');

            // This will be each individual step (or a number in the same direction)
            foreach (string step in list)
            {
                if (step.ContainsNumber())
                {
                    string stepsStr = "";
                    string direction = "";

                    // Pluck the number off the front (e.g. 4n)
                    foreach (char c in step)
                    {
                        if (char.IsNumber(c))
                        {
                            stepsStr += c;
                        }
                        else
                        {
                            direction += c;
                        }
                    }

                    // The number of steps to repeat this specific step
                    int steps = int.Parse(stepsStr);

                    for (int i = 1; i <= steps; i++)
                    {
                        sb.Append(direction);
                        sb.Append(';');
                    }

                }
                else
                {
                    // No number, put it in verbatim.
                    sb.Append(step);
                    sb.Append(';');
                }

            }

            sb.TrimEnd(';');

            // Finally, look for parens and turn semi-colons in between there into spaces.  This might be hacky but should
            // allow for commands in the middle of our directions as long as they're surrounded by ().
            bool on = false;

            for (int i = 0; i < sb.Length; i++)
            {
                if (sb[i] == '(')
                {
                    on = true;
                }
                else if (sb[i] == ')')
                {
                    on = false;
                }

                if (on == true && sb[i] == ';')
                {
                    sb[i] = ' ';
                }
            }

            // Now that the command has been properly placed, remove any parents.
            sb.Replace("(", "").Replace(")", "");

            return sb.ToString();
        }
    }
}