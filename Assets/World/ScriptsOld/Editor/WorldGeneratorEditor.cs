using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PolyWorldOld {

	[CustomEditor(typeof(WorldGenerator))]
	public class WorldGeneratorEditor : Editor {

		public override void OnInspectorGUI() {

			DrawDefaultInspector();
			WorldGenerator gen = (WorldGenerator)target;
			if (GUILayout.Button ("Generate")) {
				gen.editor_generate ();
			}
			if (GUILayout.Button ("Clear")) {
				gen.editor_clear ();
			}
		}

	}

}