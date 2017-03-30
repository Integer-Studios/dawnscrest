using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PolyDataRipper : ScriptableObject {

	public static Dictionary<int, string> prefabs = new Dictionary<int, string>();
	public static JSONObject rippedObjects;

	public static void rip(PolyNetworkManager manager) {
		rippedObjects = new JSONObject (JSONObject.Type.ARRAY);
		prefabs = new Dictionary<int, string>();
		Dictionary<string, int> prefabsInverse = new Dictionary<string, int>();
		int persistentID = 0;
		int prefabID = 0;
		foreach (GameObject g in PolyNetworkManager.FindObjectsOfType<GameObject>()) {
			
			Saveable saveable = g.GetComponent<Saveable> ();

			if (saveable == null)
				continue;

			// get prefab
			GameObject pre = PrefabUtility.GetPrefabParent (g) as GameObject;
			string path = AssetDatabase.GetAssetPath(pre);

			// get correct prefab ID, add new if necessary
			int gPreindex = prefabID;
			if (!prefabsInverse.TryGetValue (path, out gPreindex)) {

				prefabs.Add (prefabID, path);
				prefabsInverse.Add (path, prefabID);
				gPreindex = prefabID;
				prefabID++;
			}
			//on rip only because it will already have it on save
			saveable.setID (persistentID);
			saveable.setPrefab (gPreindex);
			// saves object to saves
			rippedObjects.Add(saveable.write());
			//schedule gameobject save

			// increment and destroy object
			persistentID++;
		}
		Debug.Log (prefabs [0]);
		manager.rippedPrefabs = prefabs;
		manager.rippedObjects = rippedObjects;
	}
}
