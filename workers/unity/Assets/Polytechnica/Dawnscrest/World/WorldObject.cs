using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Polytechnica.Dawnscrest.Core;

namespace Polytechnica.Dawnscrest.World {

	[System.Serializable]
	public class WorldObject {
		public string name;
		public EntityTemplateType type;
		public Vector3 position;
		public Vector3 rotation;
		public Vector3 scale;

		public WorldObject(string n, EntityTemplateType t, Vector3 p, Vector3 r, Vector3 s) {
			name = n;
			type = t;
			position = p;
			rotation = r;
			scale = s;
		}
	}

}