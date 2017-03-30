using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; 

[CustomEditor(typeof(PolyNetworkManager))]
public class PolyDataRipperEditor : Editor {

	public override void OnInspectorGUI() {

		DrawDefaultInspector();
		PolyNetworkManager manager = (PolyNetworkManager)target;
		if (GUILayout.Button ("Rip Data")) {
			PolyDataRipper.rip (manager);
		}
	}

}