using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNet;
using System.IO;

namespace PolyItem {

	public class Item : Interactable {

		// Vars : public, protected, private, hide
		public int id;
		public Sprite sprite;
		public int weight = 1;
		public int maxStackSize = 1;
		public ItemType type = ItemType.Default;
		public int quality = 0;
		public GameObject onPlaced;

		private ItemHolder holder;
		private int heldID = -1;


		/*
		* 
		* Public Interface
		* 
		*/

		// Server Interface

		public virtual void setHolder(ItemHolder h, int hid, bool network) {
			convertToHeldItem (h, hid);
			if (network)
				identity.sendBehaviourPacket (new PacketItemHeld (this, h.itemHolder_getGameObject (), hid));
		}

		// General Interface

		// Interactable Interface Overrides

		public override bool isInteractable(Interactor i) {
			return holder == null;
		}

		// Networking Interface

		public override void writeBehaviourSpawnData(ref BinaryWriter writer) {
			base.writeBehaviourSpawnData (ref writer);
			writer.Write (holder != null);
			if (holder != null) {
				PacketHelper.write (ref writer, holder.itemHolder_getGameObject ());
				writer.Write (heldID);
			}
		}

		public override void readBehaviourSpawnData(ref BinaryReader reader) {
			base.readBehaviourSpawnData (ref reader);
			bool isHeld = reader.ReadBoolean ();
			if (isHeld) {
				GameObject g = null;
				PacketHelper.read (ref reader, ref g);
				convertToHeldItem (g.GetComponent<ItemHolder> (), reader.ReadInt32 ());
			}
		}

		public override void handleBehaviourPacket (PacketBehaviour p) {
			base.handleBehaviourPacket (p);
			if (p.id == 16) {
				PacketSyncInt o = (PacketSyncInt)p;
				if (o.syncId == 0)
					quality = o.value;
			} else if (p.id == 23) {
				PacketItemHeld o = (PacketItemHeld)p;
				rpc_setHolder (o.holder, o.heldId);
			}
		}

		/*
		* 
		* Server->Client Interface
		* 
		*/

		//TODO in-hand rpc's

//		private void rpc_onLoginInfo(GameObject holder, int hid) {
//			if (!PolyServer.isActive) {
//				
//				if (holder == null)
//					return;
//				
//				ItemHolder h = holder.GetComponent<ItemHolder> ();
//				convertToHeldItem (h, hid);
//			}
//		}

		private void rpc_setHolder(GameObject holder, int hid) {
			if (!PolyServer.isActive) {
				ItemHolder h = holder.GetComponent<ItemHolder> ();
				convertToHeldItem (h, hid);
			}
		}

		/*
		* 
		* Private
		* 
		*/

		protected override void onComplete(Interactor i) {
			setMaxStrength (0);
			i.interactor_giveItem (this);
		}

		protected virtual void convertToHeldItem(ItemHolder h, int hid) {
			heldID = hid;
			if (GetComponent<PolyNetTransform> ())
				Destroy(GetComponent<PolyNetTransform> ());

			if (GetComponent<Rigidbody> ())
				Destroy(GetComponent<Rigidbody> ());

			if (GetComponent<Renderer> ())
				GetComponent<Renderer> ().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

			foreach (Collider c in GetComponents<Collider> ())
				Destroy(c);

			transform.SetParent (h.itemHolder_getHolderTransform(heldID));
			transform.localPosition = h.itemHolder_getOffset(heldID);
			transform.localEulerAngles = h.itemHolder_getRotation(heldID);
			holder = h;
			//ignore raycast for local player
			if (holder.itemHolder_isLocalPlayer ())
				gameObject.layer = 2;
		}

	}

	public enum ItemType {
		Default,
		Edible,
		Wearable,
		Placeable
	}

	public enum ToolType {
		Hand,
		Tinder,
		Axe,
		Building,
	}

	public enum ConsumableType {
		None,
		Food,
		Water,
	}

	public interface ItemHolder {
		GameObject itemHolder_getGameObject();
		Transform itemHolder_getHolderTransform (int hid);
		Vector3 itemHolder_getOffset (int hid);
		Vector3 itemHolder_getRotation (int hid);
		bool itemHolder_isLocalPlayer();
	}

}
