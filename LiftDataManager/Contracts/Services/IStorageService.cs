using Windows.Storage;

namespace LiftDataManager.Contracts.Services;

/// <summary>
/// A service that handles folders and files.
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// Gets the path of the installation directory.
    /// </summary>
    string InstallationPath { get; }

    /// <summary>
    /// Gets a readonly for a file at a specified path in the installation directory.
    /// </summary>
    /// <param name="filePath">The filepath of the file to retrieve.</param>
    /// <returns>The stream for the specified file.</returns>
    Task<Stream> OpenStreamFromInstallationPathForReadAsync(string filePath);

    /// <summary>
    /// Gets a readonly for a file at a specified path.
    /// </summary>
    /// <param name="filePath">The fullfilepath of the file to retrieve.</param>
    /// <returns>The stream for the specified file.</returns>
    Task<Stream> OpenStreamForReadAsync(string fullFilePath);

    /// <summary>
    /// Gets a readonly for a storageFile.
    /// </summary>
    /// <param name="file">A storageFile to retrieve.</param>
    /// <returns>A Task<string/> for the specified file.</returns>
    Task<string> ReadStorageFileAsync(StorageFile file);

    /// <summary>
    /// Gets a readonly for a storageFolder.
    /// </summary>
    /// <param name="folderPath">The fullfilepath of the folder to retrieve</param>
    /// <returns>A Task<IReadOnlyList<StorageFolder>?/> for the specified folder.</returns>
    Task<IReadOnlyList<StorageFolder>> GetFoldersAsync(string folderPath);
}