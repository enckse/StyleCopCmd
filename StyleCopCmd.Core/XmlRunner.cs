//------------------------------------------------------------------------------
// <copyright 
//  file="XmlRunner.cs" 
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
    /// XML runner implementation
    /// </summary>
    public sealed class XmlRunner : RunnerBase, IFileRunner
    {
        /// <inheritdoc />
        public string OutputFile { get; set; }

        /// <inheritdoc />
        protected override StyleCopRunner InitInstance()
        {
            return new StyleCopXmlRunner(
                this.Settings.StyleCopSettingsFile,
                this.OutputFile,
                this.Settings.AddInDirectories);
        }

        /// <inheritdoc />
        protected override void Run(IList<CodeProject> projects)
        {
            ((StyleCopXmlRunner)this.Console).Start(projects);
        }
    }
}
