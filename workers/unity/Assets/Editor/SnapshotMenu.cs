﻿using Polytechnica.Dawnscrest.Core;
using Improbable;
using Improbable.Worker;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Polytechnica.Dawnscrest.Editor {
	public class SnapshotMenu : MonoBehaviour {
		[MenuItem("Improbable/Snapshots/Generate Default Snapshot")]
		[UsedImplicitly]
		private static void GenerateDefaultSnapshot() {
			var snapshotEntities = new Dictionary<EntityId, SnapshotEntity>();

			var currentEntityId = 1;
//			snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateGroundTemplate());
			snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateCharacterCreatorTemplate());
			SaveSnapshot(snapshotEntities);
		}

		private static void SaveSnapshot(IDictionary<EntityId, SnapshotEntity> snapshotEntities) {
			File.Delete(SimulationSettings.DefaultSnapshotPath);
			var maybeError = Snapshot.Save(SimulationSettings.DefaultSnapshotPath, snapshotEntities);

			if (maybeError.HasValue)
				Debug.LogErrorFormat("Failed to generate initial world snapshot: {0}", maybeError.Value);
			else
				Debug.LogFormat("Successfully generated initial world snapshot at {0}", SimulationSettings.DefaultSnapshotPath);
		}
	}
}
