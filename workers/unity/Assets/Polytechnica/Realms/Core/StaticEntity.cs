using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Visualizer;

namespace Polytechnica.Realms.Core {

	public class StaticEntity : MonoBehaviour {
		
		[Require] private WorldTransform.Reader WorldTransformReader;

		// Initialize Transform
		void OnEnable () {
	    	transform.position = WorldTransformReader.Data.position.ToVector3();
			transform.eulerAngles = MathHelper.toVector3(WorldTransformReader.Data.rotation);
			transform.localScale = MathHelper.toVector3 (WorldTransformReader.Data.scale);
		}

	}

}