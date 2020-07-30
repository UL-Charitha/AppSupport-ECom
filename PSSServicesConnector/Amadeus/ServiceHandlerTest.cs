using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using com.amadeus.cs;

namespace com.amadeus.cs
{
    [TestFixture]
    class ServiceHandlerTest
    {
        [Test]
        public void testAirFlightInfo()
        {
            ((log4net.Repository.Hierarchy.Hierarchy) log4net.LogManager.GetRepository()).Root.RemoveAppender("CONSOLE");

            //Program.RunExample();
        }
    }
}
