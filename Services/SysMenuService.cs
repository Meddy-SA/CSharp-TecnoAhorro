using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TecnoCredito.Contexts;
using TecnoCredito.Models.DTOs;
using TecnoCredito.Models.Enums;
using TecnoCredito.Models.System;
using TecnoCredito.Services.Interfaces;

namespace TecnoCredito.Services;

public class SysMenuService(Context context) : ISysMenu
{
    private readonly Context context = context;

    public async Task<ResponseDTO<string>> GetMenuForRole(List<RolesEnum> rolesToFilter)
    {
        var response = new ResponseDTO<string>();
        try
        {
            var categories = await context
                .SysMenuCategories.AsNoTracking()
                .Include(c => c.Items)
                .Where(c => c.Roles == null || c.Roles.Any(r => rolesToFilter.Contains(r)))
                .ToListAsync();

            foreach (var category in categories)
            {
                category.Items =
                [
                    .. category
                        .Items.Where(i => i.Roles == null || i.Roles.Intersect(rolesToFilter).Any())
                        .OrderBy(i => i.Order)
                        .ThenBy(i => i.Id),
                ];

                await AddChildrenAsync(category.Items, rolesToFilter);
            }

            var jsonSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
            };
            response.Result = JsonConvert.SerializeObject(categories, jsonSettings);
            response.IsSuccess = true;
        }
        catch (Exception ex)
        {
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<bool> AddCategoryAsync(string name, List<RolesEnum> roles)
    {
        try
        {
            SysMenuCategory menuCategory = new() { Name = name, Roles = roles };
            await context.AddAsync(menuCategory);
            await context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }

    private async Task AddChildrenAsync(ICollection<SysMenuItem> items, List<RolesEnum> rolesFilter)
    {
        var childIds = items.Select(i => i.Id).ToList();

        var children = await context
            .SysMenuItems.AsNoTracking()
            .Where(i =>
                childIds.Contains(i.SysMenuItemId ?? 0)
                && (i.Roles == null || i.Roles.Intersect(rolesFilter).Any())
            )
            .ToListAsync();

        foreach (var item in items)
        {
            var itemChildren = children
                .Where(c => c.SysMenuItemId == item.Id)
                .OrderBy(i => i.Order)
                .ThenBy(i => i.Id)
                .ToList();
            if (itemChildren.Any())
            {
                item.Items = itemChildren;
                await AddChildrenAsync(item.Items, rolesFilter);
            }
        }
    }
}
