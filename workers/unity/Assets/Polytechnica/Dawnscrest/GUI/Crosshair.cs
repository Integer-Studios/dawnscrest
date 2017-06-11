using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Polytechnica.Dawnscrest.GUI {

	/*
	 * Crosshair Component - Requires an Image and text as children
	 * 
	 * This is probably going to have a bit of a mod when items and item holding and tools are in TODO
	 */
	public class Crosshair : MonoBehaviour {

		public Sprite minimizedSprite;
		public Color minimizedColor;
		public Sprite interactableSprite;
		public Color interactableColor;
		// Temporary plug until tools are in
		public Sprite tempTooltipSprite;
		public Color tooltipColor;

		public float animationSpeed = 8f;
		public float minSize = 10f;
		public float maxSize = 40f;

		private Image image;
		private Text tooltip;

		private int transitionToState = -1;
		private int currentState = -1;
		private CrosshairContent onCompleteContent;

		private void OnEnable() {
			image = GetComponentInChildren<Image> ();
			tooltip = GetComponentInChildren<Text> ();
		}

		/*
		 * 
		 * Public Interface
		 * 
		 */

		public void setFill(float f) {
			image.fillAmount = f;
		}

		/*
		 * Minimizes Crosshair
		 */
		public void SetMinimized() {
			Transition(new CrosshairContent(null, minimizedColor, ""), new CrosshairContent(minimizedSprite, minimizedColor, ""), -1);
		}

		/*
		 * Maximizes Crosshair with open circle and message
		 */
		public void SetInteractable(string message) {
			Transition(new CrosshairContent(interactableSprite, interactableColor, ""), new CrosshairContent(interactableSprite, interactableColor, message), 1);
		}

		/*
		 * Tooltip more or less requires tools
		 * TODO use the tooltype enum to pass into
		 * this to decide the icon
		 */
		public void SetTooltip(string message) {
			Transition(new CrosshairContent(tempTooltipSprite, tooltipColor, ""), new CrosshairContent(tempTooltipSprite, tooltipColor, message), 1);
		}

		/*
		 * 
		 * Internal
		 * 
		 */

		/*
		 * By using state variables the crosshair updates its transition or completes
	     * On update - You can call its public functions either on tick or just once, this
		 * update will ensure it works either way
		 */
		private void Update() {
			if (transitionToState == -1 && currentState == 1) {
				// minimize
				if (image.rectTransform.sizeDelta.x > minSize) {
					float delta = Mathf.Min (image.rectTransform.sizeDelta.x - minSize, animationSpeed * Time.deltaTime);
					image.rectTransform.sizeDelta -= new Vector2 (delta, delta);
				} else
					currentState = transitionToState;
			} else if (transitionToState == 1 && currentState == -1) {
				// expand
				if (image.rectTransform.sizeDelta.x < maxSize) {
					float delta = Mathf.Min (maxSize - image.rectTransform.sizeDelta.x, animationSpeed * Time.deltaTime);
					image.rectTransform.sizeDelta += new Vector2 (delta, delta);
				} else
					currentState = transitionToState;
			} else if (onCompleteContent != null) {
				// just finished
				setContent(onCompleteContent);
				onCompleteContent = null;
			}
		}

		/*
		 * This is the main abstraction to call a transition - sets content on start, sets a transition to fire
		 * and then queues a content set for on complete
		 */
		private void Transition(CrosshairContent start, CrosshairContent end, int toState) {

			// Skip Transitioning if its already expanded or minimized
			if (currentState == toState)
				setContent (end);
			else {
				image.fillAmount = 1f;
				setContent (start);
				transitionToState = toState;
				onCompleteContent = end;
			}

		}

		private void setContent(CrosshairContent c) {
			if (c.sprite != null)
				image.sprite = c.sprite;
			if (c.color != null) {
				image.color = c.color;
				tooltip.color = c.color;
			}
			if (c.msg != null)
				tooltip.text = c.msg;
		}

		private class CrosshairContent {
			public Sprite sprite;
			public Color color;
			public string msg;
			public CrosshairContent(Sprite s,Color c,  string m) {
				sprite = s;
				color = c;
				msg = m;
			}
		}

	}

}