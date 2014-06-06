//------------------------------------------------------------------------------
// <copyright 
//  file="RunnerBase.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace StyleCopCmd.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using StyleCop;

    /// <summary>
    /// Runner base class used to perform reports/analysis
    /// </summary>
    public abstract class RunnerBase
    {
        /// <summary>
        /// Indicates if the initialize steps have been properly called
        /// </summary>
        private bool initialized = false;

        /// <summary>
        /// Indicates if the settings have been set
        /// </summary>
        private bool hasSet = false;

        /// <summary>
        /// Write output (e.g. console) using this action
        /// </summary>
        private Action<Type, string> writeOutputTo;

        /// <summary>
        /// Gets the operating console for analysis
        /// </summary>
        protected StyleCopRunner Console { get; private set; }

        /// <summary>
        /// Gets the given processor symbols to execute with
        /// </summary>
        protected IList<string> Symbols { get; private set; }
        
        /// <summary>
        /// Gets the optional values loaded into the context for the runner
        /// </summary>
        protected IDictionary<string, object> Optionals { get; private set; }
        
        /// <summary>
        /// Gets the addin paths for the runner to use
        /// </summary>
        protected IList<string> AddIns { get; private set; }

        /// <summary>
        /// Gets the settings file to use for the runner
        /// </summary>
        protected string SettingsFile { get; private set; }

        /// <summary>
        /// Set the reporting settings
        /// </summary>
        /// <param name="processorSymbols">Processor symbols to include</param>
        /// <param name="optValues">Optional values for the runner</param>
        /// <param name="addInDirs">Add-in directories</param>
        /// <param name="settingsFile">Settings file</param>
        /// <param name="writeTo">Output writer</param>
        public void Set(IList<string> processorSymbols, IDictionary<string, object> optValues, IList<string> addInDirs, string settingsFile, Action<Type, string> writeTo)
        {
            this.Symbols = processorSymbols != null ? processorSymbols.ToList() : null;

            // Deep clone the array, even if the values are shallow
            this.Optionals = optValues != null ? optValues.ToDictionary(key => key.Key, val => val.Value, StringComparer.OrdinalIgnoreCase) : null;
            this.AddIns = addInDirs != null ? addInDirs.ToList() : null;
            this.SettingsFile = settingsFile;
            this.writeOutputTo = writeTo;
            this.hasSet = true;
        }

        /// <summary>
        /// Get the runner configuration
        /// </summary>
        /// <returns>Configuration for analysis</returns>
        public virtual Configuration Configure()
        {
            this.CheckSettings();
            return new Configuration(this.Symbols != null ? this.Symbols.ToArray() : null);
        }

        /// <summary>
        /// Add a source file for analysis
        /// </summary>
        /// <param name="project">Code project to add</param>
        /// <param name="path">Path to the file</param>
        public void AddFile(CodeProject project, string path)
        {
            this.CheckSettings();
            if (!this.initialized)
            {
                throw new InvalidOperationException("Instance is not initialized");
            }

            this.WriteDebugLine("Adding project with path " + path ?? string.Empty);
            this.AddSource(project, path);
        }

        /// <summary>
        /// Start the analysis
        /// </summary>
        /// <param name="projects">Projects to analyze</param>
        /// <param name="outputGenerated">Events to call when style cop generates an output</param>
        /// <param name="violation">Events to call when style cop generates violations</param>
        public void Start(IList<CodeProject> projects, EventHandler<OutputEventArgs> outputGenerated, EventHandler<ViolationEventArgs> violation)
        {
            this.CheckSettings();
            if (!this.initialized)
            {
                throw new InvalidOperationException("Instance is not initialized");
            }

            if (projects == null)
            {
                throw new ArgumentNullException("projects");
            }

            if (outputGenerated != null)
            {
                this.WriteDebugLine("Attaching output events");
                this.Console.OutputGenerated += outputGenerated;
            }
            
            if (violation != null)
            {
                this.WriteDebugLine("Attaching violation events");
                this.Console.ViolationEncountered += violation;
            }

            this.WriteDebugLine("Running analysis");
            this.Run(projects);
            if (outputGenerated != null)
            {
                this.WriteDebugLine("Removing output events");
                this.Console.OutputGenerated -= outputGenerated;
            }
            
            if (violation != null)
            {
                this.WriteDebugLine("Removing violation events");
                this.Console.ViolationEncountered -= violation;
            }
        }

        /// <summary>
        /// Initializes the instance for analysis
        /// </summary>
        public void Initialize()
        {
            this.CheckSettings();
            if (this.initialized)
            {
                this.WriteDebugLine("Already initialized");
                return;
            }

            this.WriteDebugLine("Initializing");
            this.Console = this.InitInstance();
            this.initialized = true;
        }

        /// <summary>
        /// Get an optional settings value
        /// </summary>
        /// <typeparam name="T">Parameter type</typeparam>
        /// <param name="key">Key to look for</param>
        /// <param name="defaultValue">Default value if the key is not found or invalid</param>
        /// <returns>The value as set (if set) or the default value</returns>
        protected T GetOptional<T>(string key, T defaultValue)
        {
            this.WriteDebugLine("Checking for optional value " + key ?? string.Empty);
            if (this.Optionals != null && this.Optionals.ContainsKey(key))
            {
                this.WriteDebugLine("Setting found");
                var val = this.Optionals[key];
                if (val != null)
                {
                    this.WriteDebugLine("Value is viable");
                    return (T)Convert.ChangeType(val, typeof(T), System.Globalization.CultureInfo.CurrentCulture);
                }
            }

            this.WriteDebugLine("Using default optional value for key");
            return defaultValue;
        }

        /// <summary>
        /// Initializes the instance with the given options
        /// </summary>
        /// <returns>Analysis runner</returns>
        protected abstract StyleCopRunner InitInstance();

        /// <summary>
        /// Add a source file for analysis
        /// </summary>
        /// <param name="project">Code project to add</param>
        /// <param name="path">Path to the file</param>
        protected virtual void AddSource(CodeProject project, string path)
        {
            if (project == null)
            {
                throw new ArgumentNullException("project");
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            this.WriteDebugLine("Path being added to core environment");
            this.Console.Core.Environment.AddSourceCode(project, path, null);
        }

        /// <summary>
        /// Write out a debug line
        /// </summary>
        /// <param name="message">Message to write</param>
        protected virtual void WriteDebugLine(string message)
        {
            if (this.writeOutputTo != null)
            {
                this.writeOutputTo(this.GetType(), message);
            }
        }

        /// <summary>
        /// Run the analysis
        /// </summary>
        /// <param name="projects">Projects to analyze</param>
        protected abstract void Run(IList<CodeProject> projects);

        /// <summary>
        /// Checks if the settings have been properly set or throws an exception if they have not been set
        /// </summary>
        private void CheckSettings()
        {
            if (!this.hasSet)
            {
                throw new InvalidOperationException("No settings configured");
            }
        }
    }
}
