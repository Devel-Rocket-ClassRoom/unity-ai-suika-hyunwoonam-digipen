using SuikaGame.Fruit;
using SuikaGame.Game;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SuikaGame.Spawner
{
    public sealed class FruitSpawner : MonoBehaviour
    {
        [SerializeField]
        private FruitRuntimeFactory fruitFactory;

        [SerializeField]
        private GameManager gameManager;

        [SerializeField]
        private float minX = -2.7f;

        [SerializeField]
        private float maxX = 2.7f;

        [SerializeField]
        private float spawnY = 3.4f;

        [SerializeField]
        private float keyboardMoveSpeed = 5.5f;

        [SerializeField]
        private float dropCooldown = 0.6f;

        [SerializeField]
        private float deadlineGraceSeconds = 0.9f;

        [SerializeField]
        private float dropHorizontalImpulse = 0.08f;

        [SerializeField]
        private float dropTorqueImpulse = 0.04f;

        private FruitController currentFruit;
        private int currentLevel;
        private int nextLevel;
        private float nextDropAllowedAt;
        private float startTime;
        private float aimX;
        private int dropCount;

        public int CurrentLevel => currentLevel;
        public int NextLevel => nextLevel;

        private void Awake()
        {
            fruitFactory ??= FindFirstObjectByType<FruitRuntimeFactory>();
            gameManager ??= FindFirstObjectByType<GameManager>();
            aimX = transform.position.x;
        }

        private void Start()
        {
            startTime = Time.time;
            currentLevel = fruitFactory.GetWeightedSpawnLevel(0f);
            nextLevel = fruitFactory.GetWeightedSpawnLevel(0f);
            SpawnPreview();
        }

        private void Update()
        {
            if (gameManager != null && !gameManager.IsPlaying)
            {
                return;
            }

            UpdateAim();
            UpdatePreviewPosition();

            if (Time.time >= nextDropAllowedAt && WantsDrop())
            {
                DropCurrentFruit();
            }
        }

        private void UpdateAim()
        {
            if (Camera.main != null)
            {
                var mouseDevice = Mouse.current;
                if (mouseDevice != null)
                {
                    var mouse = Camera.main.ScreenToWorldPoint(mouseDevice.position.ReadValue());
                    if (mouse.x >= minX - 2f && mouse.x <= maxX + 2f)
                    {
                        aimX = mouse.x;
                    }
                }
            }

            var horizontal = 0f;
            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed)
            {
                horizontal -= 1f;
            }

            if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed)
            {
                horizontal += 1f;
            }

            aimX += horizontal * keyboardMoveSpeed * Time.deltaTime;
        }

        private void UpdatePreviewPosition()
        {
            if (currentFruit == null)
            {
                return;
            }

            var radius = currentFruit.Radius;
            var clampedX = Mathf.Clamp(aimX, minX + radius, maxX - radius);
            currentFruit.transform.position = new Vector2(clampedX, spawnY);
        }

        private bool WantsDrop()
        {
            var mousePressed =
                Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
            var spacePressed =
                Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
            return mousePressed || spacePressed;
        }

        private void DropCurrentFruit()
        {
            if (currentFruit == null)
            {
                SpawnPreview();
                return;
            }

            var impulseSign = dropCount % 2 == 0 ? 1f : -1f;
            dropCount++;
            currentFruit.MarkDropped(
                deadlineGraceSeconds,
                dropHorizontalImpulse * impulseSign,
                dropTorqueImpulse * -impulseSign
            );
            currentFruit = null;

            nextDropAllowedAt = Time.time + dropCooldown;
            currentLevel = nextLevel;
            nextLevel = fruitFactory.GetWeightedSpawnLevel(Time.time - startTime);
            SpawnPreview();
        }

        private void SpawnPreview()
        {
            currentFruit = fruitFactory.Spawn(currentLevel, new Vector2(aimX, spawnY), true);
            if (currentFruit != null)
            {
                currentFruit.gameObject.layer = GameLayers.Spawner;
                UpdatePreviewPosition();
            }
        }
    }
}
