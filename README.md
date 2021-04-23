# Touch Portal .Net SDK
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/TouchPortalSDK)](https://www.nuget.org/packages/TouchPortalSDK)

Touch Portal SDK for making plugins in .Net

Build from documentation at [Touch Portal API](https://www.touch-portal.com/api/).
 
### Getting started:

The simplest way of getting started, is to implement the `ITouchPortalEventHandler` and use `TouchPortalFactory` to create a client.
And then Connect to Touch Portal before sending or receiving events.

```csharp
public class SamplePlugin : ITouchPortalEventHandler
{
    public string PluginId => "Plugin.Id"; //Replace "Plugin.Id" with your unique id.

    private readonly ITouchPortalClient _client;

    public SamplePlugin()
    {
        _client = TouchPortalFactory.CreateClient(this);
    }

    public void Run()
    {
        _client.Connect();
    }
    ...
```
