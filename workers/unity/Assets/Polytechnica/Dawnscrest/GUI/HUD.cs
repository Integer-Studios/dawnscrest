using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Polytechnica.Dawnscrest.Player;

namespace Polytechnica.Dawnscrest.GUI {

	public class HUD : MonoBehaviour {

		public Camera photoboothCam;
		public AppearanceVisualizer photoboothCharacter;
		public RawImage portrait;
		public Slider thirstSlider;
		public Slider hungerSlider;
		public Slider healthSlider;
		public Text thirstText;
		public Text hungerText;
		public Text healthText;

		public void setPortraitAppearanceFromUpdate(CharacterAppearance.Update update) {
			// Change the guy
			photoboothCharacter.setAppearanceFromUpdate (update);
			// Take a picture of the new guy
			photoboothCam.Render();
		}

		public void setThirst(float value, float maxValue) {
			thirstSlider.maxValue = maxValue;
			thirstSlider.value = value;
			thirstText.text = "thirst  " + Mathf.Round(value) + " / " + maxValue;
		}

		public void setHunger(float value, float maxValue) {
			hungerSlider.maxValue = maxValue;
			hungerSlider.value = value;
			hungerText.text = "hunger  " + Mathf.Round(value) + " / " + maxValue;
		}

		public void setHealth(float value, float maxValue) {
			healthSlider.maxValue = maxValue;
			healthSlider.value = value;
			healthText.text = "health  " + Mathf.Round(value) + " / " + maxValue;
		}
	}

}
