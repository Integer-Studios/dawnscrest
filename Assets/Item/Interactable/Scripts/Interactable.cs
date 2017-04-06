using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace PolyItem {

	public class Interactable : NetworkBehaviour {
		
		// Vars : public, protected, private, hide

		// Syncvars
		[SyncVar]
		protected float strength = 0f;
		[SyncVar]
		public float maxStrength = 0f;

		/*
		 * 
		 * Public Interface
		 * 
		 */

		[Server]
		public virtual void interact(Interactor i, float f) {
			strength -= f;
			if (strength <= 0)
				onComplete (i);
		}

		[Server]
		public void setTransform(Transform t) {
			setTransform (t.position, t.localScale, t.eulerAngles);
		}

		[Server]
		public void setTransform(Vector3 pos, Vector3 scale, Vector3 rot) {
			RpcSetTransform (pos, scale, rot);
		}

		[Server]
		public void setPosition(Vector3 pos) {
			RpcSetPosition (pos);
		}

		public virtual bool isInteractable(Interactor i) {
			return true;
		}

		public virtual float getPercent() {
			return strength/maxStrength;
		}

		/*
		* 
		* Server->Client Networked Interface
		* 
		*/



		[ClientRpc]
		private void RpcSetTransform(Vector3 pos, Vector3 scale, Vector3 rot) {
			gameObject.transform.position = pos; 
			gameObject.transform.localScale = scale; 
			gameObject.transform.eulerAngles = rot; 
		}

		[ClientRpc]
		private void RpcSetPosition(Vector3 pos) {
			gameObject.transform.position = pos; 
		}

		/*
		* 
		* Private
		* 
		*/

		protected virtual void Start() {
			if (!NetworkServer.active)
				return;
			
			strength = maxStrength;
		}

		protected virtual void Update() {
			if (!NetworkServer.active)
				return;
		}

		protected virtual void onComplete(Interactor i) {
			
		}

		protected virtual void setMaxStrength(float s) {
			maxStrength = s;
			strength = maxStrength;
		}

	}

	public interface Interactor {
		ItemStack interactor_getItemInHand();
		void interactor_giveItem(Item i);
		Vector3 interactor_getInteractionPosition();
		Vector3 interactor_getInteractionNormal();
	}
		

}