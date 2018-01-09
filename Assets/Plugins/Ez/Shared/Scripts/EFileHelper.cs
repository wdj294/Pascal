// Copyright (c) 2016-2017 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace Ez
{
    public static class EFileHelper
    {
        #region FileExists CreateDirectory
        /// <summary>
        /// Returns true if the file exists at the specified path
        /// </summary>
        public static bool FileExists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Creates a Directory at the specified path
        /// </summary>
        public static void CreateDirectory(string path)
        {
            FileInfo file = new FileInfo(path);
            file.Directory.Create();
        }
        #endregion

        #region GetAbsoluteDirectoryPath GetRelativeDirectoryPath
        /// <summary>
        /// Searches for the directoryName in all the project's directories and returns the absolute path of the first one it encounters
        /// </summary>
        public static string GetAbsoluteDirectoryPath(string directoryName)
        {
            string[] directoryPath = Directory.GetDirectories(Application.dataPath, directoryName, SearchOption.AllDirectories);
            if (directoryPath == null)
            {
                Debug.LogError("[Doozy] You searched for the [" + directoryName + "] folder, but no folder with that name exists in the current project.");
                return "ERROR";
            }
            else if (directoryPath.Length > 1)
            {
                //Debug.LogWarning("[Doozy] You searched for the [" + directoryName + "] folder. There are " + directoryPath.Length + " folders with that name. Returned the folder location for the first one, but it might not be the one you're looking for. Give the folder you are looking for an unique name to avoid any issues.");
            }
            return directoryPath[0];
        }

        /// <summary>
        /// Searches for the directoryName in all the project's directories and returns the relative path of the first one it encounters
        /// </summary>
        public static string GetRelativeDirectoryPath(string directoryName)
        {
            string directoryPath = GetAbsoluteDirectoryPath(directoryName);
            directoryPath = directoryPath.Replace(Application.dataPath, "Assets");
            return directoryPath;
        }
        #endregion

        #region ReadFile WriteFile DeleteFile
        public static T ReadFile<T>(string filePath, Func<FileStream, T> deserializationMethod)
        {
            if (!FileExists(filePath))
            {
                Debug.LogError("[Doozy] Can't load " + filePath + ". File does not exist.");
            }
            FileStream stream = new FileStream(filePath, FileMode.Open);
            T data = deserializationMethod(stream);
            stream.Close();
            return data;
        }

        public static void WriteFile<T>(string filePath, T obj, Action<FileStream, T> serializeMethod)
        {
            CreateDirectory(filePath);
            FileStream stream = new FileStream(filePath, FileMode.Create);
            serializeMethod(stream, obj);
            stream.Close();
        }

        public static void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }
        #endregion

        #region SerializeXML DeserializeXML GetXMLFile
        public static void SerializeXML<T>(FileStream stream, T obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stream, obj);
        }

        public static T DeserializeXML<T>(FileStream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(stream);
        }

        /// <summary>
        /// Returns a class of T type created from the file (with the given fileExtension) found at the given path and deserialized with the XML deserializer
        /// </summary>
        public static T GetXMLFile<T>(string directoryPath, string fileName, string fileExtenstion)
        {
            string filePath = directoryPath + "/" + fileName + "." + fileExtenstion;
            return (T)ReadFile<T>(filePath, DeserializeXML<T>);
        }
        #endregion

        #region GetFiles GetFilesNames GetDirectories GetDirectoriesNames
        /// <summary>
        /// Returns a FileInfo array of all the files found at the specified path
        /// </summary>
        public static FileInfo[] GetFiles(string directoryPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            return directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
        }

        /// <summary>
        /// Returns a FileInfo array of all the files, with the given fileExtension, found at the specified path
        /// </summary>
        public static FileInfo[] GetFiles(string directoryPath, string fileExtension)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            return directoryInfo.GetFiles("*." + fileExtension, SearchOption.AllDirectories);
        }

        /// <summary>
        /// Returns a string array of all the filenames found at the specified path
        /// </summary>
        public static string[] GetFilesNames(string directoryPath)
        {
            List<string> list = new List<string>();
            FileInfo[] fileInfo = GetFiles(directoryPath);
            if (fileInfo != null)
            {
                for (int i = 0; i < fileInfo.Length; i++) { list.Add(fileInfo[i].Name.Replace(fileInfo[i].Extension, "")); }
                list.Sort();
            }
            return list.ToArray();
        }

        /// <summary>
        /// Returns a string array of all the filenames, of the files with the given fileExtension, found at the specified path
        /// </summary>
        public static string[] GetFilesNames(string directoryPath, string fileExtension)
        {
            List<string> list = new List<string>();
            FileInfo[] fileInfo = GetFiles(directoryPath, fileExtension);
            if (fileInfo != null)
            {
                for (int i = 0; i < fileInfo.Length; i++) { list.Add(fileInfo[i].Name.Replace(fileInfo[i].Extension, "")); }
                list.Sort();
            }
            return list.ToArray();
        }

        /// <summary>
        /// Returns a DirectoryInfo array of all the directories (subfolders) found at the specified path
        /// </summary>
        public static DirectoryInfo[] GetDirectories(string directoryPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();
            return directoryInfoArray;
        }

        /// <summary>
        /// Returns a string array of all the directories names (subfolders) found at the specified path
        /// </summary>
        public static string[] GetDirectoriesNames(string directoryPath)
        {
            List<string> list = new List<string>();
            DirectoryInfo[] directoryInfo = GetDirectories(directoryPath);
            if (directoryInfo != null)
            {
                for (int i = 0; i < directoryInfo.Length; i++) { list.Add(directoryInfo[i].Name); }
                list.Sort();
            }
            return list.ToArray();
        }
        #endregion
    }
}
