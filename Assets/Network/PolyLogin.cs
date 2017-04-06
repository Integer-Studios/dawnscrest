using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using UnityEngine.SceneManagement;
using System.IO;

namespace PolyNetwork {

	public class PolyLogin : MonoBehaviour {

		public Canvas canvas;
		public PolyLoginScreen loginScreen;
		public string version = "0.0";
		public string password;
		public string username;
		public bool loaded = false;
		public int playerID;
		public bool debugHost = false;
		public bool debugClient = false;
		public bool isSinglePlayer = false;

		void Awake(){
			DontDestroyOnLoad (this);
		}

		void Start() {
			loginScreen = canvas.GetComponent<PolyLoginScreen> ();

			JSONObject userJSON = new JSONObject(ReadString ("Assets/Resources/JSON/user.json"));
			if (userJSON.HasField ("username")) {
				loaded = true;

				username = userJSON.GetField ("username").str;
				password = userJSON.GetField ("password").str;
				loginScreen.setLogin ();
			}
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
		//"Assets/Resources/JSON/prefabs.json"
		private string ReadString(string path) {

			try {
				//Read the text from directly from the test.txt file
				StreamReader reader = new StreamReader(path); 
				string stringData = reader.ReadToEnd();
				reader.Close();
				return stringData;

			} catch (FileNotFoundException e) {
				return "";
			}

		}

		private void saveUser() {
			JSONObject obj = new JSONObject (JSONObject.Type.OBJECT);
			obj.AddField ("username", username);
			obj.AddField ("password", password);
			string str = obj.ToString ();
			string dir = "Assets/Resources/JSON/";

			File.WriteAllText(dir + "user.json", str);
		}

		private void deleteUser() {
			string str = "";
			string dir = "Assets/Resources/JSON/";

			File.WriteAllText(dir + "user.json", str);
		}

		private IEnumerator LogIn(WWW _w) {
			yield return _w; 

			if (_w.error == null) {
				if (_w.text.Contains("!!NO!!USER!!")) {
					//failed
					loginScreen.errorText.text = "User not found!";
					deleteUser ();
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
					saveUser ();

					SceneManager.LoadScene ("main");
				}
			} else {
				Debug.Log(_w.error);
				loginScreen.errorText.text = "An Unknown Error Occured!";
				deleteUser ();
				//php error
			}
		}

	}

}