using System.Reflection;

namespace TouchPortalSDK.Tests.Client.Extensions
{
    public static class ClientPrivateFieldsExtensions
    {
        public static void SetPrivate(this ITouchPortalClient client, string fieldName, object value)
            => typeof(TouchPortalClient)
                .GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(client, value);
    }
}
