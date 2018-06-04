using Common.Logging;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using ILog = Common.Logging.ILog;

namespace iCSharp.Kernel.ScriptEngine
{
    internal class ReplEngineWrapper : IReplEngine
    {
        private readonly ILog logger;
        private ScriptState<object> repl;
        private readonly MemoryBufferConsole console;

        public ReplEngineWrapper(ILog logger, ScriptState<object> repl, MemoryBufferConsole console)
        {
            this.logger = logger;
            this.repl = repl;
            this.console = console;
        }

        public ExecutionResult Execute(string script)
        {
            this.logger.Debug(string.Format("Executing: {0}", script));
            this.console.ClearAllInBuffer();

            repl = repl == null ? CSharpScript.RunAsync(script).Result : repl.ContinueWithAsync(script).Result;

            object scriptResult = this.repl.ContinueWithAsync(script);

            ExecutionResult executionResult = new ExecutionResult()
            {
                OutputResultWithColorInformation = this.console.GetAllInBuffer()
            };

            return executionResult;
        }

       // private bool IsCompleteResult(ScriptResult scriptResult)
       // {
       //     return scriptResult.ReturnValue != null && !string.IsNullOrEmpty(scriptResult.ReturnValue.ToString());
        //}

        

        
    }
}
