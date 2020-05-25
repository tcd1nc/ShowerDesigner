using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace YankeeShower.Views
{
    /// <summary>
    /// Interaction logic for NozzlePattern.xaml
    /// </summary>
    public partial class NozzlePattern : Window
    {
      
        public NozzlePattern()
        {
            InitializeComponent();

        }
        public NozzlePattern(FullyObservableCollection<NozzleType> _nozzletypes)
        {
            InitializeComponent();

            DataContext = new ViewModels.NozzlePatternVM(_nozzletypes);
        }
            
    }

    public class SprayDesigner 
    {
        public ObservableCollection<Square> Squares { get;  set; }
        private int[] _spray;
        private int _rows;
        private int _columns;

        public SprayDesigner(int _pRows, int _pCols, int _defaultcoverage)
        {
            _rows = _pRows;
            _columns = _pCols;
            Squares = new ObservableCollection<Square>();
            _spray = new int[PatternColumns];

            Square sq;
            for (int i = 0; i < _rows; i++) //row
            {
                for (int j = 0; j < _columns; j++) //column
                {
                    sq = new Square();
                    sq.Row = i;
                    sq.Column = j;
                    sq.MouseDown += sq_MouseDown;
                    sq.MouseMove += sq_MouseMove;
                    sq.IsFilled = false;
                    Squares.Add(sq);                   
                }
            }


            for (int k = 0; k < _columns; k++) //column
            {
                refillColumn(k, _pRows - _defaultcoverage);                
            }
        }


        public int PatternColumns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        public int PatternRows
        {
            get { return _rows; }
            set { _rows = value; }
        }

        public int[] Spray
        {
            get {
                getSpray();
                return _spray;
            }
            set { _spray = setSpray(value); }
        }

        public void sq_MouseMove(object sender, MouseEventArgs e)
        {            
            if (e.LeftButton == MouseButtonState.Pressed)
            {
               if ((sender as Square).IsFilled == true)
                    (sender as Square).IsFilled = false;
               else
                    (sender as Square).IsFilled = true;
               refillColumn((sender as Square).Column, (sender as Square).Row);

           //    e.Handled = true;
            }
        }

        public void sq_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {         
                if ((sender as Square).IsFilled == true)
                    (sender as Square).IsFilled = false;
                else
                    (sender as Square).IsFilled = true;

                refillColumn((sender as Square).Column, (sender as Square).Row);

          //      e.Handled = true;

            }
        }
        
        private void refillColumn(int columnID, int rowID)
        {
            //foreach (Square sq in Squares)
            //{
            //    if (sq.Column == columnID)
            //    {
            //        if (sq.Row >= rowID)
            //            //all rows above are transparent                        
            //            sq.IsFilled = true;
            //        else
            //            sq.IsFilled = false;
            //    }
            //}
            
            var s = from sp in Squares
                    where sp.Column == columnID
                    select sp;
            
            foreach(Square v in s)
            {
                if (v.Row >= rowID)
                    //all rows above are transparent                        
                    v.IsFilled = true;
                else
                    v.IsFilled = false;
            }
            
        }

        private void getSpray()
        {
            for (int k = 0; k < PatternColumns; k++)
                _spray[k] = PatternRows;

            foreach (Square sq in Squares)
            {
                if (sq.IsFilled)
                {
                    _spray[sq.Column] = Math.Min(_spray[sq.Column], sq.Row);
                 }
            }

        }


        private int[]  setSpray( int[] _newspray)
        {
            for (int k = 0; k < _columns; k++) //column
                {
                    refillColumn(k, PatternRows);
                    refillColumn(k, _newspray[k]);                    
                }
            return _newspray;
        }



    }


    public class Square : Canvas, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private int _row;
        private int _column;
        private bool _isfilled = false;

        public int Row
        {
            get { return _row; }
            set { _row = value; }
        }
        public int Column
        {
            get { return _column; }
            set { _column= value; }
        }

        public Square()
        {

 
        }

        public bool IsFilled
        {
            get { return _isfilled; }
            set
            {
                _isfilled = value;
                
                if(_isfilled==true)
                    this.Background = Brushes.MediumBlue;
                else
                    this.Background = Brushes.Transparent;
                NotifyPropertyChanged("IsFilled");
            }
        }


    }


   
}
