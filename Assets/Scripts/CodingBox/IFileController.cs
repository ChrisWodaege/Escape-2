public interface IFileController
{
    void SaveTextToFile(string path, string text);

    string LoadTextFromFile(string path);
}
