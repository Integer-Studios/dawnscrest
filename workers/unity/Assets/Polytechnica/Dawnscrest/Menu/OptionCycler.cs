using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionCycler : MonoBehaviour {

	public Button prev, next;
	public int type;
	public int max;
	public int current;

	public void Start() {
		prev.onClick.AddListener (OnPrevious);
		next.onClick.AddListener (OnNext);

	}

	protected void OnPrevious() {
		if (current > 0) {
			current--;
		} else {
			current = max;
		}
	}

	protected void OnNext() {
		if (current < max) {
			current++;
		} else {
			current = 0; 
		} 
	}

}
