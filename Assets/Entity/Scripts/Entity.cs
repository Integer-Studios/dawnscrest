using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using PolyPlayer;
using System;

namespace PolyEntity {

	public class Entity : NetworkBehaviour, Living {

		// Vars : public, protected, private, hide
		public float maxHealth;
		public float runSpeed;
		public float walkSpeed;
		public float maxRotation;
		public int rotationalDownsample;
		public steeringInfo steeringInfo;
		public GameObject deadPrefab;
		public GameObject seekTarget;

		protected Animator anim;
		protected Rigidbody rigidBody;

		private bool grounded;
		private float health;
		private List<Action> queuedActions;
		private List<Action> currentActions;
		private List<Action> completedActions;

		// Syncvars
		[SyncVar]
		private Vector3 velocity;
		[SyncVar]
		private float rotation;

		/*
		 * 
		 * Public Interface
		 * 
		 */

		public void startAction(Action a) {
			queuedActions.Add (a);
		}

		public void stopAction(Action a) {
			completedActions.Add (a);
		}

		public void setVelocity(Vector3 v) {
			velocity = v;
			anim.SetFloat ("Vertical", speedToAnim(v.z));
		}

		public void setRotation(float f) {
			int r = (int)(f * rotationalDownsample);
			r /= rotationalDownsample;
			rotation = (float)r;
			anim.SetFloat ("Horizontal", f/maxRotation);
		}

		// Living Interface

		[Server]
		public void living_hurt(Living l, float d) {
			if (!NetworkClient.active) {
				anim.SetTrigger ("Hurt");
			}
			health -= d;
			if (health <= 0)
				die ();
			RpcHurt ();
		}

		[Server]
		public void living_setHeated(bool h) {
			
		}


		/*
		* 
		* Server->Client Networked Interface
		* 
		*/

		[ClientRpc]
		private void RpcInteracting(bool b) {
			anim.SetBool ("Interacting", b);
		}

		[ClientRpc]
		private void RpcConsuming(bool b) {
			anim.SetBool ("Eating", b);
		}

		[ClientRpc]
		private void RpcHurt() {
			anim.SetTrigger ("Hurt");
		}

		/*
		* 
		* Private
		* 
		*/

		protected virtual void Start() {
			anim = GetComponent<Animator> ();
			rigidBody = GetComponent<Rigidbody> ();
			ClientScene.RegisterPrefab(deadPrefab);

			if (!NetworkServer.active)
				return;

			health = maxHealth;
			queuedActions = new List<Action> ();
			currentActions = new List<Action> ();
			completedActions = new List<Action> ();

//			startAction (new ActionTest (this, null));
			startAction(new ActionWander(this,null,50f,60f,-1));
//			startAction(new ActionSeek(this,null,seekTarget,5f,2f,false));
		}

		protected virtual void Update() {
			updateLocomotion ();
			if (!isServer)
				return;
			updateBrain ();
	
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10f);
			foreach (Collider c in hitColliders) {
				Player l = c.GetComponent<Player> ();
				if (l != null) {
					currentActions.Clear ();
					startAction(new ActionFlee(this,null,c.gameObject).chain(new ActionWander(this,null,50f,60f,-1)));
					break;
				}
			}

			RaycastHit hit;
			if (Physics.Raycast (transform.position, -transform.up, out hit, 2f)) {
				anim.SetBool ("Grounded", true);
				grounded = true;

				if (Math.Abs(Vector3.Dot(hit.normal,Vector3.up)) > 0.3)
					transform.rotation = Quaternion.FromToRotation (transform.up, hit.normal) * transform.rotation;
			} else {
				anim.SetBool ("Grounded", false);
				grounded = false;
			}
		}


		private void updateBrain() {
			foreach (Action a in queuedActions) {currentActions.Add (a);}
			queuedActions.Clear ();

			foreach (Action a in currentActions) {a.Update ();}

			foreach (Action a in completedActions) {currentActions.Remove (a);}
			completedActions.Clear ();
		}

		private void updateLocomotion() {

//			rigidBody.velocity = new Vector3(transform.TransformDirection(velocity).x , rigidBody.velocity.y, transform.TransformDirection(velocity).z);
			transform.Translate (velocity * Time.deltaTime);
			transform.Rotate (new Vector3 (0f, rotation, 0f));

		}

		private void setInteracting(bool b) {
			if (!NetworkClient.active) {
				anim.SetBool ("Interacting", b);
			}
			RpcInteracting (b);
		}

		private void setConsuming(bool b) {
			if (!NetworkClient.active) {
				anim.SetBool ("Eating", b);
			}
			RpcConsuming (b);
		}

		private void die() {
			GameObject g = Instantiate (deadPrefab);
			g.transform.position = transform.position;
			g.transform.localScale = transform.localScale;
			g.transform.rotation = transform.rotation;
			NetworkServer.Destroy (gameObject);
			NetworkServer.Spawn (g);
		}

		private float speedToAnim(float f) {
			if (f >= runSpeed)
				return 1f;
			if (f <= -1f*walkSpeed)
				return -0.5f;

			if (f < 0f)
				return mapTo(-walkSpeed, -0.5f, f, 0f, 0f);
			if (f < walkSpeed)
				return mapTo(0f, 0f, f, walkSpeed, 0.5f);

			return mapTo(walkSpeed, 0.5f, f, runSpeed, 1f);
		}

		private float mapTo(float low1, float low2, float f, float high1, float high2) {
			return putPercent (low2, getPercent (low1, f, high1), high2);
		}

		private float getPercent(float low, float g, float high) {
			high -= low;
			g -= low;
			return  (g / high);
		}

		private float putPercent(float low, float p, float high) {
			high -= low;
			p *= high;
			return p + low;
		}

		protected void rotateX(ref Vector3 v, float theta) {
			theta *= Mathf.Deg2Rad;
			float cos = Mathf.Cos (theta);
			float sin = Mathf.Sin (theta);
			v = new Vector3(v.x,v.y*cos - v.z*sin,v.z*cos + v.y*sin);
		}

		protected float getRotationToNormal(Vector3 dif) {
			float deg = (float)(Mathf.Acos (Vector3.Dot (transform.up, dif)) * 180 / 3.14);
			Vector3 cross = Vector3.Cross (transform.up, dif).normalized;
			if (Single.IsNaN (deg))
				return 0f;
			if (cross.x < 0)
				return (float)deg * -1f;
			else
				return (float)deg;
		}

	}

	public interface Living {
		void living_hurt(Living l, float d);
		void living_setHeated(bool h);
	}

	[System.Serializable]
	public struct steeringInfo {
		public Vector3 offset;
		public float radius;
		public float interpolation;
		public float steepness;
		public float defaultRange;
		public float dotCutoff;
		public int wideRangeExponentHalf;
	}

}
