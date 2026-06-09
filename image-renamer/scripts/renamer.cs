// Exit codes:
// 0 = success
// 1 = incorrect usage
// 2 = folder not found
// 3 = unexpected error

using System.Runtime.InteropServices;

if (args.Length == 0)
{
    Console.Error.WriteLine("Usage: RenameImagesToGuid <FOLDER> [--recursive]");
    return 1;
}

var dir = args[0];
var recursive = args.Any(a => a.Equals("--recursive", StringComparison.OrdinalIgnoreCase));

if (!Directory.Exists(dir))
{
    Console.Error.WriteLine($"Folder not found {dir}");
    return 2;
}

var extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
{
    ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".tif", ".tiff", ".heic"
};

var option = recursive
    ? SearchOption.AllDirectories
    : SearchOption.TopDirectoryOnly;

int renamedCount = 0;

try
{
    foreach (var file in Directory.EnumerateFiles(dir, "*.*", option)
                                 .Where(f => extensions.Contains(Path.GetExtension(f))))
    {
        var folder = Path.GetDirectoryName(file)!;
        var ext = Path.GetExtension(file);

        string newPath;
        do
        {
            var newName = Guid.NewGuid().ToString("N") + ext;
            newPath = Path.Combine(folder, newName);
        }
        while (File.Exists(newPath));

        File.Move(file, newPath);
        renamedCount++;
    }

    Console.WriteLine(renamedCount);
    return 0;
}
catch (UnauthorizedAccessException ex)
{
    Console.WriteLine($"Access denied: {ex.Message}");
    return 3;
}
catch (IOException ex)
{
    Console.WriteLine($"I/O error: {ex.Message}");
    return 3;
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex}");
    return 3;
}
