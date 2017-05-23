using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Polytechnica.Dawnscrest.Menu {

	public class PauseMenu : Menu {

		public Button logout, close;

		public override void Start() {
			base.Start ();
			close.onClick.AddListener (OnClose);
		}

		public override void Initialize() {
		}

		public override void Update() {
			base.Update ();

		}

		protected void OnClose() {
			this.FadeOut ();
		}

		protected override void OnShow() {

		}

	}

}