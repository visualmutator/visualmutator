namespace PiotrTrzpil.VisualMutator_VSPackage.Views.AttachedBehaviors
{
    #region Usings

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    #endregion

    public class MouseDoubleClickBehavior
    {
        public static readonly DependencyProperty MouseDoubleClickProperty =
            DependencyProperty.RegisterAttached(
                "MouseDoubleClick",
                typeof(ICommand),
                typeof(MouseDoubleClickBehavior),
                new UIPropertyMetadata(MouseDoubleClickChanged));

        public static void SetMouseDoubleClick(DependencyObject target, ICommand value)
        {
            target.SetValue(MouseDoubleClickProperty, value);
        }

        private static void MouseDoubleClickChanged(
            DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var control = target as Control;
            if (control != null)
            {
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    control.MouseDoubleClick += MouseDoubleClick;
                }
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    control.MouseDoubleClick -= MouseDoubleClick;
                }
            }
        }

        private static void MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (!ReferenceEquals(sender, e.Source))
            {
                return;
            }

            var item = e.Source as TreeViewItem;
            if (item != null && item.IsSelected && e.OriginalSource is TextBlock)
            {
                // Control control = (Control)sender;
                var command = (ICommand)item.GetValue(MouseDoubleClickProperty);
                var arguments = new object[] { };
                e.Handled = true;
                command.Execute(arguments);
            }
        }
    }
}