using System.Collections.Generic;
using SuikaGame.Fruit;
using UnityEngine;

namespace SuikaGame.Game
{
    public sealed class DeadLineWatcher : MonoBehaviour
    {
        [SerializeField]
        private GameManager gameManager;

        [SerializeField]
        private float lineY = 2.55f;

        [SerializeField]
        private float requiredSeconds = 2f;

        [SerializeField]
        private Transform lineTransform;

        private readonly Dictionary<int, float> exposureTimers = new();
        private readonly HashSet<int> armedFruitIds = new();

        [field: SerializeField]
        public float MaxCurrentExposureSeconds { get; private set; }

        public float LineY
        {
            get => lineY;
            set => lineY = value;
        }

        private void Awake()
        {
            gameManager ??= FindFirstObjectByType<GameManager>();
            if (lineTransform == null)
            {
                var lineObject = GameObject.Find("DeadLine");
                lineTransform = lineObject != null ? lineObject.transform : null;
            }
        }

        private void Update()
        {
            if (gameManager == null || !gameManager.IsPlaying)
            {
                exposureTimers.Clear();
                armedFruitIds.Clear();
                MaxCurrentExposureSeconds = 0f;
                return;
            }

            var liveIds = new HashSet<int>();
            MaxCurrentExposureSeconds = 0f;
            foreach (var fruit in FindObjectsByType<FruitController>(FindObjectsSortMode.None))
            {
                if (fruit == null || fruit.IsPreview)
                {
                    continue;
                }

                var id = fruit.GetInstanceID();
                liveIds.Add(id);

                var exposure = GetExposure(id, fruit);
                if (exposure == DeadlineExposure.Exceeded)
                {
                    gameManager.TriggerGameOver();
                    return;
                }

                if (exposure != DeadlineExposure.Overlapping)
                {
                    exposureTimers[id] = 0f;
                    continue;
                }

                exposureTimers.TryGetValue(id, out var elapsed);
                elapsed += Time.deltaTime;
                exposureTimers[id] = elapsed;
                MaxCurrentExposureSeconds = Mathf.Max(MaxCurrentExposureSeconds, elapsed);

                if (elapsed >= requiredSeconds)
                {
                    gameManager.TriggerGameOver();
                    return;
                }
            }

            PruneDestroyedFruits(liveIds);
        }

        private DeadlineExposure GetExposure(int id, FruitController fruit)
        {
            if (Time.time < fruit.IgnoreDeadlineUntil)
            {
                return DeadlineExposure.Safe;
            }

            var line = CurrentLineY;
            if (!armedFruitIds.Contains(id))
            {
                if (fruit.BottomY <= line)
                {
                    armedFruitIds.Add(id);
                }
                else
                {
                    return DeadlineExposure.Safe;
                }
            }

            if (fruit.BottomY > line)
            {
                return DeadlineExposure.Exceeded;
            }

            if (fruit.TopY > line)
            {
                return DeadlineExposure.Overlapping;
            }

            return DeadlineExposure.Safe;
        }

        private float CurrentLineY => lineTransform != null ? lineTransform.position.y : lineY;

        private enum DeadlineExposure
        {
            Safe,
            Overlapping,
            Exceeded,
        }

        private void PruneDestroyedFruits(HashSet<int> liveIds)
        {
            var staleIds = new List<int>();
            foreach (var id in exposureTimers.Keys)
            {
                if (!liveIds.Contains(id))
                {
                    staleIds.Add(id);
                }
            }

            for (var index = 0; index < staleIds.Count; index++)
            {
                exposureTimers.Remove(staleIds[index]);
                armedFruitIds.Remove(staleIds[index]);
            }
        }
    }
}
