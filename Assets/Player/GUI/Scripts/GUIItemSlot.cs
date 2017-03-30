using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PolyItem;

namespace PolyPlayer {

	public class GUIItemSlot : MonoBehaviour, IPointerDownHandler {

		public GUIItemStack stackPrefab;

		protected int index;
		protected GUIInventory inventory;

		/*
		 * 
		 * Public Interface
		 * 
		 */

		public virtual void setStack(ItemStack istack) {
			if (istack == null) {
				clear ();
			} else {
				clear ();
				GameObject g = Instantiate (stackPrefab.gameObject);
				g.transform.SetParent (transform, false);
				g.GetComponent<GUIItemStack> ().init (istack);
			}
		}

		public virtual void onStackUpdate() {
			if (inventory)
				inventory.onSlotUpdate(index);
		}

		public virtual void setParentInventory(GUIInventory inv, int i) {
			inventory = inv;
			index = i;
		}

		public virtual void OnPointerDown(PointerEventData data) {
			if (data.button == PointerEventData.InputButton.Left) {
				GUIItemStack stackOnMouse = GUIManager.mouseFollower.GetComponentInChildren<GUIItemStack> ();
				if (GUIManager.mouseFollower.activeSelf && canHold(stackOnMouse)) {
					stackOnMouse.transform.SetParent (transform);
					GUIManager.mouseFollower.SetActive (false);
				}
			} else if (data.button == PointerEventData.InputButton.Right) {
				GUIItemStack stackOnMouse = GUIManager.mouseFollower.GetComponentInChildren<GUIItemStack> ();
				if (GUIManager.mouseFollower.activeSelf && canHold(stackOnMouse)) {
					if (stackOnMouse.getStack().size == 1)
						GUIManager.mouseFollower.SetActive (false);
					stackOnMouse.setSize (stackOnMouse.getStack().size - 1);
					GameObject g = Instantiate (stackPrefab.gameObject);
					g.transform.SetParent (transform);
					g.GetComponent<GUIItemStack> ().init (new ItemStack(stackOnMouse.getStack(), 1));
				}
			}
			onStackUpdate ();
		}

		public virtual GUIItemStack getStack() {
			return GetComponentInChildren<GUIItemStack> ();
		}

		public virtual bool OnStackPointerDown(PointerEventData data) {
			return true;
		}

		public virtual bool OnStackPointerUp(PointerEventData data) {
			return true;
		}

		public virtual bool OnStackPointerEnter(PointerEventData eventData) {
			return true;
		}

		public virtual bool OnStackPointerExit(PointerEventData eventData) {
			return true;
		}

		public virtual bool canHold(GUIItemStack stack) {
			return true;
		}

		/*
		 * 
		 * Private
		 * 
		 */

		protected void clear() {
			foreach (Transform t in transform)
				Destroy (t.gameObject);
		}
	}

}
