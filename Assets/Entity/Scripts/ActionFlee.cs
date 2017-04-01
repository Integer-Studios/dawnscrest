using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyEntity {
	
	public class ActionFlee : ActionLocomotion {

		public GameObject target;

		/*
		 * 
		 * Public Interface
		 * 
		 */ 

		public ActionFlee(Entity o, OnCompleteDelegate oc, GameObject t) : base(o,oc) {
			target = t;
		}

		public override void Update () {
			Vector3 dif = (owner.transform.position - flatten(target.transform.position));
			rotateX (ref dif, owner.transform.localEulerAngles.x);
			float dist = dif.magnitude;
			if (dist > 50f)
				kill ();
			dif.Normalize ();
			steer (ref dif, dist);
			seek (dif, dist);
		}

		protected void seek(Vector3 dir, float dist) {
			float deg = getScaledRotation(getRotationTo(dir));
			float speed = owner.runSpeed - ((owner.runSpeed) * (deg / owner.maxRotation));
			owner.setRotation(deg);
			owner.setVelocity (new Vector3 (0, 0, speed));
		}


	}

}