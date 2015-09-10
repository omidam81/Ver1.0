namespace Teeyoot.Module.Models
{
    public class ArtRecord
    {
        public virtual int Id { get; protected set; }
        public virtual string Name { get; set; }
        public virtual string FileName { get; set; }
        public virtual string ArtCulture { get; set; }
    }
}