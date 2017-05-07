using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyNet {

	public class JSONHelper {

		public static JSONObject wrap(PolyNetPlayer player) {
			JSONObject playerJSON = new JSONObject (JSONObject.Type.OBJECT);
			playerJSON.AddField ("id", player.playerId);
			playerJSON.AddField ("session", player.session);
			playerJSON.AddField ("world", -1);
			return playerJSON;
		}

		public static PolyNetPlayer unwrap(JSONObject data) {
			int playerId = (int)data.GetField ("id").n;
			return PolyServer.getPlayer(playerId);
		}

//		public static JSONObject wrap(Vector3 vector) {
//
//		}

		public static Vector3 unwrap(JSONObject data, string prefix) {
			float x = data.GetField (prefix + "-x").n;
			float y = data.GetField (prefix + "-y").n;
			float z = data.GetField (prefix + "-z").n;
			return new Vector3 (x, y, z);
		}

	}
			
}