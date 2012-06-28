using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Telerik.Windows.Data;

namespace GreenField.Gadgets.Helpers
{
    public class IEnumerableT<T> : IEnumerable<T>
    {
        private static readonly MethodInfo GetValueMethod =
            (from m in typeof(PropertyInfo).GetMethods()
             where m.Name == "GetValue" && !m.IsAbstract
             select m).First();

        private static readonly ConstantExpression NullObjectArrayExpression =
            System.Linq.Expressions.Expression.Constant(null, typeof(object[]));

        public IEnumerable Transpose<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            return TransposeCore(source);
        }

        private static Delegate CreateSelectorFunc<T>(IEnumerable<T> source)
        {
            T[] list = source.ToArray();
            DynamicProperty[] dynamicProperties =
                list.Select(i => new DynamicProperty(i.ToString(), typeof(object))).ToArray();

            Type transposedType = ClassFactory.Instance.GetDynamicClass(dynamicProperties);

            ParameterExpression propParam = System.Linq.Expressions.Expression.Parameter(typeof(PropertyInfo), "prop");

            var bindings = new MemberBinding[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                MethodCallExpression getter =
                    System.Linq.Expressions.Expression.Call(
                        propParam,
                        GetValueMethod,
                        System.Linq.Expressions.Expression.Constant(list[i]),
                        NullObjectArrayExpression
                        );

                bindings[i] = System.Linq.Expressions.Expression.Bind(transposedType.GetProperty(dynamicProperties[i].Name), getter);
            }

            LambdaExpression selector =
                System.Linq.Expressions.Expression.Lambda(
                    System.Linq.Expressions.Expression.MemberInit(
                        System.Linq.Expressions.Expression.New(transposedType),
                        bindings),
                    propParam);

            return selector.Compile();
        }

        private static IEnumerable TransposeCore<T>(IEnumerable<T> source)
        {
            List<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            Delegate selector = CreateSelectorFunc(source);

            foreach (PropertyInfo property in properties)
            {
                yield return selector.DynamicInvoke(property);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
