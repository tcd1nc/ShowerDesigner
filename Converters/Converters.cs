using System;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace YankeeShower
{
    public class StringToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (File.Exists(value.ToString()))
                {                    
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(@"" + value.ToString());
                    image.EndInit();
                    return image;
                }
                else
                    return null;
            }
            catch
            {
                return new BitmapImage();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public static class DialogCloser
    {
        public static readonly DependencyProperty DialogResultProperty = DependencyProperty.RegisterAttached("DialogResult", typeof(bool?),
                typeof(DialogCloser), new PropertyMetadata(DialogResultChanged));

        private static void DialogResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window)
                window.DialogResult = e.NewValue as bool?;
        }
        public static void SetDialogResult(Window target, bool? value)
        {
            target.SetValue(DialogResultProperty, value);
        }

        public static readonly DependencyProperty NumericDialogResultProperty = DependencyProperty.RegisterAttached("NumericDialogResult", typeof(double),
                typeof(DialogCloser), new PropertyMetadata(NumericDialogResultChanged));

        private static void NumericDialogResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window)
                window.Tag = (double)e.NewValue;
        }
        public static void SetNumericDialogResult(Window target, double value)
        {
            target.SetValue(NumericDialogResultProperty, value);
        }


        public static readonly DependencyProperty DialogCloseProperty = DependencyProperty.RegisterAttached("DialogClose", typeof(bool?),
                typeof(DialogCloser), new PropertyMetadata(false, DialogCloseChanged));

        private static void DialogCloseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window && (e.NewValue as bool?) == true)
                window.Close();
        }
        public static void SetDialogClose(Window target, bool? value)
        {
            target.SetValue(DialogCloseProperty, value);
        }

        public static readonly DependencyProperty ObjectDialogResultProperty = DependencyProperty.RegisterAttached("ObjectDialogResult", typeof(object),
               typeof(DialogCloser), new PropertyMetadata(ObjectDialogResultChanged));

        private static void ObjectDialogResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window)
                window.Tag = e.NewValue;
        }
        public static void SetObjectDialogResult(Window target, double value)
        {
            target.SetValue(ObjectDialogResultProperty, value);
        }
    }

}



