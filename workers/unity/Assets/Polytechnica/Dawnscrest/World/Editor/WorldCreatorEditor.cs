using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Polytechnica.Dawnscrest.Editor;
using Borodar.FarlandSkies.LowPoly;

namespace Polytechnica.Dawnscrest.World {

	[CustomEditor(typeof(WorldCreator))]
	public class WorldCreatorEditor : UnityEditor.Editor {

		public static float minX = 0;
		public static float maxX = 0;
		public static float minZ = 0;
		public static float maxZ = 0;

		public static float loadedMinX = 0;
		public static float loadedMaxX = 0;
		public static float loadedMinZ = 0;
		public static float loadedMaxZ = 0;

		public override void OnInspectorGUI() {

			DrawDefaultInspector();

			WorldCreator creator = (WorldCreator)target;

			SkyboxDayNightCycle.Instance.TimeOfDay = creator.editorTime;

			EditorGUILayout.LabelField("Chunk X Range: " + (int)minX + " to " + (int)maxX);
			EditorGUILayout.MinMaxSlider(ref minX, ref maxX, 0, WorldTerrain.size/WorldTerrain.chunkSize - 1);

			EditorGUILayout.LabelField("Chunk Z Range: " + (int)minZ + " to " + (int)maxZ);
			EditorGUILayout.MinMaxSlider(ref minZ, ref maxZ, 0, WorldTerrain.size/WorldTerrain.chunkSize - 1);

			if (GUILayout.Button ("Load")) {
				LoadObjects ();
			}

			if (GUILayout.Button ("Save")) {
				SaveObjects ();
			}

			if (GUILayout.Button ("Rip Savable Names")) {
				RipNames ();
			}

			if (GUILayout.Button ("Help")) {
				Help ();
			}
		}

		private void Help() {
			Debug.Log ("Entities: ");
			Debug.Log ("  Location: Assets/Resources/EntityPrefabs");
			Debug.Log ("  Label: Savable");
			Debug.Log ("  Component: Savable");
			Debug.Log ("  Setup Process:");
			Debug.Log ("    1. Place the entity prefab in the correct location");
			Debug.Log ("    2. Give it the correct Label and a blank Savable component");
			Debug.Log ("    3. Before using new entities, click 'Rip Savable Names'");
		}

		private void RipNames() {

			string[] paths = AssetDatabase.FindAssets ("l:Savable", null);
			foreach (string path in paths) {
				GameObject prefab = AssetDatabase.LoadAssetAtPath (AssetDatabase.GUIDToAssetPath(path), typeof(GameObject)) as GameObject;

				string name = GetEntityName (AssetDatabase.GetAssetPath (prefab));
				// Needs to be in the Entity Prefabs folder to spawn it
				if (name == null)
					continue;

				prefab.GetComponent<Savable> ().prefabName = name;
			}
		}

		private void LoadObjects() {

			// Save the actually loaded range
			loadedMinX = minX;
			loadedMaxX = maxX;
			loadedMinZ = minZ;
			loadedMaxZ = maxZ;

			int minXi = Mathf.RoundToInt(minX);
			int maxXi = Mathf.RoundToInt(maxX);
			int minZi = Mathf.RoundToInt(minZ);
			int maxZi = Mathf.RoundToInt(maxZ);

			// Load chunks in range
			// Load Objects in range
			WorldTerrain.GenerateHeightmap ();

			float progMax = (maxX-minX) * (maxZ - minZ);
			float prog = 0f;
			for (int z = minZi; z <= maxZi; z++) {
				for (int x = minXi; x <= maxXi; x++) {

					EditorUtility.DisplayProgressBar("Loading World Section", ("Building Chunk "+x+", "+z + " ("+prog+" of " + progMax + ")"), prog / progMax);

					// Load chunk heightmap into snapshot
					CreateChunk(x, z);

					//Load chunk objects from json
					string json = File.ReadAllText(WorldCreator.worldDirectory+x+"-"+z+".chunk");

					//Get object array
					WorldObjectChunk objectChunk = JsonUtility.FromJson<WorldObjectChunk>(json);
					foreach (WorldObject obj in objectChunk.objects) {
						//Add the entity
						CreateObject(obj);
					}

					prog++;
				}
			}
			EditorUtility.ClearProgressBar ();

		}

		private void CreateChunk(int x, int z) {
			GameObject g =  Instantiate (Resources.Load ("EntityPrefabs/TerrainChunk")) as GameObject;
			Vector3 c = Vector3.zero;
			c.x = WorldTerrain.chunkSize * x - (WorldTerrain.size/2);
			c.z = WorldTerrain.chunkSize * z - (WorldTerrain.size/2);
			g.transform.position = c;
			g.GetComponent<ChunkVisualizer> ().EditorVisualize (WorldTerrain.GetTerrainData (x, z));
		}

		private void CreateObject(WorldObject obj) {
			GameObject g =  Instantiate (Resources.Load ("EntityPrefabs/"+obj.name)) as GameObject;
			g.transform.position = obj.position;
			g.transform.eulerAngles = obj.rotation;
			g.transform.localScale = obj.scale;
		}
			
		private void SaveObjects() {

			// Reset the range to the actually loaded one to prevent fuck ups
			minX = loadedMinX;
			maxX = loadedMaxX;
			minZ = loadedMinZ;
			maxZ = loadedMaxZ;

			int minXi = Mathf.RoundToInt(minX);
			int maxXi = Mathf.RoundToInt(maxX);
			int minZi = Mathf.RoundToInt(minZ);
			int maxZi = Mathf.RoundToInt(maxZ);

			WorldObjectChunk[,] chunks = new WorldObjectChunk[maxXi-minXi+1, maxZi-minZi+1];
			for (int z = 0; z < chunks.GetLength (1); z++) {
				for (int x = 0; x < chunks.GetLength (0); x++) {
					chunks [x, z] = new WorldObjectChunk ();
				}
			}

			// Loop for all game objects in the scene
			Savable[] saves = FindObjectsOfType<Savable> ();
			foreach (Savable savable in saves) {
				GameObject obj = savable.gameObject;

				// Get Chunk Index
				ChunkIndex index = WorldTerrain.ToChunkIndex (obj.transform.position);
				// Check Chunk in range
				if (chunks.GetLength (0) <= index.x - minXi || index.x - minXi < 0 || chunks.GetLength (1) <= index.z - minZi || index.z - minZi < 0) {
					Debug.Log (savable.prefabName + " " + (index.x) + " " + (index.z));
					continue;
				}

				// We actually want to save this one

				// Make a world object
				WorldObject w = new WorldObject (savable.prefabName, savable.type, obj.transform.position, obj.transform.eulerAngles, obj.transform.localScale);
				// Add it to the correct chunk data (-min to shift to array index)
				chunks [index.x - minXi, index.z - minZi].objects.Add (w);
				//Destroy it
				DestroyImmediate (obj);

			}

			ChunkVisualizer[] chunksObjs = FindObjectsOfType<ChunkVisualizer> ();
			foreach (ChunkVisualizer c in chunksObjs) {
				DestroyImmediate(c.gameObject);
			}

			for (int z = 0; z < chunks.GetLength (1); z++) {
				for (int x = 0; x < chunks.GetLength (0); x++) {
					File.WriteAllText(WorldCreator.worldDirectory+(minXi+x)+"-"+(minZi+z)+".chunk", JsonUtility.ToJson (chunks [x, z]));
				}
			}

		}



		private string GetEntityName(string path) {
			
			string[] pieces = path.Split ('/');
			if (pieces.Length < 3)
				return null;
			if (pieces [1] != "Resources")
				return null;
			if (pieces [2] != "EntityPrefabs")
				return null;

			return path.Substring (31).Split('.')[0];
		}

	}

}
