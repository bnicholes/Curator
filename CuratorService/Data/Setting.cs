using LiteDB;

namespace CuratorService.Data
{
    internal class Setting : ISetting
    {
        [BsonId]
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
