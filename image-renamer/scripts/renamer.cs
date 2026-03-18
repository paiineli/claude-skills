// Exit codes:
// 0 = sucesso
// 1 = uso incorreto
// 2 = pasta inexistente
// 3 = erro inesperado

using System.Runtime.InteropServices;

if (args.Length == 0)
{
    Console.Error.WriteLine("Uso: RenameImagesToGuid <PASTA> [--recursive]");
    return 1;
}

var dir = args[0];
var recursive = args.Any(a => a.Equals("--recursive", StringComparison.OrdinalIgnoreCase));

if (!Directory.Exists(dir))
{
    Console.Error.WriteLine($"Pasta não encontrada {dir}");
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
    Console.WriteLine($"Sem permissão: {ex.Message}");
    return 3;
}
catch (IException ex)
{
    Console.WriteLine($"Erro de I/O: {ex.Message}");
    return 3;
}
catch (Exception ex)
{
    Console.WriteLine($"Erro inesperado: {ex}");
    return 3;
}