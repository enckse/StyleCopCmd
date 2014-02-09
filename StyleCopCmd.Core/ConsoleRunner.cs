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
    public sealed class ConsoleRunner : RunnerBase<StyleCopConsole>
    {
        /// <inheritdoc />
        protected override StyleCopRunner InitInstance(RunnerOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            return new StyleCopConsole(
                this.Settings.StyleCopSettingsFile,
                true,
                options.OutputFile,
                this.Settings.AddInDirectories,
                true);
        }

        /// <inheritdoc />
        protected override void Run(IList<CodeProject> projects)
        {
            this.Console.Start(projects, !this.Settings.AllowCaching);
        }
    }
}
