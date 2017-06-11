using System.Collections.Generic;
using UnityEngine;
using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Visualizer;

namespace Polytechnica.Dawnscrest.Item {

	public class InventoryVisualizer : MonoBehaviour {

		[Require] private InventoryComponent.Reader inventoryReader;

		protected ItemSlot[] slots;
		protected List<InventoryVisualizerListener> listeners;

		// Use this for initialization
		void OnEnable () {
			inventoryReader.ComponentUpdated += OnInventoryUpdated;
			UnwrapStacks (inventoryReader.Data.stacks);
		}
		
		// Update is called once per frame
		void OnDisable () {
			inventoryReader.ComponentUpdated -= OnInventoryUpdated;
		}

		protected virtual void InitializeSlots(int size) {
			slots = new ItemSlot[size];
			for (int i = 0; i < slots.GetLength(0); i++) {
				slots [i] = new ItemSlot ();
			}
		}

		/*
		 * 
		 * Networking
		 * 
		 */

		private void OnInventoryUpdated(InventoryComponent.Update update) {
			UnwrapStacks (update.stacks.Value);
		}

		private void UnwrapStacks(Improbable.Collections.List<ItemStackData> l) {
			if (slots.GetLength (0) != l.Count)
				InitializeSlots(l.Count);

			ItemStackData[] arr = l.ToArray ();
			
			for (int i = 0; i < slots.GetLength (0); i++) {
				if (slots [i].CompareAndUpdate (arr [i]))
					UpdateListeneners (i, slots [i].stack);
			}
		}

		/*
		 * 
		 * Listener
		 *
		 */

		public void StartListening(InventoryVisualizerListener l) {
			StartListening (l, false);
		}

		public void StartListening(InventoryVisualizerListener l, bool fullRefreshOnLoad) {
			if (listeners == null)
				listeners = new List<InventoryVisualizerListener> ();
			listeners.Add (l);

			if (fullRefreshOnLoad) {
				for (int i = 0; i < slots.GetLength(0); i++) {
					l.OnInventoryVisSlotChange (this, i, slots [i].stack);
				}
			}
		}

		public void StopListening(InventoryVisualizerListener l) {
			listeners.Remove (l);
		}

		/*
		 * 
		 * Helpers
		 * 
		 */


		private void UpdateListeneners(int i, ItemStack s) {
			if (listeners == null)
				return;
			foreach (InventoryVisualizerListener il in listeners) {
				il.OnInventoryVisSlotChange (this, i, s);
			}
		}
	}

	public interface InventoryVisualizerListener {
		void OnInventoryVisSlotChange (InventoryVisualizer inv, int i, ItemStack s);
	}

}