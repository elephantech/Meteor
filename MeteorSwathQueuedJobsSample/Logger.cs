using System;

namespace MeteorSwathQueuedJobsSample
{
    /// <summary>
    /// Thin wrapper around console output, for thread safe coloured logging
    /// </summary>
    static class Logger
    {
        /// <summary>
        /// Object used as a synchronisation lock, as output can be sent by multiple threads
        /// </summary>
        private static object _lock = new object();
        /// <summary>
        /// Array of colours used by the application
        /// </summary>
        private static ConsoleColor[] _colours = { ConsoleColor.White, ConsoleColor.Green, ConsoleColor.Magenta };
        /// <summary>
        /// Write the console line with one of the predefined colours in the Logger._colours array
        /// </summary>
        public static void WriteLine(int colourId, string s) {
            lock (_lock) {
                ConsoleColor currentColour = Console.ForegroundColor;
                Console.ForegroundColor = (colourId < _colours.Length) ? _colours[colourId] : ConsoleColor.Gray;
                Console.WriteLine(s);
                Console.ForegroundColor = currentColour;
            }
        }
        /// <summary>
        /// Write the console line with a specific colour
        /// </summary>
        public static void WriteLine(ConsoleColor colour, string s) {
            lock (_lock) {
                ConsoleColor currentColour = Console.ForegroundColor;
                Console.ForegroundColor = colour;
                Console.WriteLine(s);
                Console.ForegroundColor = currentColour;
            }
        }
        /// <summary>
        /// Write the console line in white
        /// </summary>
        /// <param name="s"></param>
        public static void WriteLine(string s) => WriteLine(0, s);
    }
}
