using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Security.Cryptography;

namespace PolyMenu {
	
	public class PolyMenuLogin : PolyMenu {
		
		public InputField email;
		public InputField password;
		public Text errorText;
		public Button play, register;
		private bool loaded = false;
		private string savedEmail, savedPassword;
		private string passwordFiller = "*************";
		public override void Start() {
			base.Start ();
			play.onClick.AddListener(login);
			register.onClick.AddListener(account);

		}

		public override void Initialize() {
			if (PlayerPrefs.HasKey ("email")) {
				savedEmail = PlayerPrefs.GetString ("email");
				savedPassword = PlayerPrefs.GetString ("password");
				email.text = savedEmail;
				password.text = passwordFiller;
				loaded = true;
			}
		}

		public override void Update() {
			if (loaded && (email.text != savedEmail || password.text != passwordFiller)) {
				loaded = false;
				savedPassword = "";
				savedEmail = "";
				password.text = "";
			}
			base.Update ();
		}

		protected override void onEnter() {
			login ();
		}
			
		public void login() {
			string encrypted = savedPassword;
			if (!loaded)
				encrypted = toSHA (password.text);
			if (email.text.Length <= 0) {
				errorText.text = "You must enter a username!";
				return;
			}
			if (password.text.Length <= 0) {
				errorText.text = "You must enter a password!";
				return;
			}
			if (!manager.debug) {
				WWWForm form = new WWWForm ();
				form.AddField ("email", email.text);
				form.AddField ("password", encrypted);
				form.AddField ("version", manager.version);

				WWW w = new WWW ("http://cdn.polytechni.ca/login.php", form);    
				StartCoroutine (onLogin (w));
			} else {
//				SceneManager.LoadScene ("main");
			}
		}

		private IEnumerator onLogin(WWW _w) {
			yield return _w; 

			if (_w.error == null) {
				PolyMenuManager.HouseData house = JsonUtility.FromJson<PolyMenuManager.HouseData> (_w.text);
				Debug.Log (house.ToString ());
				if (house.status != 200) {
					//failed
					errorText.text = house.error;
					Debug.Log ("NO USER");
				} else {
					//connect

					saveUser (house);
					manager.house = house;
					manager.loadMenu (PolyMenuManager.Menu.MAIN);
					//					SceneManager.LoadScene ("main");
				}
			} else {
				Debug.Log(_w.error);
				errorText.text = "An Unknown Error Occured!";
				deleteUser ();
				//php error
			}
		}
			
		public void account() {
			Application.OpenURL("http://play.polytechni.ca/register.php");
		}

		private string toSHA(string password) {
			SHA1 sha = System.Security.Cryptography.SHA1.Create();
			byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(password);
			byte[] hash = sha.ComputeHash(inputBytes);

			string t = "";
			for (int i = 0; i < hash.Length; i++)
				t += string.Format("{0:x2}", hash[i]);
			return t;
		}
			
		private void saveUser(PolyMenuManager.HouseData house) {
			PlayerPrefs.SetString ("email", house.email);
			PlayerPrefs.SetString ("password", house.password);
			PlayerPrefs.Save ();
		}

		private void deleteUser() {
			if (PlayerPrefs.HasKey("email"))
				PlayerPrefs.DeleteKey ("email");
			if (PlayerPrefs.HasKey("password"))
				PlayerPrefs.DeleteKey ("password");
			PlayerPrefs.Save ();
		}

	}

}
