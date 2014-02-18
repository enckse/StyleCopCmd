//------------------------------------------------------------------------------
// <copyright 
//  file="Optional.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace StyleCopCmd.Core
{
    /// <summary>
    ///  Optional arguments that are known by the core
    /// </summary>
    public enum Optional
    {
        /// <summary>
        /// Allow caching by the console runner
        /// </summary>
        AllowCaching,

        /// <summary>
        /// Allow cache writing by the console runner
        /// </summary>
        WriteCache
    }
}
