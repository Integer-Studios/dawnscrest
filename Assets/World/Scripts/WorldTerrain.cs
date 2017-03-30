using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using PolyPlayer;

namespace PolyWorld {

	public class WorldTerrain : NetworkBehaviour {

		// Vars : public, protected, private, hide

		public string rawFile;
		public ByteOrder rawHeightMapOrder = ByteOrder.Windows;
		public float resolution = 1f;
		public int size = 100;
		public int chunkSize = 10;
		public float height = 100f;
		public GameObject chunkPrefab;
		public GameObject ocean;
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
		public bool setColors = true;
		public Transform spawnPoint;
		public Block[] blockRegister;
		public Vector3 blockOffset;
		public float renderDistUpdateRate = 4f;

		private Chunk[,] chunks;
		private int heightmapSize;
		private float[,] heightmap;
		private blockID[,] blockMap;
		private Player player;
		private static WorldTerrain terrain;

		// Syncvars

		/*
		 * 
		 * Editor Interface
		 * 
		 */

		public void editor_generateChunks() {
			if (!Application.isEditor)
				return;

			generateHeightmap ();
			generateBlockmap ();
			Block.setBlocks (blockRegister);
			if (size % chunkSize != 0) {
				Debug.Log ("Size must be divisible by chunk size!");
				return;
			}
			int chunksLength = size / chunkSize;
			chunks = new Chunk[chunksLength, chunksLength];
			for (int cz = 0; cz < chunks.GetLength(0); cz++) {
				for (int cx = 0; cx < chunks.GetLength(0); cx++) {
					chunkIndex ci = new chunkIndex (cx, cz);
					instantiateChunk (ci);
				}
			}

		}

		public void editor_chunkify() {
			if (!Application.isEditor)
				return;

			setupRuntimeData ();
			GameObject[] decorations = GameObject.FindGameObjectsWithTag ("decoration");
			foreach (GameObject d in decorations) {
				chunkIndex c = toChunkIndex (d.transform.position);
				Chunk chunk = chunks [c.x, c.z];
				d.transform.SetParent (chunk.transform);
			}
		}

		public void editor_destroyChunks() {
			if (!Application.isEditor)
				return;

			Chunk[] c = GetComponentsInChildren<Chunk> ();
			foreach (Chunk chunk in c) {
				DestroyImmediate (chunk.gameObject);
			}
		}

		/*
		 * 
		 * Public Interface
		 * 
		 */

		[Server]
		public static void setTerrainHeight(Vector3 p, float f) {
			terrain.setHeight(p,f);
		}

		public static Transform getSpawnPoint() {
			return terrain.spawnPoint;
		}

		public static float getTerrainHeight(Vector3 p) {
			return terrain.getHeight(p);
		}

		public static Vector3 toTerrainSurface(Vector3 p) {
			return terrain.toSurface (p);
		}

		public static float getTerrainTempurature(Vector3 p) {
			return getTerrainTempurature (p, true);
		}

		public static float getTerrainTempurature(Vector3 p, bool time) {
			return terrain.getTempurature(p, time);
		}

		public static float getTerrainHumidity(Vector3 p) {
			return terrain.getHumidity (p);
		}

		public static bool isUnderwater(Vector3 p) {
			return (getTerrainHeight (p) < terrain.ocean.transform.position.y);
		}

		[Server]
		public void setHeight(Vector3 p, float f) {
			setHeight(ToHMI(p),f);
		}

		public float getHeight(Vector3 p) {
			float f = getHeight(ToHMI (p));
//			RaycastHit hit;
//			Physics.Raycast (new Vector3(p.x,f,p.z), Vector3.down, out hit);
//			f -= hit.distance;
			return f;
		}

		public Vector3 toSurface(Vector3 p) {
			return new Vector3(p.x, getHeight(p), p.z);
		}

		public float getTempurature(Vector3 p) {
			return getTempurature (ToHMI (p), true);
		}

		public float getTempurature(Vector3 p, bool time) {
			return getTempurature (ToHMI (p), time);
		}

		public float getHumidity(Vector3 p) {
			return getHumidity (ToHMI (p));
		}

		public int getBlock(Vector3 p) {
			return getBlock (ToHMI (p)).id1;
		}

		public float getSteepness(Vector3 p) {
			return getSteepness (ToHMI (p));
		}
			
		public void initialize() {
			terrain = this;
			setupRuntimeData ();
			Block.setBlocks (blockRegister);
		}


		/*
		 * 
		 * Server->Client Networked Interface
		 * 
		 */

		[ClientRpc]
		private void RpcChunkUpdate(float[,] heightmap) {

		}

		/*
		 * 
		 * Private
		 * 
		 */

		private void Start () {
			initialize ();
			StartCoroutine (renderDistanceUpdate ());
		}

		private IEnumerator renderDistanceUpdate() {
			while (true) {
				updateChunks (Camera.main.transform.position);
				yield return new WaitForSeconds (renderDistUpdateRate);
			}
		}

		private float getHeight(heightmapIndex h) {
			if (!isInBounds (h))
				return 0f;
			return heightmap [h.x,h.z];
		}

		private void setHeight(heightmapIndex h, float f) {
			if (!isInBounds (h))
				return;

			heightmap [h.x, h.z] = f;

			int mod = (int)(chunkSize / resolution);
			chunkIndex c = toChunkIndex (h);

			if (h.x % mod == 0 && h.z % mod == 0) {
				regenChunk (new chunkIndex(c.x-1, c.z-1));
			}
			if (h.x % mod == 0) {
				regenChunk (new chunkIndex(c.x-1, c.z));
			}
			if (h.z % mod == 0) {
				regenChunk (new chunkIndex(c.x, c.z-1));
			}


			if (h.x % mod == mod-1 && h.z % mod == mod-1) {
				regenChunk (new chunkIndex(c.x+1, c.z+1));
			}
			if (h.x % mod == mod-1) {
				regenChunk (new chunkIndex(c.x+1, c.z));
			}
			if (h.z % mod == mod-1) {
				regenChunk (new chunkIndex(c.x, c.z+1));
			}

			regenChunk (c);
		}

		private float getHumidity(heightmapIndex h) {
			float ele = (height - getHeight (h))/height;
			//south = 0 north = 1
			float lat = (float)(h.z) / (float)heightmapSize;
			ele *= elevationHumidity;
			lat *= latitudeHumidity;
			return humidityMultiplier* ((ele + lat)/(elevationHumidity + latitudeHumidity));
		}

		private float getTempurature(heightmapIndex h) {
			return getTempurature(h, false);
		}

		private float getTempurature(heightmapIndex h, bool time) {
			float ele = (height - getHeight (h))/height;
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

		private blockID getBlock(heightmapIndex h) {
			if (!isInBounds (h))
				return new blockID(0,0);
			return blockMap [h.x,h.z];
		}

		private float getSteepness(heightmapIndex h) {
			if (!isInBounds (h))
				return 0f;
			float up = getHeight (new heightmapIndex (h.x, h.z + 1));
			float down = getHeight (new heightmapIndex (h.x, h.z - 1));
			float right = getHeight (new heightmapIndex (h.x + 1, h.z));
			float left = getHeight (new heightmapIndex (h.x - 1, h.z));
			float mid = getHeight (h);
			float max = Mathf.Max (Mathf.Abs (mid - up), Mathf.Abs (mid - down));
			max = Mathf.Max (max, Mathf.Abs (mid - left));
			max = Mathf.Max (max, Mathf.Abs (mid - right));
			return max;
		}

		private heightmapIndex ToHMI(chunkIndex c) {
			heightmapIndex h;
			h.x = (int) ((c.x * chunkSize) / resolution);
			h.z = (int) ((c.z * chunkSize) / resolution);
			return h;
		}

		private heightmapIndex ToHMI(Vector3 p) {
			heightmapIndex h;
			h.x = (int)((p.x - transform.position.x) / resolution);
			h.z = (int)((p.z - transform.position.z) / resolution);
			return h;
		}

		private chunkIndex toChunkIndex(Vector3 p) {
			heightmapIndex h = ToHMI (p);
			chunkIndex c;
			c.x = (int)((h.x * resolution) / chunkSize);
			c.z = (int)((h.z * resolution) / chunkSize);
			return c;
		}

		private chunkIndex toChunkIndex(heightmapIndex h) {
			chunkIndex c;
			c.x = (int)((h.x * resolution) / chunkSize);
			c.z = (int)((h.z * resolution) / chunkSize);
			return c;
		}

		private Vector3 toPosition(chunkIndex c) {
			heightmapIndex h = ToHMI (c);
			return toPosition (h);
		}

		private Vector3 toPosition(heightmapIndex h) {
			return new Vector3 (h.x * resolution + transform.position.x, heightmap [h.x, h.z] + transform.position.y, h.z * resolution + transform.position.z);
		}

		private void updateChunks(Vector3 p) {
			p.y = 0;
			if (chunks != null) {
				foreach (Chunk c in chunks) {
					c.updateActive (p, renderDistanceTerrain, renderDistanceDecorations);
				}
			}
		}

		private void regenChunk(chunkIndex c) {
			if (c.x < 0 || c.z < 0 || c.x > chunks.GetLength (0) || c.z > chunks.GetLength (0))
				return;

			Mesh m = generateChunk (c);
			chunks [c.x, c.z].setMesh (m);
		}

		private Mesh generateChunk(chunkIndex c) {
			Mesh mesh = new Mesh();

			List<Vector3> vertList = new List<Vector3>();
			List<Vector2> uvList = new List<Vector2>();
			List<int> triList = new List<int> ();
			List<Color> colorList = new List<Color> ();

			chunkIndex c2 = new chunkIndex(c.x + 1, c.z + 1);
			heightmapIndex start = ToHMI (c);
			heightmapIndex end = ToHMI (c2);
			int i = 0;

			for (int zi = start.z; zi < end.z; zi++) {
				for (int xi = start.x; xi < end.x; xi++) {
					if (xi + 1 < heightmapSize && zi + 1 < heightmapSize) {
						heightmapIndex temp = new heightmapIndex (xi, zi);

						if (getBlock (temp).id1 == 0)
							addTri(xi, zi, xi, zi+1, xi+1, zi, false, ref vertList, ref uvList, ref triList, ref colorList, ref i, Color.black);
						else
							addRaisedTri(xi, zi, xi, zi+1, xi+1, zi, ref vertList, ref uvList, ref triList, ref colorList, ref i, Block.getBlock(getBlock (temp).id1).color);

						if (getBlock (temp).id2 == 0) 
							addTri(xi, zi+1, xi+1, zi+1, xi+1, zi, false, ref vertList, ref uvList, ref triList, ref colorList, ref i, Color.black);
						 else 
							addRaisedTri(xi, zi+1, xi+1, zi+1, xi+1, zi, ref vertList, ref uvList, ref triList, ref colorList, ref i, Block.getBlock(getBlock (temp).id2).color);
					}

				}
			}

			mesh.vertices = vertList.ToArray();
			mesh.uv = uvList.ToArray();
			mesh.triangles = triList.ToArray();
			if (setColors)
				mesh.colors = colorList.ToArray();
			mesh.RecalculateNormals();

			return mesh;
		}

		private void addTri(int x1, int z1, int x2, int z2, int x3, int z3, bool raised, ref List<Vector3> vertList, ref List<Vector2> uvList, ref List<int> triList, ref List<Color> colorList,  ref int i, Color color) {
			addVertex(x1, z1, raised, ref vertList, ref uvList, ref triList, ref colorList, ref i, color);
			addVertex(x2, z2, raised, ref vertList, ref uvList, ref triList, ref colorList, ref i, color);
			addVertex(x3, z3, raised, ref vertList, ref uvList, ref triList, ref colorList, ref i, color);
		}

		private void addEdge(int x1, int z1, int x2, int z2, ref List<Vector3> vertList, ref List<Vector2> uvList, ref List<int> triList, ref List<Color> colorList,  ref int i, Color color) {
			addVertex(x1, z1, false, ref vertList, ref uvList, ref triList, ref colorList, ref i, color);
			addVertex(x2, z2, false, ref vertList, ref uvList, ref triList, ref colorList, ref i, color);
			addVertex(x1, z1, true, ref vertList, ref uvList, ref triList, ref colorList, ref i, color);

			addVertex(x2, z2, false, ref vertList, ref uvList, ref triList, ref colorList, ref i, color);
			addVertex(x2, z2, true, ref vertList, ref uvList, ref triList, ref colorList, ref i, color);
			addVertex(x1, z1, true, ref vertList, ref uvList, ref triList, ref colorList, ref i, color);
		}

		private void addRaisedTri(int x1, int z1, int x2, int z2, int x3, int z3, ref List<Vector3> vertList, ref List<Vector2> uvList, ref List<int> triList, ref List<Color> colorList,  ref int i, Color color) {
			//raised tri 1
			addTri(x1, z1, x2, z2, x3, z3, true, ref vertList, ref uvList, ref triList, ref colorList, ref i, color);

			//edges
			addEdge(x1, z1, x2, z2, ref vertList, ref uvList, ref triList, ref colorList, ref i, color);
			addEdge(x2, z2, x3, z3, ref vertList, ref uvList, ref triList, ref colorList, ref i, color);
			addEdge(x3, z3, x1, z1, ref vertList, ref uvList, ref triList, ref colorList, ref i, color);
		}

		private void addVertex(int x, int z, bool useOffset, ref List<Vector3> vert, ref List<Vector2> uv, ref List<int> tri, ref List<Color> color,  ref int i, Color c) {
			heightmapIndex temp = new heightmapIndex (x, z);
			if (useOffset)
				vert.Add (toPosition (temp)+blockOffset);
			else
				vert.Add (toPosition (temp));
			uv.Add (new Vector2 (0, 0));
			tri.Add (i);
			if (c != Color.black)
				color.Add (c);
			else
				color.Add (getColor (temp));
			i++;
		}

		private void instantiateChunk(chunkIndex ci) {
			GameObject c = Instantiate (chunkPrefab);
			c.transform.position = Vector3.zero;
			c.transform.rotation = Quaternion.identity;
			Mesh m = generateChunk (ci);
			c.GetComponent<MeshFilter> ().mesh = m;
			c.GetComponent<MeshCollider> ().sharedMesh = m;
			GameObject p = new GameObject ();
			p.name = "Chunk " + ci.x + "," + ci.z;
			c.transform.SetParent (p.transform);
			p.AddComponent<Chunk> ();
			p.GetComponent<Chunk> ().initialize(new Vector3 (chunkSize * ci.x + (chunkSize / 2) + transform.position.x, 0, chunkSize * ci.z + (chunkSize / 2) + transform.position.z), c);
			p.transform.SetParent (transform);
		}

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
			blockMap = new blockID[heightmapSize, heightmapSize];
			for (int zi = 0; zi < heightmapSize; zi++) {
				for (int xi = 0; xi < heightmapSize; xi++) {
					heightmapIndex temp = new heightmapIndex (xi, zi);
					if (getTempurature(temp, false) < snowPoint)
						blockMap [xi, zi] = new blockID (1, 1);
					else
						blockMap [xi, zi] = new blockID (0, 0);

				}
			}
		}

		public void setupRuntimeData() {
			generateHeightmap ();
			generateBlockmap ();
			//(int)x, (int)y, (float)height, (int)id1, (int)id2
			int chunksLength = size / chunkSize;
			chunks = new Chunk[chunksLength, chunksLength];
			Chunk[] c = GetComponentsInChildren<Chunk> ();
			int cx = 0;
			int cz = 0;
			foreach (Chunk chunk in c) {
				chunks [cx, cz] = chunk;
				cx++;
				if (cx == chunksLength) {
					cx = 0;
					cz++;
				}
			}
		}

		private bool isInBounds(heightmapIndex h) {
			if (h.x < 0 || h.z < 0 || h.x > heightmapSize || h.z > heightmapSize)
				return false;
			else
				return true;
		}

		private Color getColor(heightmapIndex h) {
			//get full humidity color
			float t = getTempurature(h, false);

			Color c;
			if (t > grassPoint) {
				c = grass;
				c = mix (sand,c, getHumidity (h));
			} else if (t > dirtPoint) {
				c = mix (dirt, grass, getGradientPoint (dirtPoint, grassPoint, t));
				c = mix (sand,c, getHumidity (h));
			} else if (t > stonePoint) {
				c = mix (stone, dirt, getGradientPoint (stonePoint, dirtPoint, t));
				c = mix (sand,c, getHumidity (h));
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


		/*
	 * 
	 * Types
	 * 
	 * 
	*/

		public enum ByteOrder {
			Mac,
			Windows,
		}

		public struct heightmapIndex {
			public int x;
			public int z;
			public heightmapIndex(int xi, int zi) {
				x = xi;
				z = zi;
			}
		};

		public struct chunkIndex {
			public int x;
			public int z;
			public chunkIndex(int cx, int cz) {
				x = cx;
				z = cz;
			}
		};

		public struct blockID {
			public int id1;
			public int id2;
			public blockID(int i1, int i2) {
				id1 = i1;
				id2 = i2;
			}
		};

	}

}