using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace PolyMenu {

	public class PolyMenuHouseCreation : PolyMenu {

		public Button next,back;
		public Text houseTitle;
		public InputField surname;

		public override void Start() {
			base.Start ();
			next.onClick.AddListener (onNext);
			back.onClick.AddListener (onBack);
		}

		public override void Initialize() {

		}

		public override void Update() {
			base.Update ();
			houseTitle.text = "House of: " + surname.text;
		}

		protected override void onEnter() {

		}

		protected void onNext() {
			manager.loadMenu (PolyMenuManager.Menu.CUSTOMIZATION);
		}

		protected void onBack() {
			manager.loadMenu (PolyMenuManager.Menu.MAIN);
		}

	}

}
