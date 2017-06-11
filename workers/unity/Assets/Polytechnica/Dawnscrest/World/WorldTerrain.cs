using Improbable.Collections;
using UnityEngine;

namespace Polytechnica.Dawnscrest.World {

	public class WorldTerrain : MonoBehaviour {

		public Vector3 climateMin;
		public Vector3 climateMax;

		public Color grass;
		public float grassPoint = 0.8f;
		public Color dirt;
		public float dirtPoint = 0.6f;
		public Color stone;
		public float stonePoint = 0.3f;
		public Color snow;
		public float snowPoint = 0.2f;
		public Color sand;
		public float humidityOffset = 1f;

		public static WorldTerrain terrain;
		public static int resolution = 5;
		public static int size = 1400;
		public static int chunkSize = 50;
		public static ByteOrder rawHeightMapOrder = ByteOrder.Windows;
		public static float height = 300f;

		private static float[,] heightmap;

		/*
		 * Static Gen
		 */

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

		public static ChunkIndex ToChunkIndex(Vector3 pos) {
			ChunkIndex i = new ChunkIndex ();
			i.x = (int) ((pos.x + (size/2)) / chunkSize);
			i.z = (int) ((pos.z + (size/2)) / chunkSize);
			return i;
		}

		public static void GenerateHeightmap() {
			// Usually if someone is generating the heightmap it's just safer to have this reference working
			if (terrain == null)
				terrain = FindObjectOfType<WorldTerrain> ();
			
			Debug.Log ("Loading Heightmap From Raw...");
			var info = new System.IO.FileInfo(WorldCreator.worldDirectory+"terrain.raw");
			int rawHeightMapSize = Mathf.RoundToInt(Mathf.Sqrt(info.Length / sizeof(System.UInt16)));

			var rawBytes = System.IO.File.ReadAllBytes(WorldCreator.worldDirectory+"terrain.raw");
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
			Debug.Log ("Successfully Loaded Heightmap Of Size: " + heightmap.Length + ". Refreshing Chunks...");
		}

		public static TerrainChunk.Data GetTerrainData(int x, int z) {
			ChunkIndex index = new ChunkIndex (x, z);
			float[,] map = GetChunkHeightmap (index);
			return new TerrainChunk.Data(WrapHeightmap (GetChunkHeightmap (index)),x,z,(uint)map.GetLength(0),(uint)map.GetLength(1));
		}

		public static float[,] GetChunkHeightmap(ChunkIndex i) {
			HeightmapIndex start = ToHMI (i);
			int chunkSizeX = (WorldTerrain.chunkSize / WorldTerrain.resolution) + 1;
			int chunkSizeZ = chunkSizeX;
			if (start.x + chunkSizeX >= heightmap.GetLength(0))
				chunkSizeX--;
			if (start.z + chunkSizeZ >= heightmap.GetLength(1))
				chunkSizeZ--;

			float[,] chunkHeightmap = new float[chunkSizeX,chunkSizeZ];
			for (int z = 0; z < chunkHeightmap.GetLength (1); z++) {
				for (int x = 0; x < chunkHeightmap.GetLength (0); x++) {
					chunkHeightmap [x, z] = heightmap [start.x + x, start.z + z];
				}
			}
			return chunkHeightmap;
		}

		public static List<float> WrapHeightmap(float[,] map) {
			List<float> l = new List<float> ();
			for (int z = 0; z < map.GetLength (1); z++) {
				for (int x = 0; x < map.GetLength (0); x++) {
					l.Add (map [x, z]);
				}
			}
			return l;
		}

		/*
		 * Client & Server Gen Helpers
		 */

		private void OnEnable() {
			terrain = this;
		}

		public float GetTempurature(Vector3 pos) {
			// inverse
			return (pos.y - climateMax.y) / (climateMin.y - climateMax.y);
		}

		public float GetHumidity(Vector3 pos) {
			return (pos.z - climateMax.z) / (climateMin.z - climateMax.z);
		}

		public Color GetColor(Vector3 pos) {
			//get full humidity color
			float t = GetTempurature(pos);

			Color c;
			if (t > grassPoint) {
				c = grass;
				c = mix (sand,c, GetHumidity (pos)+humidityOffset);
			} else if (t > dirtPoint) {
				c = mix (dirt, grass, getGradientPoint (dirtPoint, grassPoint, t));
				c = mix (sand,c, GetHumidity (pos)+humidityOffset);
			} else if (t > stonePoint) {
				c = mix (stone, dirt, getGradientPoint (stonePoint, dirtPoint, t));
				c = mix (sand,c, GetHumidity (pos)+humidityOffset);
			} else if (t > snowPoint) {
				c = mix (snow, stone, getGradientPoint (snowPoint, stonePoint, t));
			} else
				c = snow;

			return c;
		}

		private float getGradientPoint(float p1, float p2, float ptarget) {
			p2 -= p1;
			ptarget -= p1;
			return (ptarget / p2);
		}

		private Color mix(Color c1, Color c2, float p) {
			if (p < 0f)
				return c1;
			if (p > 1f)
				return c2;

			Color delta = c2 - c1;
			return c1 + (p * delta);
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