using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace YankeeShower
{
    /// <summary>
    /// Interaction logic for PrintPreview.xaml
    /// </summary>
    public partial class PrintPreview : Window
    {      
        const int LetterWidth= 816;
        const int LetterHeight = 1056;
        const int A4Width = 794;
        const int A4Height = 1123;       
        Visual vsPattern;

        public PrintPreview()
        {
            InitializeComponent();           
        }
        
        public PrintPreview(Visual vsPattern1)
        {
            InitializeComponent();
            try
            {
                orientation.ItemsSource = new Collection<string> {"Portrait","Landscape" };
                papersize.ItemsSource = new Collection<string> {"A4", "Letter" };

                thedocument.Width = A4Width;
                thedocument.Height = A4Height;
                orientation.SelectedIndex = 0;
                papersize.SelectedIndex = 0;
                GetPrinters();

            }
            catch
            {
            }            
            vsPattern = vsPattern1;
        }
                      
        private void GetPrinters()
        {
            LocalPrintServer localPrintServer = new LocalPrintServer();
            // Retrieving collection of local printers on user's machine
            PrintQueueCollection localPrinterCollection = localPrintServer.GetPrintQueues();
            System.Collections.IEnumerator localPrinterEnumerator = localPrinterCollection.GetEnumerator();
            int index = 0;
            int defindx = -1;
            PrintQueue printer = LocalPrintServer.GetDefaultPrintQueue();
            while (localPrinterEnumerator.MoveNext())
            {
                printers.Items.Add(((PrintQueue)localPrinterEnumerator.Current).FullName);
                if (((PrintQueue)localPrinterEnumerator.Current).FullName == printer.FullName)
                    defindx = index;
                index++;
            }
            printers.SelectedIndex = defindx;
        }

        private PrintQueue GetPrintQueue(string printername)
        {
            LocalPrintServer localPrintServer = new LocalPrintServer();
            // Retrieving collection of local printers on user's machine
            PrintQueueCollection localPrinterCollection = localPrintServer.GetPrintQueues();
            System.Collections.IEnumerator localPrinterEnumerator = localPrinterCollection.GetEnumerator();
            while (localPrinterEnumerator.MoveNext())
            {
                if (((PrintQueue)localPrinterEnumerator.Current).FullName == printername)
                    return ((PrintQueue)localPrinterEnumerator.Current);
            }
            return null;
        }
             
        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
           // ChangePaperSize();
            PrintDialog pd = new PrintDialog
            {
                PrintQueue = GetPrintQueue(printers.Text)
            };
            PrintTicket pt = pd.PrintTicket;

            if (orientation.SelectedIndex == 0)
                pt.PageOrientation = PageOrientation.Portrait;
            else
                pt.PageOrientation = PageOrientation.Landscape;

            pt.PageBorderless = PageBorderless.Borderless;
            // pt.PageMediaSize = new PageMediaSize(_pagewidth, _pageheight);
            pd.PrintTicket = pt;

            PrintCapabilities capabilities = pd.PrintQueue.GetPrintCapabilities(pd.PrintTicket);

            const double inch = 96;
            // Set the margins.
            double xMargin = 0.75 * inch;
            double yMargin = 0.75 * inch;
            double printableWidth =  pt.PageMediaSize.Width.Value;           
            double printableHeight =  pt.PageMediaSize.Height.Value;
            double xScale = (printableWidth - xMargin * 2) / printableWidth;
            double yScale = (printableHeight - yMargin * 2) / printableHeight;

            thedocument.Width = thedocument.Width * xScale;
            thedocument.Height = thedocument.Height * yScale;

             Size sz = new Size( capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);
            //Size sz = new Size(thedocument.Width, thedocument.Height);
            //if (papersize.Text == "A4")
            //    sz = new Size(A4Width, A4Height);
            //else
            //    sz = new Size(LetterWidth, LetterHeight);

            //update the layout of the visual to the printer page size.
            thedocument.Measure(sz);
            thedocument.Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), sz));

            pd.PrintVisual(thedocument, "Printing...");
            //apply the original transform.
            ChangePaperSize();

        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            nozzletypes.Text = GetNozzleTypes();
            ShowRotationRange();
            GetNozzleSpacing();
            if (!string.IsNullOrEmpty(Application.Current.Resources[Constants.LeftLogoStr].ToString()))
            {
                if (File.Exists(Application.Current.Resources[Constants.LeftLogoStr].ToString()))
                {
                    BitmapImage leftimage = new BitmapImage();
                    leftimage.BeginInit();
                    leftimage.UriSource = new Uri(@"" + Application.Current.Resources[Constants.LeftLogoStr].ToString());
                    leftimage.EndInit();
                    logoleft.Source = leftimage;
                }
                else
                {
                    logoleft.Height = 0;
                }
            }
            if (!string.IsNullOrEmpty(Application.Current.Resources[Constants.RightLogoStr].ToString()))
            {
                if (File.Exists(Application.Current.Resources[Constants.RightLogoStr].ToString()))
                {
                    BitmapImage rightimage = new BitmapImage();
                    rightimage.BeginInit();
                    rightimage.UriSource = new Uri(@"" + Application.Current.Resources[Constants.RightLogoStr].ToString());
                    rightimage.EndInit();
                    logoright.Source = rightimage;
                }
                else
                {
                    logoright.Height = 0;
                }
            }
            if (!string.IsNullOrEmpty(Application.Current.Resources[Constants.FooterTextStr].ToString()))
            {
                footer.Text = Application.Current.Resources[Constants.FooterTextStr].ToString();
                Brush br = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Application.Current.Resources[Constants.FooterTextColorStr].ToString()));
                footer.Foreground = br;
            }
            if (string.IsNullOrEmpty(Application.Current.Resources[Constants.LeftLogoStr].ToString()) && string.IsNullOrEmpty(Application.Current.Resources[Constants.RightLogoStr].ToString()))
            {
                logoleft.Height = 0;
                logoright.Height = 0;
            }
           //   showerpattern.Source = VisualExtensions.BitmapSource(vsPattern);
            //System.Diagnostics.Debug.Print((DataContext as ViewModels.ShowerVM).TotalShowerWidth.ToString());


            showerpattern.Source = VisualExtensions.CropBitMap(vsPattern,(int)((double)(DataContext as ViewModels.ShowerVM).TotalShowerWidth * 1000));
        }

        private int GetAverageNozzleAngle()
        {
            int _angle = 0;
            int _ctr = 0;
            int _sum = 0;
            foreach (NozzleControl n in (DataContext as ViewModels.ShowerVM).Nozzles)
            {
                _sum = _sum + n.SprayRotation;
                _ctr++;
            }
            if (_ctr > 0)
                _angle = _sum / _ctr;

            return _angle;
        }

        private void ShowRotationRange()
        {
            if ((DataContext as ViewModels.ShowerVM).Nozzles.Count > 0)
            {

                int _min = (DataContext as ViewModels.ShowerVM).Nozzles[0].SprayRotation;
                int _max = (DataContext as ViewModels.ShowerVM).Nozzles[0].SprayRotation;
                int _angle = 0;
                int _ctr = 0;
                int _sum = 0;
                foreach (NozzleControl n in (DataContext as ViewModels.ShowerVM).Nozzles)
                {
                    if (n.SprayRotation > _max)
                        _max = n.SprayRotation;
                    if (n.SprayRotation < _min)
                        _min = n.SprayRotation;
                    _sum = _sum + n.SprayRotation;
                    _ctr++;
                }
                if (_ctr > 0)
                {
                    _angle = _sum / _ctr;
                    nozzlerotation.Text = _angle.ToString();
                    char degree = (char)176;
                    if (_angle != _min || _angle != _max)
                        nozzlerotationunit.Text = _min + degree.ToString() + " to " + _max + degree.ToString();
                }
            }
        }

        private string GetNozzleTypes()
        {
            var myDict = new Dictionary<string, string>();
            char degree = (char)176;
            foreach (NozzleControl n in (DataContext as ViewModels.ShowerVM).Nozzles)
            {
                if (!myDict.ContainsKey(n.NozzleTypeName))
                {
                    myDict.Add(n.NozzleTypeName, n.NozzleTypeName + "-" + n.SprayAngle + degree.ToString() + " Orifice Size: " + StaticData.GetOrificeFromID(n.NozzleOrificeID));
                }
            }
            string s = string.Join("; ", myDict.Select(x => x.Value));
            return s;
        }

        private void GetNozzleSpacing()
        {
            if ((DataContext as ViewModels.ShowerVM).Nozzles.Count > 0)
            {
                int _min = 0;
                int _max = 0;
                foreach (NozzleControl n in (DataContext as ViewModels.ShowerVM).Nozzles)
                {
                    if (n.NozzleSpacing > _max)
                        _max = n.NozzleSpacing;
                    if (n.NozzleSpacing < _min)
                        _min = n.NozzleSpacing;
                }
                if ((DataContext as ViewModels.ShowerVM).Nozzles.Count > 0)
                {
                    if (_min == _max)
                        nozzlespacing.Text = "";
                    else
                    {
                        if(_min != 0)
                           nozzlespacing.Text = _min.ToString() + " to " + _max.ToString(); 
                        else
                            nozzlespacing.Text = _max.ToString();
                    }
                        
                }
            }
        }

        private void ChangePaperSize()
        {
            if(papersize.SelectedIndex == 0) //A4
            {
                if (orientation.SelectedIndex == 0 ) //Portrait
                {
                    thedocument.Width = A4Width;
                    thedocument.Height = A4Height;
                }
                else//Landscape
                {
                    thedocument.Height = A4Width;
                    thedocument.Width = A4Height;
                }
            }
            else //Letter
            {
                if (orientation.SelectedIndex == 0)
                {
                    thedocument.Width = LetterWidth;
                    thedocument.Height = LetterHeight;
                }
                else
                {
                    thedocument.Height = LetterWidth;
                    thedocument.Width = LetterHeight;
                }
            }

        }

        private void Orientation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangePaperSize();
        }

        private void Papersize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangePaperSize();
        }
    }

}
