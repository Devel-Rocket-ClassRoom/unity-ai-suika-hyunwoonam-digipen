# Codex Mission 1 Implementation Notes

Mission 1 implements the GitHub issue #1 gameplay loop from `Docs/GDD_SuikaGame.md`:

- drop aiming with mouse X, arrow keys, `A`/`D`, and drop via left click or Space
- runtime 2D fruit bodies using `Rigidbody2D` and `CircleCollider2D`
- queued same-level merges with per-fruit locking to prevent duplicate merges
- watermelon-level merge bonus that removes both max-level fruits
- deadline timing with grace after drops and merge impulses
- lightweight runtime bootstrap so `SampleScene` can be played without hand-built prefabs

The runtime bootstrap is intentionally MVP-oriented. It generates simple circular fruit sprites and wood-color container walls at play time, so art prefabs and ScriptableObject catalogs can replace the generated defaults later without rewriting the core loop.
