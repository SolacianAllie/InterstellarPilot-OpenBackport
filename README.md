# InterstellarPilot: Open Backport

**InterstellarPilot: Open Backport** is a compatibility branch of [InterstellarPilot: Open Frontier](https://github.com/SolacianAllie/InterstellarPilot-OpenFrontier), an open-source project dedicated to preserving and modernizing [InterstellarPilot 2](https://store.steampowered.com/app/2199580/Interstellar_Pilot_2/), a beloved mobile game originally developed by Pixelfactor.

After being abandoned multiple times and now being removed from app stores for an extended period, the game is at risk of dying out again.

## Why this branch exists

Open Frontier runs on **Unity 6.6**, which raised the Android graphics floor to **OpenGL ES 3.1**. That single change locks out a large band of still-in-use hardware — most notably anything with an **Adreno 3xx** GPU (Adreno 308, 320, 330) and older Mali parts. Those devices fail at startup with *"Unable to initialize the Unity Engine Graphics API"*. It is an engine limitation and cannot be configured away in 6.6.

This branch runs the same game on **Unity 6.3 LTS**, which still supports **OpenGL ES 3.0**, so those players can keep flying.

**Use Open Frontier if you can.** It is the actively developed line and gets new features. Use this branch only if your device cannot reach GLES 3.1 or Vulkan.

There are also long term future plans to overhaul the game with long requested features and abilities.

PLATFORM SUPPORT
------------------------------------------------------------
Support for both 32 and 64 bit systems (Excluding linux, which is stuck to 64 only)

| Platform | Format | Instructions |
|----------|--------|--------------|
| **Android** | APK | Download and install directly on your device |
| **Windows** | InterstellarPilot-OpenBackport.exe (tar.gz) | Extract the .tar.gz file and run the executable |
| **Linux** | InterstellarPilot-OpenBackport.x86_64 (tar.gz) | Extract the .tar.gz file and run the executable |

INSTALLATION
------------------------------------------------------------
1. **Android**: Download the APK and install via your device's app manager, OR
2. **[Obtainium](https://github.com/ImranR98/Obtainium)**: Add an app, Paste the Github URL for this project as a source, set the override source to github, then press + and install.
3. **Windows/Linux**: Download the appropriate `.tar.gz` file, extract it, and run the executable

DEVICE COMPATIBILITY
------------------------------------------------------------
| Requirement | This branch (Unity 6.3 LTS) | Open Frontier (Unity 6.6) |
|-------------|----------------------------|---------------------------|
| Graphics API | **OpenGL ES 3.0+** or Vulkan | **OpenGL ES 3.1+** or Vulkan |
| Android version | **7.1 Nougat (API 25)** | 8.0 Oreo (API 26) |
| 32-bit ARM (armeabi-v7a) | Supported | Supported |

This branch reaches roughly **2013–2016-era Android hardware** that Open Frontier cannot run on. The reference device that motivated the backport is the **Samsung Galaxy Tab A8 (SM-T387W)** — Snapdragon 425, **Adreno 308**, Android 10, 2 GB RAM, **OpenGL ES 3.0 only with no Vulkan driver at all**.

Both `armeabi-v7a` and `arm64-v8a` are shipped in the APK, so 32-bit-only devices install correctly. Google Play requires 64-bit support, but shipping both is what makes this branch reach the hardware it exists for.

UNITY VERSION DIFFERENCES
------------------------------------------------------------
Running on an older LTS has real costs. This branch is maintained separately and merges from Open Frontier periodically:

- **Render pipeline:** URP 17.3 instead of 17.6. Rendering is **Forward only** — URP Deferred and Forward+ both require features GLES 3.0 lacks (Shader Model 4.5 and compute shaders respectively).
- **Baked lighting:** lighting data authored by Unity 6.6 cannot be read by 6.3 and is regenerated. Scenes fall back to realtime lighting.
- **Shader stripping** settings differ from the main branch.
- Engine and package updates land here later than on Open Frontier, by design.

DEVELOPING
------------------------------------------------------------
This branch is a separate Unity project targeting **Unity 6000.3.26f1**, and a separate git repository.

| Remote | Repository |
|--------|------------|
| `origin` | `SolacianAllie/InterstellarPilot-OpenBackport` — this project |
| `frontier` | `SolacianAllie/InterstellarPilot-OpenFrontier` — upstream |

Merging Open Frontier's changes in is automated:

```
./tools/backport-sync.sh --dry-run     # see what's incoming
./tools/backport-sync.sh               # merge, auto-resolve, commit
./tools/backport-sync.sh --verify      # check the Unity 6.3 invariants
```

It merges `frontier/Dev` by default; pass another ref to override. The script reverts editor-owned files (which Unity rewrites on every open) to the 6.3 side and re-asserts the URP global-settings asset version, which silently breaks every build if a merge bumps it. Files carrying real intent are listed for manual review.

**Close Unity before syncing**, and **build** afterwards — this branch has produced build failures that compiled without a single error.

LICENSING & CONTRIBUTING
------------------------------------------------------------
**The code is AGPL-3.0. The name is not.**

The [GNU Affero General Public License v3.0](LICENSE) means anyone may use, modify and redistribute this project — and if they ship a modified version, they must publish their source and keep the copyright notices. That is the deal, and it is deliberate: this is a preservation project, and the point is that improvements stay visible to the community.

What the AGPL does *not* cover is the project's identity. The "InterstellarPilot: Open Frontier" and "Open Backport" names, the logo, app icon and store presence are not licensed, per [TRADEMARK_POLICY.md](TRADEMARK_POLICY.md). Fork freely; build it under your own name. Please don't ship your version under either name — it makes it impossible for players to tell a community release from a re-release of a game that already disappeared from the stores once.

This branch is the compatibility line for OpenGL ES 3.0 hardware and is maintained alongside [Open Frontier](https://github.com/SolacianAllie/InterstellarPilot-OpenFrontier), which is the actively developed branch. Both are covered by the same licence; changes here are expected to flow back to the main project.

**Contributions are welcome** via pull request. Please read [CONTRIBUTING.md](CONTRIBUTING.md) first — and note [`CLA.md`](CLA.md), which records the rights you grant so the project can keep being published (that document is a **draft that has not been lawyer-reviewed**, and we mean it).

CREDIT
------------------------------------------------------------
Original game by **Pixelfactor Ltd.** Interstellar Pilot 2 is available on [Steam](https://store.steampowered.com/app/2199580/Interstellar_Pilot_2/).

The interface font is [Noto Sans](https://fonts.google.com/noto/specimen/Noto+Sans), licensed under the SIL Open Font License 1.1 — see `Assets/Font/NotoSans - OFL.txt`.
