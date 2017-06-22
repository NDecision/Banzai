﻿using Banzai.Logging;
using FluentAssertions;
using NUnit.Framework;

namespace Banzai.Log4Net.Test
{
    [TestFixture]
    public class WhenSettingLog4NetFactory
    {

        [Test]
        public void Factory_Returns_Correct_Logger()
        {
            LogWriter.SetFactory(new Log4NetWriterFactory());

            var writer = LogWriter.GetLogger(typeof(object));

            writer.Should().BeOfType<Log4NetWriter>();
        }

    }
}