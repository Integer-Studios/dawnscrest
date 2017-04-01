using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PolyWorld;

namespace PolyEntity {

	public class ActionLocomotion : Action {
		
		/*
		 * 
		 * Public Interface
		 * 
		 */ 

		public ActionLocomotion(Entity o, OnCompleteDelegate oc) : base(o,oc) {}

		public override void kill() {
			owner.setRotation(0f);
			owner.setVelocity (Vector3.zero);
			base.kill ();
		}

		/*
		 * 
		 * Private Interface
		 * 
		 */ 

		protected void rotateY(ref Vector3 v, float theta) {
			theta *= Mathf.Deg2Rad;
			float cos = Mathf.Cos (theta);
			float sin = Mathf.Sin (theta);
			v = owner.transform.InverseTransformDirection (v);
			v = new Vector3(v.x*cos - v.z*sin,v.y,v.z*cos + v.x*sin);
			v = owner.transform.TransformDirection (v);
		}

		protected void rotateX(ref Vector3 v, float theta) {
			theta *= Mathf.Deg2Rad;
			float cos = Mathf.Cos (theta);
			float sin = Mathf.Sin (theta);
			v = owner.transform.InverseTransformDirection (v);
			v = new Vector3(v.x,v.y*cos - v.z*sin,v.z*cos + v.y*sin);
			v = owner.transform.TransformDirection (v);
		}

		protected float getRotationTo(Vector3 dif) {
			float deg = (float)(Mathf.Acos (Vector3.Dot (owner.transform.forward, dif)) * 180 / 3.14);
			Vector3 cross = Vector3.Cross (owner.transform.forward, dif).normalized;

			if (Single.IsNaN (deg))
				return 0f;
			if (cross.y < 0)
				return (float)deg * -1f;
			else
				return (float)deg;
		}

		protected float getScaledRotation(float rotation) {
			if (Mathf.Abs(rotation) < owner.maxRotation*Time.deltaTime)
				return rotation;
			if (rotation < -owner.maxRotation*Time.deltaTime)
				return -owner.maxRotation*Time.deltaTime;
			return owner.maxRotation*Time.deltaTime;
		}

		protected Vector3 flatten(Vector3 v) {
			return new Vector3 (v.x, owner.transform.position.y, v.z);
		}

		protected bool steer(ref Vector3 dir, float dist) {
			
			float castDist = calcCastDist(dir);
			Debug.DrawRay (castPos (), dir * castDist, Color.blue);
			bool flag = true;
			if (!canWalk (dir, castDist))
				flag = avoid (ref dir, castDist);
			Debug.DrawRay (castPos (), dir * castDist, Color.red);
			return flag;
		}


		protected bool avoid (ref Vector3 dir, float dist) {
			Vector3 dirP = dir;
			Vector3 dirN = dir;
			float tests = 0;
			bool flag1 = true;
			while (!canWalk(dirP.normalized, dist)) {
				rotateY (ref dirP, owner.steeringInfo.interpolation);
				tests += owner.steeringInfo.interpolation;
				if (tests > 360f) {
					flag1 = false;
					break;
				}
			}
			tests = 0;
			bool flag2 = true;
			while (!canWalk(dirN.normalized, dist)) {
				rotateY (ref dirN, -1*owner.steeringInfo.interpolation);
				tests += owner.steeringInfo.interpolation;
				if (tests > 360f) {
					flag2 = false;
					break;
				}
			}
			if (Vector3.Distance (dirP, owner.transform.forward) < Vector3.Distance (dirN, owner.transform.forward))
				dir = dirP;
			else
				dir = dirN;
			return flag1 || flag2;
		}

		protected bool canWalk(Vector3 dir, float castDist) {
			if (WorldTerrain.isUnderwater (owner.transform.position + dir*castDist)) {
				return false;
			}
			RaycastHit[] hits = Physics.SphereCastAll (castPos (), owner.steeringInfo.radius, dir, castDist);
			bool flag = true;
			foreach (RaycastHit hit in hits) {
				if (hit.collider.gameObject == owner.gameObject)
					continue;
				if (hit.collider.gameObject.layer == 4)
					flag = false;
				if (!hit.collider.isTrigger && Math.Abs (Vector3.Dot (hit.normal, Vector3.up)) < owner.steeringInfo.steepness) {
					flag = false;
				}
			}
			return flag;
		}

		private float calcCastDist(Vector3 dir) {
			float dot = Vector3.Dot (dir, owner.transform.forward);
			dot -= 1f;
			dot /= -2f;
			return owner.steeringInfo.defaultRange + Mathf.Pow(dot*owner.steeringInfo.dotCutoff, owner.steeringInfo.wideRangeExponentHalf*2);
		}

		private Vector3 castPos() {
			return owner.transform.position + owner.transform.TransformDirection(owner.steeringInfo.offset);
		}
	}

}