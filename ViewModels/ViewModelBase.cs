using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace YankeeShower
{
   
    public class ViewModelBase : INotifyPropertyChanged
    {
        ICommand _closewindowcommand;
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string caller = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(caller));
        }
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public ICommand CloseWindowCommand
        {
            get
            {
                if (_closewindowcommand == null)
                {
                    _closewindowcommand = new DelegateCommand(CanExecute, ExecuteCloseWindow);
                }
                return _closewindowcommand;
            }
        }

        public void ExecuteCloseWindow(object parameter)
        {
            Window wnd = parameter as Window;
            //if (wnd != null)
                wnd?.Close();
        }


        bool? closewindowflag;
        public bool? CloseWindowFlag
        {
            get { return closewindowflag; }
            set { SetField(ref closewindowflag, value); }
        }

        public virtual void CloseWindow(bool? result = true)
        {
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
            {
                CloseWindowFlag = CloseWindowFlag == null ? true : !CloseWindowFlag;
            }));
        }

        ICommand close;
        public ICommand Close
        {
            get
            {
                if (close == null)
                    close = new DelegateCommand(CanExecute, ExecuteClose);
                return close;
            }
        }

        private void ExecuteClose(object parameter)
        {
            CloseWindow();
        }

    }
    
    public class DelegateCommand : ICommand
    {
        #region Commands


        Predicate<object> canExecute;
        Action<object> execute;

        public DelegateCommand(Predicate<object> _canexecute, Action<object> _execute) : this()
        {
            canExecute = _canexecute;
            execute = _execute;
        }

        public DelegateCommand()
        {
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }


        public void Execute(object parameter)
        {
            execute(parameter);
        }

        #endregion

    }
}
