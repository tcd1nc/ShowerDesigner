using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace YankeeShower.ViewModels
{
    public class NozzlePatternVM :ViewModelBase
    {
        private List<Pattern> _nozzlepatterns;
        private ObservableCollection<string> _nozzlenames;

        bool _canexecutenew = true;
        bool _canexecutesave = true;
        bool _canexecutesaveas = true;
        bool _canexecutedelete = true;

        public ICommand NewSprayPattern { get; set; }
        public ICommand SaveSprayPattern { get; set; }
        public ICommand SaveAsSprayPattern { get; set; }
        public ICommand DeleteSprayPattern { get; set; }

        public NozzlePatternVM(FullyObservableCollection<NozzleType> _noztypes)
        {
            _nozzlepatterns = new List<Pattern>();
            _nozzlenames = new ObservableCollection<string>();
            NozzleTypes = _noztypes;

            try
            {
                if (File.Exists(Constants._nozzlefilename))
                {
                    ObjectToSerialize objectToSerialize = new ObjectToSerialize();
                    Serializer serializer = new Serializer();
                    objectToSerialize = serializer.DeSerializeObject<ObjectToSerialize>(Constants._nozzlefilename);
                    _nozzlepatterns = objectToSerialize.NozzlePatterns;

                    if (_nozzlepatterns.Count > 0)
                    {
                        _spraydesigner = new YankeeShower.Views.SprayDesigner(100, 100, 30)
                        {
                            Spray = (int[])_nozzlepatterns[0].PPattern.Clone()
                        };

                        SprayDesigner = _spraydesigner;

                        //populate combo

                        for (int i = 0; i < _nozzlepatterns.Count; i++)
                            _nozzlenames.Add(_nozzlepatterns[i].PName);
                        SelectedNozzle = _nozzlepatterns[0].PName;
                    }
                }
                else
                {
                    MessageBox.Show("The nozzle pattern file cannot be found.", "File missing", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    CloseWindowFlag = true;
                }

            }
            catch
            {
                MessageBox.Show("The nozzle pattern file cannot be loaded.", "Unable to load file", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                CloseWindowFlag = true;
            }
            
            NewSprayPattern = new RelayCommand(ExecuteNew, CanExecuteNew);
            SaveSprayPattern = new RelayCommand(ExecuteSave, CanExecuteSave);
            SaveAsSprayPattern = new RelayCommand(ExecuteSaveAs, CanExecuteSaveAs);
            DeleteSprayPattern = new RelayCommand(ExecuteDelete, CanExecuteDelete);
            
        }

        #region Properties

        FullyObservableCollection<NozzleType> _nozzletypes;
        public FullyObservableCollection<NozzleType> NozzleTypes
        {
            get { return _nozzletypes; }
            set { SetField(ref _nozzletypes, value); }
        }

        string _selectednozzle;
        public string SelectedNozzle
        {
            get { return _selectednozzle; }
            set { SetField(ref _selectednozzle, value);

                if (FindNozzleIndex((string)value) != -1)
                {
                    SprayDesigner.Spray = (int[])_nozzlepatterns[FindNozzleIndex((string)value)].PPattern.Clone();
                    _canexecutesave = true;
                    _canexecutesaveas = true;
                    _canexecutedelete = true;
                }
            }
        }

        public ObservableCollection<string> NozzleNames
        {
            get { return _nozzlenames; }
            set { SetField(ref _nozzlenames, value); }
        }

        YankeeShower.Views.SprayDesigner _spraydesigner;
        public YankeeShower.Views.SprayDesigner SprayDesigner
        {
            get { return _spraydesigner; }
            set { SetField(ref _spraydesigner, value); }
        }

        #endregion

        #region Commands
        
        private bool CanExecuteNew(object obj)
        {
            return _canexecutenew;
        }

        private void ExecuteNew(object parameter)
        {
            SprayDesigner = new YankeeShower.Views.SprayDesigner(100, 100, 30);
            SelectedNozzle = string.Empty;
            _canexecutesave = false;
            _canexecutedelete = false;
        }
               
        private bool CanExecuteSave(object obj)
        {
            return _canexecutesave;
        }

        private Pattern SaveNozzlePattern(string _nozzlename, int _indx)
        {
            if (_nozzlepatterns == null)
                _nozzlepatterns = new List<Pattern>();

            Pattern p = new Pattern();
            if(_indx > -1)
                p = _nozzlepatterns[_indx];
            p.PName = _nozzlename;

            int[] _spray = new int[SprayDesigner.Spray.Length];  //??
            _spray = SprayDesigner.Spray;

            for (int i = 0; i < _spray.Length; i++)
                p.PPattern[i] = _spray[i];

            if(_indx == -1)
                _nozzlepatterns.Add(p);

            //save entire list
            ObjectToSerialize objectToSerialize = new ObjectToSerialize
            {
                NozzlePatterns = _nozzlepatterns
            };
            Serializer serializer = new Serializer();
            serializer.SerializeObject<ObjectToSerialize>(Constants._nozzlefilename, objectToSerialize);

            return p;
        }

        private void ExecuteSave(object parameter)
        {
            string nozzlename = SelectedNozzle;
            int _nozzleindex = -1;
           
            _nozzleindex = FindNozzleIndex(nozzlename);

            if (_nozzleindex > -1)
            {
                //confirm update                   
                if (System.Windows.MessageBox.Show("Do you want to overwrite this nozzle pattern?", "Overwrite Nozzle Pattern", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel) == MessageBoxResult.OK)
                {
                    //get nozzle in list
                    //update by replacing nozzle pattern in list
                    int index = _nozzlepatterns.FindIndex(pat => pat.PName == nozzlename);
                   
                    if (index > -1)
                    {
                        Pattern p = SaveNozzlePattern(nozzlename, index);
                        NozzleType _t = new NozzleType();
                        foreach (NozzleType n in NozzleTypes)
                        {
                            if (n.NozzleTypeName == nozzlename)
                            {
                                _t = n;
                                break;
                            }
                        }
                        _t.Pattern = p.PPattern;
                                                
                    }       
                }
            }
        }
                      
        private bool CanExecuteSaveAs(object obj)
        {
            return _canexecutesaveas;
        }

        private bool IsValidName(string nozzlename)
        {
            //check for length > 0
            //check for valid characters
            if ((nozzlename.Trim().Length > 0))
            {
                return true;
            }
            return false;
        }

        private void ExecuteSaveAs(object parameter)
        {
            string nozzlename;          

            TextInputDialog inputDialog = new TextInputDialog("New nozzle spray pattern","Please enter the nozzle name:", "");
            if (inputDialog.ShowDialog() == true)
            {
                nozzlename = inputDialog.Answer;

                if (IsValidName(nozzlename))
                {
                    if (FindNozzleIndex(nozzlename) == -1)
                    {
                        //add new to nozzle list
                        //save all               
                        Pattern p = SaveNozzlePattern(nozzlename, -1);

                        _nozzlenames.Add(nozzlename);

                        NozzleType _t = new NozzleType
                        {
                            NozzleTypeName = nozzlename,
                            Pattern = p.PPattern
                        };
                        NozzleTypes.Add(_t);
                        SelectedNozzle = nozzlename;
                        _canexecutedelete = true;
                    }
                    else
                        MessageBox.Show("Nozzle already exists. Unable to save.", "Duplicate Name", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
                else
                    MessageBox.Show("Nozzle name is invalid. Unable to save.", "Invalid Name", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);

            }
        }

        private bool CanExecuteDelete(object obj)
        {
            if (string.IsNullOrEmpty(SelectedNozzle))
                return false;
            return _canexecutedelete;
        }
        
        private int FindNozzleIndex(string nozzlename)
        {
            // return -1;
            Pattern p = new Pattern
            {
                PName = nozzlename
            };
            return _nozzlepatterns.IndexOf(p);
        }

        private void ExecuteDelete(object parameter)
        {
            if (MessageBox.Show("Do you want to delete this nozzle pattern?", "Delete Nozzle Pattern", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel) == MessageBoxResult.OK)
            {

                string _selectednozzle = (string)parameter;

                _nozzlepatterns.RemoveAt(FindNozzleIndex((string)_selectednozzle));
                //save entire list
                ObjectToSerialize objectToSerialize = new ObjectToSerialize
                {
                    NozzlePatterns = _nozzlepatterns
                };
                Serializer serializer = new Serializer();
                serializer.SerializeObject<ObjectToSerialize>(Constants._nozzlefilename, objectToSerialize);

                _nozzlenames.Remove(_selectednozzle);
                SprayDesigner = new YankeeShower.Views.SprayDesigner(100, 100, 30);

                NozzleType _t = new NozzleType();
                foreach(NozzleType n in NozzleTypes)
                {
                    if(n.NozzleTypeName == _selectednozzle)
                    {
                        _t=n;
                        break;
                    }
                }
                NozzleTypes.Remove(_t);

                //  NozzleTypes.Remove()
                // StaticData.CreateNozzleList();
           //      ViewModels.ShowerVM.UpdateNozzleTypes();
                //ViewModels.NozzleSettingsVM.UpdateNozzleTypes();
            }
        }

        #endregion


    }
}
