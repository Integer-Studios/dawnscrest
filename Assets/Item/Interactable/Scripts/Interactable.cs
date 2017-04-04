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


		/*
		* 
		* Private
		* 
		*/

		protected virtual void Start() {
			if (!isServer)
				return;
			
			strength = maxStrength;
		}

		protected virtual void Update() {
			if (!isServer)
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