﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace YankeeShower
{
    /// <summary>
    /// Interaction logic for TextInputDialog.xaml
    /// </summary>
    public partial class TextInputDialog : Window
    {
        public TextInputDialog()
        {
            InitializeComponent();
        }

        public TextInputDialog(string _title, string question, string defaultAnswer = "")
                {
                        InitializeComponent();
                        txtdialog.Title = _title;
                        lblQuestion.Text = question;
                        txtAnswer.Text = defaultAnswer;
                }

                private void btnDialogOk_Click(object sender, RoutedEventArgs e)
                {
                        this.DialogResult = true;
                }

                private void Window_ContentRendered(object sender, EventArgs e)
                {
                        txtAnswer.SelectAll();
                        txtAnswer.Focus();
                }

                public string Answer
                {
                        get { return txtAnswer.Text; }
                }





    }
}
