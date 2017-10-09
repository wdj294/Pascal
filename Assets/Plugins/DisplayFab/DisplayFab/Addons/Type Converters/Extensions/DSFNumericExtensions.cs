/******************************
DisplayFab by Techooka Labs 
http://displayfab.techooka.com
******************************/


using UnityEngine;

namespace Techooka.DisplayFab.Addons
{

    /// <summary>
    /// These are some extension examples for the DisplayFab Converter. 
    /// You can add similar extensions as per your requirements.
    /// </summary>
    public static class DSFNumericExtensions
    {

        /// <summary>
        /// Converts a string to int
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt(this string value)
        {
            return System.Convert.ToInt32(value);
        }

        /// <summary>
        /// Converts a string to float
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float ToFloat(this string value)
        {
            return (float)System.Convert.ToDouble(value);
        }

        /// <summary>
        /// Converts an int to string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConvertToString(this int value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Converts a float to string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConvertToString(this float value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Converts a double to string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConvertToString(this double value)
        {
            return value.ToString();
        }


        /// <summary>
        /// Converts a float to string and appends a prefix
        /// </summary>
        /// <param name="value"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string ToStringWithPrefix(this float value, string prefix)
        {
            return prefix + (value).ToString();
        }

        /// <summary>
        /// Converts a float to string and appends a suffix string 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string ToStringWithSuffix(this float value, string suffix)
        {
            return (value).ToString() + suffix;
        }

        /// <summary>
        /// Converts a float to string and appends a prefix and suffix string to it
        /// </summary>
        /// <param name="value"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string ToStringWithPrefixAndSuffix(this float value, string prefix, string suffix)
        {
            return prefix+ (value).ToString() + suffix;
        }

    //    public static float RoundToInt(this float value) { return Mathf.RoundToInt(value); }

       /// <summary>
       /// Rounds the float value to the number of decimal places specified and returns a float
       /// </summary>
       /// <param name="value"></param>
       /// <param name="decimalPlaces"></param>
       /// <returns></returns>
       public static float Round(this float value,int decimalPlaces) { return (float) System.Math.Round((float) value, decimalPlaces); }

        /// <summary>
        /// Converts a float value to % value by multiplying by 100 and returns the float value, rounded to the number of decimal places specified
        /// </summary>
        /// <param name="value">value to convert to percentage</param>
        /// <param name="decimalPlaces">number of decimal places to round to</param>
        /// <returns></returns>
        public static float ConvertToPercent(this float value,int decimalPlaces) { return (float)System.Math.Round((float) (value*100.0f), decimalPlaces); }

    
       /// <summary>
       /// Converts a float value to % value by multiplying by 100 and returns the float value
       /// </summary>
       /// <param name="value"></param>
       /// <returns></returns>
       public static float ConvertToPercent(this float value) { return Mathf.RoundToInt(value * 100.0f); }


        /// <summary>
        /// Prefixes a $ symbol to an integer and returns as a string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDollarPrefix(this int value)
        {
            return "$" + value;
        }

        /// <summary>
        /// Prefixes a $ symbol to a float and returns as a string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDollarPrefix(this float value)
        {
            return "$" + value;
        }

        /// <summary>
        /// Adds a % as a suffix to an float and returns as a string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToPercentSuffix(this float value)
        {
            return value + "%";
        }

        /// <summary>
        /// Prefixes a $ symbol to a float, rounds it using Mathf.Round and returns as a string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDollarPrefixAndRounded(this float value)
        {
            return "$" + Mathf.Round(value);
        }

        /// <summary>
        /// Prefixes a $ symbol to a float, rounds it to the nearest integer using Mathf.RoundToInt and returns as a string 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDollarPrefixAndRoundToInt(this float value)
        {
            return "$" + Mathf.RoundToInt(value);
        }

        /// <summary>
        /// Test function
        /// Randomizes a value between 0.0 to 1.0 using UnityEngine.Random.Range and returns it. 
        /// Note: The value parameter is not used in this case.
        /// </summary>
        /// <param name="value">Dummy value</param>
        /// <returns></returns>
        public static float RandomizeColor(this float value)
        {
            return UnityEngine.Random.Range(0.0f, 1.0f);
        }


    }
}