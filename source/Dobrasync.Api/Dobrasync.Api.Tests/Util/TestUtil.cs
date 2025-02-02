using Dobrasync.Api.BusinessLogic.Services.Main.Blocks;
using Dobrasync.Api.BusinessLogic.Services.Main.Files;
using Dobrasync.Api.BusinessLogic.Services.Main.Libraries;
using Dobrasync.Api.BusinessLogic.Services.Main.Transactions;
using Dobrasync.Api.Database.Repos;

namespace Dobrasync.Api.Tests.Util;

public class TestUtil
{
    public static async Task EnsureFileCreatedInLibrary(string path, byte[] content, Guid libraryId, IVersionService versionService)
    {
        versionService.CreateAsync(new()
        {
            FilePath = path,
            LibraryId = libraryId,
        });
    }
}