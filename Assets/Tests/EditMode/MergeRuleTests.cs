using NUnit.Framework;
using SuikaGame.Fruit;

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
    }
}
