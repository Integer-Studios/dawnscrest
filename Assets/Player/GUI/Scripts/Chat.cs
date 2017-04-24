using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PolyPlayer {

	public class Chat : MonoBehaviour {
		
		public InputField input;
		public Text textPrefab;

		public void setInputEnabled(bool e) {
			if (!e)
				input.text = "";
			
			input.gameObject.SetActive(e);
			if (e) {
				input.transform.SetAsLastSibling ();			
				input.ActivateInputField ();
			}
		}

		public void displayMessage(string s, float distance) {
			GameObject g = Instantiate (textPrefab.gameObject);
			Text t = g.GetComponent<Text> ();
			t.color = new Color(255, 255, 255, 1 - distance);
			t.transform.SetParent (transform, false);
			t.text = s;
			input.transform.SetAsLastSibling ();
		}

		public void onInputSubmitted(string s) {
//			GUIManager.player.sendChat(s);
//			displayMessage("User: " + s);
			GUIManager.setChatOpen (false);
		}

		private void Start() {
			setInputEnabled (false);
		}
	}

}