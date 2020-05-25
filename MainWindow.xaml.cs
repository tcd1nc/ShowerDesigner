using System.Windows;
using System.Windows.Media;

namespace YankeeShower
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {      
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModels.ShowerVM();
        }
  
       private void MenuItem_Click_2(object sender, RoutedEventArgs e)
       {
           this.Close();
       }

        private void PrintPreview_Click(object sender, RoutedEventArgs e)
       {
            Visual containerPattern = (adDec1) as Visual;
           
            PrintPreview pp = new PrintPreview(containerPattern)
            {
                Title = "Print Preview",
                DataContext = this.DataContext
            };
            pp.ShowDialog();
       }

    }
   
}


