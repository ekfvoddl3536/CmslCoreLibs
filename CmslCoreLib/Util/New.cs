// using System;
// using System.Linq.Expressions;
// using System.Runtime.Serialization;
// 
// namespace CmslCore.Util
// {
//     public static class New<T> where T : class, new()
//     {
//         public static readonly Func<T> Instance = Creator();
// 
//         private static Func<T> Creator()
//         {
//             Type t = typeof(T);
//             return t == typeof(string)
//                 ? Expression.Lambda<Func<T>>(Expression.Constant(string.Empty)).Compile()
//                 : t.GetConstructor(Type.EmptyTypes) != null
//                 ? Expression.Lambda<Func<T>>(Expression.New(t)).Compile()
//                 : () => FormatterServices.GetUninitializedObject(t) as T;
//         }
//     }
// }
// 