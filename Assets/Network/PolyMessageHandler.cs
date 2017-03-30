using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PolyMessageHandler {

	void registerClientHandlers ();
	void unregisterClientHandlers ();
	void registerServerHandlers ();
	void unregisterServerHandlers ();

}
