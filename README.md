# InterstellarPilot: Open Frontier

**InterstellarPilot: Open Frontier** is an open-source project dedicated to preserving and modernizing [InterstellarPilot 2](https://store.steampowered.com/app/2199580/Interstellar_Pilot_2/), a beloved mobile game originally developed by Pixelfactor.

After being abandoned multiple times and now being removed from app stores for an extended period, the game is at risk of dying out again.

This project aims to decompile, rebuild, and port the game to **Unity 6 with URP**, breathing new life into it for the community to modify, enhance, and enjoy for years to come.

There are also long term future plans to overhaul the game with long requested features and abilities.

PLATFORM SUPPORT
------------------------------------------------------------
This release is **playable on all platforms**
Support for both 32 and 64 bit systems (Excluding linux, which is stuck to 64 only)
Runs on all devices capable of running OpenGL ES 3.1+ (Older versions will not work)
| Platform | Format | Instructions |
|----------|--------|--------------|
| **Android** | APK | Download and install directly on your device |
| **Windows** | InterstellarPilot-OpenFrontier.exe (tar.gz) | Extract the .tar.gz file and run the executable |
| **Linux** | InterstellarPilot-OpenFrontier.x86_64 (tar.gz) | Extract the .tar.gz file and run the executable |

INSTALLATION
------------------------------------------------------------
1. **Android**: Download the APK and install via your device's app manager, OR
2. **[Obtainium](https://github.com/ImranR98/Obtainium)**: Add an app, Paste the Github URL for this project as a source, set the override source to github, then press + and install.
2. **Windows/Linux**: Download the appropriate `.tar.gz` file, extract it, and run the executable

DEVICE COMPATIBILITY
------------------------------------------------------------
Unity 6 sets the floor: Android devices need **OpenGL ES 3.1+ or Vulkan** (roughly 2016-era mid-range hardware and newer). Devices below that line (e.g. Adreno 3xx GPUs such as the Adreno 308) fail at startup with "Unable to initialize the Unity Engine Graphics API" - this is an engine limitation, not something the project can configure away.

**We will not leave these players behind.** A backport effort is planned for the future (for example a legacy branch on an older Unity LTS, or a compatibility build) so that players on older hardware can keep flying too. Until then, the Play Store listing is filtered to compatible devices automatically.

SUPPORT THIS PROJECT
------------------------------------------------------------
You can support development of this project by funding me on [Ko-Fi](https://ko-fi.com/solacianallie).
To avoid needing to monetize the game directly, Any desire from the community to support me in development of this project can be done there!
Depending on how much support i get, i will start releasing development builds of the game for people to try out and updates about things as i work on them, just to reward those who sink their own hard earned cash into helping support me while i develop this.
