namespace Serenity.CodeGenerator;

public class CodeFileHelper(IFileSystem fileSystem, IGeneratorConsole console) : ICodeFileHelper
{
    private static readonly UTF8Encoding utf8 = new(true);
    private readonly IFileSystem fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    private readonly IGeneratorConsole console = console ?? throw new ArgumentNullException(nameof(console));

    public bool Interactive { get; set; }

    private bool? overwriteAll;

    public static byte[] ToUTF8BOM(string s)
    {
        return [.. Encoding.UTF8.GetPreamble(), .. utf8.GetBytes(s)];
    }

    public void CheckoutAndWrite(string file, string contents)
    {
        CheckoutAndWrite(file, ToUTF8BOM(contents));
    }

    public void CheckoutAndWrite(string file, byte[] contents)
    {
        fileSystem.WriteAllBytes(file, contents);
    }

    public bool FileContentsEqual(string file1, string file2)
    {
        var content1 = fileSystem.ReadAllText(file1, utf8);
        var content2 = fileSystem.ReadAllText(file2, utf8);
        return content1.Trim().Replace("\r", "", StringComparison.Ordinal) ==
            content2.Trim().Replace("\r", "", StringComparison.Ordinal);
    }

    public void MergeChanges(string backup, string file)
    {
        if (backup == null || !fileSystem.FileExists(backup) || 
            !fileSystem.FileExists(file))
            return;

        bool isEqual = FileContentsEqual(backup, file);

        if (isEqual || !Interactive)
        {
            CheckoutAndWrite(file, fileSystem.ReadAllBytes(backup));
            fileSystem.DeleteFile(backup);
            return;
        }
        
        string answer;
        if (overwriteAll == true)
            answer = "y";
        else if (overwriteAll == false)
            answer = "n";
        else
        {
            while (true)
            {
                console.Write("Overwrite " + fileSystem.GetFileName(file) + "? ([Y]es, [N]o, Yes to [A]ll, [S]kip All): ");
                answer = console.ReadLine();

                if (answer != null)
                {
                    answer = answer.Length > 0 ? answer.ToLowerInvariant()[0].ToString() : " ";
                    if (answer == "a")
                    {
                        overwriteAll = true;
                        break;
                    }
                    else if (answer == "s")
                    {
                        overwriteAll = false;
                        break;
                    }
                    else if (answer == "y" || answer == "n")
                        break;
                }
            }
        }

        if (answer == "y" || answer == "a")
        {
            fileSystem.DeleteFile(backup);
        }
        else
        {
            CheckoutAndWrite(file, fileSystem.ReadAllBytes(backup));
            fileSystem.DeleteFile(backup);
        }
    }
}