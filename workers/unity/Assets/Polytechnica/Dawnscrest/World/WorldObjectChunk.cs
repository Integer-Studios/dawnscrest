using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polytechnica.Dawnscrest.World {

	[System.Serializable]
	public class WorldObjectChunk {
		public List<WorldObject> objects;
		public WorldObjectChunk() {
			objects = new List<WorldObject>();
		}
		public WorldObjectChunk(List<WorldObject> o) {
			objects = o;
		}
	}

}