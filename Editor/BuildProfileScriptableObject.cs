using UnityEngine;

namespace BuildMaker
{
    public enum Platform
    {
        Windows_x86_64,
        Windows_x86,
        macOS,
        Linux,
        Android,
        iOS,
        WebGL,
        PlayStation4,
        XboxOne
    }

    [CreateAssetMenu (menuName ="Configs/Build Profile")]
    public class BuildProfileScriptableObject : ScriptableObject
    {
        public string executableName;
        public string args;
        public string directory;
        public string[] sceneDirectories;
        public Platform platform;
        public bool devBuild;
        public bool headlessBuild;
    }
}
