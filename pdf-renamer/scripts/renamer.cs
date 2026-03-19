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
    Console.Error.WriteLine($"Pasta não encontrada: {path}");
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
            Console.Error.WriteLine($"PULAR: {Path.GetFileName(file)} -> {sanitized}");
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
    Console.Error.WriteLine($"Sem permissão: {ex.Message}");
    return 3;
}
catch (IOException ex)
{
    Console.Error.WriteLine($"Erro de I/O: {ex.Message}");
    return 3;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Erro inesperado: {ex}");
    return 3;
}

static string Sanitize(string name)
{
    // Normalizar para NFD e remover caracteres combinatórios (acentos)
    var normalized = name.Normalize(NormalizationForm.FormD);
    var withoutAccents = new string(
        normalized.Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)
                              != System.Globalization.UnicodeCategory.NonSpacingMark)
                  .ToArray()
    );

    // Substituir espaços e hífens por sublinhados
    var underscored = Regex.Replace(withoutAccents, @"[\s\-]+", "_");

    // Remover qualquer caractere que não seja alfanumérico ou sublinhado
    var clean = Regex.Replace(underscored, @"[^\w]", "");

    // Colapsar vários sublinhados e aparar
    var collapsed = Regex.Replace(clean, @"_+", "_").Trim('_');

    return collapsed.ToLowerInvariant();
}

static string? GetArgValue(string[] args, string flag)
{
    var idx = Array.IndexOf(args, flag);
    return idx >= 0 && idx + 1 < args.Length ? args[idx + 1] : null;
}
