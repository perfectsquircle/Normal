using System;

namespace Normal.UnitTests.Fixtures
{
    public class Bar
    {
        public int Alpha { get; set; }
        public string Beta { get; set; }
        public string Charlie { get; } = "Can't set me";
        public DateTimeOffset? CreateDate { get; set; }
    }
}