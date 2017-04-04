using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyNetwork {

	public interface PolyMessageHandler {

		void registerClientHandlers ();
		void unregisterClientHandlers ();
		void registerServerHandlers ();
		void unregisterServerHandlers ();

	}

}