using System.ComponentModel;

namespace MinAPI.Data.DTOs
{
    public class PostQueryParameters
    {
        public string? Search { get; set; }
        public string? Sort { get; set; }
        public string? Order { get; set; }

        [DefaultValue(1)]
        public int? Page { get; set; } = 1;

        [DefaultValue(50)]
        public int? PageSize { get; set; } = 50;
    }
}
