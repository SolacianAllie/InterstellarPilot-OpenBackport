# Trademark Policy

*InterstellarPilot: Open Frontier* is licensed under the GNU Affero General
Public License v3.0. **That license covers the code only. It grants no
rights to the project's name, logo, artwork, or store presence** — those are
separate rights, and this policy is where you find out what you may and may
not do with them.

## Scope

The AGPL guarantees that any derivative release publishes its source and keeps
the copyright notices. This policy covers the other half: making sure the
community's work isn't shipped under someone else's product name, or mistaken
for a re-release of a game that already disappeared from the stores once.

Forking is permitted and this policy doesn't restrict it — we'd still rather
people contributed here, for the reasons in
[`CONTRIBUTING.md`](CONTRIBUTING.md). What we hold is the name.

## Protected marks

The following are trademarks of this project and are not licensed by the AGPL,
by the CLA, or by any contribution:

- **InterstellarPilot: Open Frontier**
- **Open Frontier** (when used as the product/game name)
- The project logo, app icon, title artwork, and store screenshots
- Store listing names, icons, and descriptions for this project

## What you may do

- Fork the code, modify it, and build your own version
- Release your modified version under a **different name and branding**
- Describe your project truthfully, including saying it is "a fork of
  InterstellarPilot: Open Frontier" or "based on Open Frontier"
- Discuss the project, its features, and your changes in any medium
- Use the marks in plain textual reference to the original project, where the
  use doesn't imply sponsorship or endorsement (for example, a blog post titled
  "Adding Discord Rich Presence to InterstellarPilot: Open Frontier")

## If you fork: the rebranding checklist

The AGPL lets you fork. This project asks that you rebrand if you ship the
result. Everything in the first list below is **required**, because those are
the marks; everything in the second is **strongly encouraged** but genuinely
optional.

### Required — these present the product to players, and they're the marks

| What to change | Where it lives in a checkout of this project |
|---|---|
| Product and company name in the build | `ProjectSettings/ProjectSettings.asset` — `productName: InterstellarPilot - Open Frontier`, `companyName: Open Frontier` |
| Android package / application id | `ProjectSettings.asset` → `applicationIdentifier`, plus whatever your store listing uses |
| Launcher icon and app icon | `Assets/Texture2D/app icon.png` and the Android icon slots in `ProjectSettings.asset` |
| Title screen and store artwork | The title art, icon and screenshots used by your store listing |
| Discord presence title | `DiscordPresenceManager.GameDisplayName` in the Discord integration |
| Your Discord application | Register your own application; you cannot list yourself under ours |
| Support contact shown in-game | `GameController.SupportEmail` and the hardcoded string in the store screen. **Point these at you.** Leaving them pointing here means players contact this project about bugs in a build it did not ship — the single most damaging thing in this list |

Renaming the build, the icon, the store listing and the support contact is the
whole of the legal requirement. Get those right and you are in compliance.

### Strongly encouraged, but not required

- Update player-visible strings that still announce "Open Frontier" — the
  version label on the main menu, Discord presence copy, about/credits text
- Note the change honestly in your release notes: "fork of InterstellarPilot:
  Open Frontier, rebranded"

### Not required: renaming the code itself

You do **not** need to rename internal identifiers — the `OpenFrontier.*`
namespaces, the assembly definitions, prefab paths or asset folders. Only the
marks are withheld; the copyright and patent licences continue to apply in full.

Doing it anyway is a large, risky job for no legal benefit: the name appears in
roughly **2,100 source files** across **20 assembly definitions**, plus
`Resources/` paths the engine resolves by string. Renaming breaks prefab and
`Resources` references if done carelessly and can invalidate existing save
files. Your call, nobody will complain — but don't think the licence requires it.

## What you may not do without written permission

- Publishing, distributing, or offering for download any build under the
  protected names above, including modified builds
- Using the logo, app icon, title artwork, or store art in your own product,
  fork, website, or promotional material
- Storing a fork under an application id, package name, or URL that suggests it
  is the official release, or that implies endorsement or affiliation with the
  project
- Registering, or applying to register, any of the protected marks (or
  confusingly similar ones such as "Open Frontier 2", "OpenFrontier", or
  "Interstellar Pilot: Open Frontier Edition") for goods or services

## Attribution

Derivative releases must keep the original copyright and license notices
intact (AGPL-3.0 sections 4 and 5) and should state clearly which parts are
original work. This policy does not require you to advertise the project, hide
your changes, or ask permission to build something new — it only asks that you
don't take the name.

## Permission

If you need an exception for a specific use, ask first. Requests for
permission to use the marks (store listing, promotional video, a special
edition, etc.) go to **nightvizla@gmail.com**. Written permission will be
granted or declined at the project's discretion.

## Enforcement

Trademark misuse is handled by asking first, then escalating. The maintainer
may ask for the listing to be renamed, may issue a takedown request against a
store listing, and may pursue other remedies available under trademark law in
the relevant jurisdiction. Because these are real legal rights, enforcement is
a deliberate choice rather than an automatic action.

## No trademark licence from the AGPL

Section 7(e) of the AGPL explicitly contemplates this: a licensor may
"decline to grant rights under trademark law for use of some trade names,
trademarks, or service marks." That is exactly what this project does. The
copyright and patent licences in the AGPL and in `CLA.md` continue to apply in
full; only the marks are withheld.