﻿// ****************************************************************
// Copyright (c) 2011 NUnit Software. All rights reserved.
// ****************************************************************

using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using NUnit.Framework;

namespace NUnit.VisualStudio.TestAdapter.Tests
{
    public class TestDiscoveryTests : IMessageLogger, ITestCaseDiscoverySink
    {
        static readonly string mockAssemblyPath = Path.GetFullPath("mock-assembly.dll");

        static readonly List<TestCase> testCases = new List<TestCase>();

        [TestFixtureSetUp]
        public void LoadMockassembly()
        {
            // Sanity check to be sure we have the correct version of mock-assembly.dll
            Assert.That(NUnit.Tests.Assemblies.MockAssembly.Tests, Is.EqualTo(31),
                "The reference to mock-assembly.dll appears to be the wrong version");

            // Load the NUnit mock-assembly.dll once for this test, saving
            // the list of test cases sent to the discovery sink
            ((ITestDiscoverer)new NUnitTestDiscoverer()).DiscoverTests(new[] { mockAssemblyPath }, null, this, this);
        }

        [Test]
        [Category("TestDiscovery")]
        public void VerifyTestCaseCount()
        {
            Assert.That(testCases.Count, Is.EqualTo(NUnit.Tests.Assemblies.MockAssembly.Tests));
        }

        [TestCase("MockTest3", @"^\[.*\]NUnit.Tests.Assemblies.MockTestFixture.MockTest3$", Category = "TestDiscovery")]
        [TestCase("MockTest4", @"^\[.*\]NUnit.Tests.Assemblies.MockTestFixture.MockTest4$", Category = "TestDiscovery")]
        [TestCase("ExplicitlyRunTest", @"^\[.*\]NUnit.Tests.Assemblies.MockTestFixture.ExplicitlyRunTest$", Category = "TestDiscovery")]
        [TestCase("MethodWithParameters(9,11)", @"^\[.*\]NUnit.Tests.FixtureWithTestCases.MethodWithParameters\(9,11\)$", Category = "TestDiscovery")]
        public void VerifyTestCaseIsFound(string name, string pattern)
        {
            var testCase = testCases.Find(tc => tc.DisplayName == name);
            Assert.That(testCase.FullyQualifiedName, Is.StringMatching(pattern));
        }

        #region IMessageLogger Methods

        void IMessageLogger.SendMessage(TestMessageLevel testMessageLevel, string message)
        {
        }

        #endregion

        #region ITestCaseDiscoverySink Methods

        void ITestCaseDiscoverySink.SendTestCase(TestCase discoveredTest)
        {
            testCases.Add(discoveredTest);
        }

        #endregion
    }
}
