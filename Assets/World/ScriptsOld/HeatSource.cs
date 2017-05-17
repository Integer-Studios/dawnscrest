using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyEntity;

namespace PolyWorldOld {

	public class HeatSource : MonoBehaviour {
		public float radius;

		private bool heating = false;

		public void setHeating(bool h) {
			heating = h;
		}

		private void Update() {
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
			foreach (Collider c in hitColliders) {
				Living l = c.GetComponent<Living> ();
				if (l != null) {
					l.living_setHeated (true);
					break;
				}
			}
		}
	}

}