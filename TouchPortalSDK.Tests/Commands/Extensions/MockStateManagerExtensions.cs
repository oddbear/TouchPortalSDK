using System;
using Moq;
using TouchPortalSDK.Messages.Items;
using TouchPortalSDK.Utils;

namespace TouchPortalSDK.Tests.Commands.Extensions
{
    public static class MockStateManagerExtensions
    {
        public static void LogMessage_Verify(this Mock<IStateManager> stateManager, Func<Times> times)
            => stateManager.Verify(mock => mock.LogMessage(It.IsAny<Identifier>(), It.IsAny<string>()), times);
    }
}