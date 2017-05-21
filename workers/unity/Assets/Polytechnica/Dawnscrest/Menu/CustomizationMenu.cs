using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Polytechnica.Dawnscrest.Menu {

	public class CustomizationMenu : Menu {

		public Button play,back;

		public override void Start() {
			base.Start ();

			play.onClick.AddListener (OnPlay);
		}

		public override void Initialize() {

		}

		public override void Update() {
			base.Update ();
		}

		protected override void OnEnter() {

		}

		protected void OnPlay() {
			manager.LoadMenu (MenuManager.MenuType.LOADING);
		}


		protected void OnBack() {
			manager.LoadMenu (MenuManager.MenuType.MAIN);
		}

	}

}
