using System;

namespace Normal.UnitTests.Fixtures
{
    public struct TheStruct
    {
        public int Alpha { get; set; }
        public string Bravo { get; set; }
        public string Charlie { get; }
        public DateTimeOffset? CreateDate { get; set; }
    }
}