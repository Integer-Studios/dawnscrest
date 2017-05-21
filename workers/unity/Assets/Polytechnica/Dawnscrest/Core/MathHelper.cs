using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Math;

namespace Polytechnica.Dawnscrest.Core {

	public class MathHelper : MonoBehaviour {

		public static Vector3 toVector3(Vector3d v3) {
			return new Vector3 ((float)v3.X, (float)v3.Y, (float)v3.Z);
		}

		public static Vector3d toVector3d(Vector3 v3) {
			return new Vector3d (v3.x, v3.y, v3.z);
		}

	}

	public static class Vector3Extensions {
		public static Coordinates ToCoordinates(this Vector3 vector3) {
			return new Coordinates(vector3.x, vector3.y, vector3.z);
		}
	}

	public static class CoordinatesExtensions
	{
		public static Vector3 ToVector3(this Coordinates coordinates)
		{
			return new Vector3((float)coordinates.X, (float)coordinates.Y, (float)coordinates.Z);
		}
	}
}