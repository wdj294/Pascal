/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013-2016
 *	
 *	"AssetLoader.cs"
 * 
 *	This handles the management and retrieval of "Resources"
 *	assets when loading saved games.
 * 
 */

using UnityEngine;
using System.Collections;

namespace AC
{

	/**
	 * A class that handles the retrieves of Resources assets stored within save game files.
	 */
	public static class AssetLoader
	{

		private static Object[] textureAssets;
		private static Object[] audioAssets;
		private static Object[] animationAssets;
		private static Object[] materialAssets;
		private static Object[] actionListAssets;


		/**
		 * <summary>Gets a unique name for an asset file that can be used to find it later.</summary>
		 * <param name = "originalFile">The asset file</param>
		 * <returns>A unique identifier for the asset file</returns>
		 */
		public static string GetAssetInstanceID <T> (T originalFile) where T : Object
		{
			if (originalFile != null)
			{
				string name = originalFile.GetType () + originalFile.name;
				name = name.Replace (" (Instance)", "");
				return name;
			}
			return "";
		}


		/**
		 * <summary>Retrieves an asset file.</summary>
		 * <param name = "originalFile">The current asset used in the scene already. If this is null, the operation will not work and null will be returned</param>
		 * <param name = "_name">A unique identifier for the asset file</param>
		 * <returns>The asset file, or the current asset if it wasn't found</returns>
		 */
		public static T RetrieveAsset <T> (T originalFile, string _name) where T : Object
		{
			if (_name == "")
			{
				return originalFile;
			}

			if (originalFile == null)
			{
				return null;
			}

			Object newFile = null;

			if (originalFile is Texture)
			{
				newFile = RetrieveTextures (_name);
			}
			else if (originalFile is AudioClip)
			{
				newFile = RetrieveAudioClip (_name);
			}
			else if (originalFile is AnimationClip)
			{
				newFile = RetrieveAnimClips (_name);
			}
			else if (originalFile is Material)
			{
				newFile = RetrieveMaterials (_name);
			}
			else if (originalFile is ActionListAsset)
			{
				newFile = RetrieveActionListAssets (_name);
			}

			return (newFile != null) ? (T) newFile : originalFile;
		}


		private static Texture RetrieveTextures (string _name)
		{
			textureAssets = RetrieveAssetFiles <Texture> (textureAssets, "Textures");
			return GetAssetFile <Texture> (textureAssets, _name);
		}


		/**
		 * <summary>Retrieves an AudioClip asset file</summary>
		 * <param name = "_name">A unique identifier for the AudioClipe</param>
		 * <returns>The AudioClip, or null if it wasn't found</returns>
		 */
		public static AudioClip RetrieveAudioClip (string _name)
		{
			audioAssets = RetrieveAssetFiles <AudioClip> (audioAssets, "Audio");
			return GetAssetFile <AudioClip> (audioAssets, _name);
		}


		private static AnimationClip RetrieveAnimClips (string _name)
		{
			animationAssets = RetrieveAssetFiles <AnimationClip> (animationAssets, "Animations");
			return GetAssetFile <AnimationClip> (animationAssets, _name);
		}


		private static Material RetrieveMaterials (string _name)
		{
			materialAssets = RetrieveAssetFiles <Material> (materialAssets, "Materials");
			return GetAssetFile <Material> (materialAssets, _name);
		}


		private static ActionListAsset RetrieveActionListAssets (string _name)
		{
			actionListAssets = RetrieveAssetFiles <ActionListAsset> (actionListAssets, "ActionLists");
			return GetAssetFile <ActionListAsset> (actionListAssets, _name);
		}


		private static T GetAssetFile <T> (Object[] assetFiles, string _name) where T : Object
		{
			if (assetFiles != null && _name != null)
			{
				_name = _name.Replace (" (Instance)", "");
				foreach (Object assetFile in assetFiles)
				{
					if (assetFile != null && (_name == (assetFile.GetType () + assetFile.name) || _name == assetFile.name))
					{
						return (T) assetFile;
					}
				}
			}

			return null;
		}


		private static Object[] RetrieveAssetFiles <T> (Object[] assetFiles, string saveableFolderName) where T : Object
		{
			if (assetFiles == null)
			{
				assetFiles = Resources.LoadAll ("SaveableData/" + saveableFolderName, typeof (T));
			}
			if (assetFiles == null || assetFiles.Length == 0)
			{
				assetFiles = Resources.LoadAll ("", typeof (T));
			}

			return assetFiles;
		}


		/**
		 * Clears the cache of stored assets from memory.
		 */
		public static void UnloadAssets ()
		{
			textureAssets = null;
			audioAssets = null;
			animationAssets = null;
			materialAssets = null;
			actionListAssets = null;
			Resources.UnloadUnusedAssets ();
		}

	}

}