using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Polytechnica.Dawnscrest.Player;
using System.Text.RegularExpressions;

namespace Polytechnica.Dawnscrest.Menu {

	public class CustomizationMenu : Menu {

		public Button play, left, right, randomize;
		public Text errorText;
		public Dropdown gender, heritage, virtue, vice;
		public GameObject character;
		public OptionCycler hair, hairColor, eyeColor, build, facialHair, name, eyebrowFuck;
		private AppearanceSet appearance;
		public Text nameText;

		private int heritageValue;

		public override void Start() {
			base.Start ();
			character.SetActive (false);

			play.onClick.AddListener (OnPlay);
			randomize.onClick.AddListener (NewName);
		}

		public override void Initialize() {
			errorText.text = "";
			character.SetActive (true);
			appearance = new AppearanceSet ();
			UpdateAppearance ();
			nameText.text = "Ramos " + manager.house.name;
			NewName ();

		}

		public override void Update() {
			base.Update ();
			if (r == 1) {
				character.transform.eulerAngles = new Vector3 (character.transform.eulerAngles.x, character.transform.eulerAngles.y + 2F, character.transform.eulerAngles.z);
			} else if (r == -1) {
				character.transform.eulerAngles = new Vector3 (character.transform.eulerAngles.x, character.transform.eulerAngles.y - 2F, character.transform.eulerAngles.z);
			}

			if (heritageValue != heritage.value) {
				heritageValue = heritage.value;
				NewName ();
			}

			if (appearance != null)
				UpdateAppearance ();
		}

		public void UpdateAppearance() {
			if (gender.value == 0)
				appearance.sex = false;
			else
				appearance.sex = true;
			appearance.build = build.current;
			appearance.hair = hair.current;
			appearance.hairColor = hairColor.current;
			appearance.facialHair = facialHair.current;
			appearance.eyeColor = eyeColor.current;
			appearance.eyebrows = eyebrowFuck.current;
			character.GetComponent<AppearanceVisualizer> ().SetAppearance (appearance);
		}

		protected IEnumerator OnSave(WWW _w) {
			yield return _w; 
			Debug.Log (_w.text);
			Debug.Log (appearance.ToString());
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
						manager.genetics = appearance;
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

		protected void NewName() {
			WWWForm form = new WWWForm ();

			form.AddField ("heritage", heritage.value);
			form.AddField ("gender", gender.value);

			WWW w = new WWW ("http://cdn.polytechni.ca/fetch_name.php", form);    
			StartCoroutine (OnNewName (w));
		}

		protected IEnumerator OnNewName(WWW _w) {
			yield return _w; 
			
			if (_w.error == null) {
				try {
					nameText.text = Regex.Replace(_w.text, @"\s+", "") + " " + manager.house.name;
				} catch (System.ArgumentException e) {
					errorText.text = "Name Fetch Failed! Please Try Again.";

				}

			} else {
				Debug.Log(_w.error);
				errorText.text = "Name Fetch Failed! Please Try Again.";

				//php error
			}
		}

		int r = 0; 

		public override void OnPointerDown(PointerEventData eventData, int id) {


			if (id == 0) {
				//left
				r = 1;
			} else if (id == 1) {
				r = -1;
			} else {
				r = 0;
			}
		}

		public override void OnPointerUp(PointerEventData eventData, int id) {
			r = 0;
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

		public override void FadeOut(bool fade = true) {
			base.FadeOut (fade);
			character.SetActive (false);
		}

	}

}
