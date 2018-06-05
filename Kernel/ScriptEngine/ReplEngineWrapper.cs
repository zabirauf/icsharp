using Common.Logging;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Text;
using System.Threading;
using ILog = Common.Logging.ILog;

namespace iCSharp.Kernel.ScriptEngine
{
	
	internal class ReplEngineWrapper : IReplEngine
	{
		private readonly ILog logger;
		private ScriptState<object> repl = null;
		private readonly MemoryBufferConsole console;
		//private 
		 

		public ReplEngineWrapper(ILog logger, ScriptState<object> repl, MemoryBufferConsole console)
		{
			this.logger = logger;
			this.repl = repl;
			this.console = console;
		}

		public ExecutionResult Execute(string script)
		{
            this.console.ResetColor();
			this.logger.Debug(string.Format("Executing: {0}", script));
			this.console.ClearAllInBuffer();
			this.console.Clear();

			//this.console.WriteLine("Script:");
			//this.console.WriteLine(script);
			this.console.WriteLine();

			var cancellationToken = new CancellationToken();

			Script<object> newScript;
            
			if (repl == null)
			{
				newScript = CSharpScript.Create<object>(script);//, ScriptOptions.Default);//, scriptOptions, globals.GetType(), assemblyLoader: null);
				//console.WriteLine("made new repl");
			}
			else
			{
				newScript = repl.Script.ContinueWith(script);//, ScriptOptions.Default); //scriptOptions
				//console.WriteLine("continuing with same repl");
			}

			// For errors
			var diagnostics = newScript.Compile(cancellationToken);

			//this.console.WriteLine("newscript code " + newScript.Code);

			ExecutionResult executionResult;

			if (diagnostics.Length > 0)
			{
                this.console.ForegroundColor = System.ConsoleColor.Red;
				foreach (var error in diagnostics)
				{
					this.console.WriteLine(error.ToString());
					//this.console.WriteLine(error.ToString())
				}

				return new ExecutionResult() { OutputResultWithColorInformation = this.console.GetAllInBuffer() };
			}


			var task = (repl == null) ?
							newScript.RunAsync(catchException: e => false, cancellationToken: cancellationToken) :
									 newScript.RunFromAsync(repl, catchException: e => false, cancellationToken: cancellationToken);

			repl = task.GetAwaiter().GetResult();

			// this.console.WriteLine("Result:");
			//this.console.WriteLine(newScript.);

			//this.console.WriteLine("Variables:");
            

			foreach (var _var in repl.Variables)
			{
				//this.console.WriteLine("Var_Name: " + _var.Name + " Var_Val: " + _var.Value);
			}

			if (repl.ReturnValue != null && !string.IsNullOrEmpty(repl.ReturnValue.ToString()))
			{
				//this.console.WriteLine("Result:");
				//this.console.WriteLine(repl.ReturnValue.ToString());
				this.console.Write(repl.ReturnValue.ToString());
			}


			executionResult = new ExecutionResult()
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
