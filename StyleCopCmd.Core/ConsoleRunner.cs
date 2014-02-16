//------------------------------------------------------------------------------
// <copyright 
//  file="ConsoleRunner.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace StyleCopCmd.Core
{
    using System;
    using System.Collections.Generic;
    using StyleCop;

    /// <summary>
    /// Console runner implementation
    /// </summary>
    public sealed class ConsoleRunner : RunnerBase, IFileRunner
    {
        /// <inheritdoc />
        public string OutputFile { get; set; }

        /// <inheritdoc />
        protected override StyleCopRunner InitInstance()
        {
            return new StyleCopConsole(
                this.Settings.StyleCopSettingsFile,
                true,
                this.OutputFile,
                this.Settings.AddInDirectories,
                true);
        }

        /// <inheritdoc />
        protected override void Run(IList<CodeProject> projects)
        {
            ((StyleCopConsole)this.Console).Start(projects, !this.Settings.AllowCaching);
        }
    }
}
