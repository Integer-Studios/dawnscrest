﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyNet {

	public class PolySaveManager {

		private static PolyNetManager.StartSequenceDelegate onComplete;
		private static int startSequenceId;
		private static PolyNetManager manager;

		public static void initialize(PolyNetManager m, PolyNetManager.StartSequenceDelegate del, int ssid) {
			manager = m;
			onComplete = del;
			startSequenceId = ssid;
			loadHeightmap ();
		}

		private static void loadHeightmap() {
			JSONObject jsonObj = new JSONObject (JSONObject.Type.OBJECT);
			jsonObj.AddField ("world", manager.worldID);
			PolyNodeHandler.sendRequest ("heightmap", jsonObj, onHeightmapData);
		}

		private static void onHeightmapData(JSONObject data) {
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

		private static void onTerrainGenerated() {
			finishLoad ();
		}

		private static void finishLoad() {
			Debug.Log ("Startup[" + startSequenceId + "]: World Successfully Loaded From Database.");
			onComplete (startSequenceId);
		}

	}

}
