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
        /// Gets the report settings
        /// </summary>
        protected ReportSettings Settings { get; private set; }

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

            this.Run(projects, outputGenerated, violation);
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

            this.InitInstance(options);
            this.initialized = true;
        }

        /// <summary>
        /// Initializes the instance with the given options
        /// </summary>
        /// <param name="options">Analysis options</param>
        protected abstract void InitInstance(RunnerOptions options);

        /// <summary>
        /// Add a source file for analysis
        /// </summary>
        /// <param name="project">Code project to add</param>
        /// <param name="path">Path to the file</param>
        protected abstract void AddSource(CodeProject project, string path);

        /// <summary>
        /// Run the analysis
        /// </summary>
        /// <param name="projects">Projects to analyze</param>
        /// <param name="outputGenerated">Events to call when style cop generates an output</param>
        /// <param name="violation">Events to call when style cop generates violations</param>
        protected abstract void Run(IList<CodeProject> projects, EventHandler<OutputEventArgs> outputGenerated, EventHandler<ViolationEventArgs> violation);

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
