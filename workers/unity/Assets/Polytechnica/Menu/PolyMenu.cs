using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace PolyMenu {

	public class PolyMenu : MonoBehaviour {

		public InputField[] inputs;
		public PolyMenuManager.Menu type;
		private EventSystem eventSystem;
		private bool hide;
		private CanvasGroup canvas;
		protected PolyMenuManager manager;

		public virtual void Start() {
			canvas = GetComponent<CanvasGroup> ();

			this.eventSystem = EventSystem.current;
			manager = FindObjectOfType<PolyMenuManager> ();
			manager.onMenuStart (this);
		}

		public virtual void Initialize() {

		}
			
		public virtual void Update() {
			handleInputs ();
			handleFade ();
		}

		private void handleInputs() {

			Selectable current = null;

			// Figure out if we have a valid current selected gameobject
			if (eventSystem.currentSelectedGameObject != null) {
				// Unity doesn't seem to "deselect" an object that is made inactive
				if (eventSystem.currentSelectedGameObject.activeInHierarchy) {
					current = eventSystem.currentSelectedGameObject.GetComponent<Selectable> ();
				}
			}
			int index = 0;
			if (current != null) {
				for (int x = 0; x < inputs.Length; x++) {
					InputField i = inputs [x];
					if (current.name == i.name) {
						index = x;
					}
				}
			}
			if (Input.GetKeyDown (KeyCode.Tab) && inputs.Length != 0) {
				Selectable next = null;
			
				if (current != null) {
					int newIndex = index + 1;
					if (newIndex > inputs.Length - 1) {
						//restart at top
						next = inputs [0];
					} else {
						next = inputs [newIndex];
					}
				} else if (inputs.Length >= 1) {
					next = inputs [0];
				}

				if (next != null) {
					next.Select ();
				}

			}

			if (Input.GetKeyDown (KeyCode.Return) && current != null && !manager.debug) {
				onEnter ();
			}
		}

		protected virtual void onEnter() {

		}

		protected virtual void onShow() {

		}
			
		private void handleFade() {
			if (hide && canvas.alpha > 0) {
				canvas.alpha -= 0.1F;
			} else if (hide) {
				canvas.alpha = 0;
			} 
			if (!hide && canvas.alpha < 1) {
				canvas.alpha += 0.1F;
				if (canvas.alpha >= 1) {
					onShow ();
				}
			} else if (!hide) {
				if (canvas.alpha < 1) {
					onShow ();
				}
				canvas.alpha = 1;
			}
		}

		public void fadeOut(bool fade = true) {
			if (!fade)
				canvas.alpha = 0;

			canvas.blocksRaycasts = false;
			canvas.interactable = false;
			hide = true;
		}

		public void fadeIn(bool fade = true) {
			if (canvas != null) {
				if (!fade) {
					canvas.alpha = 1;
					onShow ();

				}
				canvas.blocksRaycasts = true;

				canvas.interactable = true;
				hide = false;
			}
		}

	}
}
