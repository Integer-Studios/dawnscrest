using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Security.Cryptography;

namespace PolyMenu {

	public class PolyMenuMain : PolyMenu {

		public Text nameText;
		public Button enter, house, settings, logout;


		public override void Start() {
			base.Start ();
			enter.onClick.AddListener(onEnterWorld);
			house.onClick.AddListener(onHouse);
			settings.onClick.AddListener(onSettings);
			logout.onClick.AddListener(onLogout);

		}

		public override void Initialize() {
			nameText.text = manager.house.irl;
			if (!manager.house.spawned) {
				house.interactable = false;
			}
		}

		public override void Update() {
			base.Update ();
		}

		protected override void onEnter() {

		}

		protected void onEnterWorld() {
			if (manager.house.spawned == false) {
				manager.loadMenu (PolyMenuManager.Menu.HOUSE_CREATION);
			} else {
				manager.loadMenu (PolyMenuManager.Menu.LOADING);
			}
		}

		protected void onHouse() {
			Debug.Log ("House Click");
		}

		protected void onSettings() {
			Debug.Log ("Set Click");
		}

		protected void onLogout() {
			manager.loadMenu (PolyMenuManager.Menu.LOGIN);
		}

	}

}
