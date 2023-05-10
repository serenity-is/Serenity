namespace Serenity.CodeGenerator;

public interface ICodeFileHelper
{
    void CheckoutAndWrite(string file, byte[] contents);
    void CheckoutAndWrite(string file, string contents);
    void ExecuteTSC(string workingDirectory, string arguments);
    bool FileContentsEqual(string file1, string file2);
    void MergeChanges(string backup, string file);
}