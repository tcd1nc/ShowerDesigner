using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace YankeeShower.ViewModels
{
    class NozzleSettingsVM : ViewModelBase
    {
        int _numnozzles = 1;
        bool _nozzledelete = false;
        FullyObservableCollection<NozzleControl> _nozzles = new FullyObservableCollection<NozzleControl>();
        bool _canexecutedelete = true;
       
        public NozzleSettingsVM(ShowerVM vm)
        {
            try
            {
                _nozzles = vm.Nozzles;
                Nozzledata = StaticData.Nozzledata;
                NozzleTypes =vm.NozzleTypes;

                NozzleTypes.CollectionChanged += NozzleTypes_CollectionChanged;
                NozzleTypes.ItemPropertyChanged += NozzleTypes_ItemPropertyChanged;

                _numnozzles = vm.Nozzles.Count;
                if (Nozzles.Count == 0)
                    NumNozzles = 1;
            }
            catch
            {
                MessageBox.Show("The nozzle settings cannot be loaded.", "Unable to load nozzle settings", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                CloseWindowFlag = true;
            }
            if (vm.NozzleTypes.Count == 0)
            {
                MessageBox.Show("The nozzle types cannot be loaded.", "Unable to load nozzle types", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                CloseWindowFlag = true;
            }

            MaximumNozzleToYankee = vm.MaximumNozzleToYankee;
        }

        #region Event Handlers

        private void NozzleTypes_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            //UpdateNozzleNames();
            foreach (NozzleControl n in Nozzles)
            {
                if ((sender as Collection<NozzleType>)[e.CollectionIndex].NozzleTypeName == n.NozzleTypeName)
                {
                    //trigger change in drawn pattern
                    n.NozzleTypeName = Constants.StandardNozzle;
                    n.NozzleTypeName = (sender as Collection<NozzleType>)[e.CollectionIndex].NozzleTypeName;                   
                }
            }
        }

        private void NozzleTypes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action.Equals(System.Collections.Specialized.NotifyCollectionChangedAction.Remove))
            {
                UpdateNozzleNames();
            }
        }

        #endregion

        #region Properties

        int maximumnozzletoyankee;
        public int MaximumNozzleToYankee
        {
            get { return maximumnozzletoyankee; }
            set { SetField(ref maximumnozzletoyankee, value); }
        }

        FullyObservableCollection<NozzleType> _nozzletypes;
        public FullyObservableCollection<NozzleType> NozzleTypes
        {
            get { return _nozzletypes; }
            set { SetField(ref _nozzletypes, value); }
        }

        public FullyObservableCollection<NozzleControl> Nozzles
        {
            get { return _nozzles; }
            set { SetField(ref _nozzles, value); }
        }

        FullyObservableCollection<Models.NozzleDataModel> _nozzledata;
        public FullyObservableCollection<Models.NozzleDataModel> Nozzledata
        {
            get { return _nozzledata; }
            set { SetField(ref _nozzledata, value); }
        }

        public int NumNozzles
        {
            get { return _numnozzles; }
            set {
                if (!_nozzledelete)
                {
                    if (value > _numnozzles)
                        AddNewNozzle();
                    else
                        if (value < _numnozzles)
                            RemoveLastNozzle();
                }
                else
                    _nozzledelete = false;                
                SetField(ref _numnozzles, value); }
        }

        #endregion

        #region Private functions

        private void UpdateNozzleNames()
        {
            FullyObservableCollection<NozzleControl> nz = Nozzles;
            Nozzles = null;
            Nozzles = nz;
            NozzleTypes.CollectionChanged += NozzleTypes_CollectionChanged;
            NozzleTypes.ItemPropertyChanged += NozzleTypes_ItemPropertyChanged;
        }

        private void AddNewNozzle()
        {
            NozzleControl n = new NozzleControl
            {
                NozzleID = _nozzles.Count + 1
            };
            if (n.NozzleID == 1)
            {
                n.NozzleOrificeID = Constants._defaultOrificeID;
                n.NozzleSpacing = 0;
                n.NozzleToYankee = Constants._defaultNozzletoYankee;
                n.SprayRotation = Constants._defaultNozzlerotation;
                n.SprayAngle = Constants._defaultNozzleAngle;
                n.NozzlePressure = Constants._operatingpressure;
                n.NozzleTypeName = Constants.StandardNozzle;
                n.NozzleTypes = NozzleTypes;
            }
            else
            {
                n.NozzleOrificeID = Nozzles[n.NozzleID - 2].NozzleOrificeID;
                if (n.NozzleID > 2)
                    n.NozzleSpacing = Nozzles[n.NozzleID - 2].NozzleSpacing;
                else
                    n.NozzleSpacing = Constants._defaultNozzleSpacing;
                n.NozzleToYankee = Nozzles[n.NozzleID - 2].NozzleToYankee;
                               
                n.SprayRotation = Nozzles[n.NozzleID - 2].SprayRotation;
                n.SprayAngle = Nozzles[n.NozzleID - 2].SprayAngle;                
                n.NozzlePressure = Nozzles[0].NozzlePressure;
                n.NozzleTypeName = Nozzles[n.NozzleID - 2].NozzleTypeName;
                n.NozzleTypes = NozzleTypes;                                                               
                n.BoomTop = MaximumNozzleToYankee + Constants._nozzleheight + 20;

            }               
            Nozzles.Add(n);

            if (Nozzles.Count > 1)
            {                
                Nozzles[Nozzles.Count - 1].NozzleToYankee = MaximumNozzleToYankee;
            }
        }

        private void RemoveNozzle(NozzleControl n)
        {
            if (n.NozzleID != 1)
                Nozzles.Remove(n);
        }

        private void RemoveLastNozzle()
        {
            if (_nozzles.Count > 1)
                _nozzles.Remove(_nozzles[_nozzles.Count-1]);
        }

        private void RenumberNozzles()
        {
            int _id = 1;
            foreach(NozzleControl n in Nozzles)
            {
                n.NozzleID = _id;
                _id++; 
            }
        }

        #endregion

        #region Commands

        ICommand _deletenozzle;
        public ICommand DeleteNozzle
        {
            get
            {
                if (_deletenozzle == null)
                    _deletenozzle = new DelegateCommand(CanExecuteDelete, ExecuteDelete);
                return _deletenozzle;
            }
        }
        
        private bool CanExecuteDelete(object obj)
        {
            return _canexecutedelete;
        }

        private void ExecuteDelete(object parameter)
        {
            RemoveNozzle(parameter as NozzleControl);

            _nozzledelete = true;
            NumNozzles = _nozzles.Count;
            RenumberNozzles();
        }

        #endregion
        
    }
}
