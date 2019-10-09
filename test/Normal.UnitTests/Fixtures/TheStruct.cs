using System;

namespace Normal.UnitTests.Fixtures
{
    public struct TheStruct
    {
        public int Alpha { get; set; }
        public string Beta { get; set; }
        public string Charlie { get; }
        public DateTimeOffset? CreateDate { get; set; }
    }
}