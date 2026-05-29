using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SuikaGame.Fruit
{
    public static class FruitSpriteFactory
    {
        private static Sprite circleSprite;

        public static Sprite GetCircleSprite()
        {
            if (circleSprite != null)
            {
                return circleSprite;
            }

            const int size = 96;
            const float center = (size - 1) * 0.5f;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
            };

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var distance =
                        Vector2.Distance(new Vector2(x, y), new Vector2(center, center)) / center;
                    var alpha = Mathf.SmoothStep(1f, 0f, Mathf.InverseLerp(0.94f, 1f, distance));
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            texture.Apply();
            circleSprite = Sprite.Create(
                texture,
                new Rect(0, 0, size, size),
                new Vector2(0.5f, 0.5f),
                size
            );
            circleSprite.name = "Generated Fruit Circle";
            return circleSprite;
        }
    }
}

namespace SuikaGame.Art
{
    public static class SuikaAssetProvider
    {
        private const string FruitPathFormat = "Assets/Art/Fruits/Fruit_{0:00}_{1}.png";
        private const string BackgroundRoot = "Assets/Art/Backgrounds/";
        private const string UiRoot = "Assets/Art/UI/";

        private static readonly string[] FruitNames =
        {
            "Cherry",
            "Strawberry",
            "Grape",
            "Dekopon",
            "Persimmon",
            "Apple",
            "Pear",
            "Peach",
            "Pineapple",
            "Melon",
            "Watermelon",
        };

        private static bool warnedMissingArt;

        public static Sprite LoadFruitSprite(int level)
        {
            if (level < 1 || level > FruitNames.Length)
            {
                return null;
            }

            return LoadSprite(
                string.Format(FruitPathFormat, level, FruitNames[level - 1]),
                warnIfMissing: true
            );
        }

        public static Sprite LoadBackgroundSprite(string assetName)
        {
            return LoadSprite($"{BackgroundRoot}{assetName}.png", warnIfMissing: false);
        }

        public static Sprite LoadUiSprite(string assetName)
        {
            return LoadSprite($"{UiRoot}{assetName}.png", warnIfMissing: false);
        }

        private static Sprite LoadSprite(string assetPath, bool warnIfMissing)
        {
#if UNITY_EDITOR
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (sprite != null)
            {
                return sprite;
            }

            if (warnIfMissing)
            {
                WarnMissingFruitArt();
            }
#endif
            return null;
        }

        private static void WarnMissingFruitArt()
        {
            if (warnedMissingArt)
            {
                return;
            }

            warnedMissingArt = true;
            Debug.LogWarning(
                "Suika fruit sprites were not found under Assets/Art/Fruits. Using generated fallback fruit visuals until the approved PNG/Sprite assets are added."
            );
        }
    }
}
