using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using Polytechnica.Dawnscrest.GUI;

namespace Polytechnica.Dawnscrest.Menu {

	public class MenuManager : MonoBehaviour {

		public Canvas canvas;
		public Menu[] menus;
		public UnityEngine.UI.Image menuBackground;

		public string version = "0.0";
		public bool debug = false;

		[HideInInspector]
		public HouseData house;
		public BannerData banner = null;

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
			//Waits for all menus to fade out before fading new one in
			StartCoroutine (FadeInMenu(toLoad));

		}

		/*
		 *	Fades out all menus AND BG
		 */
		public void HideMenus() {
			foreach (Menu m in menus) {
				m.FadeOut ();
			}
			menuBackground.color = new Color (0, 0, 0, 0);
		}

		/*
		 *	Triggered when loading screen is fully visible
		 */
		public IEnumerator StartGame(LoadingMenu m) {
			m.async =  SceneManager.LoadSceneAsync ("UnityClient");
			yield return m.async;
		}

		/*
		 *	Triggered when local player has SPAWNED
		 */
		public void OnGameStarted() {

			canvas.gameObject.GetComponent<CanvasGroup> ();

			HideMenus ();
//			GUIManager.Show ();
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
			public string banner;
			public override string ToString () {
				return string.Format ("[HouseData: loginId={0}, name={1}, character={2}, spawned={3}, email={4}, password={5}, status={6}, error={7}, house={8}, irl={9}], banner={10}", loginId, name, character, spawned, email, password, status, error, id, irl, banner);
			}
		}

		[Serializable]
		public class BannerData {
			public int layout;
			public int colorOne;
			public int colorTwo;
			public int sigilColor;
			public override string ToString ()	{
				return string.Format ("[BannerData]: layout={0}, colorOne={1}", layout, colorOne);
			}
		}

		[Serializable]
		public class SaveResponseData {
			public int status;
			public string error;
			public string response;
			public override string ToString () {
				return string.Format ("[SaveResponseData]: status={0}, error={1}", status, error);
			}
		}

		private IEnumerator FadeInMenu(Menu toLoad) {
			yield return new WaitForSeconds(0.5F);
			if (toLoad != null) {
				toLoad.Initialize ();
				toLoad.FadeIn ();
			}
		}

	}

}