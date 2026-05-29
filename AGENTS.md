# Codex Project Guide

## Project

This repository is a Unity project. Treat `Assets`, `Packages`, and `ProjectSettings` as the primary project surface. Avoid editing generated Unity folders such as `Library`, `Temp`, `Logs`, and `UserSettings` unless the user explicitly asks.

## Codex Equivalents

- `CLAUDE.md` is replaced by this `AGENTS.md` file.
- `.claude/settings.json` is replaced by Codex app settings plus repo-local instructions in `AGENTS.md`.
- Claude `PostToolUse` formatting is replaced by this instruction: after Codex edits any `.cs` file, run `pwsh -NoProfile -ExecutionPolicy Bypass -File scripts/format-csharpier.ps1`.
- Unity access is provided through the Unity MCP server configured in Codex.

## C# Formatting

CSharpier is installed as a local dotnet tool in `dotnet-tools.json`.

When changing C# files:

1. Run `dotnet tool restore` if the local tool is unavailable.
2. Run `pwsh -NoProfile -ExecutionPolicy Bypass -File scripts/format-csharpier.ps1`.
3. If PowerShell is unavailable, run `dotnet csharpier format .`.

The Git pre-commit hook in `.githooks/pre-commit` also formats C# before commit when `core.hooksPath` points to `.githooks`.

## Verification

- For Unity editor state, prefer Unity MCP console checks before assuming the editor is clean.
- For GitHub work, use the GitHub plugin when available and `git` CLI for local repository state.
