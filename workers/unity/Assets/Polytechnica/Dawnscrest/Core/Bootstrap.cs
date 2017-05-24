﻿using Improbable.Unity;
using Improbable.Unity.Configuration;
using Improbable.Unity.Core;
using UnityEngine;
using Polytechnica.Dawnscrest.Menu;

namespace Polytechnica.Dawnscrest.Core {
    public class Bootstrap : MonoBehaviour {
		
        public WorkerConfigurationData configuration = new WorkerConfigurationData();

		public int debugPlayerID = 1;
		private static int debugPlayerIDStatic;

		public static bool isServer;

		public static MenuManager menuManager;

        private void Start() {
			debugPlayerIDStatic = debugPlayerID;
			SpatialOS.ApplyConfiguration(configuration);

            Time.fixedDeltaTime = 1.0f / SimulationSettings.FixedFramerate;

            switch (SpatialOS.Configuration.WorkerPlatform) {
			case WorkerPlatform.UnityWorker:
				isServer = true;
				Debug.Log ("Starting Worker");
                Application.targetFrameRate = SimulationSettings.TargetServerFramerate;
                SpatialOS.OnDisconnected += reason => Application.Quit();
                break;
			case WorkerPlatform.UnityClient:
				isServer = false;
				Debug.Log ("Starting Client");
				Bootstrap.menuManager = FindObjectOfType<MenuManager> ();
                Application.targetFrameRate = SimulationSettings.TargetClientFramerate;
                SpatialOS.OnConnected += OnSpatialOsConnection;
                break;
            }

            SpatialOS.Connect(gameObject);
        }

		/*
		 * On Connection to spatial, client start
		 */
        private static void OnSpatialOsConnection() {
			if (menuManager != null) {
				BodyFinder.FindBody (menuManager.house.id);
			} else {
				BodyFinder.FindBody (debugPlayerIDStatic);
			}
        }
			
		public static void OnPlayerSpawn() {
			if (menuManager != null) {
				menuManager.OnGameStarted ();
			}
		}
    }
}