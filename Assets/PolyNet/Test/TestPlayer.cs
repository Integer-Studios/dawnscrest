using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNet;

public class TestPlayer : PolyNetBehaviour {

	// Use this for initialization
	void Start () {
		if (identity.isLocalPlayer) {
			Camera.main.transform.SetParent (transform);
			Camera.main.transform.rotation = Quaternion.identity;
			Camera.main.transform.position = new Vector3 (0f, 3f, -3f);
		}
	}

	// Update is called once per frame
	void Update () {
		if (!identity.isLocalPlayer)
			return;

		transform.Translate (new Vector3 (0, 0, 10f * Time.deltaTime * Input.GetAxis("Vertical")));
		transform.Rotate (new Vector3 (0, 1f * Input.GetAxis("Horizontal"), 0f));
	}

}
