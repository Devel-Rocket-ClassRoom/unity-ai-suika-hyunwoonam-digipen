using SuikaGame.Game;
using UnityEngine;

namespace SuikaGame.Fruit
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D), typeof(SpriteRenderer))]
    public sealed class FruitController : MonoBehaviour
    {
        private const float UnitCircleRadius = 0.5f;

        [SerializeField]
        private int level;

        [SerializeField]
        private float radius;

        [SerializeField]
        private bool isPreview;

        private Rigidbody2D body;
        private CircleCollider2D circleCollider;
        private MergeManager mergeManager;

        public int Level => level;
        public float Radius => radius;
        public bool IsMerging { get; private set; }
        public bool IsPreview => isPreview;
        public float IgnoreDeadlineUntil { get; private set; }
        public float BottomY =>
            circleCollider != null ? circleCollider.bounds.min.y : transform.position.y - radius;
        public float TopY =>
            circleCollider != null ? circleCollider.bounds.max.y : transform.position.y + radius;
        public Rigidbody2D Body => body;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            circleCollider = GetComponent<CircleCollider2D>();
            mergeManager = FindFirstObjectByType<MergeManager>();
        }

        public void Configure(
            FruitDefinition definition,
            bool preview,
            PhysicsMaterial2D physicsMaterial
        )
        {
            level = definition.Level;
            radius = definition.Radius;
            isPreview = preview;

            gameObject.name = preview
                ? $"Preview {definition.DisplayName}"
                : definition.DisplayName;

            body = GetComponent<Rigidbody2D>();
            circleCollider = GetComponent<CircleCollider2D>();
            var renderer = GetComponent<SpriteRenderer>();

            body.bodyType = preview ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
            body.gravityScale = preview ? 0f : 1f;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.sleepMode = RigidbodySleepMode2D.StartAwake;
            body.freezeRotation = false;

            circleCollider.radius = UnitCircleRadius;
            circleCollider.isTrigger = preview;
            circleCollider.sharedMaterial = physicsMaterial;

            renderer.sprite =
                definition.Sprite != null
                    ? definition.Sprite
                    : FruitSpriteFactory.GetCircleSprite();
            renderer.color = definition.Sprite != null ? Color.white : definition.Color;
            transform.localScale = Vector3.one * (radius * 2f);
        }

        public void MarkDropped(
            float deadlineGraceSeconds,
            float horizontalImpulse,
            float torqueImpulse
        )
        {
            isPreview = false;
            gameObject.layer = GameLayers.Fruit;
            circleCollider.isTrigger = false;
            IgnoreDeadlineUntil = Time.time + deadlineGraceSeconds;
            body.bodyType = RigidbodyType2D.Dynamic;
            body.gravityScale = 1f;
            body.WakeUp();
            body.AddForce(Vector2.right * horizontalImpulse, ForceMode2D.Impulse);
            body.AddTorque(torqueImpulse, ForceMode2D.Impulse);
        }

        public void MarkMerging()
        {
            IsMerging = true;
        }

        public void ApplyMergeImpulse(float force)
        {
            IgnoreDeadlineUntil = Time.time + 0.8f;
            body.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (isPreview || IsMerging || mergeManager == null)
            {
                return;
            }

            if (collision.collider.TryGetComponent(out FruitController other))
            {
                var contactPoint =
                    collision.contactCount > 0
                        ? collision.GetContact(0).point
                        : (Vector2)transform.position;
                mergeManager.TryQueueMerge(this, other, contactPoint);
            }
        }
    }
}
