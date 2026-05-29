using NUnit.Framework;
using SuikaGame.Art;
using SuikaGame.Fruit;
using SuikaGame.Game;
using UnityEngine;
using UnityEngine.TestTools;

namespace SuikaGame.Tests.EditMode
{
    public sealed class MergeRuleTests
    {
        [Test]
        public void DefaultCatalog_ContainsElevenOrderedFruitLevels()
        {
            var definitions = FruitCatalog.CreateDefaultDefinitions();

            Assert.That(definitions.Count, Is.EqualTo(11));
            for (var index = 0; index < definitions.Count; index++)
            {
                Assert.That(definitions[index].Level, Is.EqualTo(index + 1));
            }
        }

        [Test]
        public void ScoreFormula_MatchesGddTriangularScores()
        {
            var definitions = FruitCatalog.CreateDefaultDefinitions();

            foreach (var definition in definitions)
            {
                var expectedScore = definition.Level * (definition.Level + 1) / 2;
                Assert.That(definition.MergeScore, Is.EqualTo(expectedScore));
            }
        }

        [Test]
        public void DefaultCatalog_UsesDoubleSizedMissionFruitRadii()
        {
            var definitions = FruitCatalog.CreateDefaultDefinitions();
            var expectedRadii = new[]
            {
                0.46f,
                0.58f,
                0.72f,
                0.9f,
                1.1f,
                1.36f,
                1.66f,
                2f,
                2.4f,
                2.86f,
                3.4f,
            };

            for (var index = 0; index < expectedRadii.Length; index++)
            {
                Assert.That(definitions[index].Radius, Is.EqualTo(expectedRadii[index]));
            }
        }

        [Test]
        public void MissionTwoFruitArt_LoadsSpritesForEveryFruitLevel()
        {
            var definitions = FruitCatalog.CreateDefaultDefinitions();

            for (var index = 0; index < definitions.Count; index++)
            {
                Assert.That(
                    definitions[index].Sprite,
                    Is.Not.Null,
                    $"Missing Mission 2 sprite for level {definitions[index].Level}."
                );
            }
        }

        [Test]
        public void FruitController_LeavesRealSpritesUntinted()
        {
            var texture = new Texture2D(2, 2);
            var sprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, 2f, 2f),
                new Vector2(0.5f, 0.5f),
                2f
            );
            var fruitObject = new GameObject("Fruit Under Test");
            fruitObject.AddComponent<SpriteRenderer>();
            fruitObject.AddComponent<Rigidbody2D>();
            fruitObject.AddComponent<CircleCollider2D>();
            var fruit = fruitObject.AddComponent<FruitController>();
            var definition = new FruitDefinition(1, "Cherry", 0.23f, 1, Color.red, sprite);

            fruit.Configure(definition, false, new PhysicsMaterial2D());

            Assert.That(fruitObject.GetComponent<SpriteRenderer>().color, Is.EqualTo(Color.white));

            Object.DestroyImmediate(sprite);
            Object.DestroyImmediate(texture);
            Object.DestroyImmediate(fruitObject);
        }

        [Test]
        public void OptionalArtLookup_DoesNotWarnWhenFallbackWillBeUsed()
        {
            Assert.That(
                SuikaAssetProvider.LoadBackgroundSprite("MissingOptionalBackground"),
                Is.Null
            );
            Assert.That(SuikaAssetProvider.LoadUiSprite("MissingOptionalUiSprite"), Is.Null);
        }

        [Test]
        public void GameOver_RetainsFinalScoreAndRaisesResultEvent()
        {
            var managerObject = new GameObject("Game Manager Under Test");
            var manager = managerObject.AddComponent<GameManager>();
            var gameOverRaised = false;
            manager.GameOver += () => gameOverRaised = true;

            manager.StartGame();
            manager.AddScore(21);
            manager.TriggerGameOver();

            Assert.That(manager.IsPlaying, Is.False);
            Assert.That(manager.Score, Is.EqualTo(21));
            Assert.That(gameOverRaised, Is.True);

            Object.DestroyImmediate(managerObject);
        }
    }
}
