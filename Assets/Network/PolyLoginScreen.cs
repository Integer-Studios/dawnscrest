using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace PolyNetwork {

	public class PolyLoginScreen : MonoBehaviour {

		public InputField username;
		public InputField password;
		public Text errorText;
		public Button playBtn;
		public Button debugHost;
		public Button debugClient;
		public PolyLogin login;
		private EventSystem eventSystem;

		void Start() {
			this.eventSystem = EventSystem.current;
			login = FindObjectOfType<PolyLogin> ();

			playBtn.onClick.AddListener(onPlay);
			if (!login.isSinglePlayer) {
				debugHost.onClick.AddListener (onDebugHost);
				debugClient.onClick.AddListener (onDebugClient);
			} else {
				debugHost.enabled = false;
				debugClient.enabled = false;
			}
		}

		public void setLogin() {

			if (login.loaded) {
				username.text = login.username;
				password.text = "**************";
			}
		}

		void Update() {
			Selectable current = null;
			if (username.text != login.username && login.loaded) {
				login.password = "";
				login.username = "";
				login.loaded = false;
				password.text = "";
			}
			// Figure out if we have a valid current selected gameobject
			if (eventSystem.currentSelectedGameObject != null) {
				// Unity doesn't seem to "deselect" an object that is made inactive
				if (eventSystem.currentSelectedGameObject.activeInHierarchy) {
					current = eventSystem.currentSelectedGameObject.GetComponent<Selectable>();
				}
			}
			if (Input.GetKeyDown (KeyCode.Tab)) {
				Selectable next = null;
				if (current != null) {
					if (current.name == "Username") {
						next = password;
					} else if (current.name == "Password") {
						next = username;
					}
				}

				if (next != null) {
					next.Select ();
				}

			}

			if (Input.GetKeyDown (KeyCode.Return) && current != null && !debugClient && !debugHost) {
				onPlay ();
			}
		}

		private void onPlay() {
			login.username = username.text;
			if (!login.loaded)
				login.password = login.toSHA (password.text);
			if (username.text.Length <= 0) {
				errorText.text = "You must enter a username!";
				return;
			}
			if (password.text.Length <= 0) {
				errorText.text = "You must enter a password!";
				return;
			}
			login.login ();
			Debug.Log (username.text + " " + password.text);
		}

		private void onDebugHost() {

			login.debugHost = true;
			login.login ();
			Debug.Log ("Debug Host");
		}

		private void onDebugClient() {

			login.debugClient = true;
			login.login ();
			Debug.Log ("Debug Client");
		}
	}
}
