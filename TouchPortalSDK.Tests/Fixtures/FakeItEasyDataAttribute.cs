using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using AutoFixture.NUnit3;
using FakeItEasy;

namespace TouchPortalSDK.Tests.Fixtures;

public class InlineFakeItEasyDataAttribute : InlineAutoDataAttribute
{
    public InlineFakeItEasyDataAttribute(params object[] arguments)
        : base(() => new Fixture()
                .Customize(new AutoFakeItEasyCustomization { ConfigureMembers = true })
                .Customize(new FrozenEventHandlerCustomization())
            , arguments)
    {
        //
    }
}

public class FakeItEasyDataAttribute : AutoDataAttribute
{
    public FakeItEasyDataAttribute()
        : base(() => new Fixture()
            .Customize(new AutoFakeItEasyCustomization { ConfigureMembers = true })
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
        var fake = fixture.Freeze<ITouchPortalEventHandler>();
        A.CallTo(() => fake.PluginId).Returns("UnitTest");
    }
}