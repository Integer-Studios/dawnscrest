using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNet;
using PolyPlayer;
using PolyEffects;
using System.IO;

namespace PolyWorld {

	public class WorldTerrain : PolyNetBehaviour {

		public delegate void TerrainGenerated();

		public string rawFile;
		public ByteOrder rawHeightMapOrder = ByteOrder.Windows;
		public int resolution = 1;
		public int size = 100;
		public int chunkSize = 10;
		public float height = 100f;
		public Color grass;
		public float grassPoint = 0.8f;
		public Color dirt;
		public float dirtPoint = 0.6f;
		public Color stone;
		public float stonePoint = 0.3f;
		public Color snow;
		public float snowPoint = 0.2f;
		public Color sand;
		public float elevationTempurature = 1f;
		public float latitudeTempurature = 1f;
		public float elevationHumidity = 1f;
		public float latitudeHumidity = 1f;
		public float humidityMultiplier = 1f;
		public float tempuratureMultiplier = 1f;
		public float renderDistanceTerrain = 100f;
		public float renderDistanceDecorations = 100f;
		public Block[] blockRegister;
		public Vector3 blockOffset;
		public GameObject chunkPrefab;

		public static WorldTerrain terrain;
		private float[,] heightmap;
		private BlockID[,] blockMap;
		public int heightmapSize;

		/*
		 * 
		 * Private
		 * 
		 */

		private void Start() {
			heightmapSize = (int)(size / resolution);
			terrain = this;
			Block.setBlocks (blockRegister);
			if (!PolyServer.isActive)
				return;
		}

		public void createTerrain(float[,] map, TerrainGenerated g) {
//			heightmap = map;
			generateHeightmap();
			generateBlockmap ();
			StartCoroutine (generateChunks (g));
		}

		private IEnumerator generateChunks(TerrainGenerated onTerrainGenerated) {
			int chunkLength = size / chunkSize;
			for (int z = 0; z < chunkLength; z++) {
				for (int x = 0; x < chunkLength; x++) {
					GameObject g = Instantiate (chunkPrefab);
					Chunk c = g.GetComponent<Chunk> ();
					c.instantiate (new ChunkIndex (x, z), heightmap, blockMap, heightmapSize);
//					PolyNetWorld.spawnObject (g);
					yield return null;
				}
			}
			onTerrainGenerated ();
		}

		// World Info

		private float getHeight(HeightmapIndex h) {
			if (!isInBounds (h))
				return 0f;
			else 
				return heightmap [h.x, h.z];
		}

		private float getHumidity(HeightmapIndex h) {
			float ele = (height - getHeight (h))/height;
			//south = 0 north = 1
			float lat = (float)(h.z) / (float)heightmapSize;
			ele *= elevationHumidity;
			lat *= latitudeHumidity;
			return humidityMultiplier* ((ele + lat)/(elevationHumidity + latitudeHumidity));
		}

		private float getHumidity(HeightmapIndex h, float indexHeight) {
			float ele = (height - indexHeight)/height;
			//south = 0 north = 1
			float lat = (float)(h.z) / (float)heightmapSize;
			ele *= elevationHumidity;
			lat *= latitudeHumidity;
			return humidityMultiplier* ((ele + lat)/(elevationHumidity + latitudeHumidity));
		}

		private float getTempurature(HeightmapIndex h) {
			return getTempurature(h, false);
		}

		private float getTempurature(HeightmapIndex h, bool time) {
			return getTempurature (h, time, getHeight (h));
		}

		private float getTempurature(HeightmapIndex h, bool time, float indexHeight) {
			float ele = (height - indexHeight)/height;

			//south = 1 north = 0
			float lat = (float)(heightmapSize - h.z) / (float)heightmapSize;
			ele *= elevationTempurature;
			lat *= latitudeTempurature;
			float raw = tempuratureMultiplier * ((ele + lat) / (elevationTempurature+latitudeTempurature));
			if (time)
				return raw * WorldTime.getTempuratureMultiple ();
			else
				return raw;
		}

		private BlockID getBlock(HeightmapIndex h) {
			if (!isInBounds (h))
				return new BlockID(0,0);
			return blockMap [h.x,h.z];
		}

//		private float getSteepness(HeightmapIndex h) {
//			if (!isInBounds (h))
//				return 0f;
//			float up = getHeight (new HeightmapIndex (h.x, h.z + 1));
//			float down = getHeight (new HeightmapIndex (h.x, h.z - 1));
//			float right = getHeight (new HeightmapIndex (h.x + 1, h.z));
//			float left = getHeight (new HeightmapIndex (h.x - 1, h.z));
//			float mid = getHeight (h);
//			float max = Mathf.Max (Mathf.Abs (mid - up), Mathf.Abs (mid - down));
//			max = Mathf.Max (max, Mathf.Abs (mid - left));
//			max = Mathf.Max (max, Mathf.Abs (mid - right));
//			return max;
//		}

		public Color getColor(HeightmapIndex h) {
			return getColor(h, getHeight(h));
		}

		public Color getColor(HeightmapIndex h, float indexHeight) {
			//get full humidity color
			float t = getTempurature(h, false, indexHeight);

			Color c;
			if (t > grassPoint) {
				c = grass;
				c = mix (sand,c, getHumidity (h, indexHeight));
			} else if (t > dirtPoint) {
				c = mix (dirt, grass, getGradientPoint (dirtPoint, grassPoint, t));
				c = mix (sand,c, getHumidity (h, indexHeight));
			} else if (t > stonePoint) {
				c = mix (stone, dirt, getGradientPoint (stonePoint, dirtPoint, t));
				c = mix (sand,c, getHumidity (h, indexHeight));
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

		public HeightmapIndex toHMI(ChunkIndex c) {
			HeightmapIndex h;
			h.x = (int) ((c.x * chunkSize) / resolution);
			h.z = (int) ((c.z * chunkSize) / resolution);
			return h;
		}

		private HeightmapIndex toHMI(Vector3 p) {
			HeightmapIndex h;
			h.x = (int)((p.x - transform.position.x) / resolution);
			h.z = (int)((p.z - transform.position.z) / resolution);
			return h;
		}

		private ChunkIndex toChunkIndex(Vector3 p) {
			HeightmapIndex h = toHMI (p);
			ChunkIndex c;
			c.x = (int)((h.x * resolution) / chunkSize);
			c.z = (int)((h.z * resolution) / chunkSize);
			return c;
		}

		private ChunkIndex toChunkIndex(HeightmapIndex h) {
			ChunkIndex c;
			c.x = (int)((h.x * resolution) / chunkSize);
			c.z = (int)((h.z * resolution) / chunkSize);
			return c;
		}

		public Vector3 toPosition(ChunkIndex c) {
			HeightmapIndex h = toHMI (c);
			return toPosition (h);
		}

		public Vector3 toPosition(HeightmapIndex h) {
			return new Vector3 (h.x * resolution + transform.position.x, heightmap [h.x, h.z] + transform.position.y, h.z * resolution + transform.position.z);
		}

		public Vector3 toPosition(HeightmapIndex h, float indexHeight) {
			return new Vector3 (h.x * resolution + transform.position.x, indexHeight + transform.position.y, h.z * resolution + transform.position.z);
		}

		private bool isInBounds(HeightmapIndex h) {
			if (h.x < heightmap.GetLowerBound(0) || h.z < heightmap.GetLowerBound(1) || h.x > heightmap.GetUpperBound(0) || h.z > heightmap.GetUpperBound(1))
				return false;
			else
				return true;
		}

		// Heightmap Helpers

		private void generateHeightmap() {
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
			heightmapSize = (int)(size / resolution);
			heightmap = new float[heightmapSize, heightmapSize];
			for (int zi = 0; zi < heightmapSize; ++zi) {
				for (int xi = 0; xi < heightmapSize; ++xi) {
					float u = ((float)xi) / ((float)heightmapSize);
					float v = ((float)zi) / ((float)heightmapSize);

					heightmap [xi, zi] = height * rawHeightmap [(int)(u * rawHeightMapSize), (int)(v * rawHeightMapSize)];
				}
			}
		}

		private void generateBlockmap() {
			blockMap = new BlockID[heightmapSize, heightmapSize];
			for (int zi = 0; zi < heightmapSize; zi++) {
				for (int xi = 0; xi < heightmapSize; xi++) {
					HeightmapIndex temp = new HeightmapIndex (xi, zi);
					if (getTempurature(temp, false) < snowPoint)
						blockMap [xi, zi] = new BlockID (1, 1);
					else
						blockMap [xi, zi] = new BlockID (0, 0);

				}
			}
		}

		private JSONObject heightmapToJSON(float[,] heightmapTemp) {
			JSONObject heightMapData = new JSONObject (JSONObject.Type.ARRAY);
			for (int zi = 0; zi < heightmapTemp.GetLength(0); ++zi) {
				for (int xi = 0; xi < heightmapTemp.GetLength(0); ++xi) {
					JSONObject heightIndex = new JSONObject (JSONObject.Type.OBJECT);
					heightIndex.AddField ("x", xi);
					heightIndex.AddField ("z", zi);
					heightIndex.AddField ("y", heightmapTemp[xi,zi]);
					heightMapData.Add (heightIndex);
				}
			}
			JSONObject heightData = new JSONObject (JSONObject.Type.OBJECT);
			heightData.AddField ("map", heightMapData);
			heightData.AddField ("size", heightmapTemp.GetLength(0));
			return heightData;
		}

		private void JSONtoHeightmap(JSONObject heightMapData) {
			heightmapSize = (int)heightMapData.GetField ("size").n;
			heightmap = new float[heightmapSize, heightmapSize];
			foreach (JSONObject heightIndex in heightMapData.GetField("map").list) {
				heightmap [(int)heightIndex.GetField ("x").n, (int)heightIndex.GetField ("z").n] = heightIndex.GetField ("y").n;
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

	public struct BlockID {
		public int id1;
		public int id2;
		public BlockID(int i1, int i2) {
			id1 = i1;
			id2 = i2;
		}
	};

	public enum ByteOrder {
		Mac,
		Windows,
	}

}