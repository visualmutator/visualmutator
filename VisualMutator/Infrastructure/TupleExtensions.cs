namespace VisualMutator.Controllers
{
    using System;
    using System.Threading.Tasks;

    public static class TupleExtensions
    {
        public static async Task<Tuple<T1, T2, T3, T4>> WhenAll<T1, T2, T3, T4>
            (this Tuple<Task<T1>, Task<T2>, Task<T3>, Task<T4>> tasks)
        {
            await Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4);
            return Tuple.Create(tasks.Item1.Result, tasks.Item2.Result, tasks.Item3.Result, tasks.Item4.Result);
        }
        public static async Task<Tuple<T1, T2, T3>> WhenAll<T1, T2, T3>
            (this Tuple<Task<T1>, Task<T2>, Task<T3>> tasks)
        {
            await Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3);
            return Tuple.Create(tasks.Item1.Result, tasks.Item2.Result, tasks.Item3.Result);
        }
        public static async Task<Tuple<T1, T2>> WhenAll<T1, T2>
            (this Tuple<Task<T1>, Task<T2>> tasks)
        {
            await Task.WhenAll(tasks.Item1, tasks.Item2);
            return Tuple.Create(tasks.Item1.Result, tasks.Item2.Result);
        }
    }
}