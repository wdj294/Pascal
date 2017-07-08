using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GameDataEditor
{
	public static class EditorGUIStyleExtensions
	{
		public static bool IsNullOrEmpty(this GUIStyle variable)
		{
			return variable == null || string.IsNullOrEmpty(variable.name);
		}
	}

	public static class EditorStringExtensions
	{
		static char[] dirSeparators = {'\\', '/'};
		public static string TrimLeadingDirChars(this string variable)
		{
			return variable.TrimStart(dirSeparators);
		}
	}

    public static class EditorDictionaryExtensions
    {
        public static List<bool> ToBoolList(this IList variable)
        {
            var result = new List<bool>();
            try
            {
                for(int x=0; x<variable.Count;  x++)
                    result.Add(Convert.ToBoolean(variable[x]));
            }
            catch
            {
                
            }
            
            return result;
        }

        public static List<List<bool>> ToBoolTwoDList(this IList variable)
        {
            var result = new List<List<bool>>();
            try
            {
                for(int x=0; x<variable.Count;  x++)
                {
                    IList sublist = variable[x] as IList;
                    List<bool> newSublist = new List<bool>();

                    for(int y=0;  y<sublist.Count;  y++)
                        newSublist.Add(Convert.ToBoolean(sublist[y]));

                    result.Add(newSublist);
                }
            }
            catch
            {
                
            }
            
            return result;
        }

        public static List<int> ToIntList(this IList variable)
        {
            var result = new List<int>();
            try
            {
                for(int x=0; x<variable.Count;  x++)
                    result.Add(Convert.ToInt32(variable[x]));
            }
            catch
            {

            }

            return result;
        }

        public static List<List<int>> ToIntTwoDList(this IList variable)
        {
            var result = new List<List<int>>();
            try
            {
                for(int x=0; x<variable.Count;  x++)
                {
                    IList sublist = variable[x] as IList;
                    List<int> newSublist = new List<int>();
                    
                    for(int y=0;  y<sublist.Count;  y++)
                        newSublist.Add(Convert.ToInt32(sublist[y]));
                    
                    result.Add(newSublist);
                }
            }
            catch
            {
                
            }
            
            return result;
        }

        public static List<float> ToFloatList(this IList variable)
        {
            var result = new List<float>();
            try
            {
                for(int x=0; x<variable.Count;  x++)
                    result.Add(Convert.ToSingle(variable[x]));
            }
            catch
            {
                
            }
            
            return result;
        }

        public static List<List<float>> ToFloatTwoDList(this IList variable)
        {
            var result = new List<List<float>>();
            try
            {
                for(int x=0; x<variable.Count;  x++)
                {
                    IList sublist = variable[x] as IList;
                    List<float> newSublist = new List<float>();
                    
                    for(int y=0;  y<sublist.Count;  y++)
                        newSublist.Add(Convert.ToSingle(sublist[y]));
                    
                    result.Add(newSublist);
                }
            }
            catch
            {
                
            }
            
            return result;
        }

        public static List<string> ToStringList(this IList variable)
        {
            var result = new List<string>();
            try
            {
                for(int x=0;  x<variable.Count;  x++)
                    result.Add(variable[x].ToString());
            }
            catch
            {
                
            }
            
            return result;
        }

        public static List<List<string>> ToStringTwoDList(this IList variable)
        {
            var result = new List<List<string>>();
            try
            {
                for(int x=0; x<variable.Count;  x++)
                {
                    IList sublist = variable[x] as IList;
                    List<string> newSublist = new List<string>();
                    
                    for(int y=0;  y<sublist.Count;  y++)
                        newSublist.Add(sublist[y].ToString());
                    
                    result.Add(newSublist);
                }
            }
            catch
            {
                
            }
            
            return result;
        }

        public static Vector2 ToVector2(this Dictionary<string, object> variable)
        {
            Vector2 result = Vector2.zero;
            try
            {
                if (variable != null)
                {
                    result = new Vector2(Convert.ToSingle(variable["x"]),
                                         Convert.ToSingle(variable["y"]));
                }
            }
            catch
            {

            }

            return result;
        }

        public static List<Vector2> ToVector2List(this IList variable)
        {
            List<Vector2> result = new List<Vector2>();

            try
            {
                if (variable != null)
                {
                    foreach(var entry in variable)
                    {
                        var dict = entry as Dictionary<string, object>;
                        result.Add(dict.ToVector2());
                    }
                }
            }
            catch
            {

            }

            return result;
        }

        public static List<List<Vector2>> ToVector2TwoDList(this IList variable)
        {
            List<List<Vector2>> result = new List<List<Vector2>>();

            try
            {
                if (variable != null)
                {
                    foreach(var entry in variable)
                    {
                        var sublist = entry as IList;
                        result.Add(sublist.ToVector2List());
                    }
                }
            }
            catch
            {

            }

            return result;
        }

        public static Vector3 ToVector3(this Dictionary<string, object> variable)
        {
            Vector3 result = Vector3.zero;
            try
            {
                if (variable != null)
                {
                    result = new Vector3(Convert.ToSingle(variable["x"]),
                                         Convert.ToSingle(variable["y"]),
                                         Convert.ToSingle(variable["z"]));
                }
            }
            catch
            {
                
            }
            
            return result;
        }

        public static List<Vector3> ToVector3List(this IList variable)
        {
            List<Vector3> result = new List<Vector3>();
            
            try
            {
                if (variable != null)
                {
                    foreach(var entry in variable)
                    {
                        var dict = entry as Dictionary<string, object>;
                        result.Add(dict.ToVector3());
                    }
                }
            }
            catch
            {
                
            }
            
            return result;
        }

        public static List<List<Vector3>> ToVector3TwoDList(this IList variable)
        {
            List<List<Vector3>> result = new List<List<Vector3>>();
            
            try
            {
                if (variable != null)
                {
                    foreach(var entry in variable)
                    {
                        var sublist = entry as IList;
                        result.Add(sublist.ToVector3List());
                    }
                }
            }
            catch
            {
                
            }
            
            return result;
        }

        public static Vector4 ToVector4(this Dictionary<string, object> variable)
        {
            Vector4 result = Vector4.zero;
            try
            {
                if (variable != null)
                {
                    result = new Vector4(Convert.ToSingle(variable["x"]),
                                         Convert.ToSingle(variable["y"]),
                                         Convert.ToSingle(variable["z"]),
                                         Convert.ToSingle(variable["w"]));
                }
            }
            catch
            {
                
            }
            
            return result;
        }

        public static List<Vector4> ToVector4List(this IList variable)
        {
            List<Vector4> result = new List<Vector4>();
            
            try
            {
                if (variable != null)
                {
                    foreach(var entry in variable)
                    {
                        var dict = entry as Dictionary<string, object>;
                        result.Add(dict.ToVector4());
                    }
                }
            }
            catch
            {
                
            }
            
            return result;
        }

        public static List<List<Vector4>> ToVector4TwoDList(this IList variable)
        {
            List<List<Vector4>> result = new List<List<Vector4>>();
            
            try
            {
                if (variable != null)
                {
                    foreach(var entry in variable)
                    {
                        var sublist = entry as IList;
                        result.Add(sublist.ToVector4List());
                    }
                }
            }
            catch
            {
                
            }
            
            return result;
        }

        public static Color ToColor(this Dictionary<string, object> variable)
        {
            Color result = Color.black;
            try
            {
                if (variable != null)
                {
                    result = new Vector4(Convert.ToSingle(variable["r"]),
                                         Convert.ToSingle(variable["g"]),
                                         Convert.ToSingle(variable["b"]),
                                         Convert.ToSingle(variable["a"]));
                }
            }
            catch
            {
                
            }
            
            return result;
        }
        
        public static List<Color> ToColorList(this IList variable)
        {
            List<Color> result = new List<Color>();
            
            try
            {
                if (variable != null)
                {
                    foreach(var entry in variable)
                    {
                        var dict = entry as Dictionary<string, object>;
                        result.Add(dict.ToColor());
                    }
                }
            }
            catch
            {
                
            }
            
            return result;
        }

        public static List<List<Color>> ToColorTwoDList(this IList variable)
        {
            List<List<Color>> result = new List<List<Color>>();
            
            try
            {
                if (variable != null)
                {
                    foreach(var entry in variable)
                    {
                        var sublist = entry as IList;
                        result.Add(sublist.ToColorList());
                    }
                }
            }
            catch
            {
                
            }
            
            return result;
        }
    }
}