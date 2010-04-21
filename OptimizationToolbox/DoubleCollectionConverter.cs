using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;

namespace OptimizationToolbox
{
    public class DoubleCollectionConverter : System.ComponentModel.TypeConverter
    {
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            else return false;
        }

        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if ((typeof(string)).IsInstanceOfType(value))
            {
                return DoubleCollectionConverter.convert((string)value);
            }
            else
                return null;
        }

        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
        {
            if ((sourceType == typeof(List<double>)) || (sourceType == typeof(string[])))
                return true;
            else return false;
        }

        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context,
            System.Globalization.CultureInfo culture, object value, Type s)
        {
            if ((typeof(List<double>)).IsInstanceOfType(value))
            {
                return DoubleCollectionConverter.convert((List<double>)value);
            }
            else
                return null;
        }

        public static List<double> convert(string value)
        {
            List<double> items = new List<double>();
            char[] charSeparators = new char[] { ',', '(', ')', ' ', ':', ';', '/', '\\', '\'', '\"' };
            double temp;
            string[] results = value.Split(charSeparators);

            for (int i = 0; i < results.GetLength(0); i++)
                if ((results[i] != "") && (double.TryParse(results[i].Trim(), out temp)))
                    items.Add(temp);

            return items;
        }

        public static string convert(List<double> values)
        {
            string text = "";
            if (values.Count > 0)
                text = values[0].ToString();
            if (values.Count > 1)
            {
                for (int i = 1; i < values.Count; i++)
                    text += ", " + values[i].ToString();
            }
            return text;
        }

        public static string convert(double[] values)
        {
            string text = "";
            if (values.GetLength(0) > 0)
                text = values[0].ToString();
            if (values.GetLength(0) > 1)
            {
                for (int i = 1; i < values.GetLength(0); i++)
                    text += ", " + values[i].ToString();
            }
            return text;
        }
    }

}
