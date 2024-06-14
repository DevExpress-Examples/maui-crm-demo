namespace CrmDemo.Helpers;

public static class FileHelper {
    public static async Task<string> EnsureAssetInAppDataAsync(string fileName) {
        string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);
        if (!File.Exists(targetFile)) {
            await CopyAssetToAppDataAsync(fileName, targetFile);
        }
        return targetFile;

        static async Task CopyAssetToAppDataAsync(string fileName, string targetFile) {
            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(fileName);

            using MemoryStream buf = new();
            fileStream.CopyTo(buf);
            buf.Seek(0, SeekOrigin.Begin);

            using FileStream outputStream = File.OpenWrite(targetFile);
            buf.CopyTo(outputStream);
            outputStream.Flush();
        }
    }

    public static string GetFilePathInCacheFolder(string fileName) {
        string fullFilePath = Path.Combine(FileSystem.Current.CacheDirectory, fileName);
        if (File.Exists(fullFilePath))
            return fullFilePath;
        return null;
    }
    public static void DeleteFile(string fullFilePath) {
        if (File.Exists(fullFilePath))
            File.Delete(fullFilePath);
    }
    public static string CopyBytesToCacheFolder(byte[] data, string fileName) {
        string outputFullPath = Path.Combine(FileSystem.Current.CacheDirectory, fileName);
        using FileStream outputStream = File.OpenWrite(outputFullPath);
        using MemoryStream fileStream = new MemoryStream(data);
        fileStream.Seek(0, SeekOrigin.Begin);
        fileStream.CopyTo(outputStream);
        return outputFullPath;
    }
}