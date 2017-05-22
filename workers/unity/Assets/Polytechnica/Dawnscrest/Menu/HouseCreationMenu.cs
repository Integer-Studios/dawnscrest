using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;

namespace Polytechnica.Dawnscrest.Menu {

	public class HouseCreationMenu : Menu {

		public Button next,back;
		public Text houseTitle, errorText;
		public InputField houseName;

		public override void Start() {
			base.Start ();
			next.onClick.AddListener (OnNext);
			back.onClick.AddListener (OnBack);
		}

		public override void Initialize() {
			if (manager.house.name.Length != 0) {
				houseName.text = manager.house.name;
			}
			errorText.text = "";

		}

		public override void Update() {
			base.Update ();
			houseTitle.text = houseName.text.ToUpper();
		}

		protected IEnumerator OnSave(WWW _w) {
			yield return _w; 
			Debug.Log (_w.text);

			if (_w.error == null) {
				try {
					MenuManager.SaveResponseData response = JsonUtility.FromJson<MenuManager.SaveResponseData> (_w.text);
					Debug.Log (response.ToString ());
					if (response.status != 200) {
						//failed
						if (response.status == 500)
							errorText.text = "House Creation Failed! Please Try Again.";
						else if (response.status == 403)
							errorText.text = "House names must be more than one character!";


						Debug.Log ("Failed");
					} else {
						manager.house.name = response.response;
						manager.LoadMenu (MenuManager.MenuType.CUSTOMIZATION);

					}
				} catch (System.ArgumentException e) {
					errorText.text = "House Creation Failed! Please Try Again.";

				}

			} else {
				Debug.Log(_w.error);
				errorText.text = "House Creation Failed! Please Try Again.";

				//php error
			}
		}

		protected void OnNext() {

			WWWForm form = new WWWForm ();

			form.AddField ("login", manager.house.loginId);
			form.AddField ("house", manager.house.id);
			form.AddField ("name", houseName.text);

			WWW w = new WWW ("http://cdn.polytechni.ca/house.php", form);    
			StartCoroutine (OnSave (w));

		}

		protected void OnBack() {
			manager.LoadMenu (MenuManager.MenuType.MAIN);
		}

	}

}
