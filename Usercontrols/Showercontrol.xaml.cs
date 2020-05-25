using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace YankeeShower.Usercontrols
{
    /// <summary>
    /// Interaction logic for Showercontrol.xaml
    /// </summary>
    public partial class Showercontrol : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Showercontrol()
        {
            InitializeComponent();        
        }

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        public static readonly DependencyProperty NozzlesProperty = DependencyProperty.Register("Nozzles", typeof(FullyObservableCollection<NozzleControl>), typeof(Showercontrol),
                    new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, UpdateShower));
        
        public FullyObservableCollection<NozzleControl> Nozzles
        {
            get { return (FullyObservableCollection<NozzleControl>)GetValue(NozzlesProperty); }
            set { SetValue(NozzlesProperty, value); }
        }

        //set up event handlers
        private static void UpdateShower(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var control = (Showercontrol)source;

            if (e.OldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= control.ColumnsCollectionChanged;
            }

            if (e.NewValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += control.ColumnsCollectionChanged;
            }

            //INotifyCollectionChanged oldCollection = e.OldValue as INotifyCollectionChanged;
            //INotifyCollectionChanged newCollection = e.NewValue as INotifyCollectionChanged;

            //if (oldCollection != null)
            //{
            //    oldCollection.CollectionChanged -= control.ColumnsCollectionChanged;
            //}

            //if (newCollection != null)
            //{
            //    newCollection.CollectionChanged += control.ColumnsCollectionChanged;
            //}

        }

        private void ColumnsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // optionally take e.Action into account
            //         UpdateColumns();

            //if (e.Action == NotifyCollectionChangedAction.Add)
            //{
            //    foreach (NozzleControl item in e.NewItems)
            //    {
            //        // Subscribe for changes on item
            //        item.PropertyChanged += OnNozzleChanged;
            //        item.NozzleThroatHeight = item.BoomTop - item.NozzleToYankee - 50;
            //    }
            //}

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                // Clear and update entire collection
            }

            if (e.NewItems != null)
            {
                foreach (NozzleControl item in e.NewItems)
                {
                    // Subscribe for changes on item
                    item.PropertyChanged += OnNozzleChanged;
                   
                    // Add item to internal collection
                    
                }
                UpdateNozzles();
            }

            if (e.OldItems != null)
            {
                foreach (NozzleControl item in e.OldItems)
                {
                    // Unsubscribe for changes on item
                    item.PropertyChanged -= OnNozzleChanged;

                    // Remove item from internal collection
                    
                }
                UpdateNozzles();
            }

        }

        private void OnNozzleChanged(object sender, PropertyChangedEventArgs e)
        {
            // Modify existing item in internal collection
            UpdateNozzles();
        }

        #region OperatingPressure

        public static readonly DependencyProperty OperatingPressureProperty = DependencyProperty.Register("OperatingPressure", typeof(double), typeof(Showercontrol),
                   new FrameworkPropertyMetadata(Constants._defshowerpressure));//, new PropertyChangedCallback(AdjustOperatingPressure)));

        public double OperatingPressure
        {
            get { return (double)GetValue(OperatingPressureProperty); }
            set { SetValue(OperatingPressureProperty, value); }
        }

        //private static void AdjustOperatingPressure(DependencyObject source, DependencyPropertyChangedEventArgs e)
        //{
            
        //}

        #endregion

        #region ShowerWidth

        public static readonly DependencyProperty ShowerWidthProperty = DependencyProperty.Register("ShowerWidth", typeof(double), typeof(Showercontrol),
                  new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(AdjustShowerWidth)));

        public double ShowerWidth
        {
            get { return (double)GetValue(ShowerWidthProperty); }
            set { SetValue(ShowerWidthProperty, value); }
        }

        private static void AdjustShowerWidth(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            (source as Showercontrol).boom1.ShowerWidth = 1000 * (double)e.NewValue;
        }

        #endregion

        #region BoomTop

        public static readonly DependencyProperty BoomTopProperty = DependencyProperty.Register("BoomTop", typeof(double), typeof(Showercontrol),
                 new FrameworkPropertyMetadata(Constants._defaultBoomToYankee, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, AdjustBoomPosition));

        public double BoomTop
        {
            get { return (double)GetValue(BoomTopProperty); }
            set { SetValue(BoomTopProperty, value); }
        }

        private static void AdjustBoomPosition(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            (source as Showercontrol).BoomTop = (double)e.NewValue;           
            (source as Showercontrol).MaximumNozzleToYankee = (int)((double)e.NewValue - Constants._nozzleheight);
        }

        #endregion

        #region Maximum Nozzle To Yankee

        public static readonly DependencyProperty MaximumNozzleToYankeeProperty = DependencyProperty.Register("MaximumNozzleToYankee", typeof(int), typeof(Showercontrol),
                            new FrameworkPropertyMetadata(Constants._defaultNozzletoYankee, AdjustMaximumNozzleToYankee));

        public int MaximumNozzleToYankee
        {
            get { return (int)GetValue(MaximumNozzleToYankeeProperty); }
            set { SetValue(MaximumNozzleToYankeeProperty, value); }
        }

        private static void AdjustMaximumNozzleToYankee(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            (source as Showercontrol).MaximumNozzleToYankee = (int)e.NewValue;
            (source as Showercontrol).NotifyPropertyChanged("MaximumNozzleToYankee");
        }

        #endregion
        
        private void UpdateNozzles()
        {                        
            int _nozzleleft = 0;          
            foreach (NozzleControl n in Nozzles)
            {
                _nozzleleft = _nozzleleft + CalcNozzleLeft(n);
                Canvas.SetLeft(n, _nozzleleft);
                Canvas.SetTop(n, n.NozzleToYankee);                
            }
            if (Nozzles?.Count > 0)
                BoomTop = Nozzles[0].BoomTop;
            else
                BoomTop = Constants._defaultNozzletoYankee + Constants._nozzleheight;
                         
            t.ItemsSource = Nozzles;
            MoveBoom();
        }

//update this for custom nozzles
        private int CalcNozzleLeft(NozzleControl n)
        {           

            if (n.NozzleID == 1)
            {
                if (n.NozzleTypeName.Equals(Constants.StandardNozzle))
                    return Convert.ToInt32((n.NozzleSprayWidth - Constants._nozzlewidth) / 2);
                else
                if (n.NozzleTypeName.Equals(Constants.AsymmetricLeftNozzle) || n.NozzleTypeName.Equals(Constants.AsymmetricRightNozzle))
                    return Convert.ToInt32((n.NozzleSprayWidth - Constants._nozzlewidth / 2));
                else
                    //custom nozzle
                    return Convert.ToInt32((n.NozzleSprayWidth - Constants._nozzlewidth) / 2);       

            }
            return n.NozzleSpacing;           
        }

        private void Boom_MouseWheel(object sender, MouseWheelEventArgs e)
        {         
            double _mouseincrement =  e.Delta / 120;
            MoveNozzlesY(_mouseincrement);
            e.Handled = true;
        }

        private void MoveNozzlesY(double _mouseincrement)
        {
            if (BoomTop + _mouseincrement - Constants._nozzleheight <= Constants._maxnozzletoyankee)
            {
                BoomTop = BoomTop + _mouseincrement;
                foreach (NozzleControl n in Nozzles)
                {
                    if (n.NozzleToYankee + _mouseincrement > 0)
                    {
                        n.BoomTop = BoomTop;
                        n.NozzleToYankee = Convert.ToInt32(n.NozzleToYankee + _mouseincrement);
                        Canvas.SetTop(n, n.NozzleToYankee);
                    }
                }
            }
        }

        private void MoveBoom()
        {
            Canvas.SetTop(boom1, BoomTop);           
        }

    }
}
