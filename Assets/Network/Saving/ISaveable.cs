using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable {

	JSONObject write ();
	void read (JSONObject obj);
	string getType();

}
