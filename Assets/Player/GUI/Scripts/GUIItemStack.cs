using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using PolyItem;

namespace PolyPlayer {
	
	public class GUIItemStack : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler {

		public GameObject stackPrefab;
		public Sprite weight1;
		public Sprite weight2;
		public Sprite weight3;

		protected ItemStack stack;
		protected Image weightImage;
		protected Image itemImage;
		protected Text text;

		/*
		* 
		* Public Interface
		* 
		*/

		public ItemStack getStack() {
			return stack;
		}

		public void init(ItemStack s) {

			weightImage = GetComponent<Image> ();
			itemImage = GetComponentsInChildren<Image> ()[1];
			text = GetComponentInChildren<Text> ();

			stack = s;
			switch (ItemManager.getWeight(stack)) {
			case 1:
				weightImage.sprite = weight1;
				break;
			case 2:
				weightImage.sprite = weight2;
				break;
			case 3:
				weightImage.sprite = weight3;
				break;
			}

			itemImage.sprite = ItemManager.getSprite(stack);
			text.text = s.size + "";
		}

		public void setSize(int s) {
			stack.size = s;
			text.text = stack.size + "";
			if (stack.size <= 0) {
				Destroy (gameObject);
			}
		}

		public GUIItemSlot getSlot() {
			return transform.parent.GetComponent<GUIItemSlot> ();
		}

		public void OnPointerDown(PointerEventData data) {

			GUIItemSlot slot = getSlot();

			if (slot) {
				if (!slot.OnStackPointerDown (data))
					return;
			}

			// Left Click
			if (data.button == PointerEventData.InputButton.Left) {
				GUIItemStack stackOnMouse = GUIManager.mouseFollower.GetComponentInChildren<GUIItemStack> ();

				// Item in hand
				if (GUIManager.mouseFollower.activeSelf) {
					int max = ItemManager.getMaxStackSize(stack);

					// Same Item ID
					if (stackOnMouse.stack.id == stack.id && stack.size < max) {
						int total = stack.size + stackOnMouse.stack.size;
						int remainder = total - max;
						if (remainder <= 0) {

							// Incorporate whole in hand stack
							setSize (total);
							stackOnMouse.setSize (0);
							GUIManager.mouseFollower.SetActive (false);
						} else {

							// Max out stack, leave remainder in hand
							setSize (max);
							stackOnMouse.setSize (remainder);
						}
						// Different Item ID
					} else {

						// Swap Item Stacks
						stackOnMouse.transform.SetParent (transform.parent);
						GUIManager.mouseFollower.SetActive (true);
						transform.SetParent (GUIManager.mouseFollower.transform);
					}
					// No item In Hand
				} else {

					// Put Stack in Hand
					GUIManager.mouseFollower.SetActive (true);
					transform.SetParent (GUIManager.mouseFollower.transform);
				}

				// Right Click
			} else if (data.button == PointerEventData.InputButton.Right) {

				// Item In Hand
				if (GUIManager.mouseFollower.activeSelf) {
					GUIItemStack stackOnMouse = GUIManager.mouseFollower.GetComponentInChildren<GUIItemStack> ();

					//Same Item ID
					if (stackOnMouse.stack.id == stack.id) {

						// Stack not full
						if (stack.size < ItemManager.getMaxStackSize(stack)) {
							if (stackOnMouse.stack.size == 1)
								GUIManager.mouseFollower.SetActive (false);
							stackOnMouse.setSize (stackOnMouse.stack.size - 1);
							setSize (stack.size + 1);
						}

						// Different Item ID
					} else {

					}

					// No Item In Hand, Splittable Stack
				} else if (stack.size > 1) {

					//Split stack in half, spawn new stack to put on mouse
					int newSize = stack.size / 2;
					int otherSize = stack.size - newSize;

					setSize (newSize);
					GameObject g = Instantiate (stackPrefab);
					g.transform.SetParent (GUIManager.mouseFollower.transform);
					g.GetComponent<GUIItemStack> ().init (new ItemStack(stack));
					g.GetComponent<GUIItemStack> ().setSize (otherSize);
					GUIManager.mouseFollower.SetActive (true);
					Debug.Log (stack.size + "," + g.GetComponent<GUIItemStack> ().getStack().size);
				}

			}

			if (slot)
				slot.onStackUpdate ();
		}

		public void OnPointerUp(PointerEventData data) {
			GUIItemSlot slot = getSlot();
			if (slot) {
				if (!slot.OnStackPointerUp (data))
					return;
			}
		}

		public void OnPointerEnter(PointerEventData data) {
			GUIItemSlot slot = getSlot();
			if (slot) {
				if (!slot.OnStackPointerEnter (data))
					return;
			}
		}
		public void OnPointerExit(PointerEventData data) {
			GUIItemSlot slot = getSlot();
			if (slot) {
				if (!slot.OnStackPointerExit (data))
					return;
			}
		}
	}

}