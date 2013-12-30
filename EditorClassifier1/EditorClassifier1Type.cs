using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace EditorClassifier1
{
    internal static class EditorClassifier1ClassificationDefinition
    {
        /// <summary>
        /// Defines the "EditorClassifier1" classification type.
        /// </summary>
       // [Export(typeof(ClassificationTypeDefinition))]
        //[Name("EditorClassifier1")]
        internal static ClassificationTypeDefinition EditorClassifier1Type = null;
    }
}
