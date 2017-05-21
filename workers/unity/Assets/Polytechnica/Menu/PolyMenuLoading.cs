using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyMenu {

	public class PolyMenuLoading : PolyMenu {

		public override void Start() {
			base.Start ();

		}

		public override void Initialize() {

		}

		public override void Update() {
			base.Update ();
		}
			
		protected override void onShow() {
			Debug.Log ("Starting Game...");
			manager.startGame ();
		}

	}

}