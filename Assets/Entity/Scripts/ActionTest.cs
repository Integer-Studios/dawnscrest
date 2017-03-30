using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyEntity {

	public class ActionTest : ActionLocomotion {

		float cur;

		public ActionTest(Entity o, OnCompleteDelegate oc) : base(o,oc) {
			cur = 0f;
		}

		public override void Update () {
			Vector3 v = owner.transform.forward;
			cur += 30f * Time.deltaTime;
			rotateY (ref v, cur);
			Debug.DrawRay (owner.transform.position + owner.transform.up, v * 5f, Color.blue);
		}


	}

}