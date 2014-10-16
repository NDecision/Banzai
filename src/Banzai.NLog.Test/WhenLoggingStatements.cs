using System;
using System.Linq;
using Banzai.Logging;
using NLog;
using NLog.Config;
using NLog.Targets;
using NUnit.Framework;
using Should;

namespace Banzai.NLog.Test
{
    [TestFixture]
    public class WhenWritingToLog
    {
        private MemoryTarget _target;

        [SetUp]
        public void Setup()
        {
            _target = new MemoryTarget {Layout = "${message}"};
            SimpleConfigurator.ConfigureForTargetLogging(_target, LogLevel.Trace);
            LogWriter.SetFactory(new NLogWriterFactory());
            _target.Logs.Clear();
        }

        [Test]
        public void Can_Write_Fatal_With_Exception()
        {
        
            LogWriter.GetLogger(this).Fatal("Testing Fatal with exception.", new Exception("Test Exception"));
            _target.Logs.Any().ShouldBeTrue();
            
        }

        [Test]
        public void Can_Write_Fatal_Without_Exception()
        {
            LogWriter.GetLogger(this).Fatal("Testing Fatal without exception.");
            _target.Logs.Any().ShouldBeTrue();
        }

        [Test]
        public void Can_Write_Error_With_Exception()
        {
            LogWriter.GetLogger(this).Error("Testing Error with exception.", new Exception("Test Exception"));
            _target.Logs.Any().ShouldBeTrue();
        }

        [Test]
        public void Can_Write_Error_Without_Exception()
        {
            LogWriter.GetLogger(this).Error("Testing Error without exception.");
            _target.Logs.Any().ShouldBeTrue();
        }

        [Test]
        public void Can_Write_Warn_With_Exception()
        {
            LogWriter.GetLogger(this).Warn("Testing Warn with exception.", new Exception("Test Exception"));
            _target.Logs.Any().ShouldBeTrue();
        }

        [Test]
        public void Can_Write_Warn_Without_Exception()
        {
            LogWriter.GetLogger(this).Warn("Testing Warn without exception.");
            _target.Logs.Any().ShouldBeTrue();
        }

        [Test]
        public void Can_Write_Info_With_Exception()
        {
            LogWriter.GetLogger(this).Info("Testing Info with exception.", new Exception("Test Exception"));
            _target.Logs.Any().ShouldBeTrue();
        }

        [Test]
        public void Can_Write_Info_Without_Exception()
        {
            LogWriter.GetLogger(this).Info("Testing Info without exception.");
            _target.Logs.Any().ShouldBeTrue();
        }

        [Test]
        public void Can_Write_Debug_With_Exception()
        {
            LogWriter.GetLogger(this).Debug("Testing Debug with exception.", new Exception("Test Exception"));
            _target.Logs.Any().ShouldBeTrue();
        }

        [Test]
        public void Can_Write_Debug_Without_Exception()
        {
            LogWriter.GetLogger(this).Debug("Testing Debug without exception.");
            _target.Logs.Any().ShouldBeTrue();
        }

    }
}