# Notes for Coding Agents

Only non-obvious traps live here — things you cannot learn from the project
structure. Add new ones as they are found.

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

Headless Unity does not work in this environment (licensing client fails,
exit 198) — the user performs all editor actions.

## Non-obvious traps

- **Script GUIDs are deterministic:** `Guid(MD5(assemblyName + namespace + className))`
  (AssetRipper decompile scheme). Never regenerate .meta files; renaming
  namespaces/assemblies is safe because GUIDs live in the .meta files, but the
  scheme no longer matches names after the `Pixelfactor.*` → `OpenFrontier.*` rename.
- **URP material postprocessor strips hand-edited YAML** for materials whose
  shader has a known URP ShaderID. Recreate those materials natively via the
  `Material` API; never hand-edit their YAML.
- **The BIRP project** (`InterstellarPilot - Open Frontier BIRP`, sibling
  directory) is a read-only reference of the original game — never modify it.
- **Soft particles are standardized Near=1/Far=0** (empirically verified on
  this OpenGL stack; URP's formula reads 0 behind empty space here).
- **`Screen.dpi` can return 0** (Android foldables, Linux editor). Never feed
  it into touch/drag thresholds directly — it once set
  `EventSystem.pixelDragThreshold` to 0 and ate every tap on the sector map.
  Use `OpenFrontier.ScreenDpi.Value` (Screen.dpi → JNI DisplayMetrics → 200
  fallback). Fingers.dll reads `Screen.dpi` in its Awake and falls back to
  its serialized DefaultDPI with a red error; `FingersDpiFix` on the
  GameController prefab's EventSystem object overrides `DeviceInfo` after
  each scene load. The one-time red error at startup is cosmetic and cannot
  be silenced without patching the DLL.
- **uGUI positions on ScreenSpace-Overlay canvases are in screen pixels**;
  any hit-test radius authored in "pixels" must be multiplied by
  `Canvas.scaleFactor` or it shrinks physically on high-DPI screens (see
  `SectorMapUnitSelector`).
