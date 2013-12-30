namespace EditorClassifier1
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IGlyphFactoryProvider))]
[Name("TodoGlyph")]
[Order(After = "VsTextMarker")]
[ContentType("code")]
[TagType(typeof(TodoTag))]
internal sealed class TodoGlyphFactoryProvider : IGlyphFactoryProvider
{
        public IGlyphFactory GetGlyphFactory(IWpfTextView view, IWpfTextViewMargin margin)
        {
            return new TodoGlyphFactory();
        }
    }
}