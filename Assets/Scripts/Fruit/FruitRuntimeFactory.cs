using System.Collections.Generic;
using SuikaGame.Game;
using UnityEngine;

namespace SuikaGame.Fruit
{
    public sealed class FruitRuntimeFactory : MonoBehaviour
    {
        [SerializeField]
        private FruitCatalog catalog;

        [SerializeField]
        private PhysicsMaterial2D physicsMaterial;

        private IReadOnlyList<FruitDefinition> runtimeDefinitions;

        public int MaxLevel => 11;

        private void Awake()
        {
            EnsureInitialized();
        }

        private void EnsureInitialized()
        {
            if (runtimeDefinitions != null)
            {
                return;
            }

            runtimeDefinitions =
                catalog != null && catalog.Fruits.Count > 0
                    ? catalog.Fruits
                    : FruitCatalog.CreateDefaultDefinitions();

            if (physicsMaterial == null)
            {
                physicsMaterial = new PhysicsMaterial2D("Fruit Sticky Roll")
                {
                    bounciness = 0.05f,
                    friction = 0.4f,
                };
            }
        }

        public FruitDefinition GetDefinition(int level)
        {
            EnsureInitialized();

            if (catalog != null && catalog.Fruits.Count > 0)
            {
                return catalog.GetByLevel(level);
            }

            for (var index = 0; index < runtimeDefinitions.Count; index++)
            {
                if (runtimeDefinitions[index].Level == level)
                {
                    return runtimeDefinitions[index];
                }
            }

            return null;
        }

        public FruitController Spawn(int level, Vector2 position, bool preview)
        {
            var definition = GetDefinition(level);
            if (definition == null)
            {
                Debug.LogError($"No fruit definition for level {level}.");
                return null;
            }

            var fruitObject = new GameObject(definition.DisplayName);
            fruitObject.transform.position = position;
            fruitObject.layer = GameLayers.Fruit;

            fruitObject.AddComponent<SpriteRenderer>().sortingOrder = 10 + level;
            fruitObject.AddComponent<Rigidbody2D>();
            fruitObject.AddComponent<CircleCollider2D>();

            var fruit = fruitObject.AddComponent<FruitController>();
            fruit.Configure(definition, preview, physicsMaterial);
            return fruit;
        }

        public int GetWeightedSpawnLevel(float elapsedSeconds)
        {
            if (elapsedSeconds < 5f)
            {
                return Random.value < 0.58f ? 1 : 2;
            }

            var roll = Random.value;
            if (roll < 0.35f)
            {
                return 1;
            }

            if (roll < 0.65f)
            {
                return 2;
            }

            if (roll < 0.85f)
            {
                return 3;
            }

            return roll < 0.95f ? 4 : 5;
        }
    }
}
