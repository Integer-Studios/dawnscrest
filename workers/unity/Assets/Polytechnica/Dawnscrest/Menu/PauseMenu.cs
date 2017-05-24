using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Polytechnica.Dawnscrest.Core;

namespace Polytechnica.Dawnscrest.Menu {

	public class PauseMenu : Menu {

		public Button logout, close;

		public override void Start() {
			base.Start ();
			logout.onClick.AddListener (OnLogout);

			close.onClick.AddListener (OnClose);
		}

		public override void Initialize() {
		}

		public override void Update() {
			base.Update ();

		}

		protected void OnLogout() {
			BodyFinder.Logout ();
		}

		protected void OnClose() {
			this.FadeOut ();
		}

		protected override void OnShow() {

		}

	}

}