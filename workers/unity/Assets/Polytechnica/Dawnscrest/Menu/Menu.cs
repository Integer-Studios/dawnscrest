using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Polytechnica.Dawnscrest.Menu {

	public class Menu : MonoBehaviour {

		public InputField[] inputs;
		public MenuManager.MenuType type;
		private EventSystem eventSystem;
		private bool hide;
		private CanvasGroup canvas;
		protected MenuManager manager;

		public virtual void Start() {
			canvas = GetComponent<CanvasGroup> ();

			this.eventSystem = EventSystem.current;
			manager = FindObjectOfType<MenuManager> ();
			manager.OnMenuStart (this);
		}

		public virtual void Initialize() {

		}
			
		public virtual void Update() {
			HandleInputs ();
			HandleFade ();
		}

		/*
		 *	Handles tab key and enter, using inputs array set in editor
		 */
		private void HandleInputs() {

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
				OnEnter ();
			}
		}

		/*
		 *	Triggered when enter key pressed while input field selected (input field MUST be in inputs array)
		 */
		protected virtual void OnEnter() {

		}

		/*
		 *	Overrideable Triggered when fade in finished or otherwise alpha = 1
		 */
		protected virtual void OnShow() {

		}
			
		/*
		 *	Called on update 
		 */
		private void HandleFade() {
			if (hide && canvas.alpha > 0) {
				canvas.alpha -= 0.1F;
			} else if (hide) {
				canvas.alpha = 0;
			} 
			if (!hide && canvas.alpha < 1) {
				canvas.alpha += 0.1F;
				if (canvas.alpha >= 1) {
					OnShow ();
				}
			} else if (!hide) {
				if (canvas.alpha < 1) {
					OnShow ();
				}
				canvas.alpha = 1;
			}
		}

		/*
		 *	Optional boolean input makes it instante if false
		 */
		public void FadeOut(bool fade = true) {
			if (!fade)
				canvas.alpha = 0;

			canvas.blocksRaycasts = false;
			canvas.interactable = false;
			hide = true;
		}

		public void FadeIn(bool fade = true) {
			if (canvas != null) {
				if (!fade) {
					canvas.alpha = 1;
					OnShow ();

				}
				canvas.blocksRaycasts = true;

				canvas.interactable = true;
				hide = false;
			}
		}

		public virtual void OnPointerDown(PointerEventData eventData, int id)
		{
		}

		public virtual void OnPointerUp(PointerEventData eventData, int id)
		{
			
		}

	}
}
