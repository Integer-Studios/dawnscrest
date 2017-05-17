using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	public float speed = 5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (new Vector3 (Input.GetAxis ("Horizontal")*Time.deltaTime*speed, 0f, Input.GetAxis ("Vertical")*Time.deltaTime*speed));
	}
}
