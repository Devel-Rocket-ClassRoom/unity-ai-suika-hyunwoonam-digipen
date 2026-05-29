using SuikaGame.Art;
using SuikaGame.Fruit;
using SuikaGame.Game;
using SuikaGame.Spawner;
using UnityEngine;
using UnityEngine.UI;

namespace SuikaGame.UI
{
    public sealed class GameHud : MonoBehaviour
    {
        [SerializeField]
        private GameManager gameManager;

        [SerializeField]
        private FruitSpawner spawner;

        [SerializeField]
        private FruitRuntimeFactory fruitFactory;

        private Text scoreText;
        private Text bestText;
        private Text nextTitleText;
        private Image nextFruitImage;
        private Text gameOverText;

        private void Awake()
        {
            gameManager ??= FindFirstObjectByType<GameManager>();
            spawner ??= FindFirstObjectByType<FruitSpawner>();
            fruitFactory ??= FindFirstObjectByType<FruitRuntimeFactory>();
            BuildCanvasHud();
        }

        private void OnEnable()
        {
            if (gameManager != null)
            {
                gameManager.ScoreChanged += HandleScoreChanged;
            }
        }

        private void OnDisable()
        {
            if (gameManager != null)
            {
                gameManager.ScoreChanged -= HandleScoreChanged;
            }
        }

        private void Update()
        {
            RefreshHud();
        }

        private void HandleScoreChanged(int score)
        {
            RefreshHud();
        }

        private void BuildCanvasHud()
        {
            if (GameObject.Find("Mission 1 Canvas") != null)
            {
                return;
            }

            var canvasObject = new GameObject("Mission 1 Canvas");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1152f, 768f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObject.AddComponent<GraphicRaycaster>();

            CreateScoreBubble(canvasObject.transform);
            CreateNextBubble(canvasObject.transform);
            CreateEvolutionRing(canvasObject.transform);
            CreateGameOverLabel(canvasObject.transform);
            RefreshHud();
        }

        private void CreateScoreBubble(Transform parent)
        {
            var bubble = CreateImage(
                "Score Bubble",
                parent,
                SuikaAssetProvider.LoadUiSprite("Bubble")
            );
            SetRect(
                bubble.rectTransform,
                new Vector2(126f, -126f),
                new Vector2(192f, 192f),
                new Vector2(0f, 1f)
            );

            CreateText(
                "Score Title",
                bubble.transform,
                "스코어",
                24,
                new Vector2(0f, 44f),
                new Vector2(160f, 34f)
            );
            scoreText = CreateText(
                "Score",
                bubble.transform,
                "0",
                34,
                new Vector2(0f, 2f),
                new Vector2(160f, 46f)
            );
        }

        private void CreateNextBubble(Transform parent)
        {
            nextTitleText = CreateText(
                "Next Title",
                parent,
                "다음",
                28,
                new Vector2(-84f, -52f),
                new Vector2(180f, 40f),
                new Vector2(1f, 1f)
            );
            var bubble = CreateImage(
                "Next Bubble",
                parent,
                SuikaAssetProvider.LoadUiSprite("Bubble")
            );
            SetRect(
                bubble.rectTransform,
                new Vector2(-108f, -146f),
                new Vector2(184f, 184f),
                new Vector2(1f, 1f)
            );

            nextFruitImage = CreateImage("Next Fruit", bubble.transform, null);
            SetRect(
                nextFruitImage.rectTransform,
                Vector2.zero,
                new Vector2(74f, 74f),
                new Vector2(0.5f, 0.5f)
            );
        }

        private void CreateEvolutionRing(Transform parent)
        {
            var title = CreateText(
                "Evolution Title",
                parent,
                "신카의 고리",
                26,
                new Vector2(-128f, -282f),
                new Vector2(220f, 42f),
                new Vector2(1f, 1f)
            );
            title.alignment = TextAnchor.MiddleCenter;

            var ring = CreateImage(
                "Evolution Ring",
                parent,
                SuikaAssetProvider.LoadUiSprite("EvolutionRing")
            );
            SetRect(
                ring.rectTransform,
                new Vector2(-132f, -392f),
                new Vector2(176f, 176f),
                new Vector2(1f, 1f)
            );

            const float radius = 68f;
            for (var level = 1; level <= 11; level++)
            {
                var angle = Mathf.PI * 2f * (level - 1) / 11f + Mathf.PI * 0.5f;
                var position = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                var fruit = fruitFactory != null ? fruitFactory.GetDefinition(level) : null;
                var icon = CreateImage($"Evolution Fruit {level}", ring.transform, fruit?.Sprite);
                icon.color =
                    fruit?.Sprite != null ? Color.white
                    : fruit != null ? fruit.Color
                    : Color.white;
                SetRect(icon.rectTransform, position, Vector2.one * 30f, new Vector2(0.5f, 0.5f));
            }
        }

        private void CreateGameOverLabel(Transform parent)
        {
            gameOverText = CreateText(
                "Game Over",
                parent,
                "Game Over",
                54,
                Vector2.zero,
                new Vector2(420f, 90f),
                new Vector2(0.5f, 0.5f)
            );
            gameOverText.color = Color.black;
            gameOverText.enabled = false;
        }

        private void RefreshHud()
        {
            if (scoreText != null)
            {
                scoreText.text = (gameManager != null ? gameManager.Score : 0).ToString();
            }

            if (bestText != null)
            {
                bestText.text = (gameManager != null ? gameManager.HighScore : 0).ToString();
            }

            if (spawner != null && fruitFactory != null && nextFruitImage != null)
            {
                var next = fruitFactory.GetDefinition(spawner.NextLevel);
                nextFruitImage.sprite = next?.Sprite ?? FruitSpriteFactory.GetCircleSprite();
                nextFruitImage.color =
                    next?.Sprite != null ? Color.white
                    : next != null ? next.Color
                    : Color.white;
            }

            if (gameOverText != null)
            {
                gameOverText.enabled = gameManager != null && !gameManager.IsPlaying;
            }
        }

        private static Image CreateImage(string name, Transform parent, Sprite sprite)
        {
            var imageObject = new GameObject(name);
            imageObject.transform.SetParent(parent, false);
            var image = imageObject.AddComponent<Image>();
            image.sprite = sprite ?? FruitSpriteFactory.GetCircleSprite();
            image.color = sprite != null ? Color.white : new Color(0.93f, 0.7f, 0.38f, 0.78f);
            image.type = Image.Type.Simple;
            return image;
        }

        private static Text CreateText(
            string name,
            Transform parent,
            string value,
            int fontSize,
            Vector2 position,
            Vector2 size
        )
        {
            return CreateText(
                name,
                parent,
                value,
                fontSize,
                position,
                size,
                new Vector2(0.5f, 0.5f)
            );
        }

        private static Text CreateText(
            string name,
            Transform parent,
            string value,
            int fontSize,
            Vector2 position,
            Vector2 size,
            Vector2 anchor
        )
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            var text = textObject.AddComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.black;
            text.raycastTarget = false;
            SetRect(text.rectTransform, position, size, anchor);
            return text;
        }

        private static void SetRect(
            RectTransform rectTransform,
            Vector2 anchoredPosition,
            Vector2 size,
            Vector2 anchor
        )
        {
            rectTransform.anchorMin = anchor;
            rectTransform.anchorMax = anchor;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = size;
        }
    }
}
