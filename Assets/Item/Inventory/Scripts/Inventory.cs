using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace PolyItem {

	public class Inventory : NetworkBehaviour, ISaveable {

		// Vars : public, protected, private, hide
		public int networkID;
		public int size;
		protected ItemSlot[] slots;
		protected List<InventoryListener> listeners;

		protected JSONObject data; 

		// Syncvars

		/*
		 * 
		 * Public Interface
		 * 
		 */

		[Server]
		public virtual void dropAll(Vector3 pos) {
			for (int i = 0; i < slots.GetLength(0); i++) {
				if (slots [i].stack == null)
					continue;
				
				for (int j = 0; j < slots [i].stack.size; j++) {
					GameObject g = ItemManager.createItem (slots [i].stack);
					g.transform.position = pos;
				}
				setSlot (i, null);
			}
		}

		[Server]
		public virtual void clearAll() {
			for (int i = 0; i < slots.GetLength (0); i++) {
				setSlot (i, null);
			}
		}

		[Server]
		public virtual void transfer(Inventory other) {
			foreach (ItemSlot s in other.slots) {
				if (s.stack != null)
					insert (s.stack);
			}
		}

		[Server]
		public virtual bool insert(Item item) {
			return insert(new ItemStack (item));
		}

		[Server]
		public virtual bool insert(ItemStack stack) {
			if (slots == null)
				initializeSlots ();
			
			for (int i = 0; i < slots.GetLength(0); i++) {
				if (insertSlot (i, stack))
					return true;
			}
			for (int i = 0; i < slots.GetLength(0); i++) {
				if (slots [i].stack == null && slots [i].canHold(stack)) {
					setSlot (i, stack);
					return true;
				}
			}
			return false;
		}

		[Server]
		public void setSlot(int i, ItemStack s) {
			Debug.Log (networkID + " " + i + " " + slots.Length + " " + s.id + " " + s.size);
//			if (s != null && s.size != 0) {
//				slots [i].stack = s;
//			} else {
//				slots [i].stack = null;
//			}
//			onSlotUpdate (i);
		}

		[Server]
		public void decreaseSlot(int i) {
			if (slots [i].stack == null)
				return;

			slots [i].stack.decrease ();
			if (slots [i].stack.size <= 0)
				setSlot (i, null);
			else {
				onSlotUpdate (i);
			}
		}

		public ItemStack getSlotCopy(int i) {
			if (slots [i].stack == null)
				return null;
			return new ItemStack(slots [i].stack);
		}
			
		public void startListening(InventoryListener l) {
			startListening (l, false);
		}

		public void startListening(InventoryListener l, bool fullRefreshOnLoad) {
			if (listeners == null)
				listeners = new List<InventoryListener> ();
			listeners.Add (l);

			if (fullRefreshOnLoad) {
				for (int i = 0; i < size; i++) {
					l.inventoryListener_onSlotChange (this, i, slots [i].stack);
				}
			}
		}

		public void stopListening(InventoryListener l) {
			listeners.Remove (l);
		}

		public void log() {
			Debug.Log ("====Inventory====");
			for (int i = 0; i < size; i++) {
				ItemStack s = slots [i].stack;
				if (s != null)
					Debug.Log (i + ": " + ItemManager.getName (s.id) + ", Q:" + s.quality + ", S:" + s.size);
				else
					Debug.Log (i + ": Empty");
			}
			Debug.Log ("=================");
		}

		/*
		* 
		* Server->Client Networked Interface
		* 
		*/

		[ClientRpc]
		private void RpcSlotUpdate(int netID, int i, NetworkItemStack ns) {
			
			if (isServer)
				return;

			// redirect RPC to correct Inventory
			foreach (Inventory inv in GetComponents<Inventory>()) {
				if (inv.networkID == netID) {
					inv.directed_RpcSlotUpdate (i, ns);
				}
			}

		}

		// called in the correct inventory
		private void directed_RpcSlotUpdate(int i, NetworkItemStack ns) {
			slots [i].stack = ItemStack.unwrapNetworkStack(ns);
			updateListeneners (i, slots[i].stack);
		}

		/*
		* 
		* Private
		* 
		*/

		public virtual void Start() {
			initializeSlots ();
			if (data != null)
				this.read (data);
		}

		protected virtual void initializeSlots() {
			slots = new ItemSlot[size];
			for (int i = 0; i < size; i++) {
				slots [i] = new ItemSlot ();
			}
		}

		protected bool insertSlot(int i, ItemStack s) {
			if (slots [i].stack == null)
				return false;

			if (!slots [i].canHold(s))
				return false;

			if (!slots [i].stack.insert (s))
				return false;

			onSlotUpdate (i);
			return true;
		}

		private void updateListeneners(int i, ItemStack s) {
			if (listeners == null)
				return;
			foreach (InventoryListener il in listeners) {
				il.inventoryListener_onSlotChange (this, i, s);
			}
		}

		private void onSlotUpdate(int i) {
			RpcSlotUpdate (networkID, i, new NetworkItemStack(slots[i].stack));
			updateListeneners (i, slots[i].stack);
		}

		public virtual JSONObject write () {
			JSONObject obj = new JSONObject (JSONObject.Type.OBJECT);
			JSONObject slotsJSON = new JSONObject (JSONObject.Type.ARRAY);
			int index = 0;
			foreach (ItemSlot i in slots) {
				JSONObject slotJSON = new JSONObject (JSONObject.Type.OBJECT);
				if (i.stack != null) {
					slotJSON.AddField ("id", index);
					slotJSON.AddField ("item", i.stack.id);
					slotJSON.AddField ("size", i.stack.size);
					slotJSON.AddField ("quality", i.stack.quality);
				} else {
					slotJSON.AddField ("id", index);
					slotJSON.AddField ("item", -1);
					slotJSON.AddField ("size", -1);
					slotJSON.AddField ("quality", -1);
				}
				slotsJSON.Add (slotJSON);
				index++;
			}
			obj.AddField ("slots", slotsJSON);
			return obj;
		}

		public virtual void read (JSONObject obj) {
			JSONObject slotsJSON = obj.GetField ("slots");

			if (slots != null) {
				foreach(JSONObject slot in slotsJSON.list) {
					int index = (int)slot.GetField ("id").n;
					int item = (int)slot.GetField ("item").n;
					int size = (int)slot.GetField ("size").n;
					int quality = (int)slot.GetField ("quality").n;
//					Debug.Log (index + " " + item + " " + size + " " + quality);
//					if (item != -1)
//						setSlot (index, new ItemStack (item, quality, size));
				}
			} else {

				this.data = slotsJSON;
			}
		}

		public string getType() {
			return "inventory";
		}

	}
		
	public interface InventoryListener {
		void inventoryListener_onSlotChange (Inventory inv, int i, ItemStack s);
	}

}