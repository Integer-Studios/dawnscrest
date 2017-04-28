using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyWorldOld {

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

		public void editor_combine() {
			if (!Application.isEditor)
				return;

			MeshFilter[] filters = GetComponentsInChildren<MeshFilter> ();
			// combine meshes
			CombineInstance[] combine = new CombineInstance[filters.GetLength(0)];
			int i = 0;
			while (i < filters.GetLength(0)) {
				if (filters [i].transform.tag != "decoration") {
					i++;
					continue;
				}
				combine[i].mesh = filters[i].sharedMesh;
				combine[i].transform = filters[i].transform.localToWorldMatrix;
				i++;
			}
			Debug.Log (i);

			gameObject.AddComponent<MeshFilter>();
			gameObject.AddComponent<MeshRenderer>();
			gameObject.GetComponent<MeshFilter>().sharedMesh = new Mesh();
			gameObject.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);
		}
	}

}