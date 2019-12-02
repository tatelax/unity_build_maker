using Sirenix.OdinInspector;

namespace BuildMaker
{
    public class BuildProfileTable
    {
        [TableColumnWidth(25, Resizable = true)]
        public bool Enabled;
        [AssetsOnly]
        [Required]
        public BuildProfileScriptableObject ScriptableObject;
    }
}
