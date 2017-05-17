using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Pirates.Behaviours {
    public class TransformSender : MonoBehaviour {
		
        [Require]
        private WorldTransform.Writer WorldTransformWriter;

        void OnEnable() {
			transform.position = WorldTransformWriter.Data.position.ToVector3();
        }

		void FixedUpdate() {
			SendPositionAndRotationUpdates ();
		}

		private void SendPositionAndRotationUpdates() {
			WorldTransformWriter.Send(new WorldTransform.Update()
				.SetPosition(transform.position.ToCoordinates()));
		}

    }

	public static class Vector3Extensions {
		public static Coordinates ToCoordinates(this Vector3 vector3) {
			return new Coordinates(vector3.x, vector3.y, vector3.z);
		}
	}

}