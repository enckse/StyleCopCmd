//------------------------------------------------------------------------------
// <copyright 
//  file="FileType.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace StyleCopCmd.Core
{
   /// <summary>
   /// File types that are available via settings
   /// </summary>
   public enum FileType
   {
       /// <summary>
       /// File type unknown
       /// </summary>
       Unknown = 0, 

       /// <summary>
       /// Single file
       /// </summary>
       File, 

       /// <summary>
       /// Directory of files
       /// </summary>
       Directory, 

       /// <summary>
       /// Solution file
       /// </summary>
       Solution, 

       /// <summary>
       /// Project file
       /// </summary>
       Project
   }
}
