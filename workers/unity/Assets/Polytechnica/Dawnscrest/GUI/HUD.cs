using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Polytechnica.Dawnscrest.Player;

namespace Polytechnica.Dawnscrest.GUI {

	public class HUD : MonoBehaviour {

		public Image profilePic;
		public Slider thirstSlider;
		public Slider hungerSlider;
		public Slider healthSlider;
		public Text thirstText;
		public Text hungerText;
		public Text healthText;

		private void Start() {
			SetPortraitAppearance (null);
		}

		public void SetPortraitAppearance(AppearanceSet a) {
			profilePic.sprite = GUIManager.photobooth.GetProfilePicture (a);
		}

		public void SetThirst(float value, float maxValue) {
			thirstSlider.maxValue = maxValue;
			thirstSlider.value = value;
			thirstText.text = "thirst  " + Mathf.Round(value) + " / " + maxValue;
		}

		public void SetHunger(float value, float maxValue) {
			hungerSlider.maxValue = maxValue;
			hungerSlider.value = value;
			hungerText.text = "hunger  " + Mathf.Round(value) + " / " + maxValue;
		}

		public void SetHealth(float value, float maxValue) {
			healthSlider.maxValue = maxValue;
			healthSlider.value = value;
			healthText.text = "health  " + Mathf.Round(value) + " / " + maxValue;
		}
	}

}
