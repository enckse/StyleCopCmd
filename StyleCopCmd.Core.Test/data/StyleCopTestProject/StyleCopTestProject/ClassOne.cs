//-----------------------------------------------------------------------
// <copyright 
//  file="ClassOne.cs" 
//  company="Schley Andrew Kutz">
//  Copyright (c) Schley Andrew Kutz. All rights reserved.
// </copyright>
// <authors>
//   <author>Schley Andrew Kutz</author>
// </authors>
//-----------------------------------------------------------------------
namespace StyleCopTestProject
{
    public class ClassOne
    {

#if SOMECONDITIONAL
        private string Test;
#endif

#if SOMEOTHER
	private string Test2;
#endif
   
    }
}
