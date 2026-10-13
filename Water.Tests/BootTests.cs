using System;

namespace Water.Tests
{
    public static class BootTests
    {
        public static void Run()
        {
            if (ItemCatalog.Name(20) != "E11-SR")
                throw new InvalidOperationException("e11");
            if (RoleCatalog.Team(12) != "Foundation")
                throw new InvalidOperationException("captain");
        }
    }
}
