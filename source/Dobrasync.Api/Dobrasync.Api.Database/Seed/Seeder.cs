using Dobrasync.Api.Database.Entities;
using Dobrasync.Api.Shared.SeedData;
using Dobrasync.Common.Util;
using Microsoft.EntityFrameworkCore;

namespace Dobrasync.Api.Database.Seed;

public class Seeder
{
    public static void SeedUniversal(ModelBuilder builder)
    {
        builder.Entity<User>().HasData([
            new()
            {
                Id = UserSeedData.DefaultUserId,
                Username = UserSeedData.DefaultUserUsername,
                Password = PasswordHashUtil.HashPassword(UserSeedData.DefaultUserPassword),
            }
        ]);
    }
}