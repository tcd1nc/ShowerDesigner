namespace YankeeShower.Models
{
    public class NozzleDataModel :ViewModelBase
    {
        int _id;
        public int id
        {
            get { return _id; }
            set { SetField(ref _id, value); }
        }

        double _coeff;
        public double Coefficient
        {
            get { return _coeff; }
            set { SetField(ref _coeff, value); }
        }

        double _power;
        public double Power
        {
            get { return _power; }
            set { SetField(ref _power, value); }
        }

        string _orifice;
        public string Orifice
        {
            get { return _orifice; }
            set { SetField(ref _orifice, value); }
        }
    }
}
