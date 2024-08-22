using UnityEditor;
using UnityEngine;

namespace Marker.Editor
{
    public static class ResetSaveEditorForStajyer
    {
        [MenuItem("Marker Stajer/Reset All Save", false, 0)]
        public static void ResetAllSave()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}