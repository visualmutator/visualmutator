namespace VisualMutator.Tests.Util
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using NUnit.Framework;

    using VisualMutator.Infrastructure.Factories;

    #endregion

    public static class TestUtils
    {
        [DebuggerStepThrough]
        public static void ShouldEqual<T>(this T obj, T another)
        {
            Assert.AreEqual(another, obj);
        }

        
    }

    public static class Factory
    {
   
        public static FuncFactory<DateTime> DateTime(DateTime d)
        {
            return new FuncFactory<DateTime>(()=>d);
        }

        public static FuncFactory<T> New<T>(Func<T> func)
        {
            return new FuncFactory<T>(func);
        }

    }




}