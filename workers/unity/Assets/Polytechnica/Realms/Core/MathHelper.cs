using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Math;

namespace Polytechnica.Realms.Core {

	public class MathHelper : MonoBehaviour {

		public static Vector3 toVector3(Vector3d v3) {
			return new Vector3 ((float)v3.X, (float)v3.Y, (float)v3.Z);
		}

		public static Vector3d toVector3d(Vector3 v3) {
			return new Vector3d (v3.x, v3.y, v3.z);
		}

	}

}