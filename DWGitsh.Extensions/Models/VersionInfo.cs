namespace DWGitsh.Extensions.Models
{
    public class VersionInfo
    {
        public string Name { get; set; }
        public  int Major { get; set; }
        public int Minor { get; set; }
        public int Revision { get; set; }
        public int Build { get; set; }
        public string BuildNote { get; set; }
        public string Text { get; set; }
        public string TextFull { get; set; }
    }
}
