using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;

[Overlay(typeof(SceneView),"Scene Selection")]
[Icon(ICON_PATH)]
public class SceneSelectionOverlay : ToolbarOverlay
{
    public const string ICON_PATH = "Assets/Editor/Icons/UnityIcon.png";

    SceneSelectionOverlay() : base(SceneDropdownToggle.k_id){}
    

    [EditorToolbarElement(k_id,typeof(SceneView))]
    class SceneDropdownToggle : EditorToolbarDropdownToggle , IAccessContainerWindow
    {
        public const string k_id = "SceneChangeOverlay/SceneDropdownToggle";
        
        public EditorWindow containerWindow { get; set; }

        SceneDropdownToggle() 
        {
            name = "Scenes";
            tooltip = "Select a scene to load";
            icon = AssetDatabase.LoadAssetAtPath<Texture2D>(SceneSelectionOverlay.ICON_PATH);

            dropdownClicked += ShowContextMenu;
        }

        private void ShowContextMenu()
        {
            GenericMenu menu = new GenericMenu();

            var currentScene = EditorSceneManager.GetActiveScene();
            string[] sceneGuids = AssetDatabase.FindAssets("t:scene",new[] { "Assets/Scenes" });

            for (int i = 0; i < sceneGuids.Length; i++)
            {
                
                var path = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
                var name = Path.GetFileNameWithoutExtension(path);
                
                    menu.AddItem(new GUIContent(name), currentScene.name == name, () =>
                    {
                        if (currentScene.isDirty)
                        {
                            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                        }

                        EditorSceneManager.OpenScene(path);
                    });
            }
            
            menu.ShowAsContext();
        }


 
    }
}
