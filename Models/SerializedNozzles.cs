using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace YankeeShower
{
    public class Serializer
    {
        public Serializer() {}

        //Generic binary serializer
        public void SerializeObject<T>(string filename, T objectToSerialize)
        {
            Stream stream = File.Open(filename, FileMode.Create);
            try
            {
                
                BinaryFormatter bFormatter = new BinaryFormatter();
                bFormatter.Serialize(stream, objectToSerialize);
                stream.Flush();
                //stream.Close();
                stream.Dispose();
            }
            catch
            {
                if (stream != null)
                    stream.Dispose();
            }
            
        }

        //Generic binary deserializer
        public T DeSerializeObject<T>(string filename)
        {
            Stream stream = File.Open(filename, FileMode.Open);
            try
            {
                T objectToSerialize;
                //Stream stream = File.Open(filename, FileMode.Open);
                //BinaryFormatter bFormatter = new BinaryFormatter();
                //objectToSerialize = (T)bFormatter.Deserialize(stream);
                //stream.Flush();
                //stream.Close();
                //stream.Dispose();
                //return objectToSerialize;
                               
                BinaryFormatter bFormatter = new BinaryFormatter();
                objectToSerialize = (T)bFormatter.Deserialize(stream);
                stream.Flush();
               // stream.Close();
                                                                              
                stream.Dispose();
                return objectToSerialize;
            }
            catch
            {
                if (stream != null)
                    stream.Dispose();
            }
            return default(T);
        }
    }

        [Serializable()]
        public class ObjectToSerialize : ISerializable
        {
            private List<Pattern> _nozzlepatterns;

        public ObjectToSerialize()
            {
            }

            public List<Pattern> NozzlePatterns
            {
                get { return _nozzlepatterns; }
                set { _nozzlepatterns = value; }
            }

        public ObjectToSerialize(SerializationInfo info, StreamingContext ctxt)
            {
                this._nozzlepatterns = (List<Pattern>)info.GetValue("CustomNozzlePatterns", typeof(List<Pattern>));
            }      

            public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
            {
                info.AddValue("CustomNozzlePatterns", _nozzlepatterns);           
            }
        }


        [Serializable()]
        public class Pattern : ISerializable
        {
            private string _name;
            private int[] _pattern;

        public Pattern()
            {
                _pattern = new int[100];
            }

        public Pattern(SerializationInfo info, StreamingContext ctxt)
            {
                this._name = (string)info.GetValue("Name", typeof(string));
                this._pattern = (int[])info.GetValue("Pattern", typeof(int[]));              
            }

            public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
            {
                info.AddValue("Name", this._name);
                info.AddValue("Pattern", this._pattern);
            }

            public string PName
            {
                get { return _name;}
                set { _name = value;}
            }

            public int[] PPattern
            {
                get {return _pattern;}
                set {_pattern = value;}
            }

            public override bool Equals(object obj)
            {
                Pattern p = (Pattern)obj;
                if (p.PName == this.PName)
                    return true;
                else
                    return false;
              //  return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }


        }

}
