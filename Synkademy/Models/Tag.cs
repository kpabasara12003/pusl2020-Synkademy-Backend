namespace Synkademy.Models
{
    public class Tag
    {

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<ProjectTag> ProjectLinks { get; set; } = new List<ProjectTag>();
    }
}
