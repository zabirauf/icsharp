using System;
using System.Collections.Generic;
using ScriptCs.Contracts;

namespace iCSharp.Kernel.ScriptEngine
{
    public class MemoryBufferConsole : IConsole
    {
        private List<string> buffer;

        public IEnumerable<string> GetAllInBuffer()
        {
            return this.buffer.ToArray();
        }

        public void ClearAllInBuffer()
        {
            this.buffer.Clear();    
        }

        public MemoryBufferConsole()
        {
            this.buffer = new List<string>();
        }

        public void Write(string value)
        {
            this.buffer.Add(value);
        }

        public void WriteLine()
        {
            this.buffer.Add(Environment.NewLine);
        }

        public void WriteLine(string value)
        {
            this.buffer.Add(value + Environment.NewLine);
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
        }

        public ConsoleColor ForegroundColor { get; set; }
    }
}
