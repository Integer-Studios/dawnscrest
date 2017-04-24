using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNet;
using System.IO;

namespace PolyItem {

	public class Interactable : PolyNetBehaviour {
		
		// Vars : public, protected, private, hide

		protected float strength = 0f;
		public float maxStrength = 0f;

		/*
		 * 
		 * Public Interface
		 * 
		 */

		public virtual void interact(Interactor i, float f) {
			strength -= f;
			identity.sendBehaviourPacket (new PacketSyncFloat (this, 0, strength));
			if (strength <= 0)
				onComplete (i);
		}

		public virtual bool isInteractable(Interactor i) {
			return true;
		}

		public virtual float getPercent() {
			return strength/maxStrength;
		}

		public override void handleBehaviourPacket (PacketBehaviour p) {
			base.handleBehaviourPacket (p);
			if (p.id == 13) {
				PacketSyncFloat o = (PacketSyncFloat)p;
				if (o.syncId == 0)
					strength = o.value;
				else if (o.syncId == 1)
					maxStrength = o.value;
			}
		}

		public override void writeBehaviourSpawnData(ref BinaryWriter writer) {
			writer.Write (strength);
		}

		public override void readBehaviourSpawnData(ref BinaryReader reader) {
			strength = reader.ReadInt32 ();
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
			if (!PolyServer.isActive)
				return;
			
			strength = maxStrength;
		}

		protected virtual void Update() {
			if (!PolyServer.isActive)
				return;
		}

		protected virtual void onComplete(Interactor i) {
			
		}

		protected virtual void setMaxStrength(float s) {
			maxStrength = s;
			strength = maxStrength;
			identity.sendBehaviourPacket (new PacketSyncFloat (this, 0, strength));
			identity.sendBehaviourPacket (new PacketSyncFloat (this, 1, maxStrength));
		}

	}

	public interface Interactor {
		ItemStack interactor_getItemInHand();
		void interactor_giveItem(Item i);
		Vector3 interactor_getInteractionPosition();
		Vector3 interactor_getInteractionNormal();
	}
		

}