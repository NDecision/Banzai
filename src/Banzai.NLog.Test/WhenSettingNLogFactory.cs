﻿using Banzai.Logging;
using FluentAssertions;
using NUnit.Framework;

namespace Banzai.NLog.Test
{
    [TestFixture]
    public class WhenSettingNLogFactory
    {

        [Test]
        public void Factory_Returns_Correct_Logger()
        {
            LogWriter.SetFactory(new NLogWriterFactory());

            var writer = LogWriter.GetLogger(typeof(object));

            writer.Should().BeOfType<NLogWriter>();
        }

    }
}