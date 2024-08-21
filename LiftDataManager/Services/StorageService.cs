using Windows.Storage;

namespace LiftDataManager.Services;
public class StorageService : IStorageService
{
    public string InstallationPath => AppDomain.CurrentDomain.BaseDirectory;

    public async Task<Stream> OpenStreamFromInstallationPathForReadAsync(string filePath)
    {
        var file = await StorageFile.GetFileFromPathAsync(Path.Combine(InstallationPath, filePath));
        return await file.OpenStreamForReadAsync();
    }

    public async Task<Stream> OpenStreamForReadAsync(string fullFilePath)
    {
        var file = await StorageFile.GetFileFromPathAsync(fullFilePath);
        return await file.OpenStreamForReadAsync();
    }

    public async Task<string> ReadStorageFileAsync(StorageFile file) 
    {
        var stream = await file.OpenAsync(FileAccessMode.Read);
        ulong size = stream.Size;
        using var inputStream = stream.GetInputStreamAt(0);
        using var dataReader = new Windows.Storage.Streams.DataReader(inputStream);
        uint numBytesLoaded = await dataReader.LoadAsync((uint)size);
        return dataReader.ReadString(numBytesLoaded);
    }

    public async Task<IReadOnlyList<StorageFolder>> GetFoldersAsync(string folderPath) 
    {
        var folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
        return await folder.GetFoldersAsync();
    }
}
