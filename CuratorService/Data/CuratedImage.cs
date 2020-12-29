
using LiteDB;

namespace CuratorService.Data
{
    /// <summary>
    /// Curated Image
    /// </summary>
    public class CuratedImage : ICuratedImage
    {
        /// <summary>
        /// Key
        /// </summary>
        [BsonId]
        public string Key { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Path
        /// </summary>
        public string Path { get; set; }
    }
}
