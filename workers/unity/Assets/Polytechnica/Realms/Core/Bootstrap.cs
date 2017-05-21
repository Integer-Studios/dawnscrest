using Improbable.Unity;
using Improbable.Unity.Configuration;
using Improbable.Unity.Core;
using UnityEngine;

namespace Polytechnica.Realms.Core {
    public class Bootstrap : MonoBehaviour {
        public WorkerConfigurationData Configuration = new WorkerConfigurationData();

		public static bool isServer;

        private void Start() {
            SpatialOS.ApplyConfiguration(Configuration);

            Time.fixedDeltaTime = 1.0f / SimulationSettings.FixedFramerate;

            switch (SpatialOS.Configuration.WorkerPlatform) {
			case WorkerPlatform.UnityWorker:
					isServer = true;
					Debug.Log ("Starting Worker");
                    Application.targetFrameRate = SimulationSettings.TargetServerFramerate;
                    SpatialOS.OnDisconnected += reason => Application.Quit();
                    break;
                case WorkerPlatform.UnityClient:
					Debug.Log ("Starting Client");
                    Application.targetFrameRate = SimulationSettings.TargetClientFramerate;
                    SpatialOS.OnConnected += OnSpatialOsConnection;
                    break;
            }

            SpatialOS.Connect(gameObject);
        }

        private static void OnSpatialOsConnection() {
			BodyFinder.FindBody();
        }
    }
}
