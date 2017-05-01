using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNet;
using System.IO;

namespace PolyWorld {

	public class Chunk : PolyNetBehaviour {

		private ChunkIndex index;
		private HeightmapIndex start;
		private float[,] heightmap;
		private BlockID[,] blockMap;

		public void instantiate(ChunkIndex i, float[,] worldHeight, BlockID[,] worldBlock, int worldMapSize) {
			index = i;
			start = WorldTerrain.terrain.toHMI (index);
			transform.position = WorldTerrain.terrain.toPosition (index);
			refreshMaps (worldHeight, worldBlock, worldMapSize);
			refreshMesh ();
		}

		public override int getBehaviourSpawnSize() {
			return 50000;
		}

		public override void writeBehaviourSpawnData(ref BinaryWriter writer) {
			writer.Write(index.x);
			writer.Write(index.z);
			writer.Write(heightmap.GetLength (0));
			writer.Write(heightmap.GetLength (1));
			for (int z = 0; z < heightmap.GetLength (1); z++) {
				for (int x = 0; x < heightmap.GetLength (0); x++) {
					writer.Write ((decimal)heightmap [x, z]);
					writer.Write (blockMap [x, z].id1);
					writer.Write (blockMap [x, z].id2);
				}
			}
		}

		public override void readBehaviourSpawnData(ref BinaryReader reader) {
			index = new ChunkIndex (reader.ReadInt32 (), reader.ReadInt32 ());
			start = WorldTerrain.terrain.toHMI (index);
			int xw = reader.ReadInt32();
			int zw = reader.ReadInt32();
			heightmap = new float[xw, zw];
			blockMap = new BlockID[xw, zw];
			for (int z = 0; z < heightmap.GetLength (1); z++) {
				for (int x = 0; x < heightmap.GetLength (0); x++) {
					heightmap [x, z] = (float)reader.ReadDecimal ();
					BlockID b = new BlockID (reader.ReadInt32 (), reader.ReadInt32 ());
					blockMap [x, z] = b;
				}
			}
			refreshMesh ();
		}

		/*
		 * 
		 * Private
		 * 
		 */

		private void refreshMaps(float[,] worldHeight, BlockID[,] worldBlock, int worldMapSize) {
			int chunkSizeX = (WorldTerrain.terrain.chunkSize / WorldTerrain.terrain.resolution) + 1;
			int chunkSizeZ = chunkSizeX;
			if (start.x + chunkSizeX >= worldMapSize)
				chunkSizeX--;
			if (start.z + chunkSizeZ >= worldMapSize)
				chunkSizeZ--;
			heightmap = new float[chunkSizeX,chunkSizeZ];
			blockMap = new BlockID[chunkSizeX,chunkSizeZ];
			for (int z = 0; z < heightmap.GetLength (1); z++) {
				for (int x = 0; x < heightmap.GetLength (0); x++) {
					heightmap [x, z] = worldHeight [start.x + x, start.z + z];
					blockMap [x, z] = worldBlock [start.x + x, start.z + z];
				}
			}
		}

		private void refreshMesh() {
			Mesh mesh = new Mesh();

			List<Vector3> vertList = new List<Vector3>();
			List<Vector2> uvList = new List<Vector2>();
			List<int> triList = new List<int> ();
			List<Color> colorList = new List<Color> ();

			int i = 0;
			for (int zi = 0; zi < heightmap.GetLength(1); zi++) {
				for (int xi = 0; xi <  heightmap.GetLength(0); xi++) {
					if (xi + 1 <  heightmap.GetLength(0) && zi + 1 <  heightmap.GetLength(1)) {
						HeightmapIndex temp = new HeightmapIndex (xi, zi);

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
			mesh.colors = colorList.ToArray();
			mesh.RecalculateNormals();

			setMesh(mesh);

		}

		private void setMesh(Mesh m) {
			GetComponent<MeshFilter> ().mesh = m;
			GetComponent<MeshCollider> ().sharedMesh = m;
		}

		private BlockID getBlock(HeightmapIndex h) {
			if (!isInBounds (h))
				return new BlockID(0,0);
			return blockMap [h.x,h.z];
		}

		private HeightmapIndex toWorld(HeightmapIndex h) {
			return new HeightmapIndex (start.x + h.x, start.z + h.z);
		}

		private bool isInBounds(HeightmapIndex h) {
			if (h.x < heightmap.GetLowerBound(0) || h.z < heightmap.GetLowerBound(1) || h.x > heightmap.GetUpperBound(0) || h.z > heightmap.GetUpperBound(1))
				return false;
			else
				return true;
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
			HeightmapIndex temp = new HeightmapIndex (x, z);
			if (useOffset)
				vert.Add (toPosition(temp)+WorldTerrain.terrain.blockOffset);
			else
				vert.Add (toPosition(temp));
			uv.Add (new Vector2 (0, 0));
			tri.Add (i);
			if (c != Color.black)
				color.Add (c);
			else
				color.Add (WorldTerrain.terrain.getColor (toWorld(temp), heightmap[temp.x,temp.z]));
			i++;
		}

		private Vector3 toPosition(HeightmapIndex h) {
			Vector3 t = WorldTerrain.terrain.toPosition (toWorld (h), heightmap[h.x,h.z]);
			t.x -= transform.position.x;
			t.z -= transform.position.z;
			t.y -= transform.position.y;
			return t;
		}
		
	}

}