using System;
namespace YankeeShower
{
    public static class EngineeringCalcs
    {

        //==============================================================================
        //ENGINEERING CALCULATIONS

        public static double CalcEvaporationEnergy(double mixpotTemp, double flowrate)
        {
            if (mixpotTemp > 0 && flowrate > 0)
            {
                double k = 4.184;
                int Hevap = 2260;

                double q1;
                double q2;

                q1 = flowrate * 1000 * k * (100 - mixpotTemp);  //joules
                q2 = flowrate * 1000 * Hevap;                   // joules
                return (q1 + q2) / 1000000;                     //MJ/min
            }
            else
                return 0;
        }

        public static int CalcRevolutionTime(double _yankeediameter, int _yankeespeed)
        {
            if (_yankeespeed > 0 && _yankeediameter > 0)
            {
                double _circumference = Math.PI * _yankeediameter;                             
                double _secondsperrev = (_circumference / Convert.ToDouble(_yankeespeed)) * 60;
                return Convert.ToInt32(_secondsperrev * 1000);
            }
            else
                return 0;
        }


        public static int CalcDwellTime(double _yankeediameter, int _yankeespeed, int _dwelldistance)
        {
            if (_yankeespeed > 0 && _yankeediameter > 0 && _dwelldistance > 0)
            {
                double _circumference = Math.PI * _yankeediameter;
                double _hypotenuse = _yankeediameter / 2;
                double _opposite = Convert.ToDouble(_dwelldistance / 2) / 1000;
                double _sin = _opposite / _hypotenuse;
                double _angle = 2 * Math.Asin(_sin) * 180 / Math.PI;
                double _circlefraction = _angle / 360;
                double _secondsperrev = (_circumference / Convert.ToDouble(_yankeespeed)) * 60;
                return Convert.ToInt32(_circlefraction * _secondsperrev * 1000);
            }
            else
                return 0;
        }

        public static double CalcNipToSPREvaporativeLoad(double _yankeediameter, int _yankeespeed, int _dwelldistance, double _yankeewidth, double _totalshowerflow)
        {
            if (_yankeediameter > 0 && _yankeespeed > 0 && _dwelldistance > 0 && _yankeewidth > 0 && _totalshowerflow > 0)
            {
                double _circumference = Math.PI * _yankeediameter;
                double _hypotenuse = _yankeediameter / 2;
                double _opposite = Convert.ToDouble(_dwelldistance / 2) / 1000;
                double _sin = _opposite / _hypotenuse;
                double _angle = 2 * Math.Asin(_sin) * 180 / Math.PI;
                double _circlefraction = _angle / 360;
                double _dwellarea = _circlefraction * _circumference * _yankeewidth;
                return 60 * _totalshowerflow / _dwellarea;
            }
            else
                return 0;
        }


        public static double CalcTotalEvaporativeLoad(double yankeediameter, int yankeespeed, double yankeewidth, double totalshowerflow)
        {
            if (yankeediameter > 0 && yankeespeed > 0 && yankeewidth > 0 && totalshowerflow > 0)
            {
                double circumference = Math.PI * yankeediameter;               
                double totalarea = circumference * yankeewidth;
                return 1000 * 60 * totalshowerflow / (totalarea * yankeespeed);
            }
            else
                return 0;
        }


    }
}
