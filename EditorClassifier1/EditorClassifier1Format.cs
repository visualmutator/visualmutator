using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace EditorClassifier1
{
    #region Format definition
    /// <summary>
    /// Defines an editor format for the EditorClassifier1 type that has a purple background
    /// and is underlined.
    /// </summary>
  /*  [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "EditorClassifier1")]
    [Name("EditorClassifier1")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers*/
    internal sealed class EditorClassifier1Format : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "EditorClassifier1" classification type
        /// </summary>
        public EditorClassifier1Format()
        {
            this.DisplayName = "EditorClassifier1"; //human readable version of the name
        //    this.BackgroundColor = Colors.BlueViolet;
        // . ,  this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }
    #endregion //Format definition
}
