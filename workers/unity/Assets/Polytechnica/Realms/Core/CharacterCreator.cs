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

	public class CharacterCreator : MonoBehaviour {

		[Require] CharacterCreatorController.Writer CreatorControllerWriter;

		private void OnEnable() {
			CreatorControllerWriter.CommandReceiver.OnCreateFamily.RegisterResponse (CreateFamily);
		}

		private void OnDisable() {
			CreatorControllerWriter.CommandReceiver.OnCreateFamily.DeregisterResponse ();
		}

		private Nothing CreateFamily(CreateFamilyRequest request, ICommandCallerInfo info) {
			int houseId = (int)request.houseId;
			CreateCharacterWithReservedId (houseId, true);
			CreateCharacterWithReservedId (houseId, false);
			CreateCharacterWithReservedId (houseId, false);
			return new Nothing ();
		}

		private void CreateCharacterWithReservedId(int houseId, bool active) {
			SpatialOS.Commands.ReserveEntityId(CreatorControllerWriter)
				.OnSuccess(reservedEntityId => CreateCharacter(houseId, active, reservedEntityId))
				.OnFailure(failure => OnFailedReservation(failure, houseId, active));
		}

		private void OnFailedReservation(ICommandErrorDetails response, int houseId, bool active) {
			Debug.LogError("Failed to Reserve EntityId for Player: " + response.ErrorMessage + ". Retrying...");
			CreateCharacterWithReservedId(houseId, active);
		}

		private void CreateCharacter(int houseId, bool active, EntityId entityId) {
			var charEntityTemplate = EntityTemplateFactory.CreateCharacterTemplate(houseId, true);
			SpatialOS.Commands.CreateEntity(CreatorControllerWriter, entityId, "Character", charEntityTemplate)
				.OnFailure(failure => OnFailedPlayerCreation(failure, houseId, active,  entityId));
		}

		private void OnFailedPlayerCreation(ICommandErrorDetails response, int houseId, bool active, EntityId entityId) {
			Debug.LogError("Failed to Create PlayerShip Entity: " + response.ErrorMessage + ". Retrying...");
			CreateCharacter(houseId, active, entityId);
		}

	}

}
