using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Polytechnica.Dawnscrest.World {

	[CustomEditor(typeof(WorldCreator))]
	public class WorldCreatorEditor : UnityEditor.Editor {
		
		public override void OnInspectorGUI() {

			DrawDefaultInspector();
			WorldCreator creator = (WorldCreator)target;

			if (GUILayout.Button ("Save Objects")) {
				SaveObjects ();
			}
		}

		private void SaveObjects() {

			WorldObjectChunk[,] chunks = new WorldObjectChunk[WorldTerrain.size / WorldTerrain.chunkSize, WorldTerrain.size / WorldTerrain.chunkSize];
			for (int z = 0; z < chunks.GetLength (1); z++) {
				for (int x = 0; x < chunks.GetLength (0); x++) {
					chunks [x, z] = new WorldObjectChunk ();
				}
			}

			GameObject[] g = FindObjectsOfType<GameObject> ();
			foreach (GameObject obj in g) {
				Object prefab = PrefabUtility.GetPrefabParent (obj);
				if (prefab == null)
					continue;
				string name = GetEntityName(AssetDatabase.GetAssetPath(prefab));
				if (name == null)
					continue;
				WorldObject w = new WorldObject (name, Polytechnica.Dawnscrest.Core.EntityTemplateType.Basic, obj.transform.position, obj.transform.eulerAngles, obj.transform.localScale);
				ChunkIndex index = WorldTerrain.ToChunkIndex (obj.transform.position);
				chunks [index.x, index.z].objects.Add (w);
			}

			for (int z = 0; z < chunks.GetLength (1); z++) {
				for (int x = 0; x < chunks.GetLength (0); x++) {
					File.WriteAllText(WorldLoader.worldObjectsDirectory+x+"-"+z+".chunk", JsonUtility.ToJson (chunks [x, z]));
				}
			}
		}

		private string GetEntityName(string path) {
			string[] pieces = path.Split ('/');
			if (pieces.Length < 2)
				return null;
			if (pieces [1] != "EntityPrefabs")
				return null;
			return path.Substring (21).Split('.')[0];
		}

	}

}
