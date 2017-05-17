using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PolyWorld {
	
	[CustomEditor(typeof(WorldTerrain))]
	public class WorldTerrainEditor : Editor {

		public override void OnInspectorGUI() {

			DrawDefaultInspector();
			WorldTerrain terrain = (WorldTerrain)target;

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.TextField("Raw-16 Heightmap", System.IO.Path.GetFileName(terrain.rawFile));
			if (GUILayout.Button("Select", EditorStyles.miniButton, GUILayout.Width(45.0f)))
			{
				terrain.rawFile = EditorUtility.OpenFilePanelWithFilters("Select Raw file", terrain.rawFile, new string[] { "16-bit Raw", "r16,raw", "Allfiles", "*" });

			}
			EditorGUILayout.EndHorizontal();
//
//			if (GUILayout.Button ("Load Chunks")) {
//				terrain.editor_loadChunks ();
//			}
//			if (GUILayout.Button ("Save Chunks")) {
//				terrain.editor_saveChunks ();
//			}
//			if (GUILayout.Button ("Clear")) {
//				terrain.editor_clearChunks ();
//			}
//			if (GUILayout.Button ("Overwrite Heightmap")) {
//				terrain.editor_saveHeightmap ();
//			}
		}

	}

}