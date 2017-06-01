
namespace Polytechnica.Dawnscrest.Core {
    public static class BuildSettings {
        public static readonly string UnityClientScene = "UnityClient";
		public static readonly string MenuScene = "MenuScene";

		public static readonly string ClientDefaultActiveScene = MenuScene;
        public static readonly string[] ClientScenes = { MenuScene, UnityClientScene, "SettingsScene" };

        public static readonly string UnityWorkerScene = "UnityWorker";
        public static readonly string WorkerDefaultActiveScene = UnityWorkerScene;
        public static readonly string[] WorkerScenes = { UnityWorkerScene };

        public const string SceneDirectory = "Assets";
    }
}