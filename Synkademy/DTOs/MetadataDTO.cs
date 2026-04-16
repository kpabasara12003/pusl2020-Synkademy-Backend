namespace Synkademy.DTOs
{
    public class MetadataDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class CreateMetadataRequest
    {
        public string Name { get; set; } = string.Empty;
    }
}