using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace YankeeShower
{
    /// <summary>
    /// Interaction logic for Coverage.xaml
    /// </summary>
    public partial class Coverage : UserControl
    {      
        public Coverage()
        {          
            InitializeComponent();  
            this.Loaded += (o, e) =>
            {
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);
                CoverageAdorner myAdorner = new CoverageAdorner(this);
                layer.Add(myAdorner);
                
                FlowScaleAdorner ScaleAdorner = new FlowScaleAdorner(this);
                layer.Add(ScaleAdorner);
                
            };
        }

        #region Shower Pattern

        public static readonly DependencyProperty ShowerPatternProperty = DependencyProperty.Register("ShowerPattern", typeof(double[]), typeof(Coverage),
                     new FrameworkPropertyMetadata(new PropertyChangedCallback(AdjustShowerPattern)));

        public double[] ShowerPattern
        {
            get { return (double[])GetValue(ShowerPatternProperty); }
            set { SetValue(ShowerPatternProperty, value); }
        }

        private static void AdjustShowerPattern(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (((double[])(e.NewValue)).GetUpperBound(0)>-1) 
            (source as Coverage).cover.Width = ((double[])(e.NewValue)).GetUpperBound(0);
        }

        #endregion

        #region Yankee Speed

        public static readonly DependencyProperty YankeeSpeedProperty = DependencyProperty.Register("YankeeSpeed", typeof(int), typeof(Coverage),
                    new FrameworkPropertyMetadata(Constants._defaultYankeeSpeed));//, AdjustYankeeSpeed));

        public int YankeeSpeed
        {
            get { return (int)GetValue(YankeeSpeedProperty); }
            set { SetValue(YankeeSpeedProperty, value); }
        }

        //private static void AdjustYankeeSpeed(DependencyObject source, DependencyPropertyChangedEventArgs e)
        //{
        //    //not used
        //}

        #endregion

        #region Coverage Colour

        public static readonly DependencyProperty CoverageColourProperty = DependencyProperty.Register("CoverageColour", typeof(string), typeof(Coverage),
                   new FrameworkPropertyMetadata(Constants._patternPenColor));

        public string CoverageColour
        {
            get { return (string)GetValue(CoverageColourProperty); }
            set { SetValue(CoverageColourProperty, value); }
        }

        #endregion
              

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);          
        }
        
    }
   
    public class CoverageAdorner : Adorner
    {
        public static DependencyProperty ShowerPatternProperty = DependencyProperty.Register("ShowerPattern", typeof(double[]), typeof(CoverageAdorner), new PropertyMetadata(null, (o, e) => { ((CoverageAdorner)o).InvalidateVisual(); }));
        public static DependencyProperty CoverageColourProperty = DependencyProperty.Register("CoverageColour", typeof(string), typeof(CoverageAdorner), new PropertyMetadata(null, (o, e) => { ((CoverageAdorner)o).InvalidateVisual(); }));

        // Be sure to call the base class constructor.
        public CoverageAdorner(UIElement adornedElement) : base(adornedElement)
        {
            this.DataContext = this.AdornedElement;
            this.SetUpBindings();
            this.IsClipEnabled = true;
            this.ClipToBounds = true;
            this.IsHitTestVisible = false;            
        }

        private void SetUpBindings()
        {
            BindingOperations.SetBinding(this, CoverageAdorner.ShowerPatternProperty, new Binding() { Path = new PropertyPath("ShowerPattern"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            BindingOperations.SetBinding(this, CoverageAdorner.CoverageColourProperty, new Binding() { Path = new PropertyPath("CoverageColour"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
        }

        public double[] ShowerPattern
        {
            get { return (double[])this.GetValue(CoverageAdorner.ShowerPatternProperty); }
            set { this.SetValue(CoverageAdorner.ShowerPatternProperty, value); }
        }


        public string CoverageColour
        {
            get { return (string)GetValue(CoverageAdorner.CoverageColourProperty); }
            set { SetValue(CoverageAdorner.CoverageColourProperty, value); }
        }
               
        protected override void OnRender(DrawingContext drawingContext)
        {
            try
            {
                RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
                RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.LowQuality);
                if (ShowerPattern != null)
                {
                    Color color = (Color)ColorConverter.ConvertFromString(CoverageColour);

                    Pen patternPen = new Pen(new SolidColorBrush(color), Constants._patternPenThickness);
                    patternPen.Freeze();

                    for (int i = 0; i < ShowerPattern.Length; i++)
                    {
                        drawingContext.DrawLine(patternPen, new Point(i, Constants._showerpatternheight - ShowerPattern[i]), new Point(i, Constants._showerpatternheight));
                    }
                }
            }
            catch
            {

            }
        }
    }

    public class FlowScaleAdorner : Adorner
    {
        public static DependencyProperty YankeeSpeedProperty = DependencyProperty.Register("YankeeSpeed", typeof(int), typeof(FlowScaleAdorner), new PropertyMetadata(Constants._defaultYankeeSpeed, (o, e) => { ((FlowScaleAdorner)o).InvalidateVisual(); }));

        // Be sure to call the base class constructor.
        public FlowScaleAdorner(UIElement adornedElement) : base(adornedElement)
        {
            this.DataContext = this.AdornedElement;
            this.SetUpBindings();
            this.IsClipEnabled = true;
            this.ClipToBounds = true;
            this.IsHitTestVisible = false;
        }

        private void SetUpBindings()
        {
            BindingOperations.SetBinding(this, FlowScaleAdorner.YankeeSpeedProperty, new Binding() { Path = new PropertyPath("YankeeSpeed"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
        }

        public int YankeeSpeed
        {
            get { return (int)this.GetValue(FlowScaleAdorner.YankeeSpeedProperty); }
            set { this.SetValue(FlowScaleAdorner.YankeeSpeedProperty, value); }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            try
            {
                RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
                RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.LowQuality);

                //draw line across width of spray
                Pen horzPen = new Pen(new SolidColorBrush(Colors.DarkGray), 1);
                horzPen.Freeze();

                double _spraydensity = 0;
                double _ycoord = 0;

                for (int i = 0; i < 5; i++)
                {
                    _ycoord = i * Constants._showerpatternheight / Constants._maximumlitrespermpermin;
                    drawingContext.DrawLine(horzPen, new Point(0, Constants._showerpatternheight - _ycoord), new Point(this.AdornedElement.RenderSize.Width, Constants._showerpatternheight - _ycoord));

                    if (i > 0)
                    {
                        _spraydensity = 1000 * (Constants._maximumlitrespermpermin) / (Convert.ToDouble(YankeeSpeed)) * i / 5;
                        string _spraydensitytext = _spraydensity.ToString("F2") + " ml/m²";
                        FormattedText formattedSprayDensityText = new FormattedText(_spraydensitytext, System.Globalization.CultureInfo.GetCultureInfo("en-us"),
                            FlowDirection.LeftToRight, new Typeface("Segoe UI"), 12, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);

                        drawingContext.DrawText(formattedSprayDensityText, new Point(2, Constants._showerpatternheight - _ycoord - formattedSprayDensityText.Height));
                    }
                }
            }
            catch
            {

            }
        }

    }
}
