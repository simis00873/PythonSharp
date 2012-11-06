﻿namespace PythonSharp.Tests.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using PythonSharp.Language;

    [TestClass]
    public class DefinedClassTests
    {
        [TestMethod]
        public void DefineClassWithName()
        {
            DefinedClass klass = new DefinedClass("Spam");

            Assert.AreEqual("Spam", klass.Name);
        }

        [TestMethod]
        public void SetGetMethod()
        {
            DefinedClass klass = new DefinedClass("Spam");
            IFunction method = new NativeMethod(DummyMethod);

            klass.SetMethod("foo", method);

            var result = klass.GetMethod("foo");
            Assert.IsNotNull(result);
            Assert.AreEqual(method, result);
        }

        private object DummyMethod(IList<object> arguments)
        {
            return null;
        }
    }
}
