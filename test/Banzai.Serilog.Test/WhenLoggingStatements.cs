using System;
using System.Linq;
using Banzai.Logging;
using Serilog;
using NUnit.Framework;
using FluentAssertions;
using Serilog.Formatting.Display;

namespace Banzai.Serilog.Test
{
    [TestFixture]
    public class WhenWritingToLog
    {
        private TestSink _target;

        [SetUp]
        public void Setup()
        {
            _target = new TestSink(new MessageTemplateTextFormatter("${message}", null));

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Sink(_target)
                .CreateLogger();

            LogWriter.SetFactory(new SerilogWriterFactory());
            _target.Logs.Clear();
        }

        [Test]
        public void Can_Write_Fatal_With_Exception()
        {
        
            LogWriter.GetLogger(this).Fatal("Testing Fatal with exception.", new Exception("Test Exception"));
            _target.Logs.Any().Should().BeTrue();
            
        }

        [Test]
        public void Can_Write_Fatal_Without_Exception()
        {
            LogWriter.GetLogger(this).Fatal("Testing Fatal without exception.");
            _target.Logs.Any().Should().BeTrue();
        }

        [Test]
        public void Can_Write_Error_With_Exception()
        {
            LogWriter.GetLogger(this).Error("Testing Error with exception.", new Exception("Test Exception"));
            _target.Logs.Any().Should().BeTrue();
        }

        [Test]
        public void Can_Write_Error_Without_Exception()
        {
            LogWriter.GetLogger(this).Error("Testing Error without exception.");
            _target.Logs.Any().Should().BeTrue();
        }

        [Test]
        public void Can_Write_Warn_With_Exception()
        {
            LogWriter.GetLogger(this).Warn("Testing Warn with exception.", new Exception("Test Exception"));
            _target.Logs.Any().Should().BeTrue();
        }

        [Test]
        public void Can_Write_Warn_Without_Exception()
        {
            LogWriter.GetLogger(this).Warn("Testing Warn without exception.");
            _target.Logs.Any().Should().BeTrue();
        }

        [Test]
        public void Can_Write_Info_With_Exception()
        {
            LogWriter.GetLogger(this).Info("Testing Info with exception.", new Exception("Test Exception"));
            _target.Logs.Any().Should().BeTrue();
        }

        [Test]
        public void Can_Write_Info_Without_Exception()
        {
            LogWriter.GetLogger(this).Info("Testing Info without exception.");
            _target.Logs.Any().Should().BeTrue();
        }

        [Test]
        public void Can_Write_Debug_With_Exception()
        {
            LogWriter.GetLogger(this).Debug("Testing Debug with exception.", new Exception("Test Exception"));
            _target.Logs.Any().Should().BeTrue();
        }

        [Test]
        public void Can_Write_Debug_Without_Exception()
        {
            LogWriter.GetLogger(this).Debug("Testing Debug without exception.");
            _target.Logs.Any().Should().BeTrue();
        }

    }
}