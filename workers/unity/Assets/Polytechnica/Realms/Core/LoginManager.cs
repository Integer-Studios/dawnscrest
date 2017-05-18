using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Unity.Visualizer;
using Improbable.Entity.Component;
using Improbable.Core;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable;
using Improbable.Math;
using Polytechnica.Realms.Core;

namespace Polytechnica.Realms.Core {

	[WorkerType(WorkerPlatform.UnityWorker)]
	public class LoginManager : MonoBehaviour {

		[Require] private PlayerSpawning.Writer playerSpawningWriter;

		void OnEnable () {
			playerSpawningWriter.CommandReceiver.OnSpawnPlayer.RegisterResponse (OnSpawnPlayer);
		}

		void OnDisable () {
			playerSpawningWriter.CommandReceiver.OnSpawnPlayer.DeregisterResponse();
		}

		private Nothing OnSpawnPlayer(SpawnPlayerRequest request, ICommandCallerInfo callerInfo) {
			uint i = request.playerId;
			Debug.Log ("player login with ID" + i + " and worker ID:" + callerInfo.CallerWorkerId);
			CreatePlayerWithReservedId (callerInfo.CallerWorkerId);
			return new Nothing ();
		}

		private void CreatePlayerWithReservedId(string clientWorkerId) {
			SpatialOS.Commands.ReserveEntityId(playerSpawningWriter)
				.OnSuccess(reservedEntityId => CreatePlayer(clientWorkerId, reservedEntityId))
				.OnFailure(failure => OnFailedReservation(failure, clientWorkerId));
		}

		private void OnFailedReservation(ICommandErrorDetails response, string clientWorkerId) {
			Debug.LogError("Failed to Reserve EntityId for Player: " + response.ErrorMessage + ". Retrying...");
			CreatePlayerWithReservedId(clientWorkerId);
		}

		private void CreatePlayer(string clientWorkerId, EntityId entityId) {
			var playerEntityTemplate = EntityTemplateFactory.CreateCubeTemplate(clientWorkerId);
			SpatialOS.Commands.CreateEntity(playerSpawningWriter, entityId, "Cube", playerEntityTemplate)
				.OnFailure(failure => OnFailedPlayerCreation(failure, clientWorkerId, entityId));
		}

		private void OnFailedPlayerCreation(ICommandErrorDetails response, string clientWorkerId, EntityId entityId) {
			Debug.LogError("Failed to Create PlayerShip Entity: " + response.ErrorMessage + ". Retrying...");
			CreatePlayer(clientWorkerId, entityId);
		}


	}

}