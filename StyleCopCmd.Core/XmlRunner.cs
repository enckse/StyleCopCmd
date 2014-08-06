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
    public sealed class XmlRunner : FileRunner
    {
        /// <inheritdoc />
        protected override StyleCopRunner InitInstance()
        {
            return new StyleCopXmlRunner(
                this.SettingsFile,
                this.OutputFile,
                this.AddIns);
        }

        /// <inheritdoc />
        protected override void Run(IList<CodeProject> projects)
        {
            ((StyleCopXmlRunner)this.Console).Start(projects);
        }
    }
}
