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
    public sealed class ConsoleRunner : FileRunner
    {
        /// <inheritdoc />
        protected override StyleCopRunner InitInstance()
        {
            return new StyleCopConsole(
                this.SettingsFile,
                this.GetOptional<bool>(Optional.WriteCache.ToString(), true),
                this.OutputFile,
                this.AddIns,
                true);
        }

        /// <inheritdoc />
        protected override void Run(IList<CodeProject> projects)
        {
            ((StyleCopConsole)this.Console).Start(projects, !this.GetOptional<bool>(Optional.AllowCaching.ToString(), false));
        }
    }
}
