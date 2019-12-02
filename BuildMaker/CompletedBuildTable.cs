using System.Diagnostics;
using System.Linq;
using BuildMaker.Utils;
using Sirenix.OdinInspector;
using Debug = UnityEngine.Debug;

namespace BuildMaker
{
    public class CompletedBuildTable
    {
        [ReadOnly]
        public string timestamp;

        [ReadOnly]
        public BuildProfileScriptableObject BuildProfile;

        [Button("Launch")]
        public void Play()
        {
            Debug.Log("Launching " + BuildProfile.name);

            string buildFullLocation = BuildProfile.directory +
                                       BuildProfile.executableName +
                                       SetExecutableExtension.Get(BuildProfile.platform);

            Process proc = new Process();
            proc.StartInfo.FileName = buildFullLocation;
            proc.StartInfo.Arguments = BuildProfile.args;

            proc.Start();
        }
    }
}
