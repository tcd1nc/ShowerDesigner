using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace YankeeShower
{
    [Serializable()]
    public class ShowerToSerialize : ISerializable
    {
        public ShowerToSerialize()
        {
        }

        public Shower Shower { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("CustomShower", Shower);
        }

        public ShowerToSerialize(SerializationInfo info, StreamingContext ctxt)
        {
            this.Shower = (Shower)info.GetValue("CustomShower", typeof(Shower));
        }
    }


    [Serializable()]
    public class Shower : ISerializable
    {
        public Shower()
        {
            Nozzles = new Collection<SerializedNozzle>();
        }

        public Shower(SerializationInfo info, StreamingContext ctxt)
        {
            this.ShowerName = (string)info.GetValue("Name", typeof(string));
            this.YankeeWidth = (double)info.GetValue("YankeeWidth", typeof(double));
            this.YankeeSpeed = (int)info.GetValue("YankeeSpeed", typeof(int));
            this.YankeeDiameter = (double)info.GetValue("YankeeDiameter", typeof(double));
            this.DwellDistance = (int)info.GetValue("DwellDistance", typeof(int));
            this.OperatingPressure = (double)info.GetValue("OperatingPressure", typeof(double));
            this.ShowerTemperature = (int)info.GetValue("ShowerTemperature", typeof(int));
            this.NumberNozzles = (int)info.GetValue("NumberNozzles", typeof(int));
            this.CoverageColour = (string)info.GetValue("CoverageColour", typeof(string));
            this.Nozzles = (Collection <SerializedNozzle>)info.GetValue("Nozzles", typeof(Collection<SerializedNozzle>));          
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Name", this.ShowerName);
            info.AddValue("YankeeWidth", this.YankeeWidth);
            info.AddValue("YankeeSpeed", this.YankeeSpeed);
            info.AddValue("YankeeDiameter", this.YankeeDiameter);
            info.AddValue("DwellDistance", this.DwellDistance);
            info.AddValue("OperatingPressure", this.OperatingPressure);
            info.AddValue("ShowerTemperature", this.ShowerTemperature);
            info.AddValue("NumberNozzles", this.NumberNozzles);
            info.AddValue("CoverageColour", this.CoverageColour);
            info.AddValue("Nozzles", this.Nozzles);
        }

        public string ShowerName { get; set; }

        public double YankeeWidth { get; set; }

        public int YankeeSpeed { get; set; }

        public double YankeeDiameter { get; set; }

        public int DwellDistance { get; set; }

        public double OperatingPressure { get; set; }

        public int ShowerTemperature { get; set; }

        public int NumberNozzles { get; set; }

        public string CoverageColour { get; set; }

        public Collection<SerializedNozzle> Nozzles { get; set; }

    }

    [Serializable()]
    public class SerializedNozzle :ISerializable
    {
        public SerializedNozzle() { }

        public SerializedNozzle(SerializationInfo info, StreamingContext ctxt)
        {
            NozzleSprayWidth = (int)info.GetValue("NozzleSprayWidth", typeof(int));
            NozzleSpacing = (int)info.GetValue("NozzleSpacing", typeof(int));
            SprayAngle = (int)info.GetValue("SprayAngle", typeof(int));
            MaximumSprayAngle = (int)info.GetValue("MaximumSprayAngle", typeof(int));
            NozzleToYankee = (int)info.GetValue("NozzleToYankee", typeof(int));
            SprayRotation = (int)info.GetValue("SprayRotation", typeof(int));
            NozzleFlow = (double)info.GetValue("NozzleFlow", typeof(double));
            NozzlePressure = (double)info.GetValue("NozzlePressure", typeof(double));
            NozzleOrificeID = (int)info.GetValue("NozzleOrificeID", typeof(int));
            NozzleTypeName = (string)info.GetValue("NozzleTypeName", typeof(string));
            SprayPattern = (double[])info.GetValue("SprayPattern", typeof(double[]));
        }

        public int NozzleSprayWidth { get; set; }

        public int NozzleSpacing { get; set; }

        public double SprayAngle { get; set; }

        public int MaximumSprayAngle { get; set; }

        public int NozzleToYankee { get; set; }

        public int SprayRotation { get; set; }

        public double NozzleFlow { get; set; }

        public double NozzlePressure { get; set; }

        public int NozzleOrificeID { get; set; }

        public string NozzleTypeName { get; set; }

        public double[] SprayPattern { get; set; }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("NozzleSprayWidth", NozzleSprayWidth);
            info.AddValue("NozzleSpacing", NozzleSpacing);
            info.AddValue("SprayAngle", SprayAngle);
            info.AddValue("MaximumSprayAngle", MaximumSprayAngle);
            info.AddValue("NozzleToYankee", NozzleToYankee);
            info.AddValue("SprayRotation", SprayRotation);
            info.AddValue("NozzleFlow", NozzleFlow);
            info.AddValue("NozzlePressure", NozzlePressure);
            info.AddValue("NozzleOrificeID", NozzleOrificeID);
            info.AddValue("NozzleTypeName", NozzleTypeName);
            info.AddValue("SprayPattern", SprayPattern);
        }

    }


}
