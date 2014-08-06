//------------------------------------------------------------------------------
// <copyright 
//  file="Generator.cs" 
//  company="Schley Andrew Kutz">
//  Copyright (c) Schley Andrew Kutz. All rights reserved.
// </copyright>
// <authors>
//   <author>Schley Andrew Kutz</author>
// </authors>
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