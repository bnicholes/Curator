
namespace CuratorService.Data
{
    /// <summary>
    /// Curated Image
    /// </summary>
    public interface ICuratedImage
    {
        /// <summary>
        /// Key
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Path
        /// </summary>
        string Path { get; set; }
    }
}
