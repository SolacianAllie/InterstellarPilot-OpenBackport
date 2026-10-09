# InterstellarPilot: Open Frontier

**InterstellarPilot: Open Frontier** is an open-source project dedicated to preserving and modernizing [InterstellarPilot 2](https://store.steampowered.com/app/2199580/Interstellar_Pilot_2/), a beloved mobile game originally developed by Pixelfactor.

After being abandoned multiple times and now being removed from app stores for an extended period, the game is at risk of dying out again.

This project aims to decompile, rebuild, and port the game to **Unity 6 with URP**, breathing new life into it for the community to modify, enhance, and enjoy for years to come.

There are also long term future plans to overhaul the game with long requested features and abilities.

This is the **actively developed** line of the project. If your device can run it, this is the branch to use.

PLATFORM SUPPORT
------------------------------------------------------------
Support for both 32 and 64 bit systems (Excluding linux, which is stuck to 64 only)

| Platform | Format | Instructions |
|----------|--------|--------------|
| **Android** | APK | Download and install directly on your device |
| **Windows** | InterstellarPilot-OpenFrontier.exe (tar.gz) | Extract the .tar.gz file and run the executable |
| **Linux** | InterstellarPilot-OpenFrontier.x86_64 (tar.gz) | Extract the .tar.gz file and run the executable |

INSTALLATION
------------------------------------------------------------
1. **Android**: Download the APK and install via your device's app manager, OR
2. **[Obtainium](https://github.com/ImranR98/Obtainium)**: Add an app, Paste the Github URL for this project as a source, set the override source to github, then press + and install.
3. **Windows/Linux**: Download the appropriate `.tar.gz` file, extract it, and run the executable

DEVICE COMPATIBILITY
------------------------------------------------------------
Unity 6.6 sets the floor: Android devices need **OpenGL ES 3.1+ or Vulkan** (roughly 2016-era mid-range hardware and newer). Devices below that line (e.g. Adreno 3xx GPUs such as the Adreno 308) fail at startup with "Unable to initialize the Unity Engine Graphics API" - this is an engine limitation, not something the project can configure away.

**For any devices below this threshold**, we have a special version in tandem development, [OpenBackport](https://github.com/SolacianAllie/InterstellarPilot-OpenBackport).
This version runs in Unity 6.3 LTS and supports a lower floor than 6.6, however i still need people with lesser devices to notify me of issues with it.

| Requirement | This project (Unity 6.6) | OpenBackport (Unity 6.3 LTS) |
|-------------|--------------------------|--------------------------------|
| Graphics API | **OpenGL ES 3.1+** or Vulkan | **OpenGL ES 3.0+** or Vulkan |
| Android version | **8.0 Oreo (API 26)** | 7.1 Nougat (API 25) |
| 32-bit ARM (armeabi-v7a) | Supported | Supported |

Both `armeabi-v7a` and `arm64-v8a` are shipped in the APK. Google Play requires 64-bit support, and shipping both keeps 32-bit-only devices installable. The Play Store listing is filtered to compatible devices automatically.

UNITY VERSION
------------------------------------------------------------
This project targets **Unity 6000.6.4f1** with **URP 17.6.0**, and is kept on the newest Unity release deliberately so it picks up engine fixes and platform support first.

Rendering is **Forward**.

SUPPORT THIS PROJECT
------------------------------------------------------------
You can support development of this project by funding me on [Ko-Fi](https://ko-fi.com/solacianallie).
To avoid needing to monetize the game directly, Any desire from the community to support me in development of this project can be done there!
Depending on how much support i get, i will start releasing development builds of the game for people to try out and updates about things as i work on them, just to reward those who sink their own hard earned cash into helping support me while i develop this.

CREDIT
------------------------------------------------------------
Original game by **Pixelfactor Ltd.** Interstellar Pilot 2 is available on [Steam](https://store.steampowered.com/app/2199580/Interstellar_Pilot_2/).

LICENSING & CONTRIBUTING
------------------------------------------------------------
**The code is AGPL-3.0. The name is not.**

Anyone may use, modify and redistribute this project under the
[AGPL-3.0](LICENSE), provided they publish their source and keep the copyright
notices. The name, logo, app icon and store presence are **not** licensed
([TRADEMARK_POLICY.md](TRADEMARK_POLICY.md)): fork freely, build it under your
own branding, and don't ship your version as "Open Frontier".

GitHub lets anyone fork a public repo and the AGPL grants it outright, so that
isn't ours to refuse. What we do ask is that changes come back here. This game
nearly died once; the knowledge that keeps it alive (see [AGENTS.md](AGENTS.md))
only helps if it stays in one place. A fork fixes nothing for players of this
build — a pull request lands in the one they download.

Forking anyway? Rebrand it: build name, launcher icon, store listing, your own
Discord application, and the in-game support email, which must point at you and
not us. The file-by-file checklist is in
[TRADEMARK_POLICY.md](TRADEMARK_POLICY.md). Renaming the internal namespaces is
not required, and at ~2,100 files it breaks prefab and save references if done
carelessly.

Contributions are welcome — read [CONTRIBUTING.md](CONTRIBUTING.md). Note
[`CLA.md`](CLA.md): the rights you grant so the project can keep being published
(a **draft that has not been lawyer-reviewed**, and we mean it).
