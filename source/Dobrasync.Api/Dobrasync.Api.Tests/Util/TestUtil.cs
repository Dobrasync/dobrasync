using Dobrasync.Api.BusinessLogic.Services.Main.Blocks;
using Dobrasync.Api.BusinessLogic.Services.Main.Files;
using Dobrasync.Api.BusinessLogic.Services.Main.Libraries;
using Dobrasync.Api.BusinessLogic.Services.Main.Transactions;
using Dobrasync.Api.Database.Repos;
using Dobrasync.Common.Util;
using Version = Dobrasync.Api.Database.Entities.Version;

namespace Dobrasync.Api.Tests.Util;

public class TestUtil
{
    public static async Task<Version> EnsureFileCreatedInLibrary(string path, string sourcepath, Guid libraryId, IVersionService versionService)
    {
        return await NewFileVersion(path, sourcepath, libraryId, versionService);
    }

    public static async Task<Version> NewFileVersion(string path, string sourcepath, Guid libraryId,
        IVersionService versionService)
    {
        var fileInfo = new FileInfo(sourcepath);
        int filePermissions = FileUtil.GetFilePermissionsInOctal(sourcepath);
        byte[] sourceContent = await File.ReadAllBytesAsync(sourcepath);
        List<byte[]> blocks = Chunker.ContentToBlocks(sourceContent).ToList();
        
        return await versionService.CreateAsync(new()
        {
            FilePath = path,
            LibraryId = libraryId,
            ExpectedBlocks = blocks.Select(ChecksumUtil.CalculateBlockChecksum).ToList(),
            FileChecksum = ChecksumUtil.CalculateBlockChecksum(sourceContent),
            IsDirectory = false,
            FilePermissionsOctal = filePermissions,
            FileCreatedOnUtc = fileInfo.LastWriteTimeUtc,
            FileModifiedOnUtc = fileInfo.LastWriteTimeUtc,
        });
    }
}