using System.Collections.Generic;
using SuikaGame.Art;
using UnityEngine;

namespace SuikaGame.Fruit
{
    [CreateAssetMenu(menuName = "Suika/Fruit Catalog", fileName = "FruitCatalog")]
    public sealed class FruitCatalog : ScriptableObject
    {
        [SerializeField]
        private List<FruitDefinition> fruits = new();

        public IReadOnlyList<FruitDefinition> Fruits => fruits;

        public FruitDefinition GetByLevel(int level)
        {
            for (var index = 0; index < fruits.Count; index++)
            {
                if (fruits[index].Level == level)
                {
                    return fruits[index];
                }
            }

            return null;
        }

        public static IReadOnlyList<FruitDefinition> CreateDefaultDefinitions()
        {
            return new[]
            {
                new FruitDefinition(
                    1,
                    "Cherry",
                    0.46f,
                    1,
                    new Color(0.83f, 0.12f, 0.18f),
                    SuikaAssetProvider.LoadFruitSprite(1)
                ),
                new FruitDefinition(
                    2,
                    "Strawberry",
                    0.58f,
                    3,
                    new Color(0.94f, 0.18f, 0.22f),
                    SuikaAssetProvider.LoadFruitSprite(2)
                ),
                new FruitDefinition(
                    3,
                    "Grape",
                    0.72f,
                    6,
                    new Color(0.52f, 0.28f, 0.74f),
                    SuikaAssetProvider.LoadFruitSprite(3)
                ),
                new FruitDefinition(
                    4,
                    "Dekopon",
                    0.9f,
                    10,
                    new Color(0.96f, 0.49f, 0.17f),
                    SuikaAssetProvider.LoadFruitSprite(4)
                ),
                new FruitDefinition(
                    5,
                    "Persimmon",
                    1.1f,
                    15,
                    new Color(0.91f, 0.38f, 0.13f),
                    SuikaAssetProvider.LoadFruitSprite(5)
                ),
                new FruitDefinition(
                    6,
                    "Apple",
                    1.36f,
                    21,
                    new Color(0.78f, 0.12f, 0.15f),
                    SuikaAssetProvider.LoadFruitSprite(6)
                ),
                new FruitDefinition(
                    7,
                    "Pear",
                    1.66f,
                    28,
                    new Color(0.78f, 0.82f, 0.35f),
                    SuikaAssetProvider.LoadFruitSprite(7)
                ),
                new FruitDefinition(
                    8,
                    "Peach",
                    2.00f,
                    36,
                    new Color(0.96f, 0.49f, 0.56f),
                    SuikaAssetProvider.LoadFruitSprite(8)
                ),
                new FruitDefinition(
                    9,
                    "Pineapple",
                    2.40f,
                    45,
                    new Color(0.95f, 0.74f, 0.19f),
                    SuikaAssetProvider.LoadFruitSprite(9)
                ),
                new FruitDefinition(
                    10,
                    "Melon",
                    2.86f,
                    55,
                    new Color(0.58f, 0.86f, 0.43f),
                    SuikaAssetProvider.LoadFruitSprite(10)
                ),
                new FruitDefinition(
                    11,
                    "Watermelon",
                    3.40f,
                    66,
                    new Color(0.13f, 0.58f, 0.27f),
                    SuikaAssetProvider.LoadFruitSprite(11)
                ),
            };
        }
    }
}
