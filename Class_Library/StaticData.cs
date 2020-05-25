using System;
using System.Collections.Generic;
using System.IO;

namespace YankeeShower
{
    public static class StaticData
    {

        private static FullyObservableCollection<Models.NozzleDataModel> _nozzledata;
        public static FullyObservableCollection<Models.NozzleDataModel> Nozzledata
        {
            get { return _nozzledata; }
            internal set { }
        }      

        /// <summary>
        /// Create standard list of nozzle orifice data using parameters from Excel calcs
        /// </summary>
        public static void CreateNozzledata()
        {
            FullyObservableCollection<Models.NozzleDataModel> _datacoll = new FullyObservableCollection<Models.NozzleDataModel>
            {
                new Models.NozzleDataModel() { id = 0, Orifice = "Blocked", Power = 0, Coefficient = 0 },
                new Models.NozzleDataModel() { id = 1, Orifice = "0033", Power = 0.462, Coefficient = 0.0094 },
                new Models.NozzleDataModel() { id = 2, Orifice = "0050", Power = 0.4975, Coefficient = 0.0116 },
                new Models.NozzleDataModel() { id = 3, Orifice = "0067", Power = 0.4824, Coefficient = 0.0169 },
                new Models.NozzleDataModel() { id = 4, Orifice = "01", Power = 0.5048, Coefficient = 0.022 },
                new Models.NozzleDataModel() { id = 5, Orifice = "015", Power = 0.5062, Coefficient = 0.0328 },
                new Models.NozzleDataModel() { id = 6, Orifice = "02", Power = 0.4887, Coefficient = 0.0483 },
                new Models.NozzleDataModel() { id = 7, Orifice = "03", Power = 0.4978, Coefficient = 0.0698 },
                new Models.NozzleDataModel() { id = 8, Orifice = "04", Power = 0.4713, Coefficient = 0.1075 },
                new Models.NozzleDataModel() { id = 9, Orifice = "05", Power = 0.4975, Coefficient = 0.1156 },
                new Models.NozzleDataModel() { id = 10, Orifice = "06", Power = 0.5055, Coefficient = 0.1318 },
                new Models.NozzleDataModel() { id = 11, Orifice = "07", Power = 0.4828, Coefficient = 0.1781 },
                new Models.NozzleDataModel() { id = 12, Orifice = "08", Power = 0.4951, Coefficient = 0.1885 },
                new Models.NozzleDataModel() { id = 13, Orifice = "10", Power = 0.5126, Coefficient = 0.2113 }
            };
            _nozzledata = _datacoll;
        }

        public static string GetOrificeFromID(int _id)
        {
            string _orifice = string.Empty;
            foreach(Models.NozzleDataModel nd in Nozzledata)
                if (nd.id == _id)
                {
                    _orifice = nd.Orifice;
                    break;
                }

            return _orifice;

        }


        /// <summary>
        /// Retrieve nozzles from serialized file and return list of nozzles and patterns
        /// </summary>
        /// <returns></returns>
        private static List<Pattern> GetCustomNozzles()
        {
            const string _nozzlefilename = Constants._nozzlefilename;
            List<Pattern> _nozzlepatterns = new List<Pattern>();
            try
            {
                if (File.Exists(_nozzlefilename))
                {
                    ObjectToSerialize objectToSerialize = new ObjectToSerialize();
                    Serializer serializer = new Serializer();
                    objectToSerialize = serializer.DeSerializeObject<ObjectToSerialize>(_nozzlefilename);
                    _nozzlepatterns = objectToSerialize.NozzlePatterns;
                    objectToSerialize = null;
                    serializer = null;
                }
            }
            catch
            {

            }
            return _nozzlepatterns;
        }              

        public static FullyObservableCollection<NozzleType> NozzleTypes
        {
            get { return _nozzletypes; }
            internal set { }
        }

        private static FullyObservableCollection<NozzleType> _nozzletypes;

        /// <summary>
        /// Create static list of nozzles
        /// </summary>
        public static void CreateNozzleList()
        {
            _nozzletypes = new FullyObservableCollection<NozzleType>
            {
                new NozzleType() { NozzleTypeName = Constants.StandardNozzle, SystemNozzle = true },
                new NozzleType() { NozzleTypeName = Constants.AsymmetricLeftNozzle, SystemNozzle = true },
                new NozzleType() { NozzleTypeName = Constants.AsymmetricRightNozzle, SystemNozzle = true },
                new NozzleType() { NozzleTypeName = Constants.SystemNozzle1, SystemNozzle = true },
                new NozzleType() { NozzleTypeName = Constants.SystemNozzle2, SystemNozzle = true },
                new NozzleType() { NozzleTypeName = Constants.SystemNozzle3, SystemNozzle = true },
                new NozzleType() { NozzleTypeName = Constants.SystemNozzle4, SystemNozzle = true },
                new NozzleType() { NozzleTypeName = Constants.SystemNozzle5, SystemNozzle = true }
            };

            List<Pattern> _nozzlepatterns = StaticData.GetCustomNozzles();
            foreach (Pattern cp in _nozzlepatterns)
            {
                _nozzletypes.Add(new NozzleType() { NozzleTypeName = cp.PName, Pattern = cp.PPattern, SystemNozzle = false });
            }
        }

        public static int[] GetCustomSprayPattern(string _patternname)
        {
            foreach(NozzleType nt in _nozzletypes)
            {
                if(nt.NozzleTypeName == _patternname)
                {
                    return nt.Pattern;
                }
            }
            return null;
        }

        /// <summary>
        /// Convert a designed nozzle to a spray pattern using the nozzle's flow and calculated spray width
        /// </summary>
        /// <param name="_patternname"></param>
        /// <param name="_nc"></param>
        /// <returns></returns>
        public static double[] ConvertDesignToActualSpray(string _patternname, NozzleControl _nc)
        {
            double _flow = _nc.NozzleFlow;
            int _spraywidth = _nc.NozzleSprayWidth;

            int[] _spray = GetCustomSprayPattern(_patternname);
            double[] _nozzleprofile = new double[_spraywidth];
            if (_spray!=null)
            {
                //sum all points
                double _sum = 0;
                for (int i = 0; i < _spray.Length; i++)
                     _sum = _sum + (100 - _spray[i]);              
                                
                //scale up by nozzle flow volume and width of actual spray - not allocated spray width
                double[] _temp = new double[_spray.Length];
                for (int j = 0; j < _temp.Length; j++)
                    _temp[j] = 1000*  _flow * 100/ _spraywidth * Constants._showerpatternheight / Constants._maximumlitrespermpermin * (100 - _spray[j]) / _sum;
                
                //scale array indices
                double _scalefactor = 1;
                _scalefactor = Convert.ToDouble(_spray.Length) / _nozzleprofile.Length;
                for (int k = 0; k < _nozzleprofile.Length; k++)
                {
                    if (Convert.ToInt32(k * _scalefactor) < _temp.Length)
                        _nozzleprofile[k] =  ( _temp[Convert.ToInt32(k * _scalefactor)]);
                }          
            }
            else//standard nozzle
            {
                double _linearprofile = 0;
                _linearprofile = Constants._showerpatternheight / Constants._maximumlitrespermpermin * (_flow / _spraywidth) * 1000;
                _nozzleprofile = new double[_spraywidth];
                for (int i = 0; i < _spraywidth; i++)
                     _nozzleprofile[i] = _linearprofile;
                _nc.NozzleTypeName = Constants.StandardNozzle;
            }
            //return new array
            return _nozzleprofile;
        }

        public static double[] MakeSigmoidPattern(NozzleControl _nc, string _systemnozzlename)
        {
            double _midpoint = 1;
            double _slope = 0.3;
            double _htoffset = 0;
            double _flow = _nc.NozzleFlow;

            if(_systemnozzlename == Constants.SystemNozzle1)
            {
                _midpoint = 4;
                _slope = 0.27;
                _htoffset = 0.1;
            }
            else
                if (_systemnozzlename == Constants.SystemNozzle2)
            {
                _midpoint = 1;
                _slope = 0.2;
                _htoffset = 0.1;
            }
            else
            if (_systemnozzlename == Constants.SystemNozzle3)
            {
                _midpoint = 1;
                _slope = 0.3;
                _htoffset = 0.1;
            }
            else
            if (_systemnozzlename == Constants.SystemNozzle4)
            {
                _midpoint = 2.3;
                _slope = 0.18;
                _htoffset = 0;
            }
            else
            {
                _midpoint = 1.7;
                _slope = 0.2;
                _htoffset = 0.2;
            }


            int _spraywidth = _nc.NozzleSprayWidth;
            double[] _nozzleprofile = new double[_spraywidth];
            double[] _spray = new double[100];
            for(int i=0; i<100; i++)
                _spray[i] = (1 / (1 + Math.Exp(-_slope * (25 - Math.Abs(i - 50) - _midpoint))) + _htoffset)/(1+_htoffset);                
            
            //sum all points
            double _sum = 0;
            for (int i = 0; i < _spray.Length; i++)
                 _sum = _sum + _spray[i];
            
            //scale up by nozzle flow volume and width of actual spray - not allocated spray width
            double[] _temp = new double[_spray.Length];
            for (int j = 0; j < _temp.Length; j++)
                _temp[j] = 1000 * _flow * 100 / _spraywidth * Constants._showerpatternheight / Constants._maximumlitrespermpermin *  _spray[j] / _sum;

            //scale array indices
            double _scalefactor = 1;
            _scalefactor = Convert.ToDouble(_spray.Length) / _nozzleprofile.Length;
            for (int k = 0; k < _nozzleprofile.Length; k++)
            {
                if (Convert.ToInt32(k * _scalefactor) < _temp.Length)
                    _nozzleprofile[k] = (_temp[Convert.ToInt32(k * _scalefactor)]);
            }
            return _nozzleprofile;
        }

    }

     
    public class NozzleType : ViewModelBase
    {
        string _name;
        public string NozzleTypeName
        {
            get { return _name; }
            set { SetField(ref _name, value); }
        }

        int[] _pattern;
        public int[] Pattern
        {
            get { return _pattern; }
            set { SetField(ref _pattern, value); }
        }

        bool _issystemnozzle;
        public bool SystemNozzle
        {
            get { return _issystemnozzle; }
            set { SetField(ref _issystemnozzle, value); }
        }
    }

    /// <summary>
    /// System-wide constants
    /// </summary>
    static class Constants
    {
        //file names
        public const string _nozzlefilename = "NozzlePatternFile.nzl";
        public const string _showerfilenameext = "SHR";
        public const string configfilename = @"\config.json";
        
        //config file constants
        public const string licensee = "Licensee";
        public const string eulaccepted = "EulaAccepted";
        public const string LeftLogoStr = "LeftLogo";
        public const string RightLogoStr = "RightLogo";
        public const string FooterTextStr = "FooterText";
        public const string FooterTextColorStr = "FooterTextColor";
        public const string MRUList = "MRUList";

        //nozzle types
        public const string StandardNozzle = "Standard";
        public const string AsymmetricLeftNozzle = "Asymmetric Left";
        public const string AsymmetricRightNozzle = "Asymmetric Right";
        public const string SystemNozzle1 = "System 1 [67% in middle third]";
        public const string SystemNozzle2 = "System 2 [60% in middle third]";
        public const string SystemNozzle3 = "System 3 [62% in middle third]";
        public const string SystemNozzle4 = "System 4 [66% in middle third]";
        public const string SystemNozzle5 = "System 5 [66% in middle third]";

        //nozzle defaults
        public const int _nozzlewidth = 50;
        public const int _nozzleheight = 70;

        public const int _maxshowerwidth = 10000;//10000 = 10m is practical limit for a shower
        public const int _maxnozzlespacing = 500;
        public const int _maxnozzletoyankee = 500;
        public const int _maxsprayrotation = 89;
        public const int _maxnozzleangle = 150;

        public const int _defaultOrificeID = 4;
        public const double _defaultYankeeWidth = 4.0; //m
        public const double _defaultYankeeDiameter = 4.0; //m
        public const int _defaultYankeeSpeed = 1500; //m/min
        public const int _defaultNozzletoYankee = 175;
        public const int _defaultNozzleSpacing = 180;
        public const double _defshowerpressure = 400;

        public const double _defaultBoomToYankee = _nozzleheight + _defaultNozzletoYankee;

        public const double _defaultNozzleAngle = 110.0;
        public const int _defaultNozzlerotation = 5;
        public const int _defaultDwellDistance = 650;
        
        public const double _operatingpressure = 400;
        public const int _showertemperature = 40;
        public const int _maximumlitrespermpermin = 5;
        public const int _showerpatternheight = 150;

        
        //drawing defaults
        public const string _patternPenColor = "Blue";
        public const double _patternPenThickness = 1.5;

        public const int _maxmrulist = 10;

    }

}
