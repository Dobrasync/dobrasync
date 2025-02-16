using System.Threading.Tasks;
using Dobrasync.Common.Clients.Database.DB.Entities;

namespace Dobrasync.Common.Clients.BusinessLogic.Services.Core;

public interface ISettingService
{
    public Task SaveSettingAsync(ESettingKey key, string value);
    public Task GetSettingAsync(ESettingKey key, string value);
}