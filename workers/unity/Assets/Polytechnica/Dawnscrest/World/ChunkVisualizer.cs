using Improbable.Collections;
using UnityEngine;
using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Visualizer;
using Polytechnica.Dawnscrest.Core;

namespace Polytechnica.Dawnscrest.World {


	public class ChunkVisualizer : MonoBehaviour {

		[Require] private TerrainChunk.Reader chunkReader;
		[Require] private WorldTransform.Reader worldTransformReader;

		private HeightmapIndex start;
		private ChunkIndex index;
		private float[,] heightmap;

		private void OnEnable() {
			
			transform.position = worldTransformReader.Data.position.ToVector3();
			transform.eulerAngles = MathHelper.toVector3(worldTransformReader.Data.rotation);
			transform.localScale = MathHelper.toVector3 (worldTransformReader.Data.scale);

			chunkReader.ComponentUpdated += OnChunkUpdate;

			index = new ChunkIndex (chunkReader.Data.x, chunkReader.Data.z);
			start = WorldTerrain.ToHMI (index);
			if (chunkReader.Data.heightmap.Count > 0) {
				UnwrapHeightmap ((int)chunkReader.Data.sizeX, (int)chunkReader.Data.sizeZ, chunkReader.Data.heightmap);
				refreshMesh ();
			}
		}

		private void OnChunkUpdate(TerrainChunk.Update update) {
			if (update.heightmap.Value.Count > 0) {
				UnwrapHeightmap ((int)chunkReader.Data.sizeX, (int)chunkReader.Data.sizeZ, update.heightmap.Value);
				refreshMesh ();
			}
		}
			
		private void UnwrapHeightmap(int sizeX, int sizeZ, List<float> list) {
			heightmap = new float[sizeX, sizeZ];
			float[] arr = list.ToArray ();
			int i = 0;
			for (int z = 0; z < heightmap.GetLength (1); z++) {
				for (int x = 0; x < heightmap.GetLength (0); x++) {
					heightmap [x, z] = arr [i];
					i++;
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

						addTri(xi, zi, xi, zi+1, xi+1, zi, false, ref vertList, ref uvList, ref triList, ref colorList, ref i, Color.black);
						addTri(xi, zi+1, xi+1, zi+1, xi+1, zi, false, ref vertList, ref uvList, ref triList, ref colorList, ref i, Color.black);
					
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

		private void addTri(int x1, int z1, int x2, int z2, int x3, int z3, bool raised, ref List<Vector3> vertList, ref List<Vector2> uvList, ref List<int> triList, ref List<Color> colorList,  ref int i, Color color) {
			addVertex(x1, z1, raised, ref vertList, ref uvList, ref triList, ref colorList, ref i, color);
			addVertex(x2, z2, raised, ref vertList, ref uvList, ref triList, ref colorList, ref i, color);
			addVertex(x3, z3, raised, ref vertList, ref uvList, ref triList, ref colorList, ref i, color);
		}

		private void addRaisedTri(int x1, int z1, int x2, int z2, int x3, int z3, ref List<Vector3> vertList, ref List<Vector2> uvList, ref List<int> triList, ref List<Color> colorList,  ref int i, Color color) {
			//raised tri 1
			addTri(x1, z1, x2, z2, x3, z3, true, ref vertList, ref uvList, ref triList, ref colorList, ref i, color);

			//edges
			addEdge(x1, z1, x2, z2, ref vertList, ref uvList, ref triList, ref colorList, ref i, color);
			addEdge(x2, z2, x3, z3, ref vertList, ref uvList, ref triList, ref colorList, ref i, color);
			addEdge(x3, z3, x1, z1, ref vertList, ref uvList, ref triList, ref colorList, ref i, color);
		}

		private void addEdge(int x1, int z1, int x2, int z2, ref List<Vector3> vertList, ref List<Vector2> uvList, ref List<int> triList, ref List<Color> colorList,  ref int i, Color color) {
			addVertex(x1, z1, false, ref vertList, ref uvList, ref triList, ref colorList, ref i, color);
			addVertex(x2, z2, false, ref vertList, ref uvList, ref triList, ref colorList, ref i, color);
			addVertex(x1, z1, true, ref vertList, ref uvList, ref triList, ref colorList, ref i, color);

			addVertex(x2, z2, false, ref vertList, ref uvList, ref triList, ref colorList, ref i, color);
			addVertex(x2, z2, true, ref vertList, ref uvList, ref triList, ref colorList, ref i, color);
			addVertex(x1, z1, true, ref vertList, ref uvList, ref triList, ref colorList, ref i, color);
		}

		private void addVertex(int x, int z, bool useOffset, ref List<Vector3> vert, ref List<Vector2> uv, ref List<int> tri, ref List<Color> color,  ref int i, Color c) {
			HeightmapIndex temp = new HeightmapIndex (x, z);
//			if (useOffset)
//				vert.Add (toPosition(temp)+WorldTerrain.terrain.blockOffset);
//			else
				vert.Add (toPosition(temp));
			uv.Add (new Vector2 (0, 0));
			tri.Add (i);
//			if (c != Color.black)
				color.Add (c);
//			else
//				color.Add (WorldTerrain.terrain.getColor (toWorld(temp), heightmap[temp.x,temp.z]));
			i++;
		}

		private Vector3 toPosition(HeightmapIndex h) {
			HeightmapIndex index = new HeightmapIndex (start.x + h.x, start.z + h.z);
			Vector3 prelim = WorldTerrain.ToPosition (index);
			prelim.y = heightmap [h.x, h.z];
			prelim.x -= transform.position.x;
			prelim.z -= transform.position.z;
			prelim.y -= transform.position.y;
			return prelim;
		}

	}


}