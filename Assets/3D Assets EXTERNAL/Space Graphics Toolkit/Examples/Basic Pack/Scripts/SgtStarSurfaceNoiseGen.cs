using UnityEngine;

// This component will generate the required noise LUT for the Star Surface shader
// Make sure the OffsetX/Y/Z values match the shader's 'Noise Step' value
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Star Surface Noise Gen")]
public class SgtStarSurfaceNoiseGen : MonoBehaviour
{
	public string Path = "Assets/Texture.asset";

	public int Size = 64;

	// Use unique prime numbers to minimuze repetition
	public int OffsetX = 23;
	public int OffsetY = 29;
	public int OffsetZ = 31;

	public Texture3D Generate()
	{
		var tot = Size * Size * Size;
		var pix = new Color[tot];

		for (var i = 0; i < tot; i++)
		{
			pix[i] = new Color(Random.value, 0.0f, 0.0f, 0.0f);
		}

		for (var z = 0; z < Size; z++)
		{
			for (var y = 0; y < Size; y++)
			{
				for (var x = 0; x < Size; x++)
				{
					var x2 = (x + OffsetX) % Size;
					var y2 = (y + OffsetY) % Size;
					var z2 = (z + OffsetZ) % Size;
					var i = x  + y  * Size  + z * Size * Size;
					var j = x2 + y2 * Size + z2 * Size * Size;

					pix[i].g = pix[j].r;
				}
			}
		}

		var tex = new Texture3D(Size, Size, Size, TextureFormat.RGB24, false);

		tex.SetPixels(pix);
		tex.Apply();

		return tex;
	}

#if UNITY_EDITOR
	[ContextMenu("Generate To Path")]
	public void GenerateToPath()
	{
		var tex = Generate();

		UnityEditor.AssetDatabase.CreateAsset(tex, Path);
	}
#endif
}