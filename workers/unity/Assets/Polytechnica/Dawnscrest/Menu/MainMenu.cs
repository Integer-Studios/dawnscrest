using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Security.Cryptography;
using Polytechnica.Dawnscrest.Core;

namespace Polytechnica.Dawnscrest.Menu {

	public class MainMenu : Menu {

		public Text nameText;
		public Button enter, house, settings, logout;


		public override void Start() {
			base.Start ();
			enter.onClick.AddListener(OnEnterWorld);
			house.onClick.AddListener(OnHouse);
			settings.onClick.AddListener(OnSettings);
			logout.onClick.AddListener(OnLogout);

		}

		public override void Initialize() {
			nameText.text = SettingsManager.house.irl;
			if (!SettingsManager.house.spawned) {
				house.interactable = false;
			}
		}

		public override void Update() {
			base.Update ();
		}

		protected override void OnEnter() {

		}

		protected void OnEnterWorld() {
			if (SettingsManager.house.spawned == false) {
				manager.LoadMenu (MenuManager.MenuType.HOUSE_CREATION);
			} else {
				manager.LoadMenu (MenuManager.MenuType.LOADING);
			}
		}

		protected void OnHouse() {
			Debug.Log ("House Click");
		}

		protected void OnSettings() {
			Debug.Log ("Set Click");
		}

		protected void OnLogout() {
			manager.LoadMenu (MenuManager.MenuType.LOGIN);
		}

	}

}
