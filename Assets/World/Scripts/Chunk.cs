using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyWorld {

	public class Chunk : MonoBehaviour {

		// Vars : public, protected, private, hide
		// Have to be public to save from the editor
		public Vector3 center;
		public GameObject chunkMesh;

		private bool decorationsAreActive = true;

	   /*
		* 
		* Public
		* 
		*/

		public void initialize(Vector3 c, GameObject m) {
			center = c;
			chunkMesh = m;
		}

		public void setMesh(Mesh m) {
			chunkMesh.GetComponent<MeshFilter> ().mesh = m;
			chunkMesh.GetComponent<MeshCollider> ().sharedMesh = m;
		}

		public void updateActive(Vector3 p, float ter, float dec) {
			float d = Vector3.Distance (p, center);
			if (d > ter) {
				gameObject.SetActive (false);
			} else {
				gameObject.SetActive (true);
			}

			if (d > dec && decorationsAreActive) {
				foreach (Transform t in transform) {
					if (t.tag == "decoration")
						t.gameObject.SetActive (false);
				}
				decorationsAreActive = false;
			} else if (d <= dec && !decorationsAreActive) {
				foreach (Transform t in transform) {
					if (t.tag == "decoration")
						t.gameObject.SetActive (true);
				}
				decorationsAreActive = true;
			}

		}
	}

}