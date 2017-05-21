using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

namespace Polytechnica.Dawnscrest.Menu {

	public class MenuManager : MonoBehaviour {

		public Canvas canvas;
		public Menu[] menus;

		public string version = "0.0";
		public bool debug = false;

		[HideInInspector]
		public HouseData house;

		private int started = 0;

		void Awake(){
			DontDestroyOnLoad (this);
			DontDestroyOnLoad (canvas);
		}

		void Start() {
		}

		/*
		 *	Triggered when a menu has started and grabbed its components 
		 */
		public void OnMenuStart(Menu m) {
			m.FadeOut (false);
			started++;
			if (started == menus.Length) {
				LoadMenu (MenuType.LOGIN, false);
			}
		}

		/*
		 *	Fades out all other menus and loads selected menu
		 */
		public void LoadMenu(MenuType type, bool fade = true) {
			Menu toLoad = null;
			foreach (Menu m in menus) {
				if (m.type == type) {
					toLoad = m;
				} else {
					m.FadeOut (fade);
				}
 			}
			if (toLoad != null) {
				toLoad.Initialize ();
				toLoad.FadeIn (fade);
			}
		}

		/*
		 *	Triggered when loading screen is fully visible
		 */
		public void StartGame() {
			SceneManager.LoadScene ("UnityClient");
		}

		/*
		 *	Triggered when local player has SPAWNED
		 */
		public void OnGameStarted() {

			canvas.gameObject.GetComponent<CanvasGroup> ();

			Destroy (canvas.gameObject);
		}

		/*
		 *	Must make new enum for each menu and set in editor
		 */
		public enum MenuType {
			LOGIN,
			MAIN,
			SETTINGS,
			HOUSE_CREATION,
			CUSTOMIZATION,
			LOADING
		}
			
		[Serializable]
		public class HouseData {
			public int loginId;
			public int id;
			public string irl;
			public string name;
			public int character;
			public bool spawned;
			public string email;
			public string password;
			public int status;
			public string error;
			public override string ToString () {
				return string.Format ("[HouseData: loginId={0}, name={1}, character={2}, spawned={3}, email={4}, password={5}, status={6}, error={7}, house={8}, irl={9}]", loginId, name, character, spawned, email, password, status, error, id, irl);
			}
		}

	}

}