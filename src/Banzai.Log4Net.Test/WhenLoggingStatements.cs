using System;
using System.Linq;
using Banzai.Logging;
using FluentAssertions;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using NUnit.Framework;

namespace Banzai.Log4Net.Test
{
    [TestFixture]
    public class WhenWritingToLog
    {
        private MemoryAppender _memoryAppender;

        [SetUp]
        public void Setup()
        {
             _memoryAppender = new MemoryAppender();
            BasicConfigurator.Configure(_memoryAppender);
        }

        [Test]
        public void Can_Write_Fatal_With_Exception()
        {
            LogWriter.SetFactory(new Log4NetWriterFactory());

            LogWriter.GetLogger(this).Fatal("Testing Fatal with exception.", new Exception("Test Exception"));

            _memoryAppender.GetEvents().Any(le => le.Level == Level.Fatal).Should().BeTrue();
            
        }

        [Test]
        public void Can_Write_Fatal_Without_Exception()
        {
            LogWriter.SetFactory(new Log4NetWriterFactory());

            LogWriter.GetLogger(this).Fatal("Testing Fatal without exception.");
            _memoryAppender.GetEvents().Any(le => le.Level == Level.Fatal).Should().BeTrue();
        }

        [Test]
        public void Can_Write_Error_With_Exception()
        {
            LogWriter.SetFactory(new Log4NetWriterFactory());

            LogWriter.GetLogger(this).Error("Testing Error with exception.", new Exception("Test Exception"));
            _memoryAppender.GetEvents().Any(le => le.Level == Level.Error).Should().BeTrue();
        }

        [Test]
        public void Can_Write_Error_Without_Exception()
        {
            LogWriter.SetFactory(new Log4NetWriterFactory());

            LogWriter.GetLogger(this).Error("Testing Error without exception.");
            _memoryAppender.GetEvents().Any(le => le.Level == Level.Error).Should().BeTrue();
        }

        [Test]
        public void Can_Write_Warn_With_Exception()
        {
            LogWriter.SetFactory(new Log4NetWriterFactory());

            LogWriter.GetLogger(this).Warn("Testing Warn with exception.", new Exception("Test Exception"));
            _memoryAppender.GetEvents().Any(le => le.Level == Level.Warn).Should().BeTrue();
        }

        [Test]
        public void Can_Write_Warn_Without_Exception()
        {
            LogWriter.SetFactory(new Log4NetWriterFactory());

            LogWriter.GetLogger(this).Warn("Testing Warn without exception.");
            _memoryAppender.GetEvents().Any(le => le.Level == Level.Warn).Should().BeTrue();
        }

        [Test]
        public void Can_Write_Info_With_Exception()
        {
            LogWriter.SetFactory(new Log4NetWriterFactory());

            LogWriter.GetLogger(this).Info("Testing Info with exception.", new Exception("Test Exception"));
            _memoryAppender.GetEvents().Any(le => le.Level == Level.Info).Should().BeTrue();
        }

        [Test]
        public void Can_Write_Info_Without_Exception()
        {
            LogWriter.SetFactory(new Log4NetWriterFactory());

            LogWriter.GetLogger(this).Info("Testing Info without exception.");
            _memoryAppender.GetEvents().Any(le => le.Level == Level.Info).Should().BeTrue();
        }

        [Test]
        public void Can_Write_Debug_With_Exception()
        {
            LogWriter.SetFactory(new Log4NetWriterFactory());

            LogWriter.GetLogger(this).Debug("Testing Debug with exception.", new Exception("Test Exception"));
            _memoryAppender.GetEvents().Any(le => le.Level == Level.Debug).Should().BeTrue();
        }

        [Test]
        public void Can_Write_Debug_Without_Exception()
        {
            LogWriter.SetFactory(new Log4NetWriterFactory());

            LogWriter.GetLogger(this).Debug("Testing Debug without exception.");
            _memoryAppender.GetEvents().Any(le => le.Level == Level.Debug).Should().BeTrue();
        }

    }
}