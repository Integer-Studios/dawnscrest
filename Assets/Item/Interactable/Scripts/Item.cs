using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace PolyItem {

	public class Item : Interactable {

		// Vars : public, protected, private, hide
		public int id;
		public Sprite sprite;
		public int weight = 1;
		public int maxStackSize = 1;
		public ItemType type = ItemType.Default;

		public GameObject onPlaced;

		private ItemHolder holder;
		private int heldID = -1;

		//Syncvars
		[SyncVar]
		public int quality = 0;

		/*
		* 
		* Public Interface
		* 
		*/

		// Server Interface

		//TODO we need a custom network manager to call this
		[Server]
		public void OnPlayerConnected() {
			if (holder != null)
				RpcOnLoginInfo (holder.itemHolder_getGameObject (), heldID);
			else
				RpcOnLoginInfo (null, heldID);
		}

		[Server]
		public virtual void setQuality(int i) {
			quality = i;
		}

		[Server]
		public virtual void setHolder(ItemHolder h, int hid) {
			convertToHeldItem (h, hid);
			RpcSetHolder (h.itemHolder_getGameObject (), hid);
		}

		// General Interface

		public virtual int getQuality() {
			return quality;
		}

		// Interactable Interface Overrides

		public override bool isInteractable(Interactor i) {
			return holder == null;
		}

		/*
		* 
		* Server->Client Interface
		* 
		*/

		[ClientRpc]
		private void RpcOnLoginInfo(GameObject holder, int hid) {
			if (!NetworkServer.active) {
				
				if (holder == null)
					return;
				
				ItemHolder h = holder.GetComponent<ItemHolder> ();
				convertToHeldItem (h, hid);
			}
		}

		[ClientRpc]
		private void RpcSetHolder(GameObject holder, int hid) {
			if (!NetworkServer.active) {
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
			if (GetComponent<NetworkTransform> ())
				Destroy(GetComponent<NetworkTransform> ());

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
