using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;

namespace YankeeShower
{
   
    public class EnumValue
    {
        public EnumValue() { }


        public Enum Enumvalue
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public int ID
        {
            get; set;
        }

    }

    public class EnumerationManager
    {
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static Collection<EnumValue> GetEnumList(Type enumvar)
        {
            Collection<EnumValue> _p = new Collection<EnumValue>();
            EnumValue _enumvalue;
            Array _a = Enum.GetNames(enumvar);
            foreach (Enum name in Enum.GetValues(enumvar))
            {
                _enumvalue = new EnumValue();
                _enumvalue.Description = GetEnumDescription(name);
                _enumvalue.Enumvalue = name;
                _enumvalue.ID = Convert.ToInt32(name);
                _p.Add(_enumvalue);
            }
            return _p;
        }
    }

    public static class EnumerationLists
    {
        #region Enumeration Lists

        //public static Collection<EnumValue> NozzleTypesList
        //{
        //    get { return EnumerationManager.GetEnumList(typeof(NozzleType)); }
        //}

        #endregion
    }


}
