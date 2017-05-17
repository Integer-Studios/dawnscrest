using Improbable.Unity;
using Improbable.Unity.Configuration;
using Improbable.Unity.Core;
using UnityEngine;
using Polytechnica.Realms.User;

namespace Polytechnica.Realms.Core {
    public class Bootstrap : MonoBehaviour {
        public WorkerConfigurationData Configuration = new WorkerConfigurationData();

        private void Start() {
            SpatialOS.ApplyConfiguration(Configuration);

            Time.fixedDeltaTime = 1.0f / SimulationSettings.FixedFramerate;

            switch (SpatialOS.Configuration.WorkerPlatform) {
			case WorkerPlatform.UnityWorker:
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
			ClientPlayerSpawner.SpawnPlayer();
        }
    }
}
