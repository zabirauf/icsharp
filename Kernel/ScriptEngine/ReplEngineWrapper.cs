using Common.Logging;
using ScriptCs;
using ScriptCs.Contracts;

using ILog = Common.Logging.ILog;

namespace iCSharp.Kernel.ScriptEngine
{
    internal class ReplEngineWrapper : IReplEngine
    {
        private readonly ILog logger;
        private readonly Repl repl;
        private readonly MemoryBufferConsole console;

        public ReplEngineWrapper(ILog logger, Repl repl, MemoryBufferConsole console)
        {
            this.logger = logger;
            this.repl = repl;
            this.console = console;
        }

        public ExecutionResult Execute(string script)
        {
            this.console.ClearAllInBuffer();

            ScriptResult scriptResult = this.repl.Execute(script);

            ExecutionResult executionResult = new ExecutionResult()
            {
                OutputResultWithColorInformation = this.console.GetAllInBuffer(),
                CompileError = scriptResult.CompileExceptionInfo != null ? scriptResult.CompileExceptionInfo.SourceException : null,
                ExecuteError = scriptResult.ExecuteExceptionInfo != null ? scriptResult.ExecuteExceptionInfo.SourceException : null,
            };

            return executionResult;
        }

        private bool IsCompleteResult(ScriptResult scriptResult)
        {
            return scriptResult.ReturnValue != null && !string.IsNullOrEmpty(scriptResult.ReturnValue.ToString());
        }
    }
}
