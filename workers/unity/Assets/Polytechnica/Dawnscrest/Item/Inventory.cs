using System.Collections.Generic;
using UnityEngine;
using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Visualizer;
using Improbable.Entity.Component;
using Polytechnica.Dawnscrest.Core;

namespace Polytechnica.Dawnscrest.Item {

	public class Inventory : MonoBehaviour {

		[Require] private InventoryComponent.Writer inventoryWriter;

		public int size;

		protected ItemSlot[] slots;
		protected List<InventoryListener> listeners;

		private void OnEnable() {
			Debug.LogWarning ("yo");
			inventoryWriter.CommandReceiver.OnGive.RegisterResponse (OnGive);
			UnwrapStacks (inventoryWriter.Data.stacks);
		}

		private void OnDisable() {
			inventoryWriter.CommandReceiver.OnGive.DeregisterResponse ();
		}

		protected virtual void InitializeSlots() {
			slots = new ItemSlot[size];
			for (int i = 0; i < slots.GetLength(0); i++) {
				slots [i] = new ItemSlot ();
			}
		}

		/*
		 * 
		 * Insertion
		 * 
		 */

		public virtual bool Insert(ItemStack stack) {
			if (slots == null)
				InitializeSlots ();

			for (int i = 0; i < slots.GetLength(0); i++) {
				if (InsertSlot (i, stack))
					return true;
			}
			for (int i = 0; i < slots.GetLength(0); i++) {
				if (slots [i].IsEmpty() && slots [i].CanHold(stack)) {
					SetSlot (i, stack);
					return true;
				}
			}
			return false;
		}

		protected bool InsertSlot(int i, ItemStack s) {
			if (slots [i].IsEmpty ())
				return false;
			
			if (!slots [i].CanHold(s))
				return false;

			if (!slots [i].stack.Insert (s))
				return false;

			OnSlotUpdate (i);
			return true;
		}

		/*
		 * 
		 * Decrease
		 * 
		 */

		public void DecreaseSlot(int i) {
			slots [i].stack.Decrease ();
			OnSlotUpdate (i);
		}

		/*
		 * 
		 * Low-Level
		 * 
		 */

		public void SetSlot(int i, ItemStack s) {
			slots [i].stack = s;
			OnSlotUpdate (i);
		}

		public ItemStack GetSlotCopy(int i) {
			return new ItemStack(slots [i].stack);
		}

		/*
		 * 
		 * Listener
		 *
		 */

		public void StartListening(InventoryListener l) {
			StartListening (l, false);
		}

		public void StartListening(InventoryListener l, bool fullRefreshOnLoad) {
			if (listeners == null)
				listeners = new List<InventoryListener> ();
			listeners.Add (l);

			if (fullRefreshOnLoad) {
				for (int i = 0; i < slots.GetLength(0); i++) {
					l.OnInventorySlotChange (this, i, slots [i].stack);
				}
			}
		}

		public void StopListening(InventoryListener l) {
			listeners.Remove (l);
		}


		/*
		 * 
		 * Helpers
		 * 
		 */

		public void Log() {
			Debug.Log ("====Inventory====");
			for (int i = 0; i < slots.GetLength(0); i++) {
				ItemStack s = slots [i].stack;
				if (!s.IsNull())
					Debug.Log (i + ": " + s.id + ", Q:" + s.quality + ", S:" + s.size);
				else
					Debug.Log (i + ": Empty");
			}
			Debug.Log ("=================");
		}

		private void UpdateListeneners(int i, ItemStack s) {
			if (listeners == null)
				return;
			foreach (InventoryListener il in listeners) {
				il.OnInventorySlotChange (this, i, s);
			}
		}

		private GiveResponse OnGive(ItemStackData i, ICommandCallerInfo callerInfo) {
			GiveResponse r = new GiveResponse (Insert (new ItemStack (i)));
			return r;
		}

		private void OnSlotUpdate(int i) {
			inventoryWriter.Send (new InventoryComponent.Update ()
				.SetStacks(WrapStacks())
			);
			UpdateListeneners (i, slots[i].stack);
		}

		private Improbable.Collections.List<ItemStackData> WrapStacks() {
			Improbable.Collections.List<ItemStackData> l = new Improbable.Collections.List<ItemStackData> ();
			for (int i = 0; i < slots.GetLength (0); i++) {
				l.Add (slots[i].stack.GetData());
			}
			return l;
		}

		private void UnwrapStacks(Improbable.Collections.List<ItemStackData> l) {
			if (l.Count == 0) {
				InitializeSlots ();

				inventoryWriter.Send (new InventoryComponent.Update ()
					.SetStacks (WrapStacks ())
				);
			} else {
				if (l.Count != size)
					size = l.Count;
				InitializeSlots ();

				ItemStackData[] arr = l.ToArray ();
				for (int i = 0; i < slots.GetLength (0); i++) {
					if (slots [i].CompareAndUpdate (arr [i]))
						UpdateListeneners (i, slots [i].stack);
				}
			}
		}

	}

	public interface InventoryListener {
		void OnInventorySlotChange (Inventory inv, int i, ItemStack s);
	}

}