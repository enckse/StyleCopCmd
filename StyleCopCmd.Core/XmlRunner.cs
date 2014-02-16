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
    public sealed class XmlRunner : RunnerBase<StyleCopXmlRunner>
    {
        /// <inheritdoc />
        protected override StyleCopRunner InitInstance(RunnerOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            if (this.Settings.AllowCaching)
            {
                throw new ArgumentException("Caching is not available using the XML-only runner");
            }

            return new StyleCopXmlRunner(
                this.Settings.StyleCopSettingsFile,
                options.OutputFile,
                this.Settings.AddInDirectories);
        }

        /// <inheritdoc />
        protected override void Run(IList<CodeProject> projects)
        {
            this.Console.Start(projects);
        }
    }
}
