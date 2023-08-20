using System.Collections.Generic;

namespace ScreenManagerClient.Models.RemoteConfigV3
{
    public record CachedPlaylist
    {
        public string ConnectionId { get; init; }
        public List<StorageItem> PlaylistItems { get; init; }
    }

    public record StorageModel
    {
        public List<StorageCategory> StorageCategory { get; init; }
    }

    public record StorageCategory
    {
        public string Id { get; init; }
        public string Name { get; init; }
        public List<StorageItem> StorageItems { get; init; }
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
}