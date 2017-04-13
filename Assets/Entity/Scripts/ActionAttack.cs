using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyEntity {

	public class ActionAttack : ActionLocomotion {

		public GameObject target;
		public float approach, stop;
		public bool run;
		public bool inRange = false;

		/*
		 * 
		 * Public Interface
		 * 
		 */ 

		public ActionAttack(Entity o, OnCompleteDelegate oc, GameObject t, float a, float s, bool r) : base(o,oc) {
			target = t;
			approach = a;
			stop = s;
			run = r;
		}


		public override void Update () {
			if (!inRange) {
				Vector3 dif = (flatten (target.transform.position) - owner.transform.position);
				rotateX (ref dif, owner.transform.localEulerAngles.x);
				float dist = dif.magnitude;
				dif.Normalize ();
				steer (ref dif, dist);
				seek (dif, dist);
			} else {
				owner.setRotation(0f);
				owner.setVelocity (Vector3.zero);
				Collider[] hitColliders = Physics.OverlapSphere(owner.transform.position, 10f);
				if (hitColliders.GetLength (0) == 0) {
					inRange = false;
					return;
				}
				foreach (Collider c in hitColliders) {
					Living l = c.GetComponent<Living> ();
					if (l != null) {
						owner.setAttacking (true);
					} else {
						inRange = false;
						owner.setAttacking (false);
					}
				}
			}
		}

		protected void seek(Vector3 dir, float dist) {
			float speed = owner.walkSpeed;
			if (run)
				speed = owner.runSpeed;
			if (dist < approach) {
				dist -= stop;
				speed *= dist / (approach - stop);
				if (dist <= stop + 0.01f) {
					inRange = true;
					return;
				}
			}
			float deg = getRotationTo(dir);
			owner.setRotation(getScaledRotation(deg));
			owner.setVelocity (new Vector3 (0, 0, speed));
		}

	}
}