using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using PolyWorld;
using PolyNet;

[CustomEditor(typeof(PolyNetManager))]
public class PolyNetEditor : Editor {
	PolyNetManager manager;
	public ByteOrder rawHeightMapOrder = ByteOrder.Windows;
	public int resolution = 4;
	public int size = 2048;
	public float height = 200f;
	private float[,] heightmap;
	private BlockID[,] blockMap;
	public int heightmapSize;
	public override void OnInspectorGUI() {

		DrawDefaultInspector();
		manager = (PolyNetManager)target;
		if (GUILayout.Button ("Rip Heightmap")) {
			ripHeightmap ();
		}
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.TextField("Raw-16 Heightmap", System.IO.Path.GetFileName(manager.rawFile));
		if (GUILayout.Button("Select", EditorStyles.miniButton, GUILayout.Width(45.0f)))
		{
			manager.rawFile = EditorUtility.OpenFilePanelWithFilters("Select Raw file", manager.rawFile, new string[] { "16-bit Raw", "r16,raw", "Allfiles", "*" });

		}
		EditorGUILayout.EndHorizontal();
	}

	public void ripHeightmap() {
		var info = new System.IO.FileInfo(manager.rawFile);
		int rawHeightMapSize = Mathf.RoundToInt(Mathf.Sqrt(info.Length / sizeof(System.UInt16)));

		var rawBytes = System.IO.File.ReadAllBytes(manager.rawFile);
		bool reverseBytes = System.BitConverter.IsLittleEndian == (rawHeightMapOrder == ByteOrder.Mac);
		float [,]rawHeightmap = new float[rawHeightMapSize, rawHeightMapSize];
		int index = 0;

		for (int v = 0; v < rawHeightMapSize; ++v) {
			for (int u = 0; u < rawHeightMapSize; ++u) {
				if (reverseBytes) {
					System.Array.Reverse(rawBytes, index, sizeof(System.UInt16));
				}
				rawHeightmap [u, v] = (float)System.BitConverter.ToUInt16(rawBytes, index) / System.UInt16.MaxValue;
				index += sizeof(System.UInt16);
			}
		}
		heightmapSize = (int)(size / resolution);
		heightmap = new float[heightmapSize, heightmapSize];
		for (int zi = 0; zi < heightmapSize; ++zi) {
			for (int xi = 0; xi < heightmapSize; ++xi) {
				float u = ((float)xi) / ((float)heightmapSize);
				float v = ((float)zi) / ((float)heightmapSize);
				heightmap [xi, zi] = height * rawHeightmap [(int)(u * rawHeightMapSize), (int)(v * rawHeightMapSize)];
			}
		}
		JSONObject heightmapJSON = new JSONObject (JSONObject.Type.ARRAY);
		for (int zi = 0; zi < heightmapSize; ++zi) {
			for (int xi = 0; xi < heightmapSize; ++xi) {
				JSONObject heightJSON = new JSONObject (JSONObject.Type.OBJECT);
				heightJSON.AddField ("x", xi);
				heightJSON.AddField ("z", zi);
				heightJSON.AddField ("height", heightmap [xi, zi]);
				heightmapJSON.Add (heightJSON);
			}
		}
		WWWForm form = new WWWForm ();
		form.AddField ("heightmap", heightmapJSON.ToString());
		form.AddField ("world", manager.worldID);

		string url = "http://server.integerstudios.com:4206/heightmap/save";
		WWW www = new WWW(url, form);
		ContinuationManager.Add(() => www.isDone, () => {
			if (!string.IsNullOrEmpty(www.error)) {
				Debug.Log("WWW failed: " + www.error);
			} else {
				Debug.Log("WWW result : " + www.text);
			}
		});
	}

}
