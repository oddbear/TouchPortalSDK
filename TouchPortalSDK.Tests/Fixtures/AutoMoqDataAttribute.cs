using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using Moq;

namespace TouchPortalSDK.Tests.Fixtures
{
    public class InlineAutoMoqDataAttribute : InlineAutoDataAttribute
    {
        public InlineAutoMoqDataAttribute(params object[] arguments)
            : base(() => new Fixture()
                .Customize(new AutoMoqCustomization { ConfigureMembers = true })
                .Customize(new FrozenEventHandlerCustomization())
            , arguments)
        {
            //
        }
    }

    public class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute()
            : base(() => new Fixture()
                .Customize(new AutoMoqCustomization { ConfigureMembers = true })
                .Customize(new FrozenEventHandlerCustomization())
            )
        {
            //
        }
    }

    public class FrozenEventHandlerCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Freeze<Mock<ITouchPortalEventHandler>>()
                .SetupGet(m => m.PluginId).Returns("UnitTest");
        }
    }
}
