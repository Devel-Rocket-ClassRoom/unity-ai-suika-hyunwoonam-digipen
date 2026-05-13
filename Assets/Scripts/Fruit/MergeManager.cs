using System.Collections.Generic;
using SuikaGame.Game;
using UnityEngine;

namespace SuikaGame.Fruit
{
    public sealed class MergeManager : MonoBehaviour
    {
        [SerializeField]
        private FruitRuntimeFactory fruitFactory;

        [SerializeField]
        private GameManager gameManager;

        [SerializeField]
        private float mergeImpulse = 1.8f;

        [SerializeField]
        private int watermelonBonus = 500;

        private readonly Queue<MergeRequest> mergeQueue = new();
        private readonly HashSet<int> lockedFruitIds = new();

        private void Awake()
        {
            fruitFactory ??= FindFirstObjectByType<FruitRuntimeFactory>();
            gameManager ??= FindFirstObjectByType<GameManager>();
        }

        private void FixedUpdate()
        {
            while (mergeQueue.Count > 0)
            {
                ProcessMerge(mergeQueue.Dequeue());
            }
        }

        public bool TryQueueMerge(
            FruitController first,
            FruitController second,
            Vector2 contactPoint
        )
        {
            if (first == null || second == null || first == second)
            {
                return false;
            }

            if (gameManager != null && !gameManager.IsPlaying)
            {
                return false;
            }

            if (first.Level != second.Level || first.IsMerging || second.IsMerging)
            {
                return false;
            }

            var firstId = first.GetInstanceID();
            var secondId = second.GetInstanceID();
            if (lockedFruitIds.Contains(firstId) || lockedFruitIds.Contains(secondId))
            {
                return false;
            }

            lockedFruitIds.Add(firstId);
            lockedFruitIds.Add(secondId);
            first.MarkMerging();
            second.MarkMerging();
            mergeQueue.Enqueue(new MergeRequest(first, second, contactPoint));
            return true;
        }

        private void ProcessMerge(MergeRequest request)
        {
            if (request.First == null || request.Second == null)
            {
                return;
            }

            var level = request.First.Level;
            var spawnPosition =
                (
                    (Vector2)request.First.transform.position
                    + (Vector2)request.Second.transform.position
                ) * 0.5f;

            Destroy(request.First.gameObject);
            Destroy(request.Second.gameObject);

            if (level >= fruitFactory.MaxLevel)
            {
                gameManager?.AddScore(watermelonBonus);
                return;
            }

            var nextLevel = level + 1;
            var mergedFruit = fruitFactory.Spawn(nextLevel, spawnPosition, false);
            if (mergedFruit != null)
            {
                mergedFruit.ApplyMergeImpulse(mergeImpulse);
            }

            var definition = fruitFactory.GetDefinition(nextLevel);
            gameManager?.AddScore(definition != null ? definition.MergeScore : nextLevel);
        }

        private readonly struct MergeRequest
        {
            public readonly FruitController First;
            public readonly FruitController Second;
            public readonly Vector2 ContactPoint;

            public MergeRequest(FruitController first, FruitController second, Vector2 contactPoint)
            {
                First = first;
                Second = second;
                ContactPoint = contactPoint;
            }
        }
    }
}
