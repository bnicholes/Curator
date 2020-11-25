using LiteDB;

namespace curator.Data
{
    internal class Setting : ISetting
    {
        [BsonId]
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
