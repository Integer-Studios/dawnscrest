using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace PolyMenu {

	public class PolyMenuCustomization : PolyMenu {

		public Button play,back;

		public override void Start() {
			base.Start ();

			play.onClick.AddListener (onPlay);
		}

		public override void Initialize() {

		}

		public override void Update() {
			base.Update ();
		}

		protected override void onEnter() {

		}

		protected void onPlay() {
			manager.loadMenu (PolyMenuManager.Menu.LOADING);
		}


		protected void onBack() {
			manager.loadMenu (PolyMenuManager.Menu.MAIN);
		}

	}

}
