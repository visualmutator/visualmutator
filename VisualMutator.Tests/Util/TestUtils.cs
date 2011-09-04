namespace VisualMutator.Tests.Util
{
    #region Usings

    using System.Collections.Generic;
    using System.Diagnostics;

    using NUnit.Framework;

    #endregion

    public static class TestUtils
    {
        [DebuggerStepThrough]
        public static void ShouldEqual<T>(this T obj, T another)
        {
            Assert.AreEqual(another, obj);
        }

        
    }
}