using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PolyPlayer {

	public class Crosshair : MonoBehaviour {

		public Sprite expandedImage;
		public Sprite minimizedImage;
		public float animationSpeed = 8f;

		private int state = 0;
		private Image image;

		/*
		* 
		* Public Interface
		* 
		*/

		public void expand() {
			if (state == 2) 
				return;

			if (state == 0)
				setState (1);

			if (image.rectTransform.sizeDelta.x >= 100)
				setState (2);
			else
				image.rectTransform.sizeDelta+= new Vector2(animationSpeed,animationSpeed)*Time.deltaTime;

		}

		public void minimize() {
			if (state == 0) 
				return;

			if (state == 2)
				setState (1);

			if (image.rectTransform.sizeDelta.x <= 10)
				setState (0);
			else
				image.rectTransform.sizeDelta-= new Vector2(animationSpeed,animationSpeed)*Time.deltaTime;
		}

		public void setFill(float f) {
			image.fillAmount = f;
		}


		/*
		* 
		* Private
		* 
		*/

		private void Start() {
			image = GetComponent<Image> ();
		}

		private void setState(int i) {
			state = i;
			switch (state) {
			//minimzed fully
			case 0:
				image.rectTransform.sizeDelta = new Vector2 (10, 10);
				image.sprite = minimizedImage;
				setFill(1f);
				break;
				//in transition
			case 1:
				image.sprite = expandedImage;
				break;
				//expanded fully
			case 2:
				image.rectTransform.sizeDelta = new Vector2(100,100);
				image.sprite = expandedImage;
				break;
			}
		}
	}

}