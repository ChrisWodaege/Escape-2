using System.IO;
using UnityEngine;

public class FileController : IFileController
{
    private string _sourceLoadFolder = Path.Combine(Application.dataPath, "Scripts");
    private string _sourceSaveFolder = Path.Combine(Application.streamingAssetsPath, "Scripts");

    private string _subFolder = string.Empty;

    private string _LoadPath { get { return Path.Combine(_sourceLoadFolder, _subFolder); } }
    private string _SavePath { get { return Path.Combine(_sourceSaveFolder, _subFolder); } }

    public FileController(string subFolder)
    {
        _subFolder = subFolder;

        CreateFolder(string.Empty, _LoadPath);
        CreateFolder(string.Empty, _SavePath);
    }

    public string LoadTextFromFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return string.Empty;
        }

        string path = Path.Combine(_LoadPath, fileName);

        if (!File.Exists(path))
        {
            return string.Empty;
        }

        return File.ReadAllText(path);
    }

    public void SaveTextToFile(string fileName, string text)
    {
        if (!string.IsNullOrEmpty(fileName))
        {
            string path = Path.Combine(_SavePath, fileName);

            if (!File.Exists(path))
            {
                path = CreateSaveFile(fileName);

                UnityEditor.AssetDatabase.Refresh();
            }

            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    File.WriteAllText(path, text);
                }
                catch
                {

                }
            }
        }
    }

    public string GetSaveFilePath(string fileName)
    {
        string path = Path.Combine(_SavePath, fileName);

        if (!File.Exists(path))
        {
            return string.Empty;
        }

        return path;
    }

    private string CreateSaveFile(string fileName)
    {
        string path = Path.Combine(_SavePath, fileName);

        if (!File.Exists(path))
        {
            var fileStream = File.Create(path);
            fileStream.Dispose();
        }

        return path;
    }

    private string CreateFolder(string path, string folderName)
    {
        path = Path.Combine(path, folderName);

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }
}
