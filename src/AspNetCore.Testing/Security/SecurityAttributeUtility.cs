using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace AspNetCore.Testing.Security
{
    //https://davidpine.net/blog/asp-net-core-security-unit-testing/
    public static class SecurityAttributeUtility
    {
        public static void AssertAuthorizeAttributeRolesOnMethod<TController>(
            Expression<Func<TController, object>> expression,
            string expectedRoles
            )
            where TController : ControllerBase
        {
            var methodInfo = GetMethodInfo(expression.Body);
            var methodName = methodInfo.Name;
            var methodParameters = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();

            var attribute =
                GetAttributeFromMethod<AuthorizeAttribute>(
                     typeof(TController), methodName, methodParameters);

            Assert.NotNull(attribute);

            Func<TController, object> a = x => x.Content("");

            Assert.Equal(expectedRoles, attribute.Roles);
        }

        public static void AssertAuthorizeAttributePolicyOnMethod<TController>(
            Expression<Func<TController, object>> expression,
            string expectedPolicy
            )
          where TController : ControllerBase
        {
            var methodInfo = GetMethodInfo(expression.Body);
            var methodName = methodInfo.Name;
            var methodParameters = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();

            var attribute =
                GetAttributeFromMethod<AuthorizeAttribute>(
                    typeof(TController), methodName, methodParameters);

            Assert.NotNull(attribute);

            Assert.Equal(expectedPolicy, attribute.Policy);
        }

        public static void AssertAllowAnonymousAttributeOnMethod<TController>(
             Expression<Func<TController, object>> expression
        )
         where TController : ControllerBase
        {
            var methodInfo = GetMethodInfo(expression.Body);
            var methodName = methodInfo.Name;
            var methodParameters = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();

            var attribute =
                GetAttributeFromMethod<AllowAnonymousAttribute>(
                    typeof(TController), methodName, methodParameters);

            Assert.NotNull(attribute);
        }

        public static void AssertAuthorizeAttributeRolesOnClass<TController>(
            string expectedRoles
            )
           where TController : ControllerBase
        {
            var attribute =
                GetAttributeFromClass<AuthorizeAttribute>(
                    typeof(TController));

            Assert.NotNull(attribute);

            Assert.Equal(expectedRoles, attribute.Roles);
        }

        private static MethodInfo GetMethodInfo(Expression expression)
        {
            return ((MethodCallExpression)expression).Method;
        }

        private static T GetAttributeFromMethod<T>(
            Type containingDataType,
            string methodName,
            params Type[] methodArgs) where T : Attribute
        {
            var method = containingDataType.GetMethod(
                methodName, methodArgs);

            Assert.NotNull(method);

            var attribute = method.GetCustomAttributes(
                typeof(T), true).FirstOrDefault();

            return attribute as T;
        }

        private static T GetAttributeFromClass<T>(
            Type containingDataType)
            where T : Attribute
        {

            var attribute = containingDataType.GetCustomAttributes(
                typeof(T), true).FirstOrDefault();

            return attribute as T;
        }
    }
}
