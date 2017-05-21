using Improbable.Unity;
using Improbable.Unity.Configuration;
using Improbable.Unity.Core;
using UnityEngine;
using PolyMenu;

namespace Polytechnica.Realms.Core {
    public class Bootstrap : MonoBehaviour {
        public WorkerConfigurationData Configuration = new WorkerConfigurationData();

		public static bool isServer;

		public static PolyMenuManager menuManager;

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
					Bootstrap.menuManager = FindObjectOfType<PolyMenuManager> ();
					
                    Application.targetFrameRate = SimulationSettings.TargetClientFramerate;
                    SpatialOS.OnConnected += OnSpatialOsConnection;
                    break;
            }

            SpatialOS.Connect(gameObject);
        }

        private static void OnSpatialOsConnection() {
			if (menuManager != null) {
				BodyFinder.FindBody (menuManager.house.id);
			} else {
				BodyFinder.FindBody (1);
			}
        }
    }
}
