using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using PolyItem;

public class Saveable : NetworkBehaviour {
	public int id;
	public int prefabID;

	public void setID(int i) {
		id = i;
	}

	public void setPrefab(int i) {
		prefabID = i;
	}

	public JSONObject write() {

		JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
		json.AddField ("id", id);
		json.AddField ("prefab", prefabID);
		json.AddField ("position-x", gameObject.transform.position.x);
		json.AddField ("position-y", gameObject.transform.position.y);
		json.AddField ("position-z", gameObject.transform.position.z);
		json.AddField ("rotation-x", gameObject.transform.eulerAngles.x);
		json.AddField ("rotation-y", gameObject.transform.eulerAngles.y);
		json.AddField ("rotation-z", gameObject.transform.eulerAngles.z);
		json.AddField ("scale-x", gameObject.transform.localScale.x);
		json.AddField ("scale-y", gameObject.transform.localScale.y);
		json.AddField ("scale-z", gameObject.transform.localScale.z);
		JSONObject array = new JSONObject (JSONObject.Type.ARRAY);
		foreach(ISaveable s in GetComponents<ISaveable>()) {
			JSONObject obj = s.write ();
			obj.AddField ("type", s.getType ());
			array.Add (obj);
		}
		json.AddField ("scripts", array);
		return json;
	}

	public void read(JSONObject json) {
		Vector3 pos = new Vector3(json.GetField("position-x").n, json.GetField("position-y").n, json.GetField("position-z").n);
		Vector3 rot = new Vector3(json.GetField("rotation-x").n, json.GetField("rotation-y").n, json.GetField("rotation-z").n);
		Vector3 scale = new Vector3(json.GetField("scale-x").n, json.GetField("scale-y").n, json.GetField("scale-z").n);
		transform.position = pos;
		transform.rotation = Quaternion.Euler(rot);
		transform.localScale = scale;
		Inventory[] invs = GetComponents<Inventory> ();
		int inventoryIndex = 0;

		if (!json.GetField("scripts").IsBool) {
			foreach (JSONObject obj in json.GetField("scripts").list) {

				if (obj.GetField ("type").str == "gatherable") {

					GetComponent<Gatherable> ().read (obj);
				} else if (obj.GetField ("type").str == "inventory") {

					invs[inventoryIndex].read (obj);
					inventoryIndex++;
				}  else if (obj.GetField ("type").str == "craftable") {
			
					GetComponent<Craftable> ().read (obj);
				}
			}
		}
	}

}
