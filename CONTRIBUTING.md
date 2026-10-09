# Contributing to InterstellarPilot: Open Frontier

Thanks for helping keep a game alive that its original developer walked away
from. This is a preservation and modernization project: the job is to recover
InterstellarPilot 2, get it running on modern Unity, and keep improving it for
the players who still care.

## Ground rules, up front

1. **Contributions are welcome through pull requests.**
2. **Anyone can fork** the repository — GitHub's terms give every user that
   right, and this project does not pretend otherwise. What we ask is that
   forks be built under their own name and branding: see
   [`TRADEMARK_POLICY.md`](TRADEMARK_POLICY.md). The code is AGPL-3.0; the
   "Open Frontier" name, logo, and store presence are not licensed.
3. **You keep your copyright**, and you grant the maintainer the rights needed
   to keep publishing the project (see [`CLA.md`](CLA.md)).

## Before you start

- **Read [`AGENTS.md`](AGENTS.md).** It documents the traps in this codebase
  that are not discoverable from the code itself (baked story scenarios, Unity
  6 quirks, editor-only blind spots in our compile harness). Following it will
  save you a wasted afternoon.
- **Sign the CLA** with a comment on your issue or pull request:
  > I have read and agree to the Contributor License Agreement of
  > InterstellarPilot: Open Frontier. My GitHub username is: @you
- **Open an issue first** for anything substantial. A short conversation saves
  both of you a wasted branch. Bug reports with a log excerpt are genuinely
  welcome — this project is a decompile, so oddities are expected and worth
  recording.

## Making a change

1. Fork the repository and branch from `Dev` (that is the default branch).
2. Keep the change focused. One fix or one feature per pull request — large
   mixed diffs are hard to review and hard to merge safely.
3. Match the surrounding style. The decompiled game code uses tabs and a
   distinct brace style; don't reformat files you are not otherwise changing.
   New code should read like the code around it.
4. Verify it before you send it:
   - The project's offline compile harness (see `AGENTS.md`) catches most
     mistakes in seconds.
   - Then confirm **zero `error CS` lines in `Logs/Editor.log`** after an
     editor refocus. The harness is necessary but not sufficient — it compiles
     against stubs for some dependencies and skips editor-only scripts.
   - For anything visual or runtime, play it. Runtime and rendering issues are
     verified by a human in Play mode or on device.
5. Describe what you changed, what you observed, and how you verified it.

## What we're not going to merge

- Rebranding: renaming the project, replacing the logo, or changing store
  listing identity
- Removal of attribution, copyright notices, or license headers
- Bulk dumps of generated or third-party assets (these belong in a vendored
  dependency with its own licence, not in this repository)
- Wholesale engine migrations or rewrites discussed in an issue rather than a
  pull request

## Scope guidance

Wanted, in rough priority order:

- **Playability and correctness** — crashes, save/load bugs, scenario content
  that stops progressing, platform issues (Android, Windows, Linux, foldables)
- **Device coverage** — the planned pre-Unity-6 backport, anything that widens
  the set of hardware the game runs on
- **Restoration fidelity** — behaviour from the original game that got lost in
  the port, verified against the decompiled source
- **Polish** — visual quality, combat feel, UI, accessibility
- **Feature work** — the long-requested abilities; open an issue first

## Licensing of contributions

Contributions are licensed under the GNU Affero General Public License v3.0, the
same as the rest of the project. If you contribute and want that to change, say
so before you send the code.