using System.IO;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public static class MaterialTool {

	[MenuItem("材质工具/生成噪点图/Perlin 1X")]
	public static void GeneratePerlinNoiseMap1X() {
		GeneratePerlinNoiseMap(1);
	}
	
	[MenuItem("材质工具/生成噪点图/Perlin 2X")]
	public static void GeneratePerlinNoiseMap2X() {
		GeneratePerlinNoiseMap(2);
	}
	
	[MenuItem("材质工具/生成噪点图/Perlin 3X")]
	public static void GeneratePerlinNoiseMap3X() {
		GeneratePerlinNoiseMap(3);
	}
	
	[MenuItem("材质工具/生成噪点图/Perlin 4X")]
	public static void GeneratePerlinNoiseMap4X() {
		GeneratePerlinNoiseMap(4);
	}
	
	[MenuItem("材质工具/生成噪点图/Perlin 5X")]
	public static void GeneratePerlinNoiseMap5X() {
		GeneratePerlinNoiseMap(5);
	}
	
	[MenuItem("材质工具/生成噪点图/Perlin 6X")]
	public static void GeneratePerlinNoiseMap6X() {
		GeneratePerlinNoiseMap(6);
	}
	
	[MenuItem("材质工具/生成噪点图/Perlin 7X")]
	public static void GeneratePerlinNoiseMap7X() {
		GeneratePerlinNoiseMap(7);
	}
	
	[MenuItem("材质工具/生成噪点图/Perlin 8X")]
	public static void GeneratePerlinNoiseMap8X() {
		GeneratePerlinNoiseMap(8);
	}
	
	[MenuItem("材质工具/生成噪点图/Perlin 9X")]
	public static void GeneratePerlinNoiseMap9X() {
		GeneratePerlinNoiseMap(9);
	}
	
	public static void GeneratePerlinNoiseMap(float scale) {
		string path = EditorUtility.SaveFilePanelInProject("生成柏林噪点图", "perlin_noise_texture_" + scale + "x", "png", "保存", Application.dataPath + "/Resources/Textures");
		if (!string.IsNullOrEmpty(path)) {
			if (EditorUtility.DisplayCancelableProgressBar("生成柏林噪点图", "初始化", 0f)) {
				EditorUtility.ClearProgressBar();
				return;
			}
			int size = 256;
			int sizeSqr = size * size;
            Texture2D texture2D = new Texture2D(size, size);
			float oX = Random.value;
			float oY = Random.value;
			for (int i = 0; i < size; i++) {
				for (int j = 0; j < size; j++) {
					float greyScale = Mathf.PerlinNoise(oX + ((float) i) / ((float) size) * scale, ((float) j) / ((float) size) * scale);
					texture2D.SetPixel(i, j, new Color(greyScale, greyScale, greyScale));
					if (j % 100 == 0) {
						if (EditorUtility.DisplayCancelableProgressBar("生成柏林噪点图", greyScale.ToString(), (float) (size * i + j + 1) / sizeSqr)) {
							EditorUtility.ClearProgressBar();
							return;
						}
					}
				}
			}
			texture2D.Apply();
			File.WriteAllBytes(path, texture2D.EncodeToPNG());
			EditorUtility.ClearProgressBar();
			AssetDatabase.ImportAsset(path.Substring(path.IndexOf("Assets")));
		}
	}
}