using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Snog.InteractionSystem.ScriptableObjects;

namespace Snog.InteractionSystem.Editor
{
    public class InteractionCreatorEditor : EditorWindow
    {
        #region Variables
        private string newTypeName = "NewInteraction";
        private string newPromptText = "Press {key} to interact";
        private KeyCode newInteractionKey = KeyCode.E;
        private InteractionRegistry registry;
        private Vector2 scrollPosition;
        private int selectedInteractionIndex = 0;
        private string[] interactionNames = new string[0];
        private InteractionType selectedInteractionType;
        private InteractionPrompt selectedInteractionPrompt;

        private const string NoneOption = "-- Select an Interaction --";
        private const string RootFolder = "Assets/Snog/InteractionSystem/Scripts/";
        private const string GeneratedFolder = RootFolder + "Generated/";
        private const string BehaviorFolder = GeneratedFolder + "Behaviors/";
        private const string TypeAssetFolder = GeneratedFolder + "Types/";
        private const string PromptAssetFolder = GeneratedFolder + "Prompts/";

        private static GUIStyle headerStyle;
        private static GUIStyle subHeaderStyle;
        private static Texture2D headerIcon;
        private static Texture2D createIcon;
        private static Texture2D editIcon;
        #endregion

        #region Unity Methods
        [MenuItem("Tools/Interaction Creator")]
        public static void ShowWindow()
        {
            var window = GetWindow<InteractionCreatorEditor>();

            var icon = EditorGUIUtility.IconContent("d_Settings").image as Texture2D;

            window.titleContent = new GUIContent("Interaction Creator", icon);
        }

        private void OnEnable()
        {
            headerIcon = EditorGUIUtility.IconContent("d_UnityEditor.InspectorWindow").image as Texture2D;
            createIcon = EditorGUIUtility.IconContent("d_Toolbar Plus").image as Texture2D;
            editIcon = EditorGUIUtility.IconContent("d_editicon.sml").image as Texture2D;

            AutoFindRegistry();
            RefreshInteractionNames();
        }

        private void OnFocus()
        {
            AutoFindRegistry();
            RefreshInteractionNames();
        }
        #endregion

        private void InitStyles()
        {
            if (headerStyle == null)
            {
                headerStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 14,
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset(5, 5, 5, 5)
                };
            }

            if (subHeaderStyle == null)
            {
                subHeaderStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleLeft
                };
            }
        }

        private void OnGUI()
        {
            InitStyles(); 

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DrawHeader();
            DrawRegistrySection();

            if (registry != null)
            {
                DrawSeparator();
                DrawCreateSection();
                DrawSeparator();
                DrawEditDeleteSection();
            }

            EditorGUILayout.EndScrollView();
        }

        #region UI Drawing Methods
        private void DrawHeader()
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label(new GUIContent(" Interaction System Creator", headerIcon), headerStyle, GUILayout.Height(30));
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        private void DrawRegistrySection()
        {
            registry = (InteractionRegistry)EditorGUILayout.ObjectField("Interaction Registry", registry, typeof(InteractionRegistry), false);

            if (registry == null)
            {
                string[] guids = AssetDatabase.FindAssets("t:InteractionRegistry");
                if (guids.Length == 0)
                {
                    EditorGUILayout.HelpBox("No Interaction Registry found.", MessageType.Warning);
                    if (GUILayout.Button("Create New Registry")) CreateRegistry();
                }
                else
                {
                    EditorGUILayout.HelpBox("Multiple registries found. Please assign one.", MessageType.Warning);
                }
            }
        }

        private void DrawCreateSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label(new GUIContent(" Create New Interaction", createIcon), subHeaderStyle);
            EditorGUILayout.Space(5);

            newTypeName = EditorGUILayout.TextField("Interaction Name", newTypeName);
            newPromptText = EditorGUILayout.TextField("Prompt Text", newPromptText);
            newInteractionKey = (KeyCode)EditorGUILayout.EnumPopup("Interaction Key", newInteractionKey);

            if (GUILayout.Button("Create Interaction", GUILayout.Height(25)))
            {
                CreateInteraction();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawEditDeleteSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label(new GUIContent(" Edit or Delete Interaction", editIcon), subHeaderStyle);
            EditorGUILayout.Space(5);

            if (interactionNames.Length > 1)
            {
                EditorGUI.BeginChangeCheck();
                selectedInteractionIndex = EditorGUILayout.Popup("Select Interaction", selectedInteractionIndex, interactionNames);
                if (EditorGUI.EndChangeCheck()) LoadSelectedInteractionData();

                if (selectedInteractionPrompt != null)
                {
                    EditorGUILayout.Space(5);
                    selectedInteractionPrompt.promptText = EditorGUILayout.TextField("Prompt Text", selectedInteractionPrompt.promptText);
                    selectedInteractionPrompt.interactionKey = (KeyCode)EditorGUILayout.EnumPopup("Interaction Key", selectedInteractionPrompt.interactionKey);

                    GUI.backgroundColor = new Color(0.7f, 1f, 0.7f); // Light green
                    if (GUILayout.Button("Update Interaction")) UpdateInteraction();
                    GUI.backgroundColor = Color.white;
                }

                if (selectedInteractionType != null)
                {
                    EditorGUILayout.Space(5);
                    GUI.backgroundColor = new Color(1f, 0.6f, 0.6f); // Light red
                    if (GUILayout.Button("Delete Selected Interaction")) DeleteSelectedInteraction();
                    GUI.backgroundColor = Color.white;
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No interactions found. Use the 'Create' section above to add one.", MessageType.Info);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawSeparator()
        {
            EditorGUILayout.Space(10);
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            rect.height = 1;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
            EditorGUILayout.Space(10);
        }
        #endregion

        #region Helper Methods (Unchanged)
        private void CreateRegistry()
        {
            InteractionRegistry newRegistry = CreateInstance<InteractionRegistry>();
            if (!Directory.Exists(RootFolder)) Directory.CreateDirectory(RootFolder);
            string path = AssetDatabase.GenerateUniqueAssetPath(RootFolder + "InteractionRegistry.asset");
            AssetDatabase.CreateAsset(newRegistry, path);
            AssetDatabase.SaveAssets();
            registry = newRegistry;
            EditorGUIUtility.PingObject(newRegistry);
        }

        private void AutoFindRegistry()
        {
            if (registry != null) return;
            string[] guids = AssetDatabase.FindAssets("t:InteractionRegistry");
            if (guids.Length == 1)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                registry = AssetDatabase.LoadAssetAtPath<InteractionRegistry>(path);
            }
        }

        private void RefreshInteractionNames()
        {
            if (registry != null)
            {
                List<string> names = registry.interactionTypes.Select(t => t.typeName).OrderBy(n => n).ToList();
                names.Insert(0, NoneOption);
                interactionNames = names.ToArray();
            }
            else
            {
                interactionNames = new string[] { NoneOption };
            }
            LoadSelectedInteractionData();
        }

        private void LoadSelectedInteractionData()
        {
            if (selectedInteractionIndex > 0 && selectedInteractionIndex < interactionNames.Length)
            {
                string name = interactionNames[selectedInteractionIndex];
                selectedInteractionType = registry.interactionTypes.FirstOrDefault(t => t.typeName == name);
                selectedInteractionPrompt = selectedInteractionType?.prompt;
            }
            else
            {
                selectedInteractionType = null;
                selectedInteractionPrompt = null;
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(newTypeName))
            {
                EditorUtility.DisplayDialog("Error", "Interaction name cannot be empty.", "OK");
                return false;
            }
            if (registry.interactionTypes.Any(t => t.typeName == newTypeName))
            {
                EditorUtility.DisplayDialog("Error", $"An interaction named '{newTypeName}' already exists.", "OK");
                return false;
            }
            return true;
        }

        private void CreateInteraction()
        {
            if (!ValidateInput()) return;

            Directory.CreateDirectory(BehaviorFolder);
            Directory.CreateDirectory(TypeAssetFolder);
            Directory.CreateDirectory(PromptAssetFolder);

            string scriptPath = GetBehaviorScriptPath(newTypeName);
            string scriptContent = $@"using UnityEngine;
using Snog.InteractionSystem.Core.Interfaces;

namespace Snog.InteractionSystem.Behaviors
{{
    public class {newTypeName}Interaction : MonoBehaviour, IInteractionBehavior
    {{
        public void Execute(GameObject target)
        {{
            Debug.Log(""{newTypeName} interaction executed on "" + target.name);
        }}
    }}
}}";
            File.WriteAllText(scriptPath, scriptContent);
            AssetDatabase.Refresh();

            InteractionPrompt promptAsset = CreateInstance<InteractionPrompt>();
            promptAsset.promptText = newPromptText;
            promptAsset.interactionKey = newInteractionKey;
            string promptPath = GetPromptAssetPath(newTypeName);
            AssetDatabase.CreateAsset(promptAsset, promptPath);

            InteractionType newType = CreateInstance<InteractionType>();
            newType.typeName = newTypeName;
            newType.prompt = promptAsset;

            EditorApplication.delayCall += () =>
            {
                MonoScript behaviorScript = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);
                if (behaviorScript != null)
                {
                    newType.behaviorScript = behaviorScript;
                }
                else
                {
                    Debug.LogError($"Failed to load behavior script at {scriptPath}.");
                }

                string assetPath = GetTypeAssetPath(newTypeName);
                AssetDatabase.CreateAsset(newType, assetPath);
                registry.interactionTypes.Add(newType);
                EditorUtility.SetDirty(registry);
                AssetDatabase.SaveAssets();
                EditorGUIUtility.PingObject(behaviorScript);
                RefreshInteractionNames();
            };
        }

        private void UpdateInteraction()
        {
            if (selectedInteractionPrompt != null)
            {
                EditorUtility.SetDirty(selectedInteractionPrompt);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("Success", $"Interaction '{selectedInteractionType.typeName}' updated.", "OK");
            }
        }

        private void DeleteSelectedInteraction()
        {
            if (selectedInteractionType == null) return;
            if (EditorUtility.DisplayDialog("Delete Interaction?",
                $"Delete '{selectedInteractionType.typeName}'? This cannot be undone.",
                "Yes, Delete", "Cancel"))
            {
                string nameToDelete = selectedInteractionType.typeName;
                AssetDatabase.DeleteAsset(GetBehaviorScriptPath(nameToDelete));
                AssetDatabase.DeleteAsset(GetTypeAssetPath(nameToDelete));
                AssetDatabase.DeleteAsset(GetPromptAssetPath(nameToDelete));
                registry.interactionTypes.RemoveAll(t => t.typeName == nameToDelete);
                EditorUtility.SetDirty(registry);
                AssetDatabase.Refresh();
                Debug.Log($"Deleted interaction: {nameToDelete}");
                selectedInteractionIndex = 0;
                RefreshInteractionNames();
            }
        }

        private string GetBehaviorScriptPath(string typeName) => Path.Combine(BehaviorFolder, $"{typeName}Interaction.cs");
        private string GetTypeAssetPath(string typeName) => Path.Combine(TypeAssetFolder, $"{typeName}.asset");
        private string GetPromptAssetPath(string typeName) => Path.Combine(PromptAssetFolder, $"{typeName}Prompt.asset");
        #endregion
    }
}