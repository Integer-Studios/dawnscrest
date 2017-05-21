using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polytechnica.Dawnscrest.Menu {

	public class LoadingMenu : Menu {

		public override void Start() {
			base.Start ();

		}

		public override void Initialize() {

		}

		public override void Update() {
			base.Update ();
		}
			
		protected override void OnShow() {
			Debug.Log ("Starting Game...");
			manager.StartGame ();
		}

	}

}