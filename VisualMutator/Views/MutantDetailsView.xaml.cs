using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VisualMutator.Views
{
    using System.Text.RegularExpressions;

    using CommonUtilityInfrastructure.Comparers;
    using CommonUtilityInfrastructure.WpfUtils;
    using Model.Decompilation.CodeDifference;
    using VisualMutator.Controllers;
    using VisualMutator.ViewModels;

    /// <summary>
    /// Interaction logic for MutantDetailsView.xaml
    /// </summary>
    public partial class MutantDetailsView : UserControl, IMutantDetailsView
    {
        public MutantDetailsView()
        {
            InitializeComponent();

            codeTextBox.Document.PageWidth = 2000;
            codeTextBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            codeTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

        //    codeTextBox.FontFamily = new FontFamily("Consolas");
        }

     

        public void PresentCode(CodeWithDifference codeWithDifference)
        {
            var document = codeTextBox.Document;
            TextRange range = new TextRange(document.ContentStart, document.ContentEnd);
            range.Text = codeWithDifference.Code;

            foreach (var lineChange in codeWithDifference.LineChanges)
            {
                Color color = lineChange.ChangeType == LineChangeType.Add ? Colors.GreenYellow : Colors.PaleVioletRed;

                HighlightRow(document, lineChange.Text, color);
            }

        }

        public void ClearCode()
        {
            var document = codeTextBox.Document;
            TextRange range = new TextRange(document.ContentStart, document.ContentEnd);
            range.Text = "";
        }

        public void HighlightRow(FlowDocument document, string text, Color color)
        {
            
            Regex reg = new Regex(Regex.Escape(text), RegexOptions.Compiled | RegexOptions.IgnoreCase);

            var start = document.ContentStart;
            while (start != null && start.CompareTo(document.ContentEnd) < 0)
            {
                if (start.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    var match = reg.Match(start.GetTextInRun(LogicalDirection.Forward));

                    var textrange = new TextRange(
                        start.GetPositionAtOffset(match.Index, LogicalDirection.Forward),
                        start.GetPositionAtOffset(match.Index + match.Length, LogicalDirection.Backward));

                    textrange.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(color));

                    //start = textrange.End; 
                }
                start = start.GetNextContextPosition(LogicalDirection.Forward);
            }
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 'VisualMutator.Model.Mutations.CSharpCodeLanguage' in addedItems? ... 
            if (e.AddedItems.Cast<object>().All(i => i is TabItem))
            {
                ((MutantDetailsViewModel)this.DataContext)
                   .SelectedTabHeader = (string)e.AddedItems.Cast<TabItem>().Single().Header;
            }
           
        }

    }

    public interface IMutantDetailsView : IView
    {
      

        void PresentCode(CodeWithDifference codeWithDifference);

        void ClearCode();
    }
}
