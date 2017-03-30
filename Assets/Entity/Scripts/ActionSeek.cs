using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PolyEntity {

	public class ActionSeek : ActionLocomotion {

		public GameObject target;
		public float approach, stop;
		public bool run;

		/*
		 * 
		 * Public Interface
		 * 
		 */ 

		public ActionSeek(Entity o, OnCompleteDelegate oc, GameObject t, float a, float s, bool r) : base(o,oc) {
			target = t;
			approach = a;
			stop = s;
			run = r;
		}


		public override void Update () {
			Vector3 dif = (flatten(target.transform.position) - owner.transform.position);
			rotateX (ref dif, owner.transform.localEulerAngles.x);
			float dist = dif.magnitude;
			dif.Normalize ();
			steer (ref dif, dist);
			seek (dif, dist);
		}

		protected void seek(Vector3 dir, float dist) {
			float speed = owner.walkSpeed;
			if (run)
				speed = owner.runSpeed;
			if (dist < approach) {
				dist -= stop;
				speed *= dist / (approach - stop);
				if (dist <= stop + 0.01f) {
					kill ();
					return;
				}
			}
			float deg = getRotationTo(dir);
			owner.setRotation(getScaledRotation(deg));
			owner.setVelocity (new Vector3 (0, 0, speed));
		}

	}
}