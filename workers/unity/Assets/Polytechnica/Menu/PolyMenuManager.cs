using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

namespace PolyMenu {

	public class PolyMenuManager : MonoBehaviour {

		public Canvas canvas;
		public PolyMenu[] menus;

		public string version = "0.0";
		public bool debug = false;

		[HideInInspector]
		public HouseData house;

		private int started = 0;

		void Awake(){
			DontDestroyOnLoad (this);
		}

		void Start() {
		}

		public void onMenuStart(PolyMenu m) {
			m.fadeOut (false);
			started++;
			if (started == menus.Length) {
				loadMenu (Menu.LOGIN, false);
			}
		}

		public void loadMenu(Menu type, bool fade = true) {
			PolyMenu toLoad = null;
			foreach (PolyMenu m in menus) {
				if (m.type == type) {
					toLoad = m;
				} else {
					m.fadeOut (fade);
				}
 			}
			if (toLoad != null) {
				toLoad.Initialize ();
				toLoad.fadeIn (fade);
			}
		}

		public void startGame() {
			SceneManager.LoadScene ("UnityClient");
		}

		public void onGameStarted() {
			Destroy (canvas.gameObject);
		}

		public enum Menu {
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