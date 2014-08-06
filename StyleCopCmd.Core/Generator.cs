//------------------------------------------------------------------------------
// <copyright 
//  file="FileRunner.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace StyleCopCmd.Core
{
    /// <summary>
    /// Available generators
    /// </summary>
    public enum Generator
    {
        /// <summary>
        /// Default generator (currently console runner)
        /// </summary>
        Default,

        /// <summary>
        /// Maps to the console runner
        /// </summary>
        Console,

        /// <summary>
        /// XML runner (output only, no reporting)
        /// </summary>
        Xml
    }
}