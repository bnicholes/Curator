
namespace curator.Data
{
    public interface ICuratedImage
    {
        string Name { get; set; }
        string Description { get; set; }
        string Path { get; set; }
    }
}
