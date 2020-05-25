using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.Specialized;

namespace YankeeShower
{
    /// <summary>
    /// Interaction logic for NozzleControl.xaml
    /// </summary>        
    
    public partial class NozzleControl : UserControl , INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        PointCollection imagePoints;
        FullyObservableCollection<NozzleType> _nozzletypes = new FullyObservableCollection<NozzleType>();

        public NozzleControl()
        {
            InitializeComponent();

            //Important
            this.DataContext = this;
            
            this.Loaded += (o, e) =>
            {
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);
                MyAdorner myAdorner = new MyAdorner(this);
                layer.Add(myAdorner);
                NozzleTypes.CollectionChanged += NozzleTypes_CollectionChanged;   
            };
        }

        #region Event Handlers

        private void NozzleTypes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action.Equals(System.Collections.Specialized.NotifyCollectionChangedAction.Remove))
            {
                UpdateNozzleTypes();
            }
        }

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion

        #region Properties

        public FullyObservableCollection<Models.NozzleDataModel> Nozzledata
        {
            get { return StaticData.Nozzledata; }
            set { }
        }
        
        public FullyObservableCollection<NozzleType> NozzleTypes
        {
            get { return _nozzletypes; }
            set { _nozzletypes = value;
                NotifyPropertyChanged("NozzleTypes");
            }
        }

        public PointCollection ImagePoints
        {
            get { return this.imagePoints; }
            set {
                if (this.imagePoints != value)
                {
                    this.imagePoints = value;
                    if (this.PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("ImagePoints"));
                    }
                }
            }
        }
               
        #region Spray Pattern
        public static readonly DependencyProperty SprayPatternProperty = DependencyProperty.Register("SprayPattern", typeof(double[]), typeof(NozzleControl));//,
                     //new FrameworkPropertyMetadata(new PropertyChangedCallback(AdjustSprayPattern)));
        
        public double[] SprayPattern
        {
            get { return (double[])GetValue(SprayPatternProperty); }
            set { SetValue(SprayPatternProperty, value); }
        }

        //private static void AdjustSprayPattern(DependencyObject source, DependencyPropertyChangedEventArgs e)
        //{

        //}

        #endregion

        #region NozzleID
        public static readonly DependencyProperty NozzleIDProperty = DependencyProperty.Register("NozzleID", typeof(int), typeof(NozzleControl));//,
                    // new FrameworkPropertyMetadata(new PropertyChangedCallback(AdjustControlInt)));
        public int NozzleID
        {
            get { return (int)GetValue(NozzleIDProperty); }
            set { SetValue(NozzleIDProperty, value); }
        }

        //private static void AdjustControlInt(DependencyObject source, DependencyPropertyChangedEventArgs e)
        //{
        //}


        #endregion

        #region Spray Width
        public static readonly DependencyProperty NozzleSprayWidthProperty = DependencyProperty.Register("NozzleSprayWidth", typeof(int), typeof(NozzleControl),
                     new FrameworkPropertyMetadata(500));//,AdjustWidth));
        public int NozzleSprayWidth
        {
            get { return (int)GetValue(NozzleSprayWidthProperty); }
            set { SetValue(NozzleSprayWidthProperty, value); }
        }

        //private static void AdjustWidth(DependencyObject source, DependencyPropertyChangedEventArgs e)
        //{
                 
        //}


        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged = delegate { };
        private static void OnStaticPropertyChanged(string staticPropertyName)
        {
            StaticPropertyChanged(null, new PropertyChangedEventArgs(staticPropertyName));
        }
        

        #endregion

        #region Nozzle Spacing - limit range
        public static readonly DependencyProperty NozzleSpacingProperty = DependencyProperty.Register("NozzleSpacing", typeof(int), typeof(NozzleControl),
                    new FrameworkPropertyMetadata(Constants._defaultNozzleSpacing, AdjustControl));
        public int NozzleSpacing
        {
            get { return (int)GetValue(NozzleSpacingProperty); }
            set { SetValue(NozzleSpacingProperty, value); }
        }

        private static void AdjustControl(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if ((int)e.NewValue <= Constants._maxnozzlespacing && (int)e.NewValue > 0)
                (source as NozzleControl).UpdateSpray();
            else
            (source as NozzleControl).NozzleSpacing = (int)e.OldValue;
            (source as NozzleControl).NotifyPropertyChanged("NozzleSpacing");            
        }

        #endregion

        #region Spray Angle - limit range
        public static readonly DependencyProperty SprayAngleProperty = DependencyProperty.Register("SprayAngle", typeof(double), typeof(NozzleControl),
                             new FrameworkPropertyMetadata(Constants._defaultNozzleAngle, AdjustControlAngle));
        public double SprayAngle
        {
            get { return (double)GetValue(SprayAngleProperty); }
            set { SetValue(SprayAngleProperty, value); }
        }

        private static void AdjustControlAngle(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {            
            if ((double)e.NewValue <= Constants._maxnozzleangle && (double)e.NewValue > 0)
                (source as NozzleControl).UpdateSpray();
            else
                (source as NozzleControl).SprayAngle = (double)e.OldValue;
            (source as NozzleControl).NotifyPropertyChanged("SprayAngle");
            
        }
        #endregion

        #region Maximum Spray Angle
        public static readonly DependencyProperty MaximumSprayAngleProperty = DependencyProperty.Register("MaximumSprayAngle", typeof(int), typeof(NozzleControl),
                             new FrameworkPropertyMetadata(Constants._maxnozzleangle, AdjustMaximumAngle));
        public int MaximumSprayAngle
        {
            get { return (int)GetValue(MaximumSprayAngleProperty); }
            set { SetValue(MaximumSprayAngleProperty, value); }
        }

        private static void AdjustMaximumAngle(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            (source as NozzleControl).NotifyPropertyChanged("MaximumSprayAngle");
        }
        #endregion

        #region Nozzle To Yankee - limit range

        public static readonly DependencyProperty NozzleToYankeeProperty = DependencyProperty.Register("NozzleToYankee", typeof(int), typeof(NozzleControl),
                            new FrameworkPropertyMetadata(Constants._defaultNozzletoYankee, AdjustControlDistance));
        public int NozzleToYankee
        {
            get { return (int)GetValue(NozzleToYankeeProperty); }
            set { SetValue(NozzleToYankeeProperty, value); }
        }

        private static void AdjustControlDistance(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            
          int limit = (int)(source as NozzleControl).BoomTop - Constants._nozzleheight;

          if ((int)e.NewValue <= limit && (int)e.NewValue > 0)
                {
                (source as NozzleControl).UpdateSpray();
                (source as NozzleControl).NotifyPropertyChanged("NozzleToYankee");

                if ((source as NozzleControl).BoomTop - (int)e.NewValue - 50 > 20)
                    (source as NozzleControl).NozzleThroatHeight = (source as NozzleControl).BoomTop - (int)e.NewValue - 50;
                else
                    (source as NozzleControl).NozzleThroatHeight = 20;
                (source as NozzleControl).NotifyPropertyChanged("NozzleThroatHeight");
            }
            else
            {
                (source as NozzleControl).NozzleToYankee = (int)e.OldValue;
                (source as NozzleControl).NotifyPropertyChanged("NozzleToYankee");
            }            
        }

        double nozzlethroatheight = 20;
        public double NozzleThroatHeight
        {
            get { return nozzlethroatheight; }
            set { nozzlethroatheight = value; }
        }

        #endregion       

        #region Spray Rotation - limit range
        public static readonly DependencyProperty SprayRotationProperty = DependencyProperty.Register("SprayRotation", typeof(int), typeof(NozzleControl),
                             new FrameworkPropertyMetadata(Constants._defaultNozzlerotation, AdjustControRotation));
        public int SprayRotation
        {
            get { return (int)GetValue(SprayRotationProperty); }
            set { SetValue(SprayRotationProperty, value); }
        }

        private static void AdjustControRotation(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if ((int)e.NewValue <= Constants._maxsprayrotation && (int)e.NewValue >= 0)            
                (source as NozzleControl).UpdateSpray();                                          
            else            
                (source as NozzleControl).SprayRotation = (int)e.OldValue;                
              
            (source as NozzleControl).NotifyPropertyChanged("SprayRotation");

        }

        #endregion

        #region Nozzle Flow
        public static readonly DependencyProperty NozzleFlowProperty = DependencyProperty.Register("NozzleFlow", typeof(double), typeof(NozzleControl),
                             new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));//, AdjustNozzleFlow));
        public double NozzleFlow
        {
            get { return (double)GetValue(NozzleFlowProperty); }
            set { SetValue(NozzleFlowProperty, value); }
        }

        //private static void AdjustNozzleFlow(DependencyObject source, DependencyPropertyChangedEventArgs e)
        //{
        //}
        #endregion

        #region Nozzle Pressure

        public static readonly DependencyProperty NozzlePressureProperty = DependencyProperty.Register("NozzlePressure", typeof(double), typeof(NozzleControl),
                             new FrameworkPropertyMetadata(Constants._operatingpressure, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,  AdjustPressure));
        public double NozzlePressure
        {
            get { return (double)GetValue(NozzlePressureProperty); }
            set { SetValue(NozzlePressureProperty, value); }
        }

        private static void AdjustPressure(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {           
            (source as NozzleControl).UpdateFlow();
            (source as NozzleControl).UpdateSpray();            
            (source as NozzleControl).NotifyPropertyChanged("NozzlePressure");
        }

        #endregion

        #region NozzleOrificeID
        public static readonly DependencyProperty NozzleOrificeIDProperty = DependencyProperty.Register("NozzleOrificeID", typeof(int), typeof(NozzleControl),
                     new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, AdjustOrificeID));
        public int NozzleOrificeID
        {
            get { return (int)GetValue(NozzleOrificeIDProperty); }
            set { SetValue(NozzleOrificeIDProperty, value); }
        }

        private static void AdjustOrificeID(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            (source as NozzleControl).UpdateFlow();
            
            if ((int)e.NewValue == 0)
                (source as NozzleControl).ImagePoints = null;

            (source as NozzleControl).UpdateSpray();            
            (source as NozzleControl).NotifyPropertyChanged("NozzleOrificeID");
        }

        private void UpdateFlow()
        {
            NozzleFlow = FlowCalculator();
        }
       
        private double FlowCalculator()
        {            
            double _power = 0;
            double _coeff = 0;
            
            foreach (Models.NozzleDataModel dm in StaticData.Nozzledata)
            {
                if (NozzleOrificeID == dm.id)
                {
                    _power = dm.Power;
                    _coeff = dm.Coefficient;
                    break;
                }
            }
            return Math.Pow(NozzlePressure, _power) * _coeff;
             
        }

        #endregion

        #region BoomTop

        public static readonly DependencyProperty BoomTopProperty = DependencyProperty.Register("BoomTop", typeof(double), typeof(NozzleControl),
                     new FrameworkPropertyMetadata(Constants._defaultBoomToYankee, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, AdjustNozzleThroat));
        public double BoomTop
        {
            get { return (double)GetValue(BoomTopProperty); }
            set { SetValue(BoomTopProperty, value); }
        }

        private static void AdjustNozzleThroat(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            (source as NozzleControl).BoomTop = (double)e.NewValue;  
        }

        #endregion
                
        #region Nozzle Type
        public static readonly DependencyProperty NozzleTypeNameProperty = DependencyProperty.Register("NozzleTypeName", typeof(string), typeof(NozzleControl),
                     new FrameworkPropertyMetadata(Constants.StandardNozzle, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, AdjustNozzleType));
        public string NozzleTypeName
        {
            get { return (string)GetValue(NozzleTypeNameProperty); }
            set { SetValue(NozzleTypeNameProperty, value); }
        }

        private static void AdjustNozzleType(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty((string)e.NewValue))
            {
                if (((string)e.NewValue).Equals(Constants.AsymmetricLeftNozzle) || ((string)e.NewValue).Equals(Constants.AsymmetricRightNozzle))
                {
                    if ((source as NozzleControl).SprayAngle > 90)
                    {
                        (source as NozzleControl).SprayAngle = 55;
                    }
                    (source as NozzleControl).MaximumSprayAngle = Constants._maxnozzleangle / 2;
                }
                else
                    (source as NozzleControl).MaximumSprayAngle = Constants._maxnozzleangle;
            }
            else
            {
                (source as NozzleControl).NozzleTypeName = Constants.StandardNozzle;
                (source as NozzleControl).MaximumSprayAngle = Constants._maxnozzleangle;
            }
            (source as NozzleControl).UpdateFlow();          
            (source as NozzleControl).UpdateSpray();
            (source as NozzleControl).NotifyPropertyChanged("NozzleTypeName");            
        }

        #endregion

        #endregion

        #region Private functions

        private void UpdateNozzleTypes()
        {
            string nt = NozzleTypeName;
            NozzleTypeName = null;
            NozzleTypeName = nt;
        }

        private void UpdateSpray()
        {
            double coverage;
            double rndcoverage;

            if(NozzleTypeName.Equals(Constants.AsymmetricLeftNozzle))
            {               
                coverage = CalcAsymmetricSprayWidth();
                NozzleSprayWidth = (int)coverage;
                GenerateSprayPattern();
                if (NozzleOrificeID != 0)
                    this.ImagePoints = new PointCollection(new[] { new Point(Constants._nozzlewidth / 2 - coverage, -NozzleToYankee),
                        new Point(Constants._nozzlewidth / 2 , -NozzleToYankee), new Point(Constants._nozzlewidth / 2, -1) });             
            }
            else
            if (NozzleTypeName.Equals(Constants.AsymmetricRightNozzle))
            {
                coverage = CalcAsymmetricSprayWidth();
                NozzleSprayWidth = (int)coverage;
                GenerateSprayPattern();
                if (NozzleOrificeID != 0)
                    this.ImagePoints = new PointCollection(new[] { new Point(Constants._nozzlewidth / 2, -NozzleToYankee),
                        new Point(Constants._nozzlewidth / 2 + coverage, -NozzleToYankee), new Point(Constants._nozzlewidth / 2, -1) });                
            }
            else
            {
                //custom nozzle
                coverage =  CalcSprayWidth();
                NozzleSprayWidth = (int)coverage; //allocate width of standard nozzle since it is maximum width possible
                GenerateSprayPattern();
                rndcoverage = coverage / 2;
                if (NozzleOrificeID != 0)              
                    this.ImagePoints = new PointCollection(new[] { new Point(Constants._nozzlewidth / 2 - rndcoverage, -NozzleToYankee),
                        new Point(Constants._nozzlewidth / 2 + rndcoverage, -NozzleToYankee), new Point(Constants._nozzlewidth / 2, -1) });
            }
        }
          
        public double CalcSprayWidth()
        {    
            return  Math.Cos(Convert.ToDouble(SprayRotation * Math.PI / 180)) * 2 * Math.Tan(Convert.ToDouble(SprayAngle / 2 * Math.PI / 180)) * NozzleToYankee;
        }

        public double CalcAsymmetricSprayWidth()
        {            
            return 0.5 * Math.Cos(Convert.ToDouble(SprayRotation * Math.PI / 180)) * 2 * Math.Tan(Convert.ToDouble((SprayAngle *2) / 2 * Math.PI / 180)) * NozzleToYankee;
        }

        private void GenerateSprayPattern()
        {
            if ( NozzleSprayWidth > 0)
            {
                double _linearprofile = 0;
                _linearprofile = Constants._showerpatternheight / Constants._maximumlitrespermpermin * (NozzleFlow / Convert.ToDouble(NozzleSprayWidth)) * 1000;
                double[] _nozzleprofile = new double[NozzleSprayWidth];
                if (NozzleTypeName.Equals(Constants.StandardNozzle) || NozzleTypeName.Equals(Constants.AsymmetricLeftNozzle))
                {
                    for (int i = 0; i < NozzleSprayWidth; i++) 
                        _nozzleprofile[i] = _linearprofile;
                }
                else                
                    if (NozzleTypeName.Equals(Constants.AsymmetricRightNozzle))
                    {
                        _nozzleprofile = new double[NozzleSprayWidth * 2];
                        for (int i = NozzleSprayWidth; i < NozzleSprayWidth * 2; i++)
                            _nozzleprofile[i] = _linearprofile;
                    }
                else
                    if (NozzleTypeName.Equals(Constants.SystemNozzle1) 
                    || NozzleTypeName.Equals(Constants.SystemNozzle2) 
                    || NozzleTypeName.Equals(Constants.SystemNozzle3)
                    || NozzleTypeName.Equals(Constants.SystemNozzle4)
                    || NozzleTypeName.Equals(Constants.SystemNozzle5)
                    )
                    {
                        _nozzleprofile = StaticData.MakeSigmoidPattern(this, NozzleTypeName);
                    }
                    else
                        {
                            //custom nozzle
                            _nozzleprofile =  StaticData.ConvertDesignToActualSpray(NozzleTypeName, this);                                                                                
                        }               

                SprayPattern = _nozzleprofile;
            }
        }

        private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //spray.Stroke = new SolidColorBrush(Colors.OrangeRed);

            spray.Stroke = new SolidColorBrush(Colors.LimeGreen);
            spray.StrokeThickness = 2;
        }

        private void UserControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            spray.Stroke = new SolidColorBrush(Colors.Black);
            spray.StrokeThickness = 1;
        }
        
        #endregion

    }

    public class MyAdorner : Adorner
    {
        public static DependencyProperty NozzleIDProperty = DependencyProperty.Register("NozzleID", typeof(int), typeof(MyAdorner), new PropertyMetadata(Convert.ToInt32(1), (o, e) => { ((MyAdorner)o).InvalidateVisual(); }));
        public static DependencyProperty NozzleSpacingProperty = DependencyProperty.Register("NozzleSpacing", typeof(int), typeof(MyAdorner), new PropertyMetadata(Constants._defaultNozzleSpacing, (o, e) => { ((MyAdorner)o).InvalidateVisual(); }));        
        public static DependencyProperty NozzleToYankeeProperty = DependencyProperty.Register("NozzleToYankee", typeof(int), typeof(MyAdorner), new PropertyMetadata(Constants._defaultNozzletoYankee, (o, e) => {((MyAdorner)o).InvalidateVisual();}));        
        public static DependencyProperty SprayAngleProperty = DependencyProperty.Register("SprayAngle", typeof(double), typeof(MyAdorner), new PropertyMetadata(Constants._defaultNozzleAngle, (o, e) => { ((MyAdorner)o).InvalidateVisual(); }));
        public static DependencyProperty SprayRotationProperty = DependencyProperty.Register("SprayRotation", typeof(int), typeof(MyAdorner), new PropertyMetadata(Constants._defaultNozzlerotation, (o, e) => { ((MyAdorner)o).InvalidateVisual(); }));       
        public static DependencyProperty NozzleOrificeIDProperty = DependencyProperty.Register("NozzleOrificeID", typeof(int), typeof(MyAdorner), new PropertyMetadata(Convert.ToInt32(0), (o, e) => { ((MyAdorner)o).InvalidateVisual(); }));
        public static DependencyProperty NozzleTypeNameProperty = DependencyProperty.Register("NozzleTypeName", typeof(string), typeof(MyAdorner), new PropertyMetadata(Constants.StandardNozzle, (o, e) => { ((MyAdorner)o).InvalidateVisual(); }));
        public static DependencyProperty NozzleSprayWidthProperty = DependencyProperty.Register("NozzleSprayWidth", typeof(int), typeof(MyAdorner), new PropertyMetadata(0, (o, e) => { ((MyAdorner)o).InvalidateVisual(); }));

        // Be sure to call the base class constructor.
        public MyAdorner(UIElement adornedElement) : base(adornedElement)
        {
            this.DataContext = this.AdornedElement;
            this.SetUpBindings();
            this.IsClipEnabled = true;
            this.ClipToBounds = true;
            this.IsHitTestVisible = false;
        }

        private void SetUpBindings()
        {
            BindingOperations.SetBinding(this, MyAdorner.NozzleIDProperty, new Binding() { Path = new PropertyPath("NozzleID"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            BindingOperations.SetBinding(this, MyAdorner.NozzleSpacingProperty, new Binding() { Path = new PropertyPath("NozzleSpacing"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });           
            BindingOperations.SetBinding(this, MyAdorner.NozzleToYankeeProperty, new Binding() { Path = new PropertyPath("NozzleToYankee"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });          
            BindingOperations.SetBinding(this, MyAdorner.SprayAngleProperty, new Binding() { Path = new PropertyPath("SprayAngle"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            BindingOperations.SetBinding(this, MyAdorner.SprayRotationProperty, new Binding() { Path = new PropertyPath("SprayRotation"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });          
            BindingOperations.SetBinding(this, MyAdorner.NozzleOrificeIDProperty, new Binding() { Path = new PropertyPath("NozzleOrificeID"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            BindingOperations.SetBinding(this, MyAdorner.NozzleTypeNameProperty, new Binding() { Path = new PropertyPath("NozzleTypeName"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            BindingOperations.SetBinding(this, MyAdorner.NozzleSprayWidthProperty, new Binding() { Path = new PropertyPath("NozzleSprayWidth"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

        }

        public int NozzleID
        {
            get { return (int)this.GetValue(MyAdorner.NozzleIDProperty); }
            set { this.SetValue(MyAdorner.NozzleIDProperty, value); }
        }

        public int NozzleSpacing
        {
            get { return (int)this.GetValue(MyAdorner.NozzleSpacingProperty); }
            set { this.SetValue(MyAdorner.NozzleSpacingProperty, value); }
        }

        public int NozzleToYankee
        {
            get { return (int)this.GetValue(MyAdorner.NozzleToYankeeProperty); }
            set { this.SetValue(MyAdorner.NozzleToYankeeProperty, value); }
        }       

        public double SprayAngle
        {
            get { return (double)this.GetValue(MyAdorner.SprayAngleProperty); }
            set { this.SetValue(MyAdorner.SprayAngleProperty, value); }
        }

        public int SprayRotation
        {
            get { return (int)this.GetValue(MyAdorner.SprayRotationProperty); }
            set { this.SetValue(MyAdorner.SprayRotationProperty, value); }
        }
      
        public int NozzleOrificeID
        {
            get { return (int)this.GetValue(MyAdorner.NozzleOrificeIDProperty); }
            set { this.SetValue(MyAdorner.NozzleOrificeIDProperty, value); }
        }

        public string NozzleTypeName
        {
            get { return (string)this.GetValue(MyAdorner.NozzleTypeNameProperty); }
            set { this.SetValue(MyAdorner.NozzleTypeNameProperty, value); }
        }

        public int NozzleSprayWidth
        {
            get { return (int)this.GetValue(MyAdorner.NozzleSprayWidthProperty); }
            set { this.SetValue(MyAdorner.NozzleSprayWidthProperty, value); }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Point midPointSpacing = new Point();
            Point midPointDistance = new Point();
            Point midPointAngle = new Point();
            Point a = new Point();
            Point b = new Point();
            Point c = new Point();

            string _spacingtext;
            string _distancetext;
           // string _angletext;
           // const double _mmtoinches = 0.0393700787;
            double conversionfactor = 1;

            string _units = " mm";
                      
            Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);
            Pen redPen = new Pen(new SolidColorBrush(Colors.Red), 1);
            Pen greenPen = new Pen(new SolidColorBrush(Colors.Green), 0.5);
            //Pen bluePen = new Pen(new SolidColorBrush(Colors.Blue), 0.5);

            redPen.DashStyle = DashStyles.Dash;

            redPen.Freeze();
            greenPen.Freeze();
            
            // Draw a red square at nozzle tip.
            drawingContext.DrawRectangle(Brushes.Red, null, new System.Windows.Rect(23, -3, 4, 4));

            a.X = Convert.ToInt32(Constants._nozzlewidth / 2);
            a.Y = -1;
            b.X = Convert.ToInt32(NozzleSpacing + Constants._nozzlewidth / 2);
            b.Y = -1;
            c.X = Convert.ToInt32(Constants._nozzlewidth / 2);
            c.Y = -NozzleToYankee;
           
            //vertical line from nozzle tip to surface
            drawingContext.DrawLine(redPen, a, c);
            //add distance text
            _distancetext = Math.Round(NozzleToYankee * conversionfactor, 2).ToString() + _units;
            FormattedText formattedNozzleToYankeeText = new FormattedText(_distancetext, System.Globalization.CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight, new Typeface("Segoe UI"), 14, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);


            midPointDistance.X = Constants._nozzlewidth / 2 - formattedNozzleToYankeeText.Width / 2;
            midPointDistance.Y = Convert.ToDouble(-NozzleToYankee / 2 - formattedNozzleToYankeeText.Height / 2);
            drawingContext.DrawText(formattedNozzleToYankeeText, midPointDistance);

            //if not first nozzle
            //draw connecting line
            if (NozzleID != 1)
            {
                Point previousnozzle = new Point
                {
                    X = Convert.ToInt32(-NozzleSpacing + Constants._nozzlewidth / 2),
                    Y = -1
                };
                drawingContext.DrawLine(redPen, a, previousnozzle);
                _spacingtext = Math.Round(NozzleSpacing * conversionfactor, 2).ToString() + _units;
                FormattedText formattedText = new FormattedText(_spacingtext, System.Globalization.CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight, new Typeface("Segoe UI"), 14, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                                
                midPointSpacing.X = Convert.ToDouble(-NozzleSpacing) / 2 + Constants._nozzlewidth / 2 - formattedText.Width / 2;
                midPointSpacing.Y = -18;// -formattedText.Height / 2;
                drawingContext.DrawText(formattedText, midPointSpacing);
            }

            //if not blocked nozzle
            if (NozzleOrificeID != 0)
            {
                //draw arc for angle
                double rotFactor = 1;
                rotFactor = Math.Cos(SprayRotation * Math.PI / 180.0);
                double startDegrees = 0;
                double sweepDegrees = 0;

                if (NozzleTypeName.Equals(Constants.StandardNozzle))
                    startDegrees = 270 - (SprayAngle * rotFactor / 2); //need to add spray rotation                
                else
                    if (NozzleTypeName.Equals(Constants.AsymmetricLeftNozzle))
                        startDegrees = 270 - SprayAngle;//   240 - (SprayAngle * rotFactor / 2); //need to add spray rotation                    
                    else
                        if (NozzleTypeName.Equals(Constants.AsymmetricRightNozzle))
                            startDegrees = 270 ;// 300 - (SprayAngle * rotFactor / 2); //need to add spray rotation                        
                        else
                        {
                            //custom nozzle
                            startDegrees = 270 - (SprayAngle * rotFactor / 2); //need to add spray rotation 
                        }        

                sweepDegrees = SprayAngle * rotFactor;

                // degrees to radians conversion
                double startRadians = startDegrees * Math.PI / 180.0;
                double sweepRadians = sweepDegrees * Math.PI / 180.0;

                Rect rect = new Rect(0, -30, 50, 60);
                
                // x and y radius
                double dx = rect.Width / 2;
                double dy = rect.Height / 2;

                //// determine the start point 
                double xs = rect.X + dx + (Math.Cos(startRadians) * dx);
                double ys = rect.Y + dy + (Math.Sin(startRadians) * dy);

                // determine the end point 
                double xe = rect.X + dx + (Math.Cos(startRadians + sweepRadians) * dx);
                double ye = rect.Y + dy + (Math.Sin(startRadians + sweepRadians) * dy);


                // draw the arc into a stream geometry
                StreamGeometry streamGeom = new StreamGeometry();
                using (StreamGeometryContext ctx = streamGeom.Open())
                {
                    bool isLargeArc = Math.Abs(sweepDegrees) > 180;
                    SweepDirection sweepDirection = sweepDegrees < 0 ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;
                    ctx.BeginFigure(new Point(xs, ys), false, false);
                    ctx.ArcTo(new Point(xe, ye), new Size(dx, dy), 0, isLargeArc, sweepDirection, true, false);
                }               

                // create the drawing
                GeometryDrawing drawing = new GeometryDrawing();
                drawingContext.DrawGeometry(Brushes.Green, greenPen, streamGeom);

                //int effectiveangle;
                //effectiveangle = Convert.ToInt32(Math.Cos(SprayRotation * Math.PI / 180) * SprayAngle);
                //_angletext = effectiveangle.ToString() + (char)186;
               // FormattedText formattedAngleText = new FormattedText(_angletext, System.Globalization.CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 12, Brushes.Black);

                FormattedText formattedAngleText = new FormattedText(SprayAngle.ToString() + (char)186, System.Globalization.CultureInfo.GetCultureInfo("en-us"),
                  FlowDirection.LeftToRight, new Typeface("Segoe UI"), 12, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);

                midPointAngle.X = Constants._nozzlewidth / 2 - formattedAngleText.Width / 2;
                midPointAngle.Y = Convert.ToDouble(-formattedAngleText.Height - 10);
                drawingContext.DrawText(formattedAngleText, midPointAngle);
                
                //draw line across width of spray
                Point leftspray = new Point();
                Point rightspray = new Point();
                //horizontal line across spray
                Point lefttickmarktop = new Point();
                Point lefttickmarkbottom = new Point();
                Point righttickmarktop = new Point();
                Point righttickmarkbottom = new Point();

                string _widthtext;
                _widthtext =   NozzleSprayWidth.ToString() + _units;
                FormattedText formattedSprayWidthText = new FormattedText(_widthtext, System.Globalization.CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight, new Typeface("Segoe UI"), 14, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);

                midPointDistance.Y = Convert.ToDouble(-NozzleToYankee + formattedSprayWidthText.Height / 2);
                                
                if (NozzleTypeName.Equals(Constants.StandardNozzle))
                {
                    leftspray.X = Constants._nozzlewidth / 2 - NozzleSprayWidth / 2;
                    leftspray.Y = -NozzleToYankee + 10;
                    rightspray.X = Constants._nozzlewidth / 2 + NozzleSprayWidth / 2;
                    rightspray.Y = leftspray.Y;
                    midPointDistance.X = Constants._nozzlewidth / 2 - formattedSprayWidthText.Width / 2;

                    lefttickmarktop.X = Constants._nozzlewidth / 2 - NozzleSprayWidth / 2;
                    lefttickmarktop.Y = -NozzleToYankee;
                    lefttickmarkbottom.X = Constants._nozzlewidth / 2 - NozzleSprayWidth / 2;
                    lefttickmarkbottom.Y = -NozzleToYankee + 20;
                    drawingContext.DrawLine(redPen, lefttickmarktop, lefttickmarkbottom);
                    righttickmarktop.X = Constants._nozzlewidth / 2 + NozzleSprayWidth / 2;
                    righttickmarktop.Y = -NozzleToYankee;
                    righttickmarkbottom.X = Constants._nozzlewidth / 2 + NozzleSprayWidth / 2;
                    righttickmarkbottom.Y = -NozzleToYankee + 20;
                    drawingContext.DrawLine(redPen, righttickmarktop, righttickmarkbottom);

                }
                else
                    if (NozzleTypeName.Equals(Constants.AsymmetricLeftNozzle))
                {
                    leftspray.X = Constants._nozzlewidth / 2 - NozzleSprayWidth;
                    leftspray.Y = -NozzleToYankee + 10;
                    rightspray.X = Constants._nozzlewidth / 2;
                    rightspray.Y = leftspray.Y;
                    midPointDistance.X = -NozzleSprayWidth/2 + Constants._nozzlewidth / 2 - formattedSprayWidthText.Width / 2;

                    lefttickmarktop.X = Constants._nozzlewidth / 2 - NozzleSprayWidth ;
                    lefttickmarktop.Y = -NozzleToYankee;
                    lefttickmarkbottom.X = Constants._nozzlewidth / 2 - NozzleSprayWidth;
                    lefttickmarkbottom.Y = -NozzleToYankee + 20;
                    drawingContext.DrawLine(redPen, lefttickmarktop, lefttickmarkbottom);

                }
                else
                if (NozzleTypeName.Equals(Constants.AsymmetricRightNozzle))
                {
                    leftspray.X = Constants._nozzlewidth / 2;
                    leftspray.Y = -NozzleToYankee + 10;
                    rightspray.X = Constants._nozzlewidth / 2 + NozzleSprayWidth;
                    rightspray.Y = leftspray.Y;
                    midPointDistance.X = NozzleSprayWidth / 2 + Constants._nozzlewidth / 2 - formattedSprayWidthText.Width / 2;

                    righttickmarktop.X = Constants._nozzlewidth / 2 + NozzleSprayWidth;
                    righttickmarktop.Y = -NozzleToYankee;
                    righttickmarkbottom.X = Constants._nozzlewidth / 2 + NozzleSprayWidth;
                    righttickmarkbottom.Y = -NozzleToYankee + 20;
                    drawingContext.DrawLine(redPen, righttickmarktop, righttickmarkbottom);
                }
                else
                {
                    //custom nozzle
                    leftspray.X = Constants._nozzlewidth / 2 - NozzleSprayWidth / 2;
                    leftspray.Y = -NozzleToYankee + 10;
                    rightspray.X = Constants._nozzlewidth / 2 + NozzleSprayWidth / 2;
                    rightspray.Y = leftspray.Y;
                    midPointDistance.X = Constants._nozzlewidth / 2 - formattedSprayWidthText.Width / 2;

                    lefttickmarktop.X = Constants._nozzlewidth / 2 - NozzleSprayWidth / 2;
                    lefttickmarktop.Y = -NozzleToYankee;
                    lefttickmarkbottom.X = Constants._nozzlewidth / 2 - NozzleSprayWidth / 2;
                    lefttickmarkbottom.Y = -NozzleToYankee + 20;
                    drawingContext.DrawLine(redPen, lefttickmarktop, lefttickmarkbottom);
                    righttickmarktop.X = Constants._nozzlewidth / 2 + NozzleSprayWidth / 2;
                    righttickmarktop.Y = -NozzleToYankee;
                    righttickmarkbottom.X = Constants._nozzlewidth / 2 + NozzleSprayWidth / 2;
                    righttickmarkbottom.Y = -NozzleToYankee + 20;
                    drawingContext.DrawLine(redPen, righttickmarktop, righttickmarkbottom);
                }
                drawingContext.DrawText(formattedSprayWidthText, midPointDistance);
                drawingContext.DrawLine(redPen, leftspray, rightspray);
                //draw tick marks
                
            }
            //if blocked nozzle
            else
            {
                //blocked nozzle
                Pen thickredPen = new Pen(new SolidColorBrush(Colors.Red), 1.5);
                thickredPen.Freeze();
                a.X = Convert.ToInt32(Constants._nozzlewidth);
                a.Y = -1;
                b.X = 0;
                b.Y = Convert.ToInt32(70);
                drawingContext.DrawLine(thickredPen, a, b);
                a.X = 0;
                a.Y = 0;
                b.X = Convert.ToInt32(Constants._nozzlewidth);
                b.Y = Convert.ToInt32(70);
                drawingContext.DrawLine(thickredPen, a, b);
            }
        }       
    }
                     
}
