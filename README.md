# InterstellarPilot: Open Frontier

**InterstellarPilot: Open Frontier** is an open-source project dedicated to preserving and modernizing InterstellarPilot 2, a beloved mobile game originally developed by Pixelfactor.

After being abandoned multiple times and now being removed from app stores for an extended period, the game is at risk of dying out again.

This project aims to decompile, rebuild, and port the game to **Unity 6 with URP**, breathing new life into it for the community to modify, enhance, and enjoy for years to come.

There are also long term future plans to overhaul the game with long requested features and abilities.

## Device compatibility

Unity 6 sets the floor: Android devices need **OpenGL ES 3.1+ or Vulkan** (roughly 2016-era mid-range hardware and newer). Devices below that line (e.g. Adreno 3xx GPUs such as the Adreno 308) fail at startup with "Unable to initialize the Unity Engine Graphics API" - this is an engine limitation, not something the project can configure away.

**We will not leave these players behind.** A backport effort is planned for the future (for example a legacy branch on an older Unity LTS, or a compatibility build) so that players on older hardware can keep flying too. Until then, the Play Store listing is filtered to compatible devices automatically.
