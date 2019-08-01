using System.Runtime.CompilerServices;

// to enable unit testing of internal classes and methods
[assembly: InternalsVisibleTo("Toadstool.UnitTests")]

// part of using Moq to test internals; see http://stackoverflow.com/questions/28234369/how-to-do-internal-interfaces-visible-for-moq
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]