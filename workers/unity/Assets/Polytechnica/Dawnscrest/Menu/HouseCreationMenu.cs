using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Polytechnica.Dawnscrest.Menu {

	public class HouseCreationMenu : Menu {

		public Button next,back;
		public Text houseTitle;
		public InputField surname;

		public override void Start() {
			base.Start ();
			next.onClick.AddListener (OnNext);
			back.onClick.AddListener (OnBack);
		}

		public override void Initialize() {

		}

		public override void Update() {
			base.Update ();
			houseTitle.text = "House of: " + surname.text;
		}

		protected override void OnEnter() {

		}

		protected void OnNext() {
			manager.LoadMenu (MenuManager.MenuType.CUSTOMIZATION);
		}

		protected void OnBack() {
			manager.LoadMenu (MenuManager.MenuType.MAIN);
		}

	}

}
