using SuikaGame.Art;
using SuikaGame.Fruit;
using SuikaGame.Game;
using SuikaGame.Spawner;
using SuikaGame.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SuikaGame.Editor
{
    [InitializeOnLoad]
    public static class MissionOneSceneInstaller
    {
        private const float LeftWallX = -3.1f;
        private const float RightWallX = 3.1f;
        private const float BottomWallY = -3.4f;
        private const float WallThickness = 0.25f;
        private const float ContainerHeight = 6.1f;
        private const float WallBottomOverlap = 0.75f;
        private const float DeadLineY = 2.55f;

        static MissionOneSceneInstaller()
        {
            EditorApplication.delayCall += InstallIfSampleSceneIsOpen;
        }

        [MenuItem("Suika/Mission 1/Rebuild Scene")]
        public static void RebuildActiveScene()
        {
            DeleteIfExists("Mission 1 Gameplay");
            DeleteIfExists("Left Wall");
            DeleteIfExists("Right Wall");
            DeleteIfExists("Bottom Wall");
            DeleteIfExists("DeadLine");
            DeleteIfExists("Original Style Background");
            DeleteIfExists("Container Interior");
            DeleteIfExists("Container Top Border");
            InstallIntoActiveScene();
        }

        private static void InstallIfSampleSceneIsOpen()
        {
            if (Application.isPlaying)
            {
                return;
            }

            var scene = EditorSceneManager.GetActiveScene();
            if (!scene.path.EndsWith("Assets/Scenes/SampleScene.unity"))
            {
                return;
            }

            if (GameObject.Find("Mission 1 Gameplay") != null)
            {
                if (EnsureOriginalStyleVisuals())
                {
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    EditorSceneManager.SaveOpenScenes();
                }

                return;
            }

            InstallIntoActiveScene();
        }

        private static void InstallIntoActiveScene()
        {
            EnsureLayers();
            ConfigureCamera();
            ConfigurePhysics();
            CreateOriginalStyleBackdrop();
            CreateGameplayRoot();
            CreateContainer();
            CreateDeadLine();

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            Debug.Log("Mission 1 scene objects installed into SampleScene.");
        }

        private static bool EnsureOriginalStyleVisuals()
        {
            var created = false;
            var hadBackground = GameObject.Find("Original Style Background") != null;
            var background = CreateVisualPanelIfMissing(
                "Original Style Background",
                Vector2.zero,
                new Vector2(14f, 9f),
                SuikaAssetProvider.LoadBackgroundSprite("Background_Brown"),
                new Color(0.82f, 0.58f, 0.29f),
                -10
            );
            EnsureScreenFillingBackground(background);
            created |= !hadBackground;
            var hadInterior = GameObject.Find("Container Interior") != null;
            CreateVisualPanelIfMissing(
                "Container Interior",
                Vector2.zero,
                new Vector2(5.1f, 5.2f),
                SuikaAssetProvider.LoadBackgroundSprite("ContainerInterior"),
                new Color(1f, 0.89f, 0.58f),
                0
            );
            created |= !hadInterior;
            var hadTopBorder = GameObject.Find("Container Top Border") != null;
            CreateVisualPanelIfMissing(
                "Container Top Border",
                new Vector2(0f, 2.76f),
                new Vector2(5.6f, 0.38f),
                SuikaAssetProvider.LoadBackgroundSprite("ContainerBorder"),
                new Color(0.86f, 0.74f, 0.36f),
                3
            );
            created |= !hadTopBorder;
            return created;
        }

        private static void EnsureLayers()
        {
            EnsureLayer(GameLayers.Fruit, GameLayers.FruitName);
            EnsureLayer(GameLayers.Wall, GameLayers.WallName);
            EnsureLayer(GameLayers.DeadLine, GameLayers.DeadLineName);
            EnsureLayer(GameLayers.Spawner, GameLayers.SpawnerName);
        }

        private static void EnsureLayer(int index, string name)
        {
            var tagManager = new SerializedObject(
                AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]
            );
            var layers = tagManager.FindProperty("layers");
            var layer = layers.GetArrayElementAtIndex(index);
            if (layer.stringValue == name)
            {
                return;
            }

            layer.stringValue = name;
            tagManager.ApplyModifiedProperties();
        }

        private static void ConfigureCamera()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                var cameraObject = new GameObject("Main Camera") { tag = "MainCamera" };
                camera = cameraObject.AddComponent<Camera>();
                cameraObject.AddComponent<AudioListener>();
            }

            Undo.RecordObject(camera.gameObject, "Configure Mission 1 Camera");
            camera.transform.position = new Vector3(0f, 0f, -10f);
            camera.orthographic = true;
            camera.orthographicSize = 4.6f;
            camera.backgroundColor = new Color(0.98f, 0.95f, 0.88f);
        }

        private static void ConfigurePhysics()
        {
            Physics2D.gravity = new Vector2(0f, -9.81f);
            Physics2D.velocityIterations = Mathf.Max(Physics2D.velocityIterations, 8);
            Physics2D.positionIterations = Mathf.Max(Physics2D.positionIterations, 4);
            Physics2D.defaultContactOffset = Mathf.Max(Physics2D.defaultContactOffset, 0.02f);
        }

        private static void CreateGameplayRoot()
        {
            var root = new GameObject("Mission 1 Gameplay");
            Undo.RegisterCreatedObjectUndo(root, "Create Mission 1 Gameplay");
            root.AddComponent<GameManager>();
            root.AddComponent<FruitRuntimeFactory>();
            root.AddComponent<MergeManager>();
            root.AddComponent<FruitSpawner>();
            root.AddComponent<DeadLineWatcher>().LineY = DeadLineY;
            root.AddComponent<GameHud>();
        }

        private static void CreateContainer()
        {
            CreateVisualPanel(
                "Container Interior",
                Vector2.zero,
                new Vector2(5.1f, 5.2f),
                SuikaAssetProvider.LoadBackgroundSprite("ContainerInterior"),
                new Color(1f, 0.89f, 0.58f),
                0
            );
            CreateVisualPanel(
                "Container Top Border",
                new Vector2(0f, 2.76f),
                new Vector2(5.6f, 0.38f),
                SuikaAssetProvider.LoadBackgroundSprite("ContainerBorder"),
                new Color(0.86f, 0.74f, 0.36f),
                3
            );
            CreateWall(
                "Left Wall",
                new Vector2(LeftWallX, BottomWallY + (ContainerHeight - WallBottomOverlap) * 0.5f),
                new Vector2(WallThickness, ContainerHeight + WallBottomOverlap)
            );
            CreateWall(
                "Right Wall",
                new Vector2(RightWallX, BottomWallY + (ContainerHeight - WallBottomOverlap) * 0.5f),
                new Vector2(WallThickness, ContainerHeight + WallBottomOverlap)
            );
            CreateWall(
                "Bottom Wall",
                new Vector2(0f, BottomWallY),
                new Vector2(RightWallX - LeftWallX + WallThickness * 3f, WallThickness)
            );
        }

        private static void CreateOriginalStyleBackdrop()
        {
            var background = CreateVisualPanel(
                "Original Style Background",
                Vector2.zero,
                new Vector2(14f, 9f),
                SuikaAssetProvider.LoadBackgroundSprite("Background_Brown"),
                new Color(0.82f, 0.58f, 0.29f),
                -10
            );
            EnsureScreenFillingBackground(background);
        }

        private static void CreateWall(string name, Vector2 position, Vector2 size)
        {
            var wall = new GameObject(name) { layer = GameLayers.Wall };
            Undo.RegisterCreatedObjectUndo(wall, $"Create {name}");
            wall.transform.position = position;

            var collider = wall.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;

            var renderer = wall.AddComponent<SpriteRenderer>();
            renderer.sprite = CreateSolidSprite();
            renderer.color = new Color(0.63f, 0.42f, 0.25f);
            renderer.sortingOrder = 1;
            wall.transform.localScale = size;
        }

        private static GameObject CreateVisualPanel(
            string name,
            Vector2 position,
            Vector2 size,
            Sprite sprite,
            Color fallbackColor,
            int sortingOrder
        )
        {
            var panel = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(panel, $"Create {name}");
            panel.transform.position = position;
            var renderer = panel.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite != null ? sprite : CreateSolidSprite();
            renderer.color = sprite != null ? Color.white : fallbackColor;
            renderer.sortingOrder = sortingOrder;
            panel.transform.localScale = size;
            return panel;
        }

        private static GameObject CreateVisualPanelIfMissing(
            string name,
            Vector2 position,
            Vector2 size,
            Sprite sprite,
            Color fallbackColor,
            int sortingOrder
        )
        {
            var existing = GameObject.Find(name);
            if (existing != null)
            {
                return existing;
            }

            return CreateVisualPanel(name, position, size, sprite, fallbackColor, sortingOrder);
        }

        private static void EnsureScreenFillingBackground(GameObject background)
        {
            if (background != null && background.GetComponent<ScreenFillingSprite>() == null)
            {
                background.AddComponent<ScreenFillingSprite>();
            }
        }

        private static void CreateDeadLine()
        {
            var deadLine = new GameObject("DeadLine") { layer = GameLayers.DeadLine };
            Undo.RegisterCreatedObjectUndo(deadLine, "Create DeadLine");
            deadLine.transform.position = new Vector2(0f, DeadLineY);

            var trigger = deadLine.AddComponent<BoxCollider2D>();
            trigger.isTrigger = true;
            trigger.size = new Vector2(RightWallX - LeftWallX, 4f);
            trigger.offset = new Vector2(0f, 2f);

            var line = deadLine.AddComponent<LineRenderer>();
            line.positionCount = 2;
            line.SetPosition(0, new Vector3(LeftWallX + WallThickness, DeadLineY, 0f));
            line.SetPosition(1, new Vector3(RightWallX - WallThickness, DeadLineY, 0f));
            line.startWidth = 0.035f;
            line.endWidth = 0.035f;
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = new Color(0.88f, 0.33f, 0.26f, 0.75f);
            line.endColor = new Color(0.88f, 0.33f, 0.26f, 0.75f);
        }

        private static Sprite CreateSolidSprite()
        {
            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f, 1f);
        }

        private static void DeleteIfExists(string objectName)
        {
            var existing = GameObject.Find(objectName);
            if (existing != null)
            {
                Undo.DestroyObjectImmediate(existing);
            }
        }
    }
}
