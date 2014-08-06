//------------------------------------------------------------------------------
// <copyright 
//  file="FileRunner.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace StyleCopCmd.Core
{
    using StyleCop;

    /// <summary>A runner that uses a file can use this type to set the output file name</summary>
    public abstract class FileRunner : RunnerBase
    {
        /// <summary>Gets or sets the output file to save results to</summary>
        public string OutputFile { get; set; }
    }
}
