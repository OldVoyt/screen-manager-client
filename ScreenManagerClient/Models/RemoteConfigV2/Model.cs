namespace ScreenManagerClient.Models.RemoteConfigV2;

public record AdminSettingsModel
{
    public StorageModel? StorageModel { get; init; }
    public PlaylistModel? PlaylistModel { get; init; }
    public ScreenModel? ScreenModel { get; init; }
}

public record PlaylistModel
{
    public Playlist[] Playlists { get; init; }
}

public record Playlist
{
    public string Id { get; init; }
    public string Name { get; init; }
    public PlaylistItem[] PlaylistItems { get; init; }
}

public record PlaylistItem
{
    public string Id { get; init; }
    public string StorageItemId { get; init; }
}

public record ScreenModel
{
    public ScreenDefinition[] Screens { get; init; }
}

public record ScreenDefinition
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string? PlaylistId { get; init; }
}

public record StorageModel
{
    public StorageCategory[] StorageCategory { get; init; }
}

public record StorageCategory
{
    public string Id { get; init; }
    public string Name { get; init; }
    public StorageItem[] StorageItems { get; init; }
}

public record StorageItem
{
    public string Name { get; init; }
    public string Type { get; init; }
    public string? Format { get; init; }
    public string? Resolution { get; init; }
    public string Id { get; init; }
    public bool? Active { get; init; }
    public string Url { get; init; }
    public int Duration { get; init; }
}