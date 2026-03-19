// Exit codes:
// 0 = success
// 1 = incorrect usage
// 2 = folder not found
// 3 = unexpected error

using System.Text;
using System.Text.RegularExpressions;

var path = GetArgValue(args, "--path")
    ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

if (!Directory.Exists(path))
{
    Console.Error.WriteLine($"Folder not found: {path}");
    return 2;
}

int renamedCount = 0;

try
{
    foreach (var file in Directory.EnumerateFiles(path, "*.pdf", SearchOption.TopDirectoryOnly))
    {
        var folder = Path.GetDirectoryName(file)!;
        var originalName = Path.GetFileNameWithoutExtension(file);
        var ext = Path.GetExtension(file).ToLowerInvariant();

        var sanitized = Sanitize(originalName) + ext;

        if (sanitized == Path.GetFileName(file))
            continue;

        var newPath = Path.Combine(folder, sanitized);

        if (File.Exists(newPath))
        {
            Console.Error.WriteLine($"SKIP (collision): {Path.GetFileName(file)} -> {sanitized}");
            continue;
        }

        File.Move(file, newPath);
        Console.WriteLine($"{Path.GetFileName(file)} -> {sanitized}");
        renamedCount++;
    }

    Console.WriteLine(renamedCount);
    return 0;
}
catch (UnauthorizedAccessException ex)
{
    Console.Error.WriteLine($"Access denied: {ex.Message}");
    return 3;
}
catch (IOException ex)
{
    Console.Error.WriteLine($"I/O error: {ex.Message}");
    return 3;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Unexpected error: {ex}");
    return 3;
}

static string Sanitize(string name)
{
    // Normalize to NFD and remove combining (accent) characters
    var normalized = name.Normalize(NormalizationForm.FormD);
    var withoutAccents = new string(
        normalized.Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)
                              != System.Globalization.UnicodeCategory.NonSpacingMark)
                  .ToArray()
    );

    // Replace spaces and hyphens with underscores
    var underscored = Regex.Replace(withoutAccents, @"[\s\-]+", "_");

    // Remove any character that is not alphanumeric or underscore
    var clean = Regex.Replace(underscored, @"[^\w]", "");

    // Collapse multiple underscores and trim
    var collapsed = Regex.Replace(clean, @"_+", "_").Trim('_');

    return collapsed.ToLowerInvariant();
}

static string? GetArgValue(string[] args, string flag)
{
    var idx = Array.IndexOf(args, flag);
    return idx >= 0 && idx + 1 < args.Length ? args[idx + 1] : null;
}
