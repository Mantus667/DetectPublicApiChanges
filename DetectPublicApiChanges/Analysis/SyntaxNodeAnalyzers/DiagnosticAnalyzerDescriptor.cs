﻿using DetectPublicApiChanges.Interfaces;

namespace DetectPublicApiChanges.Analysis.SyntaxNodeAnalyzers
{
    /// <summary>
    /// The DiagnosticAnalyzerDescriptor descibes an analyzer and holds meta information
    /// </summary>
    /// <seealso cref="IDiagnosticAnalyzerDescriptor" />
    public class DiagnosticAnalyzerDescriptor : IDiagnosticAnalyzerDescriptor
    {
        /// <summary>
        /// Gets the diagnostic identifier.
        /// </summary>
        /// <value>
        /// The diagnostic identifier.
        /// </value>
        public string DiagnosticId { get; set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public string Category { get; set; }

        /// <summary>
        /// Adds the description.
        /// </summary>
        /// <param name="desciption">The desciption.</param>
        /// <returns></returns>
        public IDiagnosticAnalyzerDescriptor AddDescription(string desciption)
        {
            Description = desciption;

            return this;
        }
    }
}
