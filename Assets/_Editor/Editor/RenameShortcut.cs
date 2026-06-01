using UnityEditor;
using UnityEngine;

namespace AkaitoAi
{
    public static class RenameShortcut
    {
        [MenuItem("AkaitoAi/Edit/Rename %r", false, -101)]
        public static void Rename()
        {
            var e = new Event { keyCode = KeyCode.F2, type = EventType.KeyDown };
            EditorWindow.focusedWindow.SendEvent(e);
        }
    }
}
