using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyWorld {

	public class Chunk : MonoBehaviour {

		private float[,] heightmap;
		private ChunkIndex index;

		public void instantiate(ChunkIndex i) {
			index = i;
			refreshHeightmap ();
			regenerateMesh ();
		}

		private void refreshHeightmap() {

		}

		private void regenerateMesh() {

		}

	}

}