using Improbable.Unity;
using Improbable.Unity.Configuration;
using Improbable.Unity.Core;
using UnityEngine;
using System.Collections;
using Polytechnica.Dawnscrest.Menu;
using Polytechnica.Dawnscrest.World;

namespace Polytechnica.Dawnscrest.Core {
    public class Bootstrap : MonoBehaviour {
		
        public WorkerConfigurationData configuration = new WorkerConfigurationData();

		public int debugPlayerID = 1;
		private static int debugPlayerIDStatic;
		private static Bootstrap bootstrap;

		public static bool isServer;

		public static MenuManager menuManager;
		public static bool init = false;

		private void Awake() {
//			DontDestroyOnLoad (this);
		}

        private void Start() {
			Bootstrap.bootstrap = this;
			if (FindObjectOfType<MenuManager> () == null || FindObjectOfType<MenuManager> ().loading) {
				Connect ();
			} 
        }

		private void Connect() {
			debugPlayerIDStatic = debugPlayerID;
			SpatialOS.ApplyConfiguration (configuration);

			Time.fixedDeltaTime = 1.0f / SimulationSettings.FixedFramerate;

			switch (SpatialOS.Configuration.WorkerPlatform) {
			case WorkerPlatform.UnityWorker:
				isServer = true;
				Debug.Log ("Starting Worker");
				Application.targetFrameRate = SimulationSettings.TargetServerFramerate;
				SpatialOS.OnConnected += OnServerConnected;
				SpatialOS.OnDisconnected += reason => Application.Quit ();
				break;
			case WorkerPlatform.UnityClient:
				isServer = false;
				Debug.Log ("Starting Client");
				Bootstrap.menuManager = FindObjectOfType<MenuManager> ();
				Application.targetFrameRate = SimulationSettings.TargetClientFramerate;
				if (!init)
					SpatialOS.OnConnected += OnClientConnected;

				init = true;
				break;
			}
			SpatialOS.Connect (gameObject);
		}

		private void OnServerConnected() {

		}

		/*
		 * On Connection to spatial, client start
		 */
        private static void OnClientConnected() {
			Debug.Log (menuManager.loading);
			if (menuManager != null && !menuManager.loading) {
				BodyFinder.FindBody (SettingsManager.house.id);
			} else if (menuManager == null) {
				BodyFinder.FindBody (debugPlayerIDStatic);
			} else {
				if (Polytechnica.Dawnscrest.Core.SettingsManager.house.spawned == true) {
					Polytechnica.Dawnscrest.Core.BodyFinder.AttemptWorldEntry ();
				} else {
					Polytechnica.Dawnscrest.Core.BodyFinder.CreateFamily (Polytechnica.Dawnscrest.Core.SettingsManager.house.id);

				}
			}
        }

		public static void OnLogin() {
			bootstrap.Connect ();
		}
			
		public static void OnPlayerSpawn() {
			if (menuManager != null) {
				menuManager.OnGameStarted ();
			}
		}
    }
}
