
using System.Diagnostics;
using Sirenix.Utilities;
using Debug = UnityEngine.Debug;

namespace BuildMaker.Utils
{
    public static class PreBuildChecks
    {
        public static bool Check(BuildProfileTable profile)
        {
            if (profile.ScriptableObject == null)
            {
                Debug.LogError("Scriptable object is missing!");
                return false;
            }

            if (profile.ScriptableObject.sceneDirectories == null)
            {
                Debug.LogError("No scene directories found!");
                return false;
            }

            if (profile.ScriptableObject.directory.IsNullOrWhitespace())
            {
                Debug.LogError("Directory is missing!");
                return false;
            }

            if (profile.ScriptableObject.executableName.IsNullOrWhitespace())
            {
                Debug.LogError("Executable name is missing!");
                return false;
            }

            if (profile.ScriptableObject.executableName.EndsWith(".exe"))
            {

            }

            if (!profile.ScriptableObject.directory.EndsWith("/"))
            {
                Debug.LogWarning("Your directory needs a slash at the end...adding for you.");
                profile.ScriptableObject.directory += "/";
            }
            return true;
        }
    }
}
