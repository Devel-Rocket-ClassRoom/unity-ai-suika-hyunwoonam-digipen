using System;
using UnityEngine;

namespace SuikaGame.Fruit
{
    [Serializable]
    public sealed class FruitDefinition
    {
        [SerializeField]
        private int level;

        [SerializeField]
        private string displayName = "Fruit";

        [SerializeField]
        private float radius = 0.25f;

        [SerializeField]
        private int mergeScore = 1;

        [SerializeField]
        private Color color = Color.white;

        [SerializeField]
        private Sprite sprite;

        public int Level => level;
        public string DisplayName => displayName;
        public float Radius => radius;
        public int MergeScore => mergeScore;
        public Color Color => color;
        public Sprite Sprite => sprite;

        public FruitDefinition(
            int level,
            string displayName,
            float radius,
            int mergeScore,
            Color color,
            Sprite sprite = null
        )
        {
            this.level = level;
            this.displayName = displayName;
            this.radius = radius;
            this.mergeScore = mergeScore;
            this.color = color;
            this.sprite = sprite;
        }
    }
}
