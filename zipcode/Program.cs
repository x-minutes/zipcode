using System.IO.Compression;
using System.Reflection;


void printVersion()
{
    Console.WriteLine($"zipcode {Assembly.GetEntryAssembly()?.GetName().Version}");
    Console.WriteLine();
}

void printFolderExistsError()
{
    Console.WriteLine($"The last parameter passed in should be the folder to zip or file to unzip.");
    Console.WriteLine($"  zipcode CodeFolder");
    Console.WriteLine($"  zipcode -unzip mycode.zip");
    Console.WriteLine();
}


void printOverwriteError()
{
    Console.WriteLine("Use --overwrite option to overwrite an existing zip file");
    Console.WriteLine();
}
void printSuccessCreate(string zipName)
{
    Console.WriteLine($"Zip file {zipName} created.");
    Console.WriteLine();
}

void printSuccessExtract(string zipName, string folderName)
{
    Console.WriteLine($"Zip file {zipName} extracted to {folderName}.");
    Console.WriteLine();
}
void printHelp()
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
    printHelp();
    return;
}
if (allArgs.Any() && allArgs.FirstOrDefault(a => a.Equals("--version")) != null)
{
    printVersion();
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
string name = folder.Replace("../", "").Replace("./", "");
string zipName = $"{name}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.zip";
if(useDate == false)
    zipName = $"{name}.zip";

if (unZip == false)
{
    if(!Directory.Exists(folder))
    {
        printFolderExistsError();
        return;
    }
    if (overwrite == false && File.Exists(zipName))
    {
        printOverwriteError();
        return;
    }
    if (overwrite == true)
        File.Delete(zipName);
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

    printSuccessCreate(zipName);
}
else
{
    if (!File.Exists(folder))
    {
        printFolderExistsError();
        return;
    }
    string folderName = folder.Replace(".zip","");
    ZipFile.ExtractToDirectory(folder, $"{folderName}");
    printSuccessExtract(folder, folderName);
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