using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace YankeeShower
{
    
    public static class MRUList
    {
        public static ObservableCollection<CustomMenuItem> AddFile(ObservableCollection<CustomMenuItem> _menuitems, string _filename, ICommand _command)
        {
            if (!string.IsNullOrEmpty(_filename))
            {
                ObservableCollection<CustomMenuItem> _newmenuitems = new ObservableCollection<CustomMenuItem>();

                CustomMenuItem _menuitem;
                _menuitem = new CustomMenuItem
                {
                    HeaderText = _filename,
                    Command = _command,
                    CommandParameter = _filename,
                    NumberText = "1"
                };
                _newmenuitems.Add(_menuitem);

                int ctr = 2;
                foreach (CustomMenuItem mni in _menuitems)
                {
                    if (mni.HeaderText != _filename && !string.IsNullOrEmpty(mni.HeaderText))
                    {
                        mni.NumberText = ctr.ToString();
                        _newmenuitems.Add(mni);
                        ctr++;
                        if (ctr > Constants._maxmrulist)
                            break;
                    }
                }
                SaveMRU(_newmenuitems);
                return _newmenuitems;
            }
            else
                return _menuitems;

        }
                       
        public static ObservableCollection<CustomMenuItem> InitialiseMRU(ICommand _command)
        {
            ObservableCollection<CustomMenuItem> _menuitems = new ObservableCollection<CustomMenuItem>();
            int ctr = 1;
            CustomMenuItem _menuitem;
            List<string> _mrulist = (List<string>) Application.Current.Resources[Constants.MRUList];           
            if (_mrulist.Count > 0)
            {
                foreach (string rm in _mrulist)
                {
                    _menuitem = new CustomMenuItem
                    {
                        HeaderText = rm,
                        Command = _command,
                        CommandParameter = rm,
                        NumberText = ctr.ToString()
                    };
                    _menuitems.Add(_menuitem);
                    ctr++;
                }
            }
            return _menuitems;
        }

        public static void SaveMRU(ObservableCollection<CustomMenuItem> _menuitems)
        {           
            List<string> _mru = new List<string>();
            foreach (CustomMenuItem mni in _menuitems)
            {
                if(!string.IsNullOrEmpty(mni.HeaderText))
                    _mru.Add( mni.HeaderText );
            }                      
            ConfigFileManager.WriteJsonList("MRUList", _mru);
        }

        public static ObservableCollection<CustomMenuItem> RemoveFile(ObservableCollection<CustomMenuItem> _menuitems, string _filename)
        {
            if (!string.IsNullOrEmpty(_filename))
            {
                foreach (CustomMenuItem mni in _menuitems)
                {
                    if (mni.HeaderText == _filename)
                    {
                        _menuitems.Remove(mni);
                        break;
                    }
                }
                SaveMRU(_menuitems);
            }
            return _menuitems;
        }

    }

    public class CustomMenuItem
    {
        public string HeaderText { get; set; }
        public ICommand Command { get; set; }
        public string CommandParameter { get; set; }
        public string NumberText { get; set; }        
    }


}
