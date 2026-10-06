# Notes for Coding Agents

## Offline compile verification (`/tmp/opencode/harness`)

C# changes are verified outside the editor with a generated .NET harness:

- Regenerate: `python3 /tmp/opencode/gen_harness2.py` (rebuilds csprojs against live `Assets/Scripts`)
- Build: `cd /tmp/opencode/harness && dotnet build All.csproj -v q -nologo`

**Known blind spot:** the harness compiles against a UnityEngine *reference stub*,
not the real Unity 6 assemblies. It cannot catch Unity-version-specific API
differences. Example that slipped through (fixed in commit `e330257c`):
`Resolution.refreshRateRatio.value` is a `double` in Unity 6 but `float` in the
stub, so `Mathf.RoundToInt(value)` passed the harness but failed in the editor
with CS1503 (double → float).

**Rule:** a clean harness build is necessary but not sufficient. After any code
change, confirm zero `error CS` lines in the editor's `Logs/Editor.log`
(or ask the user for the Console output) before considering the change done.

## Verification workflow

1. Harness build (fast, catches 99% of issues)
2. Editor refocus by the user → read `Logs/Editor.log` for real-Unity compile errors
3. Runtime/rendering issues are verified by the user in Play mode or on device

## Project shape (quick map)

- `Assets/Scripts/OpenFrontier.IP/` — decompiled game code (assembly `OpenFrontier.IP`)
- `Assets/Scripts/OpenFrontier/` — port shim/utilities (`LegacyInput`, `UrpCameraStacker`, `DevHud`, `AndroidDisplayMetrics`)
- `Assets/Scripts/Pixelfactor*` — nothing; everything was renamed `Pixelfactor.*` → `OpenFrontier.*` (script .meta GUIDs untouched, all serialized references hold)
- `Assets/Shader/` — ported/fixed shaders (URP); `Assets/TextMesh Pro/Shaders/` — stock TMP shader sources
- `AGENTS.md` (this file) — keep it updated when new project-specific traps are found
