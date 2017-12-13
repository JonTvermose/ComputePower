namespace ComputePower.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string  Description { get; set; }
        public string DllUrl { get; set; }
        public string DllName { get; set; }
        public string WebsiteUrl { get; set; }
        public bool IsDllDownloaded { get; set; }
    }
}
