namespace VisualMutator.ViewModels
{
    #region

    using Model;
    using UsefulTools.Wpf;
    using Views;

    #endregion

    public class OptionsViewModel : ViewModel<IOptionsView>
    {
        public OptionsViewModel(IOptionsView view)
            : base(view)
        {
        }

        public void Show()
        {
            View.SetDefaultOwnerAndShowDialog();
        }

        public void Close()
        {
            View.Close();
        }

        private OptionsModel _options;
        public OptionsModel Options
        {
            get
            {
                return _options;
            }
            set
            {
                SetAndRise(ref _options, value, () => Options);
            }
        }

       
        private SmartCommand _commandSave;
        public SmartCommand CommandSave
        {
            get
            {
                return _commandSave;
            }
            set
            {
                SetAndRise(ref _commandSave, value, () => CommandSave);
            }
        }

        private SmartCommand _commandClose;
        public SmartCommand CommandClose
        {
            get
            {
                return _commandClose;
            }
            set
            {
                SetAndRise(ref _commandClose, value, () => CommandClose);
            }
        }

     
    }
}