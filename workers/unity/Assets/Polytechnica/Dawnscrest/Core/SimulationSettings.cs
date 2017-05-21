using UnityEngine;

namespace Polytechnica.Dawnscrest.Core {
    public static class SimulationSettings {
        public static readonly int TargetClientFramerate = 60;
        public static readonly int TargetServerFramerate = 60;
        public static readonly int FixedFramerate = 20;
        public static readonly string DefaultSnapshotPath = Application.dataPath + "/../../../snapshots/default.snapshot";

		public static string PlayerPrefab = "Player";

    }
}
