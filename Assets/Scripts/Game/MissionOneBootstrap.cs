using SuikaGame.Art;
using SuikaGame.Fruit;
using SuikaGame.Spawner;
using SuikaGame.UI;
using UnityEngine;

namespace SuikaGame.Game
{
    public static class MissionOneBootstrap
    {
        private const float LeftWallX = -3.1f;
        private const float RightWallX = 3.1f;
        private const float BottomWallY = -3.4f;
        private const float WallThickness = 0.25f;
        private const float ContainerHeight = 6.1f;
        private const float WallBottomOverlap = 0.75f;
        private const float DeadLineY = 2.55f;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (Object.FindFirstObjectByType<GameManager>() != null)
            {
                ConfigureCamera();
                CreateMissingOriginalStyleVisuals();
                return;
            }

            ConfigureCamera();
            ConfigurePhysics();
            CreateOriginalStyleBackdrop();
            CreateGameplayRoot();
            CreateContainer();
            CreateDeadLine();
        }

        private static void CreateMissingOriginalStyleVisuals()
        {
            var background = CreateVisualPanelIfMissing(
                "Original Style Background",
                Vector2.zero,
                new Vector2(14f, 9f),
                SuikaAssetProvider.LoadBackgroundSprite("Background_Brown"),
                new Color(0.82f, 0.58f, 0.29f),
                -10
            );
            EnsureScreenFillingBackground(background);
            CreateVisualPanelIfMissing(
                "Container Interior",
                Vector2.zero,
                new Vector2(5.1f, 5.2f),
                SuikaAssetProvider.LoadBackgroundSprite("ContainerInterior"),
                new Color(1f, 0.89f, 0.58f),
                0
            );
            CreateVisualPanelIfMissing(
                "Container Top Border",
                new Vector2(0f, 2.76f),
                new Vector2(5.6f, 0.38f),
                SuikaAssetProvider.LoadBackgroundSprite("ContainerBorder"),
                new Color(0.86f, 0.74f, 0.36f),
                3
            );
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
            root.AddComponent<GameManager>();
            root.AddComponent<FruitRuntimeFactory>();
            root.AddComponent<MergeManager>();
            root.AddComponent<FruitSpawner>();
            root.AddComponent<DeadLineWatcher>();
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
    }

    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class ScreenFillingSprite : MonoBehaviour
    {
        [SerializeField]
        private Camera targetCamera;

        [SerializeField]
        private float padding = 0.25f;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            targetCamera = targetCamera != null ? targetCamera : Camera.main;
        }

        private void LateUpdate()
        {
            targetCamera = targetCamera != null ? targetCamera : Camera.main;
            if (targetCamera == null || !targetCamera.orthographic)
            {
                return;
            }

            var height = targetCamera.orthographicSize * 2f + padding;
            var width = height * targetCamera.aspect + padding;
            var spriteBoundsSize =
                spriteRenderer.sprite != null ? spriteRenderer.sprite.bounds.size : Vector3.one;
            var spriteSize = new Vector2(spriteBoundsSize.x, spriteBoundsSize.y);

            transform.position = new Vector3(
                targetCamera.transform.position.x,
                targetCamera.transform.position.y,
                transform.position.z
            );
            transform.localScale = new Vector3(
                width / Mathf.Max(spriteSize.x, 0.001f),
                height / Mathf.Max(spriteSize.y, 0.001f),
                1f
            );
        }
    }
}
