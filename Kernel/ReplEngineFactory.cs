using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using iCSharp.Kernel.ScriptEngine;


using IReplEngine = iCSharp.Kernel.ScriptEngine.IReplEngine;
using ILog = Common.Logging.ILog;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace iCSharp.Kernel
{
    public class ReplEngineFactory
    {
        private string[] args;

        private IReplEngine _replEngine;
        private ScriptState<object> _repl;
        private MemoryBufferConsole _console;
        private ILog _logger;

        public ReplEngineFactory(ILog logger, string[] args)
        {
            this._logger = logger;
            this.args = args;
        }

        public IReplEngine ReplEngine
        {
            get
            {
                if (this._replEngine == null)
                {
                    this._replEngine = new ReplEngineWrapper(this.Logger, this.Repl, this.Console);
                }

                return this._replEngine;
            }
        }

        public MemoryBufferConsole Console
        {
            get { return this._console; }
        }

        private ScriptState<object> Repl
        {
            get
            {
                if (this._repl == null)
                {
                    this._repl = this.GetRepl(this.args, out this._console);
                }

                return this._repl;
            }
        }

        private ILog Logger
        {
            get { return this._logger; }
        }

        private ScriptState<object> GetRepl(string[] args, out MemoryBufferConsole memoryBufferConsole)
        {
            SetProfile();
			//ScriptCsArgs arguments = ParseArguments(args);
		//	var scriptServicesBuilder = ScriptServicesBuilderFactory.Create(Config.Create(arguments), args);
         //   IInitializationServices _initializationServices = scriptServicesBuilder.InitializationServices;
          //  IFileSystem _fileSystem = _initializationServices.GetFileSystem();

           /* if (_fileSystem.PackagesFile == null)
            {
                throw new ArgumentException("The file system provided by the initialization services provided by the script services builder has a null packages file.");
            }

            if (_fileSystem.PackagesFolder == null)
            {
                throw new ArgumentException("The file system provided by the initialization services provided by the script services builder has a null package folder.");
            }*/

            //ScriptServices scriptServices = scriptServicesBuilder.Build();
            memoryBufferConsole = new MemoryBufferConsole();
            /* Repl repl = new Repl(
                 args, 
                 _fileSystem, 
                 scriptServices.Engine,
                 scriptServices.ObjectSerializer, 
                 scriptServices.LogProvider,
                 scriptServices.ScriptLibraryComposer,
                 memoryBufferConsole,
                 scriptServices.FilePreProcessor, 
                 scriptServices.ReplCommands,
                 new Printers(new ObjectSerializer()), 
                 new ScriptInfo());*/

            ScriptState<Object> repl = CSharpScript.RunAsync("").Result;

          //  var workingDirectory = _fileSystem.CurrentDirectory;
          // var assemblies = scriptServices.AssemblyResolver.GetAssemblyPaths(workingDirectory);
          // var scriptPacks = scriptServices.ScriptPackResolver.GetPacks();

            //  repl.Initialize(assemblies, scriptPacks, null);
            return repl;

        }

	//	private static ScriptCsArgs ParseArguments(string[] args)
    //    {
	//		return ScriptCsArgs.Parse (args);
   //     }
        
        private static void SetProfile()
        {
            var profileOptimizationType = Type.GetType("System.Runtime.ProfileOptimization");
            if (profileOptimizationType != null)
            {
                var setProfileRoot = profileOptimizationType.GetMethod("SetProfileRoot", BindingFlags.Public | BindingFlags.Static);
                setProfileRoot.Invoke(null, new object[] { typeof(Program).Assembly.Location });

                var startProfile = profileOptimizationType.GetMethod("StartProfile", BindingFlags.Public | BindingFlags.Static);
                startProfile.Invoke(null, new object[] { typeof(Program).Assembly.GetName().Name + ".profile" });
            }
        }
    }
}
