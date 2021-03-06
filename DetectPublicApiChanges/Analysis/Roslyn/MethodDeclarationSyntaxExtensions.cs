﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DetectPublicApiChanges.Analysis.Roslyn
{
    /// <summary>
    /// Extension methods for ConstructorDeclarationSyntax
    /// </summary>
    public static class MethodDeclarationSyntaxExtensions
    {
        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public static IEnumerable<ParameterSyntax> GetParameters(this MethodDeclarationSyntax method)
        {
            var count = method.ParameterList.ChildNodes().Count();

            if (count == 0)
                return Enumerable.Empty<ParameterSyntax>();

            var parameters = method.ParameterList
                .ChildNodes()
                .OfType<ParameterSyntax>();

            return parameters;
        }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <param name="syntax">The syntax.</param>
        /// <returns></returns>
        public static string GetFullName(this MethodDeclarationSyntax syntax)
        {
            var parentNameSpace = string.Empty;
            var classStructure = syntax.Parent as ClassDeclarationSyntax;
            if (classStructure != null)
                parentNameSpace = classStructure.GetFullName();
            else if (syntax.Parent is InterfaceDeclarationSyntax)
                parentNameSpace = ((InterfaceDeclarationSyntax)syntax.Parent).GetFullName();

            return parentNameSpace + "." + syntax.Identifier;
        }
    }
}
