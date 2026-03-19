# Rename PDFs for HTML

**Name:** Rename PDF files for HTML

**Description:** Read all PDF files in a given directory and rename them to be HTML-safe

## Instructions

When the user requests to rename PDF files, execute this script:

**Step 1** Run the script using the provided folder path (defaults to the Downloads folder if no path is given):
```
dotnet {baseDir}/scripts/renamer.cs --path "$FILE_PATH"
```

**Step 2** Return the number of updated files.
