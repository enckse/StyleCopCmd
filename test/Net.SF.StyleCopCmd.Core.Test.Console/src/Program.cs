//------------------------------------------------------------------------------
// <copyright 
//  file="Program.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
// <authors>
//   <author>enckse</author>
// </authors>
//------------------------------------------------------------------------------

namespace Net.SF.StyleCopCmd.Core.Test.Console
{
    using System;
    using System.Linq;
    using System.Reflection;
    using NUnit.Framework;
    
    /// <summary>
    /// Program to provide a testing wrapper due to some short-term issues with nunit-console
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name='args'>
        /// The command-line arguments.
        /// </param>
        public static void Main(string[] args)
        {
            foreach (var type in Assembly.GetAssembly(typeof(ReportBuilderTest)).GetTypes())
            {
                var instance = Activator.CreateInstance(type);
                foreach (var method in type.GetMethods())
                {
                    var attribute = method.GetCustomAttributes(typeof(TestAttribute), false);
                    if (attribute.Any())
                    {
                        System.Console.Write(method.Name);
                        System.Console.Write("...");
                        try
                        {
                            method.Invoke(instance, null);
                            System.Console.WriteLine("Passed");
                        }
                        catch(Exception error)
                        {
                            System.Console.WriteLine("Failed");
                            System.Console.WriteLine(error);
                        }       
                    }
                }
            }
        }
    }
}
