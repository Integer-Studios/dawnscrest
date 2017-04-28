using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyWorldOld {

	public class WorldGenerator : MonoBehaviour {

		public float resolution = 100f;
		public Generator[] generators;
		public GameObject ocean;

		public void editor_generate() {
			if (!Application.isEditor)
				return;
			
			WorldTerrain t = GetComponent<WorldTerrain> ();
			t.initialize ();
			for (float z = transform.position.z; z < transform.position.z + t.size; z+=resolution) {
				for (float x = transform.position.x; x < transform.position.x + t.size; x+=resolution) {
					Vector3 h = t.toSurface (new Vector3 (x, 0f, z));
					if (h.y < ocean.transform.position.y)
						continue;

					foreach (Generator g in generators) {
						if (g.attemptGen (h))
							break;
					}
				}
			}
		}

		public void editor_clear() {
			if (!Application.isEditor)
				return;

			foreach (GameObject g in GameObject.FindGameObjectsWithTag("decoration")) {
				DestroyImmediate (g);
			}
		}

	}

	[System.Serializable]
	public class Generator {
		public GameObject[] gens;
		public float temp;
		public float humidity;
		public float steepness;
		public bool rotateToTerrain;
		public float maxScale;
		public float minScale;
		public Vector3 rotationRange;
		public float density;

		public bool attemptGen(Vector3 h) {
			Vector3 up = Vector3.zero;
			if (rotateToTerrain) {
				RaycastHit hit;
				Physics.Raycast (h+Vector3.up, Vector3.down, out hit);
				up = hit.normal;
			}
			GameObject g = GameObject.Instantiate (getRand(),h,Quaternion.identity);
			g.transform.up = up;
			return true;
		}

		private GameObject getRand() {
			return gens [Random.Range (0, gens.Length)];
		}
	}

}