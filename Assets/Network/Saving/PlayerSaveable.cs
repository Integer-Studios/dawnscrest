using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyPlayer;
using PolyItem;

public class PlayerSaveable : MonoBehaviour {

	public int id;

	public JSONObject write() {
		Player p = GetComponent<Player> ();
		JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
		json.AddField ("id", id);
		json.AddField ("position-x", gameObject.transform.position.x);
		json.AddField ("position-y", gameObject.transform.position.y);
		json.AddField ("position-z", gameObject.transform.position.z);
		json.AddField ("rotation-x", gameObject.transform.eulerAngles.x);
		json.AddField ("rotation-y", gameObject.transform.eulerAngles.y);
		json.AddField ("rotation-z", gameObject.transform.eulerAngles.z);
		json.AddField ("health", p.getHealth());
		json.AddField ("hunger", p.getHunger());
		json.AddField ("thirst", p.getThirst());

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

		Player p = GetComponent<Player> ();
		p.playerID = (int) json.GetField ("id").n;
		Vector3 pos = new Vector3(json.GetField("position-x").n, json.GetField("position-y").n, json.GetField("position-z").n);
		Vector3 rot = new Vector3(json.GetField("rotation-x").n, json.GetField("rotation-y").n, json.GetField("rotation-z").n);
		transform.position = pos;
		transform.rotation = Quaternion.Euler(rot);
		p.loadVitals (json.GetField("health").n, json.GetField("hunger").n, json.GetField("thirst").n);
		Inventory[] invs = GetComponents<Inventory> ();
		if (json.HasField("scripts") && !json.GetField("scripts").IsBool) {
			int x = 0;
			foreach (JSONObject obj in json.GetField("scripts").list) {
				if (obj.GetField ("type").str == "inventory") {
					invs[x].read (obj);
					x++;
				}
			}
		}
	}

}
