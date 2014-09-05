using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using iCSharp.Kernel.ScriptEngine;
using ScriptCs;
using ScriptCs.Argument;
using ScriptCs.Contracts;
using ScriptCs.Hosting;

namespace iCSharp.Kernel
{
    public class ReplEngineFactory
    {
        private string[] args;

        private IReplEngine _replEngine;
        private Repl _repl;
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

        private Repl Repl
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

        private Repl GetRepl(string[] args, out MemoryBufferConsole memoryBufferConsole)
        {
            SetProfile();
            var arguments = ParseArguments(args);
            var scriptServicesBuilder = ScriptServicesBuilderFactory.Create(arguments.CommandArguments, arguments.ScriptArguments);
            IInitializationServices _initializationServices = scriptServicesBuilder.InitializationServices;
            IFileSystem _fileSystem = _initializationServices.GetFileSystem();

            if (_fileSystem.PackagesFile == null)
            {
                throw new ArgumentException("The file system provided by the initialization services provided by the script services builder has a null packages file.");
            }

            if (_fileSystem.PackagesFolder == null)
            {
                throw new ArgumentException("The file system provided by the initialization services provided by the script services builder has a null package folder.");
            }

            ScriptServices scriptServices = scriptServicesBuilder.Build();
            memoryBufferConsole = new MemoryBufferConsole();
            Repl repl = new Repl(arguments.ScriptArguments, _fileSystem, scriptServices.Engine,
                scriptServices.ObjectSerializer, scriptServices.Logger, memoryBufferConsole,
                scriptServices.FilePreProcessor, scriptServices.ReplCommands);

            var workingDirectory = _fileSystem.CurrentDirectory;
            var assemblies = scriptServices.AssemblyResolver.GetAssemblyPaths(workingDirectory);
            var scriptPacks = scriptServices.ScriptPackResolver.GetPacks();

            repl.Initialize(assemblies, scriptPacks, null);

            return repl;

        }

        private static ArgumentParseResult ParseArguments(string[] args)
        {
            var console = new ScriptConsole();
            try
            {
                var parser = new ArgumentHandler(new ArgumentParser(console), new ConfigFileParser(console), new FileSystem());
                return parser.Parse(args);
            }
            finally
            {
                console.Exit();
            }
        }

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
