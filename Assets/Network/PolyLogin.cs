using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using UnityEngine.SceneManagement;

namespace PolyNetwork {

	public class PolyLogin : MonoBehaviour {

		public Canvas canvas;
		public PolyLoginScreen loginScreen;
		public string version = "0.0";
		public string password;
		public string username;
		public int playerID;
		public bool debugHost = false;
		public bool debugClient = false;
		public bool isSinglePlayer = false;

		void Awake(){
			DontDestroyOnLoad (this);
		}

		void Start() {
			loginScreen = canvas.GetComponent<PolyLoginScreen> ();
		}

		public void login() {
			
			if (!debugClient && !debugHost) {
				WWWForm form = new WWWForm ();
				form.AddField ("username", username);
				form.AddField ("password", password);
				form.AddField ("version", version);
				if (isSinglePlayer) {
					form.AddField ("singleplayer", "true");
				}
				WWW w = new WWW ("http://server.integerstudios.com/poly/login.php", form);    
				StartCoroutine (LogIn (w));
			} else {
				SceneManager.LoadScene ("main");
			}
		}

		public string toSHA(string password) {
			SHA1 sha = System.Security.Cryptography.SHA1.Create();
			byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(password);
			byte[] hash = sha.ComputeHash(inputBytes);

			string t = "";
			for (int i = 0; i < hash.Length; i++)
				t += string.Format("{0:x2}", hash[i]);
			return t;
		}

		private IEnumerator LogIn(WWW _w) {
			yield return _w; 

			if (_w.error == null) {
				if (_w.text.Contains("!!NO!!USER!!")) {
					//failed
					loginScreen.errorText.text = "User not found!";
					Debug.Log("NO USER");
				} else if (_w.text.Contains("!!USER!!ONLINE!!")) {
					//failed
					loginScreen.errorText.text = "You are already online!";
					Debug.Log("USER ONLINE");
				} else if (_w.text.Contains("!!WRONG!!VERSION!!")) {
					//failed
					loginScreen.errorText.text = "You have a different version than the server!";
					Debug.Log("WRONG VERSION");
				} else if (_w.text.Contains("!!SERVER!!OFFLINE!!")) {
					//failed
					loginScreen.errorText.text = "The server is currently offline!";
					Debug.Log("SERVER OFFLINE");
				} else {
					//connect
					playerID = int.Parse(_w.text);
					if (isSinglePlayer) {
						debugHost = true;
					}
					SceneManager.LoadScene ("main");
				}
			} else {
				Debug.Log(_w.error);
				loginScreen.errorText.text = "An Unknown Error Occured!";

				//php error
			}
		}

	}

}