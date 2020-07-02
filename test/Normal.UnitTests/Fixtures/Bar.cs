using System;

namespace Normal.UnitTests.Fixtures
{
    public class Bar
    {
        public int Alpha { get; set; }
        public string Bravo { get; set; }
        public string Charlie { get; } = "Can't set me";
        public string Delta;
        public double Echo;
        public DateTimeOffset? CreateDate { get; set; }
        public TheEnum Tango { get; set; }
        public TheEnum Whisky { get; set; }

        public string SomeMethod()
        {
            return "Whatever";
        }
    }
}