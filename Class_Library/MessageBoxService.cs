using System.Windows;

namespace YankeeShower
{
    public interface IMessageBoxService
    {
        GenericMessageBoxResult Show(string message, string caption, GenericMessageBoxButton buttons);
        bool ShowMessage(string text, string caption, GenericMessageBoxButton messageType);
        void Show(string message, string caption);
        string OpenFileDlg(string _title, bool _validatenames, bool _multiselect, string _filter, Window _owner, string _initdir);
        string SaveFileDlg(string _title, string _filter, Window _owner, string _initdir);
        void OpenDefaultsDlg(Window _owner);
        GenericMessageBoxResult ShowMessage(string text, string caption, GenericMessageBoxButton messagebutton, GenericMessageBoxIcon messageicon);
        void OpenAboutDlg(Window _owner);
        void OpenDesignerDlg(Window _owner, FullyObservableCollection<NozzleType> _nozzletypes);        
        void OpenNozzlesDlg2(ViewModels.ShowerVM vm);
    }

    public enum GenericMessageBoxButton
    {
        OK,
        OKCancel,
        YesNo,
        YesNoCancel
    }

    public enum GenericMessageBoxResult
    {
        OK,
        Cancel,
        No,
        Yes
    }

    public enum GenericMessageBoxIcon
    {
        Asterisk,
        Error,
        Exclamation,
        Hand,
        Information,
        None,
        Question,
        Stop,
        Warning
    }

    public class MessageBoxService : IMessageBoxService
    {
        public void OpenDefaultsDlg(Window _owner)
        {
            Views.DefaultSettings dlg = new Views.DefaultSettings();

            if (_owner == null)
                dlg.ShowDialog();
            else
            {
                dlg.Owner = _owner;
                dlg.ShowDialog();
            }          
        }

        public void OpenNozzlesDlg2(ViewModels.ShowerVM vm)
        {
            Window _owner;
            _owner = Application.Current.Windows[0];
            Views.NozzleSettings dlg = new Views.NozzleSettings(vm)
            {
                Owner = _owner
            };
            dlg.ShowDialog();
        }

        public void OpenAboutDlg(Window _owner)
        {
            AboutView about = new AboutView();
            
            if (_owner == null)
                about.ShowDialog();
            else
            {
                about.Owner = _owner;
                about.ShowDialog();
            }
        }
        
        public void OpenDesignerDlg(Window _owner, FullyObservableCollection<NozzleType> _nozzletypes)
        {
            Views.NozzlePattern np = new Views.NozzlePattern(_nozzletypes);
            
            if (_owner == null)
                np.ShowDialog();
            else
            {
                np.Owner = _owner;
                np.ShowDialog();
            }
        }

        public string OpenFileDlg(string _title, bool _validatenames, bool _multiselect, string _filter, Window _owner, string _initdir)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = _filter,
                Title = _title,
                ValidateNames = _validatenames,
                Multiselect = _multiselect,
                InitialDirectory = _initdir
            };
            if (_owner == null)
                dlg.ShowDialog();
            else
                dlg.ShowDialog(_owner);
            string _filename = dlg.FileName ?? "";
            return _filename;
        }

        public string SaveFileDlg(string _title, string _filter, Window _owner, string _initdir)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                Filter = _filter,
                Title = _title,
                ValidateNames = true,
                InitialDirectory = _initdir
            };
            if (_owner == null)
                dlg.ShowDialog();
            else
                dlg.ShowDialog(_owner);
            string _filename = dlg.FileName ?? "";
            return _filename;
        }
       
        public GenericMessageBoxResult Show(string message, string caption, GenericMessageBoxButton buttons)
        {
            //   var slButtons = buttons == GenericMessageBoxButton.Ok
            //                    ? MessageBoxButton.OK
            //                   : MessageBoxButton.OKCancel;


            var slButtons = MessageBoxButton.OK;
            switch (buttons)
            {
                case GenericMessageBoxButton.OK:
                    slButtons = MessageBoxButton.OK;
                    break;
                case GenericMessageBoxButton.OKCancel:
                    slButtons = MessageBoxButton.OKCancel;
                    break;
                case GenericMessageBoxButton.YesNo:
                    slButtons = MessageBoxButton.YesNo;
                    break;
                case GenericMessageBoxButton.YesNoCancel:
                    slButtons = MessageBoxButton.YesNoCancel;
                    break;
                default:
                    break;
            }
            
            var result = System.Windows.MessageBox.Show(message, caption, slButtons);
            var returnedResult = GenericMessageBoxResult.OK;
            switch (result)
            {
                case MessageBoxResult.OK:
                    returnedResult = GenericMessageBoxResult.OK;
                    break;
                case MessageBoxResult.Cancel:
                    returnedResult = GenericMessageBoxResult.Cancel;
                    break;
                case MessageBoxResult.Yes:
                    returnedResult = GenericMessageBoxResult.Yes;
                    break;
                case MessageBoxResult.No:
                    returnedResult = GenericMessageBoxResult.No;
                    break;
                default:
                    break;

            }

            return returnedResult;
        }

        public void Show(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK);
        }

        public bool ShowMessage(string text, string caption, GenericMessageBoxButton messageType)
        {
           return MessageBox.Show(text, caption, MessageBoxButton.OK) == MessageBoxResult.OK;
        }

        public GenericMessageBoxResult ShowMessage(string text, string caption, GenericMessageBoxButton messagebutton, GenericMessageBoxIcon messageicon)
        {
            var slIcon = MessageBoxImage.None;
            switch (messageicon)
            {
                case GenericMessageBoxIcon.Asterisk:
                    slIcon = MessageBoxImage.Asterisk;
                    break;
                case GenericMessageBoxIcon.Error:
                    slIcon = MessageBoxImage.Error;
                    break;
                case GenericMessageBoxIcon.Exclamation:
                    slIcon = MessageBoxImage.Exclamation;
                    break;
                case GenericMessageBoxIcon.Hand:
                    slIcon = MessageBoxImage.Hand;
                    break;
                case GenericMessageBoxIcon.Information:
                    slIcon = MessageBoxImage.Information;
                    break;
                case GenericMessageBoxIcon.None:
                    slIcon = MessageBoxImage.None;
                    break;
                case GenericMessageBoxIcon.Question:
                    slIcon = MessageBoxImage.Question;
                    break;
                case GenericMessageBoxIcon.Stop:
                    slIcon = MessageBoxImage.Stop;
                    break;
                case GenericMessageBoxIcon.Warning:
                    slIcon = MessageBoxImage.Warning;
                    break;

                default:
                    break;
            }

            var slButtons = MessageBoxButton.OK;
            switch (messagebutton)
            {
                case GenericMessageBoxButton.OK:
                    slButtons = MessageBoxButton.OK;
                    break;
                case GenericMessageBoxButton.OKCancel:
                    slButtons = MessageBoxButton.OKCancel;
                    break;
                case GenericMessageBoxButton.YesNo:
                    slButtons = MessageBoxButton.YesNo;
                    break;
                case GenericMessageBoxButton.YesNoCancel:
                    slButtons = MessageBoxButton.YesNoCancel;
                    break;
                default:
                    break;
            }

            var result = System.Windows.MessageBox.Show(text, caption, slButtons, slIcon);
            var returnedResult = GenericMessageBoxResult.OK;
            switch (result)
            {
                case MessageBoxResult.OK:
                    returnedResult = GenericMessageBoxResult.OK;
                    break;
                case MessageBoxResult.Cancel:
                    returnedResult = GenericMessageBoxResult.Cancel;
                    break;
                case MessageBoxResult.Yes:
                    returnedResult = GenericMessageBoxResult.Yes;
                    break;
                case MessageBoxResult.No:
                    returnedResult = GenericMessageBoxResult.No;
                    break;
                default:
                    break;
            }
            return returnedResult;
        }

       
    }

}
