using Newtonsoft.Json;

namespace TecnoCredito.Models.System;

public class SysMenuItem
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("icon")]
    public string Icon { get; set; } = "pi pi-play";

    [JsonProperty("style")]
    public string? Style { get; set; }

    [JsonProperty("command")]
    public string? Command { get; set; }

    [JsonProperty("badge")]
    public string? Badge { get; set; }

    [JsonIgnore]
    public int? SysMenuItemId { get; set; }

    [JsonIgnore]
    public List<int>? Roles { get; set; }

    [JsonProperty("items")]
    public ICollection<SysMenuItem> Items { get; set; } = [];

    [JsonIgnore]
    public int? SysMenuCategoryId { get; set; }
    public SysMenuCategory? Category { get; set; } = null!;

    [JsonIgnore]
    public short Order { get; set; }
}

public class SysMenuCategory
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonIgnore]
    public List<int>? Roles { get; set; }

    [JsonProperty("items")]
    public ICollection<SysMenuItem> Items { get; set; } = [];
}
