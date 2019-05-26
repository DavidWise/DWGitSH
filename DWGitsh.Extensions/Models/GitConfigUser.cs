namespace DWGitsh.Extensions.Models
{
    public class GitConfigUser
    {
        public string Name { get; set; }
        public string Mailbox { get; set; }
        public string Email { get; set; }

        public override string ToString()
        {
            if (Name == null && Email == null) return string.Empty;

            return $"{{Name=\"{Name}\", Email=\"{Email}\"}}";
        }
    }
}