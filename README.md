<p align="center">
  <img src="water-logo.svg" width="72" alt="Water">
</p>

<p align="center">
  <a href="README.tr.md">Türkçe</a> ·
  <a href="README.ko.md">한국어</a> ·
  <a href="README.ru.md">Русский</a> ·
  <a href="README.zh-CN.md">中文</a>
</p>

Water runs an SCP: Secret Laboratory dedicated server. It copies the server to `WaterSL`, patches `Assembly-CSharp.dll`, and leaves you at a console.

[.NET 10 SDK](https://dot.net) and the Steam dedicated server have to be installed first. The game itself is not in this repo.

```bat
Start-Water.bat
```

```bash
dotnet run --project WaterServer/WaterServer.csproj -c Release -f net10.0
```

The first launch asks for a language. If `WaterSL.exe` is already in the server folder, the next question is the port. Settings go to `%APPDATA%\Water\water.json`. `WATER_HOME` moves that folder.

`exit` closes Water. Anything else is typed into the game.

A plugin is a `net472` DLL in `%APPDATA%\Water\plugins`, referenced against `WaterLib`.

```csharp
public sealed class Announce : WaterPlugin
{
    public override string Name => "Announce";

    protected override void OnEnabled()
    {
        PlayerEvents.Joined += player => Log(player.Name + " joined");
        PlayerEvents.Left += player => Log(player.Name + " left");
        RoundEvents.Started += round => Log(round.Phase);
    }
}
```

[MIT](LICENSE)
