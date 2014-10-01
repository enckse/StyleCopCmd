//------------------------------------------------------------------------------
// <copyright 
//  file="Project.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace StyleCopCmd.Core
{
    /// <summary>
    /// Representation of a project in memory
    /// </summary>
    public sealed class Project
    {   
        /// <summary>
        /// Initializes a new instance of the <see cref="StyleCopCmd.Core.Project"/> class.
        /// </summary>
        /// <param name='id'>
        /// Identifier for the project
        /// </param>
        /// <param name='location'>
        /// Location of the project
        /// </param>
        public Project(int id, string location)
        {
            this.Id = id;
            this.Location = location;
        }
        
        /// <summary>
        /// Gets the ID of the project
        /// </summary>
        /// <value>
        /// The id value
        /// </value>
        public int Id
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>
        /// The project location.
        /// </value>
        public string Location
        {
            get;
            private set;
        }
    }
}