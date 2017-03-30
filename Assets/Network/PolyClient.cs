using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyPlayer;
using UnityEngine.Networking;

[System.Serializable]
public struct PolyClient {

	public GameObject gameObject;
	public IPolyPlayer playerObject;
	public int connectionID;
	public NetworkConnection connection;
	public int loginID;
	public short controllerID;
	public string identifier;
	public JSONObject data;

	public PolyClient(int id, NetworkConnection connection) {
		this.loginID = id;
		this.connectionID = connection.connectionId;
		this.connection = connection;
		this.data = null;
		this.identifier = null;
		this.gameObject = null;
		this.playerObject = null;
		this.controllerID = -1;
	}

	public PolyClient(int id, int  connectionID, JSONObject data) {
		this.loginID = id;
		this.connectionID = connectionID;
		this.connection = null;
		this.data = data;
		this.identifier = null;
		this.gameObject = null;
		this.playerObject = null;
		this.controllerID = -1;
	}

}
