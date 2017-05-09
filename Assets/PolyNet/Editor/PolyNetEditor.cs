using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using PolyWorld;
using PolyNet;
using System.Text.RegularExpressions;

[CustomEditor(typeof(PolyNetManager))]
public class PolyNetEditor : Editor {
	PolyNetManager manager;
	public ByteOrder rawHeightMapOrder = ByteOrder.Windows;
	public int resolution = 4;
	public int size = 4096;
	public float height = 600f;
	public int maxChunkX;


	private float[,] heightmap;
	private BlockID[,] blockMap;
	public int heightmapSize;
	public override void OnInspectorGUI() {

		DrawDefaultInspector();
		manager = (PolyNetManager)target;
		if (GUILayout.Button ("Rip Heightmap")) {
			ripHeightmap ();
		}
//		if (GUILayout.Button ("Load Heightmap")) {
//			loadHeightmap();
//		}
		if (GUILayout.Button ("Load Objects")) {
			clearObjects ();
			loadHeightmap ();
			loadObjects ();
		}
		if (GUILayout.Button ("Save Objects")) {
			saveObjects ();
		}

		EditorGUILayout.BeginHorizontal();

		EditorGUILayout.TextField("Raw-16 Heightmap", System.IO.Path.GetFileName(manager.rawFile));
		if (GUILayout.Button("Select", EditorStyles.miniButton, GUILayout.Width(45.0f)))
		{
			manager.rawFile = EditorUtility.OpenFilePanelWithFilters("Select Raw file", manager.rawFile, new string[] { "16-bit Raw", "r16,raw", "Allfiles", "*" });

		}
		EditorGUILayout.EndHorizontal();
	}

	public void clearObjects() {

		PolyNetIdentity[] identities = PolyNetManager.FindObjectsOfType<PolyNetIdentity> ();

		foreach (PolyNetIdentity i in identities) {

			PolyNetManager.DestroyImmediate (i.gameObject);
		}

	}

	public void ripHeightmap() {
		Debug.Log ("Beginning Heightmap Rip...");
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
		Debug.Log ("Sending Heightmap Rip...");

		string url = "http://server.integerstudios.com:4206/heightmap/save";
		WWW www = new WWW(url, form);
		ContinuationManager.Add(() => www.isDone, () => {
			if (!string.IsNullOrEmpty(www.error)) {
				Debug.Log("WWW failed: " + www.error);
			} else {
				Debug.Log ("Heightmap Rip Completed : " + www.text);

			}
		});
	}

	public WWWForm generateWorldForm(int multiplier) {
		WWWForm form = new WWWForm ();
		form.AddField ("world", manager.worldID);
		form.AddField ("limit", "" +manager.limit);

		form.AddField ("x-max", "" + (manager.xMax * multiplier));
		form.AddField ("x-min",  "" + (manager.xMin * multiplier));
		form.AddField ("z-max",  "" + (manager.zMax * multiplier)); 
		form.AddField ("z-min",  "" + (manager.zMin * multiplier));
		return form;
	}

	public void loadHeightmap() {
		Debug.Log ("Requesting Heightmap...");
	
		WWWForm form = generateWorldForm (64 / 4);

		string url = "http://cdn.polytechni.ca/heightmap.php";
		WWW www = new WWW(url, form);
		ContinuationManager.Add(() => www.isDone, () => {
			if (!string.IsNullOrEmpty(www.error)) {
				Debug.Log("WWW failed: " + www.error);
			} else {
				Debug.Log ("Heightmap Load Completed ");
				string text = Regex.Replace(www.text, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");

					JSONObject jsonObj = new JSONObject(text);
				JSONObject mapObj = jsonObj.GetField("map");
//				JSONObject mapObj = JSONObject.Create (jsonObj.GetField("map").str, 2, true, true);
				if (mapObj.IsArray) {
				int heightmapSize = (int)(size / resolution);
				float[,] heightmap = new float[heightmapSize, heightmapSize];
				foreach (JSONObject ind in mapObj.list) {
					int x = (int)ind.list [0].n;
					int z = (int)ind.list [1].n;
					float height = ind.list [2].n;
					heightmap [x, z] = height;
				}
				PolyNetManager.FindObjectOfType<WorldTerrain> ().createEditorTerrain (heightmap, new PolyWorld.ChunkIndex(manager.xMin, manager.zMin), new PolyWorld.ChunkIndex(manager.xMax, manager.zMax));
				} else {
					Debug.Log("Empty Heightmap returned.");
				}
			} 
		});

	}

	public void saveObjects() {
		Debug.Log ("Saving Objects...");

		PolyNetIdentity[] identities = PolyNetManager.FindObjectsOfType<PolyNetIdentity> ();
		JSONObject arr = new JSONObject (JSONObject.Type.ARRAY);

		foreach (PolyNetIdentity i in identities) {

			if (i.isSaveable) {
				JSONObject o = i.writeSaveData ();
				arr.Add (o);
			}
		}

		WWWForm form = generateWorldForm (64);


		form.AddField ("objects", arr.ToString ());
		string url = "http://server.integerstudios.com:4206/objects/save";
		WWW www = new WWW(url, form);
		ContinuationManager.Add(() => www.isDone, () => {
			if (!string.IsNullOrEmpty(www.error)) {
				Debug.Log("WWW failed: " + www.error);
			} else {
				Debug.Log ("Object Save Completed : " + www.text);
				clearObjects ();

				loadHeightmap();
				loadObjects();
			}
		});

	}

	public void loadObjects() {
		Debug.Log ("Requesting Objects...");

		WWWForm form = generateWorldForm (64);

		string url = "http://cdn.polytechni.ca/objects.php";
		WWW www = new WWW(url, form);
		ContinuationManager.Add(() => www.isDone, () => {
			if (!string.IsNullOrEmpty(www.error)) {
				Debug.Log("WWW failed: " + www.error);
			} else {
				string text = Regex.Replace(www.text, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");

				JSONObject jsonObj = new JSONObject(text);

				Debug.Log("Received Objects: " + jsonObj.list.Count);

				foreach (JSONObject objJSON in jsonObj.list) {
					int p = (int)objJSON.GetField ("prefab").n;
					int id = (int)objJSON.GetField ("id").n;
			

					GameObject obj = GameObject.Instantiate (manager.GetComponent<PrefabRegistry>().prefabs [p].gameObject, JSONHelper.unwrap(objJSON, "position"), Quaternion.Euler(JSONHelper.unwrap(objJSON, "rotation")));

					PolyNetIdentity i = obj.GetComponent<PolyNetIdentity> ();
					i.initialize (id);
					i.prefabId = p;
				}
			}
		});
	}

}
