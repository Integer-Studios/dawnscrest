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

		public ChunkIndex GetIndex() {
			return index;
		}

	}

}