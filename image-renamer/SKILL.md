---
name: Rename images from a folder
description: Read all files in a given directory and rename all images
---

## Instructions

When an user asks to rename all images in a given path, execute the script below.
If user inform to rename images recursivelly or to include subfolders, add `--recurse` flag to the command below.

**Step 1** Run the script using the provided folder path:
`dotnet {baseDir}/scripts/renamer.cs --path "$FILE_PATH"`

**Step 2** Return the number of updated files.

