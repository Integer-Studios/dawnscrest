using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class PolyDataRipper {

	public static string rippedPrefabsString;
	public static string rippedString;

	public static void rip(PolyNetworkManager manager) {
		JSONObject rippedObjects = new JSONObject (JSONObject.Type.ARRAY);
		Dictionary<int, string> prefabs = new Dictionary<int, string>();
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
		JSONObject prefabsJSON = new JSONObject (JSONObject.Type.ARRAY);
		foreach (int prefabIndex in prefabs.Keys) {
			JSONObject prefabJSON = new JSONObject (JSONObject.Type.OBJECT);
			prefabJSON.AddField ("id", prefabIndex);
			prefabJSON.AddField ("path", prefabs[prefabIndex]);
			prefabsJSON.Add (prefabJSON);
		}
		rippedPrefabsString = prefabsJSON.ToString ();
		rippedString = rippedObjects.ToString ();
		string dir = "Assets/Resources/JSON/";

		File.WriteAllText(dir + "prefabs.json", rippedPrefabsString);
		File.WriteAllText(dir + "objects.json", rippedString);
	}

}