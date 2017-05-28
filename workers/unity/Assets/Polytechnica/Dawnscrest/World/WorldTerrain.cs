using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polytechnica.Dawnscrest.World {

	public class WorldTerrain : MonoBehaviour {

		public string rawFile;
		public ByteOrder rawHeightMapOrder = ByteOrder.Windows;
		public float height = 300f;

		public static WorldTerrain terrain;
		public static int resolution = 5;
		public static int size = 1400;
		public static int chunkSize = 50;

		private float[,] heightmap;

		private void OnEnable() {
			terrain = this;
		}

		public void LoadChunks() {
			Debug.LogWarning ("Loading Heightmap From Raw...");
			GenerateHeightmap ();
			Debug.LogWarning ("Successfully Loaded Heightmap Of Size: " + heightmap.Length + ". Refreshing Chunks...");
			Chunk[] chunks = FindObjectsOfType<Chunk> ();
			foreach (Chunk c in chunks) {
				c.LoadHeightmap (heightmap);
			}
			Debug.LogWarning ("Successfully Refreshed " + chunks.Length + " Chunks.");
		}

		public static HeightmapIndex ToHMI(ChunkIndex c) {
			HeightmapIndex h;
			h.x = (int) ((c.x * chunkSize) / resolution);
			h.z = (int) ((c.z * chunkSize) / resolution);
			return h;
		}

		public static Vector3 ToPosition(HeightmapIndex h) {
			Vector3 v = Vector3.zero;
			v.x = h.x * resolution - (size/2);
			v.z = h.z * resolution - (size/2);
			return v;
		}

		private void GenerateHeightmap() {
			var info = new System.IO.FileInfo(rawFile);
			int rawHeightMapSize = Mathf.RoundToInt(Mathf.Sqrt(info.Length / sizeof(System.UInt16)));

			var rawBytes = System.IO.File.ReadAllBytes(rawFile);
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

	}

	public struct HeightmapIndex {
		public int x;
		public int z;
		public HeightmapIndex(int xi, int zi) {
			x = xi;
			z = zi;
		}
	};

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