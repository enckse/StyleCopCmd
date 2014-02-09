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
    /// <typeparam name="T">StyleCop runner type</typeparam>
    public abstract class RunnerBase<T> 
        where T : StyleCopRunner
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
        /// Gets the report settings
        /// </summary>
        protected ReportSettings Settings { get; private set; }

        /// <summary>
        /// Gets the operating console for analysis
        /// </summary>
        protected T Console { get; private set; }

        /// <summary>
        /// Set the reporting settings
        /// </summary>
        /// <param name="settings">Analysis settings</param>
        public void Set(ReportSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            this.Settings = settings;
            this.hasSet = true;
        }

        /// <summary>
        /// Get the runner configuration
        /// </summary>
        /// <returns>Configuration for analysis</returns>
        public virtual Configuration Configure()
        {
            this.CheckSettings();
            return new Configuration(this.Settings.ProcessorSymbols != null ? this.Settings.ProcessorSymbols.ToArray() : null);
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
                this.Console.OutputGenerated += outputGenerated;
            }
            
            if (violation != null)
            {
                this.Console.ViolationEncountered += violation;
            }
   
            this.Run(projects);
            if (outputGenerated != null)
            {
                this.Console.OutputGenerated -= outputGenerated;
            }
            
            if (violation != null)
            {
                this.Console.ViolationEncountered -= violation;
            }
        }

        /// <summary>
        /// Initializes the instance for analysis
        /// </summary>
        /// <param name="options">Analysis options</param>
        public void Initialize(RunnerOptions options)
        {
            this.CheckSettings();
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            if (this.initialized)
            {
                return;
            }

            this.Console = (T)this.InitInstance(options);
            this.initialized = true;
        }

        /// <summary>
        /// Initializes the instance with the given options
        /// </summary>
        /// <param name="options">Analysis options</param>
        /// <returns>Analysis runner</returns>
        protected abstract StyleCopRunner InitInstance(RunnerOptions options);

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

            this.Console.Core.Environment.AddSourceCode(project, path, null);
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
