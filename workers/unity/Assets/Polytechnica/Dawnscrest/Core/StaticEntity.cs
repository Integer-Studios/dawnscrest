using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Visualizer;

namespace Polytechnica.Dawnscrest.Core {

	/*
	 * Simple behaviour to have an object spawn in the correct place
	 * via its spatial worldTransform component
	 */
	public class StaticEntity : MonoBehaviour {
		
		[Require] private WorldTransform.Reader worldTransformReader;

		// Initialize Transform
		void OnEnable () {
			transform.position = worldTransformReader.Data.position.ToVector3();
			transform.eulerAngles = MathHelper.toVector3(worldTransformReader.Data.rotation);
			transform.localScale = MathHelper.toVector3 (worldTransformReader.Data.scale);
		}

	}

}