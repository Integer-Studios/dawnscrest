using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyWorld;

namespace PolyEntity {

	public class ActionWander : ActionLocomotion {

		public Vector3 target;
		public float range,updateTime;
		public Vector3 basePosition;
		public int iterations;
		public int currentIterations = 0;
		/*
		 * 
		 * Public Interface
		 * 
		 */ 

		public ActionWander(Entity o, OnCompleteDelegate oc, float r, float u, int i) : base(o,oc) {
			basePosition = owner.transform.position;
			range = r;
			updateTime = u;
			iterations = i;
		}


		public override void Update () {
			if (iterations != -1 && currentIterations == iterations) {
				kill ();
				return;
			}
			if (target == Vector3.zero)
				owner.StartCoroutine (updateTarget());
			Vector3 dif = (flatten(target) - owner.transform.position);
			float dist = dif.magnitude;
			dif.Normalize ();
			steer (ref dif, dist);
			seek (dif, dist);
		}

		private IEnumerator updateTarget() {
			while (currentIterations < iterations || iterations == -1) {
				getNewTarget ();
				yield return new WaitForSeconds (updateTime);
			}
		}

		protected void seek(Vector3 dir, float dist) {
			if (dist < 1f) {
				getNewTarget ();
				return;
			}
			float speed = owner.walkSpeed;
			float deg = getRotationTo(dir);
			owner.setRotation(getScaledRotation(deg));
			owner.setVelocity (new Vector3 (0, 0, speed));
		}

		private void getNewTarget() {
			target = WorldTerrain.toTerrainSurface(basePosition + new Vector3 (Random.Range (-range, range), 0f, Random.Range (-range, range)));
			while (WorldTerrain.isUnderwater(target)) {
				target =  WorldTerrain.toTerrainSurface(basePosition + new Vector3 (Random.Range (-range, range), 0f, Random.Range (-range, range)));
			}
			currentIterations++;
		}
	}

}