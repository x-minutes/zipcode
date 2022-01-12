using System.IO.Compression;
using System.Reflection;
using System.Text.RegularExpressions;

void PrintWelcome(string folder, bool unZip)
{
    if(unZip == false)
        Console.WriteLine($"Compressing {folder}...");
    else
        Console.WriteLine($"Extracting {folder}...");
}

void PrintVersion()
{
    Console.WriteLine($"zipcode {Assembly.GetEntryAssembly()?.GetName().Version}");
    Console.WriteLine();
}

void PrintFolderExistsError()
{
    Console.WriteLine($"The last parameter passed in should be the folder to zip or file to unzip.");
    Console.WriteLine($"  zipcode CodeFolder");
    Console.WriteLine($"  zipcode -unzip mycode.zip");
    Console.WriteLine();
}


void PrintOverwriteError()
{
    Console.WriteLine("Use --overwrite option to overwrite an existing zip file");
    Console.WriteLine();
}

void PrintSuccessCreate(string zipName)
{
    Console.WriteLine($"Zip file {zipName} created.");
    Console.WriteLine();
}

void PrintErrorCreate(string zipName)
{
    Console.WriteLine($"Zip file {zipName} could not be created.");
    Console.WriteLine($"Make sure that none of the files are being used by another program.");
    Console.WriteLine();
}

void PrintSuccessExtract(string zipName, string folderName)
{
    Console.WriteLine($"Zip file {zipName} extracted to {folderName}.");
    Console.WriteLine();
}
void PrintHelp()
{
    Console.WriteLine();
    Console.WriteLine("Usage: zipcode [options] <folder-to-zip | file-to-unzip>");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  -h|--help\t\tDisplay help.");
    Console.WriteLine("  --exe\t\t\tInclude exe/dll files.");
    Console.WriteLine("  --nodate\t\tDo not include time/date in filename.");
    Console.WriteLine("  --overwrite\t\tOverwrites an existing zip.");
    Console.WriteLine("  --unzip\t\tUnzips the file into a local folder.");
    Console.WriteLine("  --version\t\tVersion of zipcode.");
    Console.WriteLine();
    Console.WriteLine("folder-to-zip:");
    Console.WriteLine("  The local folder to zip.");
    Console.WriteLine();
    Console.WriteLine("file-to-unzip:");
    Console.WriteLine("  The local folder to zip.");
    Console.WriteLine();
}

bool zipExe = false;
bool useDate = true;
string password = string.Empty;
bool unZip = false;
bool overwrite = false;

var allArgs = args.AsQueryable();


if(!allArgs.Any() || allArgs.FirstOrDefault(a => a.Equals("-h") || a.Equals("--help")) != null)
{
    PrintHelp();
    return;
}
if (allArgs.Any() && allArgs.FirstOrDefault(a => a.Equals("--version")) != null)
{
    PrintVersion();
    return;
}
if (allArgs.Any() && allArgs.FirstOrDefault(a => a.Equals("--nodate")) != null)
{
    useDate = false;
}
if (allArgs.Any() && allArgs.FirstOrDefault(a => a.Equals("--overwrite")) != null)
{
    overwrite = true;
}
if (allArgs.Any() && allArgs.FirstOrDefault(a => a.Equals("--exe")) != null)
{
    zipExe = true;
}
if (allArgs.Any() && allArgs.FirstOrDefault(a => a.Equals("--unzip")) != null)
{
    unZip = true;
}

string folder = allArgs.Last();
string name = folder.Replace("../", "")
                    .Replace("./", "")
                    .Replace("..\\", "")
                    .Replace($".\\", "");
string zipName = $"{name}_{DateTime.Now:yyyyMMdd_HHmmss}.zip";
if(useDate == false)
    zipName = $"{name}.zip";

PrintWelcome(name, unZip);
if (unZip == false)
{
    if(!Directory.Exists(folder))
    {
        PrintFolderExistsError();
        return;
    }
    if (overwrite == false && File.Exists(zipName))
    {
        PrintOverwriteError();
        return;
    }
    if (overwrite == true)
        File.Delete(zipName);

    try
    {
        ZipFile.CreateFromDirectory(folder, zipName, CompressionLevel.Optimal, true);

        // remove extra stuff
        RemoveFolder(zipName, ".vs");
        RemoveFolder(zipName, "obj");
        RemoveFiles(zipName, ".pdb");

        if (zipExe == false)
        {
            RemoveFolder(zipName, "bin");
            RemoveFolder(zipName, "debug");
            RemoveFolder(zipName, "release");
            RemoveFolder(zipName, "x64/debug");
            RemoveFolder(zipName, "x64/release");
        }

        PrintSuccessCreate(zipName);
    }
    catch (Exception)
    {
        File.Delete(zipName);
        PrintErrorCreate(zipName);
    }
}
else
{
    string pattern = $"^(?:(?:[.\\/\\\\ ])*)((?:[a-zA-Z0-9 ])*)(?:_+(?:[0-9])+_+(?:[0-9])+(?:\\.zip))*";
    var match = Regex.Match(folder, pattern);

    if (!File.Exists(folder))
    {
        PrintFolderExistsError();
        return;
    }
    string folderName = folder.Replace(".zip","");
    if (match.Success)
    {
        folderName = match.Groups[1].Value;
    }
    ZipFile.ExtractToDirectory(folder, $"{folderName}");
    PrintSuccessExtract(folder, folderName);
}

void RemoveFiles(string zipName, string extension)
{
    using (ZipArchive archive = ZipFile.Open(zipName, ZipArchiveMode.Update))
    {
        List<ZipArchiveEntry> dels = new();
        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            if (entry.FullName.EndsWith($"{extension}", StringComparison.OrdinalIgnoreCase))
            {
                dels.Add(entry);
            }
        }
        foreach (var entry in dels)
        {
            entry.Delete();
        }
    }
}

void RemoveFolder(string zipName, string folderName)
{
    using (ZipArchive archive = ZipFile.Open(zipName, ZipArchiveMode.Update))
    {
        List<ZipArchiveEntry> dels = new();
        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            if (entry.FullName.Contains($"/{folderName}/", StringComparison.OrdinalIgnoreCase))
            {
                dels.Add(entry);
            }
        }
        foreach (var entry in dels)
        {
            entry.Delete();
        }
    }
}