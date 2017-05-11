using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyNet {

	public class PolySaveManager {

		private static PolyNetManager.StartSequenceDelegate onComplete;
		private static int startSequenceId;
		private static PolyNetManager manager;
		private static PrefabRegistry registry; 
		private static Dictionary<int, GameObject> prefabs = new Dictionary<int, GameObject>();

		public static void initialize(PolyNetManager m, PolyNetManager.StartSequenceDelegate del, int ssid) {
			manager = m;
			onComplete = del;
			startSequenceId = ssid;
			registry = m.GetComponent<PrefabRegistry> ();
//			PolyWorld.WorldTerrain.terrain.createTerrain (null, onTerrainGenerated);
			loadHeightmap ();
		}

		private static void loadHeightmap() {
			Debug.Log ("Startup[" + startSequenceId + "]: Requesting Heightmap...");
			JSONObject jsonObj = new JSONObject (JSONObject.Type.OBJECT);
			jsonObj.AddField ("world", manager.worldID);
			PolyNodeHandler.sendRequest ("heightmap", jsonObj, onHeightmapData);
		}

		private static void onHeightmapData(JSONObject data) {
			Debug.Log ("Startup[" + startSequenceId + "]: Heightmap Successfully Received, Generating....");
			JSONObject mapObj = JSONObject.Create (data.GetField("map").str, 1, true, true);
			int size = PolyWorld.WorldTerrain.terrain.size;
			int resolution = PolyWorld.WorldTerrain.terrain.resolution;
			int heightmapSize = (int)(size / resolution);
			float[,] heightmap = new float[heightmapSize, heightmapSize];
			foreach (JSONObject ind in mapObj.list) {
				int x = (int)ind.list [0].n;
				int z = (int)ind.list [1].n;
				float height = ind.list [2].n;
				heightmap [x, z] = height;
			}
			PolyWorld.WorldTerrain.terrain.createTerrain (heightmap, onTerrainGenerated);
		}

		private static void loadObjects() {
			ripPrefabs ();
			JSONObject jsonObj = new JSONObject (JSONObject.Type.OBJECT);
			jsonObj.AddField ("world", manager.worldID);
			PolyNodeHandler.sendRequest ("objects", jsonObj, onObjectsData);
		}

		private static void onObjectsData(JSONObject data) {
			Debug.Log ("Startup[" + startSequenceId + "]: Objects Received, Instantiating Prefabs...");
			foreach (JSONObject objJSON in data.list) {
				int p = (int)objJSON.GetField ("prefab").n;
				int id = (int)objJSON.GetField ("id").n;

				GameObject pre;
				if (prefabs.TryGetValue (p, out pre)) {
					GameObject obj = GameObject.Instantiate (pre, JSONHelper.unwrap (objJSON, "position"), Quaternion.Euler (JSONHelper.unwrap (objJSON, "rotation")));
					PolyNetIdentity i = obj.GetComponent<PolyNetIdentity> ();
					i.initialize (id);
					i.prefabId = p;
				} else {
					Debug.Log ("Unrecognized Prefab ID: " + p + " Skipping Spawn");
				}
			}
			finishLoad ();
		}

		public static void ripPrefabs() {
			PrefabRegistry registry = GameObject.FindObjectOfType<PrefabRegistry> ();
			foreach (PolyNetIdentity g in registry.prefabs) {
				prefabs.Add (g.prefabId, g.gameObject);
			}
		}

		private static void onTerrainGenerated() {
			Debug.Log ("Startup[" + startSequenceId + "]: Heightmap Successfully Generated, Requesting Objects...");
			loadObjects ();
		}

		private static void finishLoad() {
			Debug.Log ("Startup[" + startSequenceId + "]: World Successfully Loaded From Database.");
			onComplete (startSequenceId);
		}

	}

}
