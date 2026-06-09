<p align="center">
  <b>skills</b>
</p>

<p align="center">
  a small collection of claude code skills.<br>
  each one is a focused tool, run on demand.
</p>

---

**// architecture**

every skill lives in its own folder with a `SKILL.md` describing when and how to run it, plus the scripts it needs. the markdown is the contract claude reads; the scripts do the work.

- one folder per skill, self-contained
- `SKILL.md` carries the trigger and the run instructions
- scripts are file-based c# apps, run directly with `dotnet`

**// stack**

`.NET 10` · `C#` · `Claude Code Skills`

**// skills**

| skill | what it does |
|-------|--------------|
| `image-renamer` | renames every image in a folder to a fresh guid, optionally recursing into subfolders |
| `pdf-renamer` | renames pdf files to html-safe names (accents stripped, spaces and symbols normalized) |