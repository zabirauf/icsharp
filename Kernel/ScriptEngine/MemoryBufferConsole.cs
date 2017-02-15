using System;
using System.Collections.Generic;
using System.Linq;
using ScriptCs.Contracts;

namespace iCSharp.Kernel.ScriptEngine
{
    public class MemoryBufferConsole : IConsole
    {
        private const ConsoleColor DefaultConsoleColor = ConsoleColor.Black;

        private List<Tuple<string,ConsoleColor>> buffer;

        private ConsoleColor foregroundColor;

        public IEnumerable<Tuple<string, ConsoleColor>> GetAllInBuffer()
        {
            return this.buffer.ToArray();
        }

        public void ClearAllInBuffer()
        {
            this.buffer.Clear();    
        }

        public MemoryBufferConsole()
        {
            this.buffer = new List<Tuple<string, ConsoleColor>>();
        }

        public void Write(string value)
        {
            this.buffer.Add(new Tuple<string, ConsoleColor>(value, this.ForegroundColor));
        }

        public void WriteLine()
        {
            this.buffer.Add(new Tuple<string, ConsoleColor>(Environment.NewLine, this.ForegroundColor));
        }

        public void WriteLine(string value)
        {
            this.buffer.Add(new Tuple<string, ConsoleColor>(value + Environment.NewLine, this.ForegroundColor));
        }

        public string ReadLine()
        {
            return "Some new line";
        }

        public void Clear()
        {
        }

        public void Exit()
        {
        }

        public void ResetColor()
        {
            ForegroundColor = DefaultConsoleColor;
        }

        public ConsoleColor ForegroundColor
        {
            get
            {
                if (this.foregroundColor == ConsoleColor.Yellow || this.foregroundColor == ConsoleColor.White ||
                    this.foregroundColor == ConsoleColor.DarkYellow)
                {
                    return DefaultConsoleColor;
                }

                return this.foregroundColor;
            }

            set { this.foregroundColor = value; }
        }

        public string ReadLine(string prompt)
        {
            throw new NotImplementedException();
        }

        public int Width { get; }
    }
}
