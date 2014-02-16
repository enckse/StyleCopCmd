//------------------------------------------------------------------------------
// <copyright 
//  file="IFileRunner.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace StyleCopCmd.Core
{
    using StyleCop;

    /// <summary>A runner that uses a file can use this interface to set the output file name</summary>
    public interface IFileRunner
    {
        /// <summary>Gets or sets the output file to save results to</summary>
        string OutputFile { get; set; }
    }
}
