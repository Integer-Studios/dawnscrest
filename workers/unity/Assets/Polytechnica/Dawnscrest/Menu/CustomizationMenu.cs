using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Polytechnica.Dawnscrest.Menu {

	public class CustomizationMenu : Menu {

		public Button play;
		public Text errorText;
		public Dropdown gender, heritage, virtue, vice;

		public override void Start() {
			base.Start ();

			play.onClick.AddListener (OnPlay);
		}

		public override void Initialize() {
			errorText.text = "";

		}

		public override void Update() {
			base.Update ();
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
							errorText.text = "Heritage Creation Failed! Please Try Again.";

						Debug.Log ("Customization Failed");
					} else {
						Debug.Log("Customization Saved");
						manager.LoadMenu (MenuManager.MenuType.LOADING);

					}
				} catch (System.ArgumentException e) {
					errorText.text = "Heritage Creation Failed! Please Try Again.";

				}

			} else {
				Debug.Log(_w.error);
				errorText.text = "Heritage Creation Failed! Please Try Again.";
			}
		}

		protected void OnPlay() {

			WWWForm form = new WWWForm ();

			form.AddField ("house", manager.house.id);
			form.AddField ("gender", gender.value);
			form.AddField ("heritage", heritage.value);
			form.AddField ("virtue", virtue.value);
			form.AddField ("vice", vice.value);

			WWW w = new WWW ("http://cdn.polytechni.ca/heritage.php", form);    
			StartCoroutine (OnSave (w));


		}

	}

}
