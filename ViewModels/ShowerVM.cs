using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static YankeeShower.EngineeringCalcs;


namespace YankeeShower.ViewModels
{
    [Serializable()]
    public class ShowerVM : ViewModelBase
    {
        double _defaultYankeeWidth = Constants._defaultYankeeWidth; //m
        int _defaultYankeeSpeed = Constants._defaultYankeeSpeed; //m/min
        int _defaultdwelldistance = Constants._defaultDwellDistance;
        double _defaultYankeeDiameter = Constants._defaultYankeeDiameter; //m
        string coveragecolor = Constants._patternPenColor;

        double _totalshowerwidth = 0;
        double _evapload = 0;
        double _evaploadkgperhrpersqm = 0;
        int _dwelltime = 0;
        double _operatingpressure = Constants._operatingpressure;
        int _showertemperature = Constants._showertemperature;
        double _showerflow = 0;
        int _numbernozzles;
        double[] _showerpattern;
        int yankeerevolution;
        double totalevapload;             
        
        FullyObservableCollection<NozzleControl> _nozzles;

        bool _canexecuteopen = true;
        bool _canexecuteresize = true;
        bool _canexecuteresizegroup = true;
        bool _canexecutenew = true;
        
        public ICommand OpenNozzleSettings { get; set; }
        public ICommand CopyToClipboardCommand { get; set; }
        public ICommand IncrSpacing { get; set; }
        public ICommand DecrSpacing { get; set; }
        public ICommand IncrNozzleToYankee { get; set; }
        public ICommand DecrNozzleToYankee { get; set; }
        public ICommand IncrNozzleRotation { get; set; }
        public ICommand DecrNozzleRotation { get; set; }
        public ICommand IncrNozzleAngle { get; set; }
        public ICommand DecrNozzleAngle { get; set; }
        public ICommand OpenDefaults { get; set; }
        public ICommand OpenAbout { get; set; }
        public ICommand OpenDesigner { get; set; }
        public ICommand SaveShower { get; set; }
        public ICommand NewShower { get; set; }
        public ICommand OpenShower { get; set; }
        public ICommand OpenShowerFile { get; set; }
        public ICommand OpenHelp { get; set; }

        ObservableCollection<CustomMenuItem> _menuitems = new ObservableCollection<CustomMenuItem>();

        public ShowerVM()
        {

            StaticData.CreateNozzledata();
            StaticData.CreateNozzleList();
            _nozzletypes = StaticData.NozzleTypes;
        
            _nozzles = new FullyObservableCollection<NozzleControl>();
            Nozzles.CollectionChanged += Nozzles_CollectionChanged;
            Nozzles.ItemPropertyChanged += Nozzles_ItemPropertyChanged;  

            RecalcThermalLoad();

            OpenNozzleSettings = new RelayCommand(ExecuteOpenNozzles, CanExecuteOpen);
            CopyToClipboardCommand = new RelayCommand(ExecuteCopyToClipboard, CanExecute);
            IncrSpacing = new RelayCommand(ExecuteIncrSpacing, CanExecuteResize);
            DecrSpacing = new RelayCommand(ExecuteDecrSpacing, CanExecuteResize);
            IncrNozzleToYankee = new RelayCommand(ExecuteIncrNozzleToYankee, CanExecuteResizeGroup);
            DecrNozzleToYankee = new RelayCommand(ExecuteDecrNozzleToYankee, CanExecuteResizeGroup);
            IncrNozzleRotation = new RelayCommand(ExecuteIncrNozzleRotation, CanExecuteResizeGroup);
            DecrNozzleRotation = new RelayCommand(ExecuteDecrNozzleRotation, CanExecuteResizeGroup);
            IncrNozzleAngle = new RelayCommand(ExecuteIncrNozzleAngle, CanExecuteResizeGroup);
            DecrNozzleAngle = new RelayCommand(ExecuteDecrNozzleAngle, CanExecuteResizeGroup);
            OpenDefaults = new RelayCommand(ExecuteOpenDefaults, CanExecute);
            OpenAbout = new RelayCommand(ExecuteOpenAbout, CanExecute);
            OpenDesigner = new RelayCommand(ExecuteOpenDesigner, CanExecute);
            SaveShower = new RelayCommand(ExecuteSaveShower, CanExecute);
            NewShower = new RelayCommand(ExecuteNewShower, CanExecuteNew);
            OpenShower = new RelayCommand(ExecuteOpenShower, CanExecute);
            OpenShowerFile = new RelayCommand(ExecuteOpenShowerFile, CanExecute);

            OpenHelp = new RelayCommand(ExecuteOpenHelp, CanExecute);

            MenuItems = MRUList.InitialiseMRU(OpenShowerFile);

            if (!(Application.Current.Resources[Constants.eulaccepted].ToString()=="true"))
                ExecuteOpenAbout(null);
        }

        #region Event Handlers

        private void Nozzles_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            //recalc shower           
            if (e.PropertyName != "ImagePoints")
            {
                MergeNozzlePatterns2();
                RecalcThermalLoad();
            }
        }

        private void Nozzles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {         
            MergeNozzlePatterns2();
            NumberNozzles = Nozzles.Count;         
            RecalcThermalLoad();
        }

        #endregion

        #region Properties
                
        public int ShowerPatternGridHeight
        {
            get { return Constants._showerpatternheight; }
            set { }
        }

        public FullyObservableCollection<NozzleControl> Nozzles
        {
            get { return _nozzles; }
            set { SetField(ref _nozzles, value); }
        }

        public double TotalShowerWidth
        {
            get { return _totalshowerwidth; }
            set { SetField(ref _totalshowerwidth, value); }
        }
        
        public double EvapEnergy
        {
            get { return _evapload; }
            set { SetField(ref _evapload, value); }
        }

        
        public double TotalEvapLoad
        {
            get { return totalevapload; }
            set { SetField(ref totalevapload, value); }
        }


        public double EvapLoadkgperhrpersqm
        {
            get { return _evaploadkgperhrpersqm; }
            set { SetField(ref _evaploadkgperhrpersqm, value); }
        }

        public double YankeeWidth
        {
            get { return _defaultYankeeWidth; }
            set { SetField(ref _defaultYankeeWidth, value);
                RecalcThermalLoad();
            }
        }

        public int YankeeSpeed
        {
            get { return _defaultYankeeSpeed; }
            set { SetField(ref _defaultYankeeSpeed, value);
                RecalcThermalLoad();
            }
        }

        public double OperatingPressure
        {
            get { return _operatingpressure; }
            set
            {
                SetField(ref _operatingpressure, value);
                PressureChanged();            
            }
        }

        public int ShowerTemperature
        {
            get { return _showertemperature; }
            set { SetField(ref _showertemperature, value);
                RecalcEvapEnergy();
            }
        }

        public int DwellTime
        {
            get { return _dwelltime; }
            set { SetField(ref _dwelltime, value); }
        }

        
        public int YankeeRevolution
        {
            get { return yankeerevolution; }
            set { SetField(ref yankeerevolution, value); }
        }


        public int DwellDistance
        {
            get { return _defaultdwelldistance; }
            set
            {
                SetField(ref _defaultdwelldistance, value);
                RecalcThermalLoad();
            }
        }

        public double YankeeDiameter
        {
            get { return _defaultYankeeDiameter; }
            set { SetField(ref _defaultYankeeDiameter, value);
                RecalcThermalLoad();
            }
        }

        public int NumberNozzles
        {
            get { return _numbernozzles; }
            set { SetField(ref _numbernozzles, value); }
        }

        public double ShowerFlow
        {
            get { return _showerflow; }
            set { SetField(ref _showerflow, value); }
        }
       
        public double[] ShowerPattern
        {
            get { return _showerpattern; }
            set { SetField(ref _showerpattern, value); }
        }

        FullyObservableCollection<NozzleType> _nozzletypes;
        public FullyObservableCollection<NozzleType> NozzleTypes
        {
            get { return _nozzletypes; }
            set { SetField(ref _nozzletypes, value); }
        }
        
        public ObservableCollection<CustomMenuItem> MenuItems
        {
            get { return _menuitems; }
            set { SetField(ref _menuitems, value); }
        }
                       
        public string CoverageColour
        {
            get { return coveragecolor; }
            set { SetField(ref coveragecolor, value); }
        }

        int smoothingdegree = 0;
        public int SmoothingDegree
        {
            get { return smoothingdegree; }
            set { SetField(ref smoothingdegree, value);
                MergeNozzlePatterns2();
            }
        }

        int maximumnozzletoyankee = Constants._defaultNozzletoYankee;
        public int MaximumNozzleToYankee
        {
            get { return maximumnozzletoyankee; }
            set { SetField(ref maximumnozzletoyankee, value); }
        }



        #endregion

        #region Private functions

        private void GetTotalShowerWidth()
        {
            int _start = -1;
            int _end = -1;

            if (_showerpattern != null)
            {
                int _l = _showerpattern.Length;

                for (int i = 0; i < _l; i++)
                {
                    if (_showerpattern[i] > 0)
                    {
                        _start = i;
                        break;
                    }
                }

                for (int j = _l - 1; j >= 0; j--)
                {
                    if (_showerpattern[j] > 0)
                    {
                        _end = j;
                        break;
                    }
                }

                int _len = 0;
                if (_end > -1 && _start > -1)
                    _len = _end - _start + 1;// 470 - 0 = 470 but 0 is a pixel so add 1
                TotalShowerWidth = 0.001 * _len;
            }
        }

        private void RecalcEvapEnergy()
        {
            RecalcFlow();           
            if (Nozzles.Count > 0)
            {
                EvapEnergy = CalcEvaporationEnergy(ShowerTemperature, ShowerFlow);
            }
        }

        private void RecalcTotalEvapLoad()
        {
            TotalEvapLoad = CalcTotalEvaporativeLoad(YankeeDiameter, YankeeSpeed, YankeeWidth, ShowerFlow);
        }

        private void RecalcThermalLoad()
        {
            DwellTime = CalcDwellTime(YankeeDiameter, YankeeSpeed, DwellDistance);
            YankeeRevolution = CalcRevolutionTime(YankeeDiameter, YankeeSpeed);
            EvapLoadkgperhrpersqm = CalcNipToSPREvaporativeLoad(YankeeDiameter, YankeeSpeed, DwellDistance, YankeeWidth, ShowerFlow);
            RecalcEvapEnergy();
            RecalcTotalEvapLoad();
        }

        private void RecalcFlow()
        {
            double flow = 0;
            foreach(NozzleControl n in Nozzles)            
                flow = flow + n.NozzleFlow; 
            
            ShowerFlow = flow;
        }          

        private void PressureChanged()
        {
            foreach(NozzleControl n in Nozzles)
                n.NozzlePressure = OperatingPressure;

            RecalcFlow();
            RecalcThermalLoad();
        }


        //update this for custom nozzles
        //private void MergeNozzlePatterns()
        //{
        //    //10000 = 10m is practical limit for a shower
        //    int _profilewidth = _maxshowerwidth;// Convert.ToInt32(ActualTotalShowerWidth * 1000) ;

        //    double[] _showerpattern = new double[_profilewidth];
        //    int ctr = 0;
        //    int nozzlestartidx = 0;
        //    //build nozzle spray pattern and add to overall spray volume
            
        //    double _nozzlecentre = 0;

        //    for (int i = 0; i < Nozzles.Count; i++)
        //    {
        //        if (i == 0)
        //        {
        //            if (Nozzles[i].NozzleTypeName.Equals("Standard"))
        //                _nozzlecentre = Nozzles[i].NozzleSprayWidth / 2;
        //            else
        //                if (Nozzles[i].NozzleTypeName.Equals("Asymmetric Left") || Nozzles[i].NozzleTypeName.Equals("Asymmetric Right"))
        //                    _nozzlecentre = Nozzles[i].NozzleSprayWidth;
        //                else
        //                {
        //                    //custom nozzle



        //                }
        //        }                    
        //        else
        //            _nozzlecentre = _nozzlecentre + Nozzles[i].NozzleSpacing;

        //        if (Nozzles[i].NozzleTypeName.Equals("Standard"))
        //            nozzlestartidx = Convert.ToInt32(_nozzlecentre - Nozzles[i].NozzleSprayWidth / 2);
        //        else
        //            if(Nozzles[i].NozzleTypeName.Equals("Asymmetric Left")|| Nozzles[i].NozzleTypeName.Equals("Asymmetric Right"))
        //               nozzlestartidx = Convert.ToInt32(_nozzlecentre - Nozzles[i].NozzleSprayWidth);
        //            else
        //            {
        //                //custom nozzle



        //            }
        //        ////combine nozzle spray patterns
        //        ctr = 0;
        //      //  for (int j = 0; j < Nozzles[i].SprayPattern.GetUpperBound(0); j++)
        //            for (int j = 0; j < Nozzles[i].SprayPattern.Length; j++)
        //            {
        //            if ((nozzlestartidx + ctr) >= 0)
        //            {
        //                if ((nozzlestartidx + ctr) < _profilewidth && (nozzlestartidx + ctr) >= 0)
        //                {
        //                    _showerpattern[nozzlestartidx + ctr] = _showerpattern[nozzlestartidx + ctr] + Nozzles[i].SprayPattern[j];
        //                    ctr++;
        //                }
        //            }
        //            else
        //                ctr++;
        //        }
        //    }
        //    //new resize to last point 0 to last point
        //    Array.Resize(ref _showerpattern, nozzlestartidx + ctr);
        //    _showerpattern = smoothProfile(_showerpattern);
        //    ShowerPattern = _showerpattern;
        //    GetTotalShowerWidth();
        //}

        private void MergeNozzlePatterns2()
        {
            //10000 = 10m is practical limit for a shower
            int _profilewidth = Constants._maxshowerwidth;

            double[] _showerpattern = new double[_profilewidth];
            int ctr = 0;
            int nozzlestartidx = 0;
            //build nozzle spray pattern and add to overall spray volume

            double _nozzlecentre = 0;

            for (int i = 0; i < Nozzles.Count; i++)
            {
                if (i == 0)
                {
                    if (Nozzles[i].NozzleTypeName.Equals(Constants.StandardNozzle))
                        _nozzlecentre = Nozzles[i].NozzleSprayWidth / 2;
                    else
                        if (Nozzles[i].NozzleTypeName.Equals(Constants.AsymmetricLeftNozzle) || Nozzles[i].NozzleTypeName.Equals(Constants.AsymmetricRightNozzle))
                            _nozzlecentre = Nozzles[i].NozzleSprayWidth;
                        else
                        {
                        //custom nozzle
                        _nozzlecentre = Nozzles[i].NozzleSprayWidth / 2;
                    }
                }
                else
                    _nozzlecentre = _nozzlecentre + Nozzles[i].NozzleSpacing;

                if (Nozzles[i].NozzleTypeName.Equals(Constants.StandardNozzle))
                    nozzlestartidx = Convert.ToInt32(_nozzlecentre - Nozzles[i].NozzleSprayWidth / 2);
                else
                    if (Nozzles[i].NozzleTypeName.Equals(Constants.AsymmetricLeftNozzle) || Nozzles[i].NozzleTypeName.Equals(Constants.AsymmetricRightNozzle))
                        nozzlestartidx = Convert.ToInt32(_nozzlecentre - Nozzles[i].NozzleSprayWidth);
                    else
                    {
                    //custom nozzle
                    nozzlestartidx = Convert.ToInt32(_nozzlecentre - Nozzles[i].NozzleSprayWidth / 2);
                }

                ////combine nozzle spray patterns
                ctr = 0;
                for (int j = 0; j < Nozzles[i].SprayPattern.Length; j++)
                {
                    if ((nozzlestartidx + ctr) >= 0)
                    {
                        if ((nozzlestartidx + ctr) < _profilewidth && (nozzlestartidx + ctr) >= 0)
                        {
                            _showerpattern[nozzlestartidx + ctr] = _showerpattern[nozzlestartidx + ctr] + Nozzles[i].SprayPattern[j];
                            ctr++;
                        }
                    }
                    else
                        ctr++;
                }
            }
            //new resize to last point 0 to last point
            Array.Resize(ref _showerpattern, nozzlestartidx + ctr);

            if(SmoothingDegree > 0)
                ShowerPattern = SmoothProfile(_showerpattern);
            else
                ShowerPattern = _showerpattern;
            GetTotalShowerWidth();
            
        }
               
        private double[] SmoothProfile(double[] profile)
        {
            int _len = profile.Length;
            int windowsize = SmoothingDegree * 2;
            double locsum = 0;
            double[] avpattern = new double[windowsize];
            int jctr = windowsize/2;
            try
            {
                for (int j = 0; j < windowsize; j++)
                {
                    avpattern[j] = profile[j];
                    locsum = locsum + profile[j];
                    profile[j] = locsum / (j + 1);
                }

                double sum = 0;
                for (int k = jctr; k < _len - jctr; k++)
                {
                    avpattern[(k + jctr) % windowsize] = profile[k + jctr];
                    sum = 0;
                    for (int l = 0; l < windowsize; l++)
                        sum = sum + avpattern[l];

                    profile[k] = sum / windowsize;
                }

                int endctr = 0;
                for (int m = profile.Length - jctr; m < profile.Length; m++)
                {
                    avpattern[(m + jctr) % windowsize] = -1;
                    sum = 0;
                    endctr = 0;
                    for (int n = 0; n < windowsize; n++)
                        if (avpattern[n] > -1)
                        {
                            sum = sum + avpattern[n];
                            endctr++;
                        }
                    profile[m] = sum / endctr;
                }
            }
            catch
            {

            }
            return profile;
        }

        #endregion

        #region Commands

        private bool CanExecuteOpen(object obj)
        {
            return _canexecuteopen;
        }

        private void ExecuteOpenNozzles(object parameter)
        {
            IMessageBoxService msg = new MessageBoxService();
            msg.OpenNozzlesDlg2(this);
            msg = null;
        }

        private void ExecuteCopyToClipboard(object parameter)                                        
        {
            if (parameter != null)
            {
                FrameworkElement _img = parameter as FrameworkElement;

                double width = _img.ActualWidth;
                double height = _img.ActualHeight;

                //old
                //RenderTargetBitmap bmpCopied = new RenderTargetBitmap((int)Math.Round(width), (int)Math.Round(height), 96, 96, PixelFormats.Pbgra32);

                Rect bounds = VisualTreeHelper.GetDescendantBounds(_img);
                RenderTargetBitmap bmpCopied = new RenderTargetBitmap((int)(bounds.Width), (int)(bounds.Height), 96, 96, PixelFormats.Pbgra32);

                DrawingVisual dv = new DrawingVisual();
                using (DrawingContext dc = dv.RenderOpen())
                {
                    VisualBrush vb = new VisualBrush(_img);
                    dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
                }
                bmpCopied.Render(dv);
                Clipboard.SetImage(bmpCopied);

           
            }
        }
             
        private bool CanExecuteResize(object obj)
        {
            if (Nozzles.Count < 2)
                return false;

            return _canexecuteresize;
        }
        
        private bool CanExecuteResizeGroup(object obj)
        {
            if (Nozzles.Count < 1)
                return false;
            return _canexecuteresizegroup;
        }

        private void ExecuteIncrSpacing(object parameter)
        {
            foreach (NozzleControl n in Nozzles)
            {
                if(n.NozzleSpacing < Constants._maxnozzlespacing && n.NozzleID !=1)
                    n.NozzleSpacing++;
            }
        }
       
        private void ExecuteDecrSpacing(object parameter)
        {
            foreach (NozzleControl n in Nozzles)
            {
                if(n.NozzleSpacing > 1 && n.NozzleID!=1)
                    n.NozzleSpacing--;
            }
        }

        private void ExecuteIncrNozzleToYankee(object parameter)
        {
            foreach (NozzleControl n in Nozzles)
            {
                if (n.NozzleToYankee < Constants._maxnozzletoyankee)
                {
                    n.NozzleToYankee++;
                    n.BoomTop++;
                }
            }
        }

        private void ExecuteDecrNozzleToYankee(object parameter)
        {
            foreach (NozzleControl n in Nozzles)
            {
                if (n.NozzleToYankee > 1)
                {
                    n.NozzleToYankee--;
                    n.BoomTop--;
                }
            }
        }

        private void ExecuteIncrNozzleRotation(object parameter)
        {
            foreach (NozzleControl n in Nozzles)
            {
                if(n.SprayRotation < Constants._maxsprayrotation)
                    n.SprayRotation++;
            }
        }

        private void ExecuteDecrNozzleRotation(object parameter)
        {
            foreach (NozzleControl n in Nozzles)
            {
                if (n.SprayRotation > 0)
                    n.SprayRotation--;
            }
        }

        private void ExecuteIncrNozzleAngle(object parameter)
        {
            foreach (NozzleControl n in Nozzles)
            {
                if(n.NozzleTypeName ==Constants.AsymmetricLeftNozzle || n.NozzleTypeName == Constants.AsymmetricRightNozzle)
                {
                    if (n.SprayAngle < Constants._maxnozzleangle/2)
                        n.SprayAngle++;
                }
                else
                if (n.SprayAngle < Constants._maxnozzleangle)
                    n.SprayAngle++;
            }
        }

        private void ExecuteDecrNozzleAngle(object parameter)
        {
            foreach (NozzleControl n in Nozzles)
            {
                if (n.SprayAngle > 0)
                    n.SprayAngle--;
            }
        }

        private void ExecuteOpenDefaults(object parameter)
        {
            IMessageBoxService _msgboxcommand = new MessageBoxService();
            _msgboxcommand.OpenDefaultsDlg(null);
            _msgboxcommand = null;
        }

        private void ExecuteOpenAbout(object parameter)
        {
            IMessageBoxService _msgboxcommand = new MessageBoxService();
            _msgboxcommand.OpenAboutDlg(null);
            _msgboxcommand = null;
        }
 
        private void ExecuteOpenDesigner(object parameter)
        {
            IMessageBoxService _msgboxcommand = new MessageBoxService();
            _msgboxcommand.OpenDesignerDlg(null, NozzleTypes);
            _msgboxcommand = null;
        }

        private void ExecuteSaveShower(object parameter)
        {
            IMessageBoxService _msgboxcommand = new MessageBoxService();
            string _exefolder = ConfigFileManager.GetLocalExePath();// Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string _filename = _msgboxcommand.SaveFileDlg("Select Shower", "Logo Files(*." + Constants._showerfilenameext + ")| *." + Constants._showerfilenameext,null, _exefolder);
            _msgboxcommand = null;

            if (!string.IsNullOrEmpty(_filename))
            {
                SerializeShower(_filename);
            }
            MenuItems = MRUList.AddFile(MenuItems, _filename, OpenShowerFile);

        }
                
        private bool CanExecuteNew(object obj)
        {
            return _canexecutenew;
        }

        private void ExecuteNewShower(object parameter)
        {
            Nozzles.Clear();
            EvapEnergy =0;
            EvapLoadkgperhrpersqm = 0;
            TotalEvapLoad = 0;
        }

        private void ExecuteOpenShower(object parameter)
        {
            //select shower from dialog
            IMessageBoxService _msgboxcommand = new MessageBoxService();
            string _exefolder = ConfigFileManager.GetLocalExePath();//  Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string _filename =  _msgboxcommand.OpenFileDlg("Select Shower", true, false, "Shower Files(*." + Constants._showerfilenameext + ")| *." + Constants._showerfilenameext, null, _exefolder);
            _msgboxcommand = null;
            OpenSelectedShowerFile(_filename);           
        }

        private void ExecuteOpenShowerFile(object parameter)
        {
            OpenSelectedShowerFile((string)parameter);
        }

        private void OpenSelectedShowerFile(string _filename)
        {
            if (!string.IsNullOrEmpty(_filename) && File.Exists(_filename))
            {
                try
                {
                    Shower _shower = DeSerializeShower(_filename);
                    if (_shower != null)
                    {               
                        YankeeSpeed = _shower.YankeeSpeed;
                        YankeeWidth = _shower.YankeeWidth;
                        YankeeDiameter = _shower.YankeeDiameter;
                        DwellDistance = _shower.DwellDistance;
                        OperatingPressure = _shower.OperatingPressure;
                        ShowerTemperature = _shower.ShowerTemperature;
                        CoverageColour = _shower.CoverageColour; 
                        Nozzles.Clear();
                        int ctr = 0;

                        NozzleControl nc;

                        foreach (SerializedNozzle _sernozzle in _shower.Nozzles)
                        {
                            ctr++;
                            nc = new NozzleControl
                            {
                                NozzleID = ctr,
                                NozzleOrificeID = _sernozzle.NozzleOrificeID,
                                NozzleSpacing = _sernozzle.NozzleSpacing,
                                NozzleToYankee = _sernozzle.NozzleToYankee,
                                SprayRotation = _sernozzle.SprayRotation,
                                SprayAngle = _sernozzle.SprayAngle,
                                NozzlePressure = _sernozzle.NozzlePressure,
                                NozzleTypeName = _sernozzle.NozzleTypeName,
                                NozzleSprayWidth = _sernozzle.NozzleSprayWidth,
                                MaximumSprayAngle = _sernozzle.MaximumSprayAngle,
                                NozzleFlow = _sernozzle.NozzleFlow,
                                SprayPattern = _sernozzle.SprayPattern,
                                NozzleTypes = NozzleTypes
                            };
                            Nozzles.Add(nc);
                        }
                        NumberNozzles = ctr;
                        MenuItems = MRUList.AddFile(MenuItems, _filename, OpenShowerFile);
                    }
                    else
                    {
                        //MenuItems = MRUList.RemoveFile(MenuItems, _filename);
                        MessageBox.Show("The shower file appears to be corrupted and cannot be loaded.", "Unable to load shower file", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);                        
                    }
                }
                catch
                {
                    //MenuItems = MRUList.RemoveFile(MenuItems, _filename);
                    MessageBox.Show("The shower file appears to be corrupted and cannot be loaded.", "Unable to load shower file", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
            }
            else
                MenuItems = MRUList.RemoveFile(MenuItems, _filename);
        }

        private void ExecuteOpenHelp(object parameter)
        {
            try
            {               
                System.Diagnostics.Process.Start(ConfigFileManager.GetLocalExePath() +  @"\Yankee Coating Shower Designer Pro.docx");                
            }
            catch
                ( System.ComponentModel.Win32Exception noBrowser)
                {
                    if (noBrowser.ErrorCode == -2147467259)
                        MessageBox.Show(noBrowser.Message);
                }
                catch (Exception other)
                {
                    MessageBox.Show(other.Message);
                }
        }

        #endregion

        #region Serialize Shower

        private void SerializeShower(string _filename)
        {
            try
            {
                Shower _shower = new Shower
                {
                    ShowerName = _filename,
                    YankeeSpeed = YankeeSpeed,
                    YankeeWidth = YankeeWidth,
                    YankeeDiameter = YankeeDiameter,
                    DwellDistance = DwellDistance,
                    OperatingPressure = OperatingPressure,
                    ShowerTemperature = ShowerTemperature,
                    NumberNozzles = NumberNozzles,
                    CoverageColour = CoverageColour
                };

                SerializedNozzle _sernozzle;
                foreach (NozzleControl nc in Nozzles)
                {
                    _sernozzle = new SerializedNozzle
                    {
                        NozzleSprayWidth = nc.NozzleSprayWidth,
                        NozzleSpacing = nc.NozzleSpacing,
                        SprayAngle = nc.SprayAngle,
                        MaximumSprayAngle = nc.MaximumSprayAngle,
                        NozzleToYankee = nc.NozzleToYankee,
                        SprayRotation = nc.SprayRotation,
                        NozzleFlow = nc.NozzleFlow,
                        NozzlePressure = nc.NozzlePressure,
                        NozzleOrificeID = nc.NozzleOrificeID,
                        NozzleTypeName = nc.NozzleTypeName,
                        SprayPattern = nc.SprayPattern
                    };
                    _shower.Nozzles.Add(_sernozzle);
                }
                ShowerToSerialize showerToSerialize = new ShowerToSerialize
                {
                    Shower = _shower
                };
                Serializer serializer = new Serializer();
                serializer.SerializeObject<ShowerToSerialize>(_shower.ShowerName, showerToSerialize);
            }
            catch { }
        }


        private Shower DeSerializeShower(string _filename)
        {
            Shower _shower = new Shower();
            try
            {                
                _shower.ShowerName = _filename;

                ShowerToSerialize showerToDeSerialize = new ShowerToSerialize();
                Serializer serializer = new Serializer();
                showerToDeSerialize = serializer.DeSerializeObject<ShowerToSerialize>(_shower.ShowerName);

                _shower.YankeeSpeed = showerToDeSerialize.Shower.YankeeSpeed;
                _shower.YankeeWidth = showerToDeSerialize.Shower.YankeeWidth;
                _shower.YankeeDiameter = showerToDeSerialize.Shower.YankeeDiameter;
                _shower.DwellDistance = showerToDeSerialize.Shower.DwellDistance;
                _shower.OperatingPressure = showerToDeSerialize.Shower.OperatingPressure;
                _shower.ShowerTemperature = showerToDeSerialize.Shower.ShowerTemperature;
                _shower.NumberNozzles = showerToDeSerialize.Shower.NumberNozzles;
                _shower.Nozzles = showerToDeSerialize.Shower.Nozzles;
                _shower.CoverageColour = showerToDeSerialize.Shower.CoverageColour;

            }
            catch 
            {
                return null;
            }

            return _shower;         
        }

        private void CloseNozzleSettings()
        {
            Window _owner;
            _owner = Application.Current.Windows[0];
            try
            {
                foreach (Window w in _owner.OwnedWindows)
                {                
                    if (w.GetType().Equals(typeof(Views.NozzleSettings)))
                    {
                        w.Close();
                        break;
                    }
                }                
            }
            catch { }
        }
        #endregion
      
    }   
}
