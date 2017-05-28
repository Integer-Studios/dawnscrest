using Improbable.Collections;
using UnityEngine;
using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Visualizer;

namespace Polytechnica.Dawnscrest.World {

	public class Chunk : MonoBehaviour {

		[Require] private TerrainChunk.Writer chunkWriter;

		private HeightmapIndex start;
		private ChunkIndex index;
		private float[,] heightmap;

		public void OnEnable() {
			index = new ChunkIndex (chunkWriter.Data.x, chunkWriter.Data.z);
			start = WorldTerrain.ToHMI (index);
		}

		public void LoadHeightmap(float[,] worldHeightmap) {
			int chunkSizeX = (WorldTerrain.chunkSize / WorldTerrain.resolution) + 1;
			int chunkSizeZ = chunkSizeX;
			if (start.x + chunkSizeX >= worldHeightmap.GetLength(0))
				chunkSizeX--;
			if (start.z + chunkSizeZ >= worldHeightmap.GetLength(1))
				chunkSizeZ--;
			
			heightmap = new float[chunkSizeX,chunkSizeZ];
			for (int z = 0; z < heightmap.GetLength (1); z++) {
				for (int x = 0; x < heightmap.GetLength (0); x++) {
					heightmap [x, z] = worldHeightmap [start.x + x, start.z + z];
				}
			}

			chunkWriter.Send (new TerrainChunk.Update ()
				.SetSizeX ((uint)heightmap.GetLength (0))
				.SetSizeZ ((uint)heightmap.GetLength (1))
				.SetHeightmap (WrapHeightmap ())
			);
		}

		public List<float> WrapHeightmap() {
			List<float> l = new List<float> ();
			for (int z = 0; z < heightmap.GetLength (1); z++) {
				for (int x = 0; x < heightmap.GetLength (0); x++) {
					l.Add (heightmap [x, z]);
				}
			}
			return l;
		}

	}

}