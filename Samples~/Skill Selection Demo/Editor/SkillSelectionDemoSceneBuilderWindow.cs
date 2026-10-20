using System.IO;
using TTest.RoguelikeSkillSelection.Data;
using TTest.RoguelikeSkillSelection.Samples.SkillSelectionDemo;
using TTest.RoguelikeSkillSelection.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TTest.RoguelikeSkillSelection.Samples.SkillSelectionDemo.Editor
{
    public sealed class SkillSelectionDemoSceneBuilderWindow : EditorWindow
    {
        private const string DefaultFolder = "Assets/Samples/TTest Roguelike Skill Selection/0.1.0/Skill Selection Demo";
        private string _outputFolder = DefaultFolder;

        [MenuItem("TTest Samples/Roguelike Skill Selection/Create Demo Scene")]
        private static void Open() => GetWindow<SkillSelectionDemoSceneBuilderWindow>("Skill Selection Demo Builder");

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Roguelike Skill Selection Demo", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Creates a UGUI scene, a reusable skill-card prefab, and three Skill Choice assets. Existing generated files are never overwritten.", MessageType.Info);
            _outputFolder = EditorGUILayout.TextField("Output Folder", _outputFolder);

            using (new EditorGUI.DisabledScope(!IsAssetFolderPath(_outputFolder)))
            {
                if (GUILayout.Button("Create Demo Scene", GUILayout.Height(30)))
                    CreateDemo();
            }

            if (!IsAssetFolderPath(_outputFolder))
                EditorGUILayout.HelpBox("Use an Assets-relative folder, for example Assets/Samples/…", MessageType.Error);
        }

        private void CreateDemo()
        {
            var scenePath = $"{_outputFolder}/SkillSelectionDemo.unity";
            var prefabPath = $"{_outputFolder}/SkillChoiceCard.prefab";
            if (File.Exists(scenePath) || File.Exists(prefabPath))
            {
                EditorUtility.DisplayDialog("Skill Selection Demo", "A generated scene or prefab already exists. Choose another folder or delete it before generating again.", "OK");
                return;
            }

            EnsureFolder(_outputFolder);
            EnsureFolder($"{_outputFolder}/Data");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            var canvas = CreateCanvas();
            CreateEventSystem();
            var panelRoot = CreatePanel(canvas.transform);
            var contentRoot = CreateContent(panelRoot.transform);
            var resultText = CreateText("Result", canvas.transform, "Choose a skill", 28, TextAnchor.MiddleCenter);
            SetRect(resultText.rectTransform, new Vector2(.5f, .08f), new Vector2(.5f, .08f), new Vector2(700f, 60f));

            var cardPrefab = CreateCardPrefab(prefabPath);
            var panelHost = new GameObject("Skill Selection Panel");
            var panel = panelHost.AddComponent<RoguelikeSkillSelectionPanel>();
            SetReference(panel, "panelRoot", panelRoot);
            SetReference(panel, "contentRoot", contentRoot);
            SetReference(panel, "itemPrefab", cardPrefab.GetComponent<SkillChoiceItemView>());

            var choices = new[]
            {
                CreateChoice("Fireball", "Fireball", "Deal a burst of fire damage.", $"{_outputFolder}/Data/Fireball.asset"),
                CreateChoice("IceBolt", "Ice Bolt", "Fire a bolt that slows enemies.", $"{_outputFolder}/Data/IceBolt.asset"),
                CreateChoice("ChainLightning", "Chain Lightning", "Strike multiple nearby enemies.", $"{_outputFolder}/Data/ChainLightning.asset")
            };

            var controllerObject = new GameObject("Demo Controller");
            var controller = controllerObject.AddComponent<SkillSelectionDemoController>();
            var input = controllerObject.AddComponent<SkillSelectionDemoInput>();
            SetReference(controller, "selectionPanel", panel);
            SetReference(controller, "resultText", resultText);
            var controllerData = new SerializedObject(controller);
            var choicesProperty = controllerData.FindProperty("choices");
            choicesProperty.arraySize = choices.Length;
            for (var index = 0; index < choices.Length; index++)
                choicesProperty.GetArrayElementAtIndex(index).objectReferenceValue = choices[index];
            controllerData.ApplyModifiedPropertiesWithoutUndo();

            var reopenButton = CreateButton("Show Choices Button", canvas.transform, "Show Choices");
            SetRect(reopenButton.GetComponent<RectTransform>(), new Vector2(.5f, .16f), new Vector2(.5f, .16f), new Vector2(240f, 56f));
            reopenButton.onClick.AddListener(controller.ShowChoices);
            SetReference(controller, "input", input);
            SetReference(input, "showChoicesButton", reopenButton);

            EditorSceneManager.SaveScene(scene, scenePath);
            AssetDatabase.SaveAssets();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            EditorUtility.DisplayDialog("Skill Selection Demo", $"Created:\n{scenePath}", "Open Scene");
            EditorSceneManager.OpenScene(scenePath);
        }

        private static Canvas CreateCanvas()
        {
            var canvasObject = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            return canvas;
        }

        private static void CreateEventSystem()
        {
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            eventSystem.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
        }

        private static GameObject CreatePanel(Transform parent)
        {
            var panel = new GameObject("Skill Choice Panel", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            var image = panel.GetComponent<Image>();
            image.color = new Color(.06f, .08f, .14f, .96f);
            SetRect(panel.GetComponent<RectTransform>(), new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(760f, 860f));
            return panel;
        }

        private static Transform CreateContent(Transform parent)
        {
            var content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            content.transform.SetParent(parent, false);
            var layout = content.GetComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(36, 36, 80, 36);
            layout.spacing = 18f;
            layout.childControlHeight = true;
            layout.childForceExpandHeight = false;
            content.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            SetRect(content.GetComponent<RectTransform>(), new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(680f, 0f));
            return content.transform;
        }

        private static GameObject CreateCardPrefab(string prefabPath)
        {
            var card = new GameObject("Skill Choice Card", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement), typeof(SkillChoiceItemView));
            card.GetComponent<Image>().color = new Color(.16f, .21f, .32f, 1f);
            card.GetComponent<LayoutElement>().preferredHeight = 170f;
            var icon = new GameObject("Icon", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            icon.transform.SetParent(card.transform, false);
            icon.preserveAspect = true;
            SetRect(icon.rectTransform, new Vector2(0f, .5f), new Vector2(0f, .5f), new Vector2(112f, 112f));
            icon.rectTransform.anchoredPosition = new Vector2(76f, 0f);

            var title = CreateText("Title", card.transform, "Skill", 30, TextAnchor.MiddleLeft);
            SetRect(title.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(-170f, 48f));
            title.rectTransform.anchoredPosition = new Vector2(146f, -38f);
            var description = CreateText("Description", card.transform, "Description", 20, TextAnchor.UpperLeft);
            SetRect(description.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(-170f, 80f));
            description.rectTransform.anchoredPosition = new Vector2(146f, -112f);

            var view = card.GetComponent<SkillChoiceItemView>();
            SetReference(view, "selectButton", card.GetComponent<Button>());
            SetReference(view, "iconImage", icon);
            SetReference(view, "titleText", title);
            SetReference(view, "descriptionText", description);
            card.SetActive(false);
            var prefab = PrefabUtility.SaveAsPrefabAsset(card, prefabPath);
            Object.DestroyImmediate(card);
            return prefab;
        }

        private static Text CreateText(string name, Transform parent, string text, int fontSize, TextAnchor alignment)
        {
            var textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);
            var textComponent = textObject.GetComponent<Text>();
            textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            textComponent.text = text;
            textComponent.fontSize = fontSize;
            textComponent.alignment = alignment;
            textComponent.color = Color.white;
            return textComponent;
        }

        private static Button CreateButton(string name, Transform parent, string label)
        {
            var buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            buttonObject.GetComponent<Image>().color = new Color(.24f, .38f, .67f, 1f);
            var text = CreateText("Label", buttonObject.transform, label, 22, TextAnchor.MiddleCenter);
            SetRect(text.rectTransform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(240f, 56f));
            return buttonObject.GetComponent<Button>();
        }

        private static SkillChoiceDefinition CreateChoice(string id, string displayName, string description, string assetPath)
        {
            var choice = CreateInstance<SkillChoiceDefinition>();
            var data = new SerializedObject(choice);
            data.FindProperty("id").stringValue = id;
            data.FindProperty("displayName").stringValue = displayName;
            data.FindProperty("description").stringValue = description;
            data.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.CreateAsset(choice, assetPath);
            return choice;
        }

        private static void SetReference(Object target, string propertyName, Object value)
        {
            var serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 size)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.sizeDelta = size;
            rect.anchoredPosition = Vector2.zero;
        }

        private static bool IsAssetFolderPath(string path) => !string.IsNullOrWhiteSpace(path) && path.Replace('\\', '/').StartsWith("Assets/");

        private static void EnsureFolder(string folder)
        {
            var parts = folder.Replace('\\', '/').Split('/');
            var current = parts[0];
            for (var index = 1; index < parts.Length; index++)
            {
                var next = $"{current}/{parts[index]}";
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[index]);
                current = next;
            }
        }
    }
}
