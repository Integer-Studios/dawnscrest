using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PolySaveable {

	JSONObject write();
	void read (JSONObject data);

}

