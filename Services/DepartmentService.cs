using Hanum.Recruit.Contracts.Services;
using Hanum.Recruit.Models;
using Microsoft.Extensions.Caching.Memory;
using Notion.Client;

namespace Hanum.Recruit.Services;

/// <summary>
/// 지원서 서비스
/// </summary>
public class DepartmentService(IConfiguration configuration, INotionClient notionClient, IMemoryCache memoryCache) : IDepartmentService {
    private readonly string databaseId = configuration.GetValue<string>("Recruit:Application:DatabaseId")!;

    public async Task<IEnumerable<Department>> GetDepartmentsAsync() {
        if (!memoryCache.TryGetValue("departments", out IEnumerable<Department>? departments)) {
            var table = await notionClient.Databases.RetrieveAsync(databaseId);
            var options = ((SelectProperty)table.Properties["지원부서"]).Select.Options;
            departments = options.Select(o => new Department {
                Id = o.Id,
                Name = o.Name
            });
            memoryCache.Set("departments", departments, TimeSpan.FromMinutes(60));
        }

        return departments!;
    }

    public async Task<Department?> GetDepartmentAsync(string id) {
        return (await GetDepartmentsAsync()).FirstOrDefault(d => d.Id == id);
    }

    public async Task<Department?> GetDepartmentByNameAsync(string name) {
        return (await GetDepartmentsAsync()).FirstOrDefault(d => d.Name == name);
    }
}