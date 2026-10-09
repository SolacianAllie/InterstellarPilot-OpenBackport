# InterstellarPilot: Open Frontier

**InterstellarPilot: Open Frontier** is an open-source project dedicated to preserving and modernizing [InterstellarPilot 2](https://store.steampowered.com/app/2199580/Interstellar_Pilot_2/), a beloved mobile game originally developed by Pixelfactor.

After being abandoned multiple times and now being removed from app stores for an extended period, the game is at risk of dying out again.

This project aims to decompile, rebuild, and port the game to **Unity 6 with URP**, breathing new life into it for the community to modify, enhance, and enjoy for years to come.

This is the **actively developed** line of the project. If your device can run it, this is the branch to use.

## If your device is too old

Unity 6.6 raised the Android graphics floor to **OpenGL ES 3.1**, which locks out a large band of still-in-use hardware — most notably anything with an **Adreno 3xx** GPU (Adreno 308, 320, 330) and older Mali parts. Those devices fail at startup with *"Unable to initialize the Unity Engine Graphics API"*. It is an engine limitation and cannot be configured away.

**We will not leave these players behind.** [InterstellarPilot: Open Backport](https://github.com/SolacianAllie/InterstellarPilot-OpenBackport) runs the same game on **Unity 6.3 LTS**, which still supports **OpenGL ES 3.0**. It is a separate project with its own releases, and it receives Open Frontier's changes by periodic automated merge.

There are also long term future plans to overhaul the game with long requested features and abilities.

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
| Requirement | This project (Unity 6.6) | Open Backport (Unity 6.3 LTS) |
|-------------|--------------------------|--------------------------------|
| Graphics API | **OpenGL ES 3.1+** or Vulkan | **OpenGL ES 3.0+** or Vulkan |
| Android version | **8.0 Oreo (API 26)** | 7.1 Nougat (API 25) |
| 32-bit ARM (armeabi-v7a) | Supported | Supported |

Requires roughly **2016-era mid-range hardware and newer**. If your device reports OpenGL ES 3.0 as its only option, or has an Adreno 3xx GPU, use [Open Backport](https://github.com/SolacianAllie/InterstellarPilot-OpenBackport).

Both `armeabi-v7a` and `arm64-v8a` are shipped in the APK. Google Play requires 64-bit support, and shipping both keeps 32-bit-only devices installable.

The Play Store listing is filtered to compatible devices automatically.

UNITY VERSION NOTES
------------------------------------------------------------
This project targets **Unity 6000.6.4f1** with **URP 17.6.0**, and is kept on the newest Unity release deliberately so it picks up engine fixes and platform support first.

Rendering is **Forward**. Both `armeabi-v7a` (ARMv7) and `arm64-v8a` are built.

CREDIT
------------------------------------------------------------
Original game by **Pixelfactor Ltd.** Interstellar Pilot 2 is available on [Steam](https://store.steampowered.com/app/2199580/Interstellar_Pilot_2/).
