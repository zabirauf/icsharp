
using System;
using System.Linq;

namespace iCSharp.Kernel.ScriptEngine
{
    using System.Collections;
    using System.Collections.Generic;

    public class ExecutionResult
    {
        public IEnumerable<string> OutputResults
        {
            get
            {
                return this.OutputResultWithColorInformation.Select(tuple => tuple.Item1).ToArray();
            }
        }

        public IEnumerable<Tuple<string, ConsoleColor>> OutputResultWithColorInformation { get; set; }
    }
}
