using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace YankeeShower.ViewModels
{
    public class DefaultSettingsViewModel : ViewModelBase
    {
        string _headerleftlogoname;
        string _headerrightlogoname;
        Color _footertextcolor = Colors.Black;
        public ICommand ChangeHeaderLeftImageCommand { get; set; }
        public ICommand ChangeHeaderRightImageCommand { get; set; }
        public ICommand DeleteHeaderRightImageCommand { get; set; }
        public ICommand DeleteHeaderLeftImageCommand { get; set; }
        public ICommand Save { get; set; }
        public ICommand CloseCommand { get; set; }

        public DefaultSettingsViewModel()
        {           
            //Load default settings
            ReportHeaderLeftLogo = Application.Current.Resources[Constants.LeftLogoStr].ToString();
            ReportHeaderRightLogo = Application.Current.Resources[Constants.RightLogoStr].ToString();
            ReportFooterText = Application.Current.Resources[Constants.FooterTextStr].ToString();
            FooterTextColor = (string.IsNullOrEmpty(Application.Current.Resources[Constants.FooterTextColorStr].ToString()) == true)
                ? Colors.Black
                : (Color)(ColorConverter.ConvertFromString(Application.Current.Resources[Constants.FooterTextColorStr].ToString()));
           
            ChangeHeaderLeftImageCommand = new RelayCommand(ExecuteChangeHeaderLeftImage, CanExecute);
            ChangeHeaderRightImageCommand = new RelayCommand(ExecuteChangeRightHeaderImage, CanExecute);
            DeleteHeaderRightImageCommand = new RelayCommand(ExecuteDeleteRightHeaderImage, CanExecute);
            DeleteHeaderLeftImageCommand = new RelayCommand(ExecuteDeleteLeftHeaderImage, CanExecute);
            Save = new RelayCommand(ExecuteSave, CanExecute);
            CloseCommand = new RelayCommand(ExecuteClose, CanExecute);

        }

        #region Commands
   
        private string GetNewImage()
        {
            IMessageBoxService _msgboxservice = new MessageBoxService();
            string _exefolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string result = _msgboxservice.OpenFileDlg("Select the file containing the customer's logo", true, false, "Logo Files(*.ICO; *.PNG; *.JPG; *.GIF)| *.ICO; *.PNG; *.JPG; *.GIF", null, _exefolder);
            _msgboxservice = null;
            return result;
        }

        public void ExecuteChangeHeaderLeftImage(object parameter)
        {
            string result = GetNewImage();
            if (!string.IsNullOrEmpty(result) && !string.IsNullOrWhiteSpace(result))
            {
                ReportHeaderLeftLogo = result;
            }           
        }

        public void ExecuteChangeRightHeaderImage(object parameter)
        {
            string result = GetNewImage();
            if (!string.IsNullOrEmpty(result) && !string.IsNullOrWhiteSpace(result))
            {
                ReportHeaderRightLogo = result;
            }
        }

        public void ExecuteDeleteRightHeaderImage(object parameter)
        {
            ReportHeaderRightLogo = string.Empty;            
        }

        public void ExecuteDeleteLeftHeaderImage(object parameter)
        {
            ReportHeaderLeftLogo = string.Empty;
        }
      
        public void ExecuteSave(object parameter)
        {
            ConfigFileManager.WriteJsonValue(Constants.LeftLogoStr, ReportHeaderLeftLogo);
            ConfigFileManager.WriteJsonValue(Constants.RightLogoStr, ReportHeaderRightLogo);
            ConfigFileManager.WriteJsonValue(Constants.FooterTextStr, ReportFooterText);
            ConfigFileManager.WriteJsonValue(Constants.FooterTextColorStr, FooterTextColor.ToString());
            ExecuteClose(parameter);

        }
      
        public void ExecuteClose(object parameter)
        {
            if (parameter is Window wnd)
                wnd.Close();
        }

        #endregion


        public string ReportFooterText
        {
            get;
            set;
        }

        public Color FooterTextColor
        {
            get { return _footertextcolor; }
            set { SetField(ref _footertextcolor, value); }
        }

        public string ReportHeaderRightLogo
        {
            get { return _headerrightlogoname; }
            set { SetField(ref _headerrightlogoname, value); }
        }
        public string ReportHeaderLeftLogo
        {
            get { return _headerleftlogoname; }
            set { SetField(ref _headerleftlogoname, value); }
        }
       

    }
}
