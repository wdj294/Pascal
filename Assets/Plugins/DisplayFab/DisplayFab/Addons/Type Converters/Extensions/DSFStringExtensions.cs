/******************************
DisplayFab by Techooka Labs 
http://displayfab.techooka.com
******************************/


namespace Techooka.DisplayFab.Addons
{
    /// <summary>
    /// Some more examples of Extensions
    /// </summary>
    public static class DSFStringExtensions
    {

        /// <summary>
        /// Converts the first letter of a string to uppercase
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToUppercase(this string value)
        {
            return value.ToUpper();
            
        }

        /// <summary>
        /// Converts the first letter of a string to uppercase
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToUppercaseFirstLetter(this string value)
        {
            //
            // Uppercase the first letter in the string.
            //
            if (value.Length > 0)
            {
                char[] array = value.ToCharArray();
                array[0] = char.ToUpper(array[0]);
                return new string(array);
            }
            return value;
        }

        /// <summary>
        /// Appends a '$' value as a prefix to a string 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDollarPrefix(this string value)
        {
            return "$"+value;
        }


        /// <summary>
        /// Appends a prefix string and returns the full string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string ToStringWithPrefix(this string value, string prefix)
        {
            return prefix + value;
        }

        /// <summary>
        /// Appends a suffix string and returns the full string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string ToStringWithSuffix(this string value, string suffix)
        {
            return value + suffix;
        }

        /// <summary>
        /// Appends a prefix and suffix string to the string value and returns the full string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string ToStringWithPrefixAndSuffix(this string value, string prefix, string suffix)
        {
            return prefix + value + suffix;
        }


    }
}