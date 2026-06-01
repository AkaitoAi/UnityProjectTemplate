using UnityEditor;
using UnityEngine;
using static System.IO.Directory;
using static System.IO.Path;
using static UnityEngine.Application;
using static UnityEditor.AssetDatabase;

namespace AkaitoAi
{
    public static class ToolsMenu
    {
        #region Create Default Folders
        [MenuItem("AkaitoAi/Setup/Create Default Folders")]
        public static void CreateDefaultFolders()
        {
            CreateDirectories("_Project", "Scripts", "Scenes", "UI", "Prefabs", "Resources", "Models", "Sounds", "Materials", "Texture2D", "Animations");
            CreateDirectories("_Editor", "Editor", "Prefabs", "Scripts", "Layouts", "ScriptableObjects");
            Refresh();
        }


        static int modelFoldersCount = -1;
        [MenuItem("AkaitoAi/Setup/Create Model Folders")] 
        public static void CreateModelFolders()
        {
            modelFoldersCount++;
            CreateDirectories("_Model_Folder " + "(" + modelFoldersCount + ")" , "FBX", "Materials", "Texture2D");
            Refresh();
        }

        public static void CreateDirectories(string root, params string[] dir)
        {
            var fullPath = Combine(dataPath, root);

            foreach (var newDirectory in dir)
                CreateDirectory(Combine(fullPath, newDirectory));
        }
        #endregion

        #region Enable/ Disable GPU Instancing
        [MenuItem("AkaitoAi/Tools/Materials/Get all Metrials GPU Instancing True")]// % mean command Key and # mean shift Key....
        static void GetAndSetAllMetrialsGUPInstTrue()
        {
            string[] paths = AssetDatabase.FindAssets("t:Material");
            Debug.Log(paths.Length);
            Material[] mat = new Material[paths.Length];
            for (int i = 0; i < paths.Length; i++)
            {
                mat[i] = (Material)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(paths[i]), typeof(Material));
                mat[i].enableInstancing = true;
            }
            Debug.Log(mat[mat.Length - 1].name);
            Debug.Log(mat[0].name);
        }

        [MenuItem("AkaitoAi/Tools/Materials/Get all Metrials GPU Instancing False")]// % mean command Key and # mean shift Key....
        static void GetAndSetAllMetrialsGUPInstFalse()
        {
            string[] paths = AssetDatabase.FindAssets("t:Material");
            Debug.Log(paths.Length);
            Material[] mat = new Material[paths.Length];

            for (int i = 0; i < paths.Length; i++)
            {
                mat[i] = (Material)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(paths[i]), typeof(Material));
                mat[i].enableInstancing = false;
            }
            Debug.Log(mat[mat.Length - 1].name);
            Debug.Log(mat[0].name);
        }
        #endregion
    }
}