using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNet;
using PolyPlayer;
using PolyEffects;
using System.IO;

namespace PolyWorld {

	public class WorldTerrain : PolyNetBehaviour {

		public string rawFile;
		public ByteOrder rawHeightMapOrder = ByteOrder.Windows;
		public float resolution = 1f;
		public int size = 100;
		public int chunkSize = 10;
		public float height = 100f;
		public GameObject chunkPrefab;

		public static float[,] heightmap;

		public void editor_generateJSON() {
			if (!Application.isEditor)
				return;
			var info = new System.IO.FileInfo(rawFile);
			int rawHeightMapSize = Mathf.RoundToInt(Mathf.Sqrt(info.Length / sizeof(System.UInt16)));

			var rawBytes = System.IO.File.ReadAllBytes(rawFile);
			bool reverseBytes = System.BitConverter.IsLittleEndian == (rawHeightMapOrder == ByteOrder.Mac);
			float [,]rawHeightmap = new float[rawHeightMapSize, rawHeightMapSize];
			int index = 0;

			JSONObject heightMapData = new JSONObject (JSONObject.Type.ARRAY);
			for (int v = 0; v < rawHeightMapSize; ++v) {
				for (int u = 0; u < rawHeightMapSize; ++u) {
					if (reverseBytes) {
						System.Array.Reverse(rawBytes, index, sizeof(System.UInt16));
					}
					JSONObject heightIndex = new JSONObject (JSONObject.Type.OBJECT);
					heightIndex.AddField ("u", u);
					heightIndex.AddField ("v", v);
					heightIndex.AddField ("h", (float)System.BitConverter.ToUInt16(rawBytes, index) / System.UInt16.MaxValue);
					index += sizeof(System.UInt16);
				}
			}
			JSONObject heightData = new JSONObject (JSONObject.Type.OBJECT);
			heightData.AddField ("map", heightMapData);
			heightData.AddField ("size", rawHeightMapSize);
			string dir = "Assets/Resources/JSON/";

			File.WriteAllText(dir + "heightmap.json", heightData.ToString());
		}

		public void editor_generateChunks() {
			loadHeightmap ();
			generateChunks ();
		}

		private void loadHeightmap() {
			string data = Resources.Load ("JSON/heightmap").ToString();

			JSONObject heightMapData = new JSONObject (data);
			int rawHeightMapSize = (int)heightMapData.GetField ("size").n;
			float[,] rawHeightmap = new float[rawHeightMapSize, rawHeightMapSize];
			foreach (JSONObject heightIndex in heightMapData.GetField("map").list) {
				rawHeightmap [(int)heightIndex.GetField ("u").n, (int)heightIndex.GetField ("v").n] = heightIndex.GetField ("h").n;
			}

			int heightmapSize = (int)(size / resolution);
			heightmap = new float[heightmapSize, heightmapSize];
			for (int zi = 0; zi < heightmapSize; ++zi) {
				for (int xi = 0; xi < heightmapSize; ++xi) {
					float u = ((float)xi) / ((float)heightmapSize);
					float v = ((float)zi) / ((float)heightmapSize);
					heightmap [xi, zi] = height * rawHeightmap [(int)(u * rawHeightMapSize), (int)(v * rawHeightMapSize)];
				}
			}
		}
			
		private void generateChunks() {
			if (!validateMetrics ()) {
				Debug.Log ("Invalid World Metrics");
				return;
			}

			int chunkAmount = size / chunkSize;
			for (int z = 0; z < chunkAmount; z++) {
				for (int x = 0; x < chunkAmount; x++) {
					ChunkIndex i = new ChunkIndex (x, z);
					GameObject c = Instantiate (chunkPrefab);
					Chunk chunk = c.GetComponent<Chunk> ();
					chunk.instantiate (i);
				}
			}
		}

		private bool validateMetrics() {
			return size % chunkSize == 0 && chunkSize % resolution == 0;
		}

	}

	public struct ChunkIndex {
		public int x;
		public int z;
		public ChunkIndex(int cx, int cz) {
			x = cx;
			z = cz;
		}
	};

	public enum ByteOrder {
		Mac,
		Windows,
	}

}