<p align="center">
  <img src="water-logo.svg" width="72" alt="Water">
</p>

<p align="center">
  <a href="README.md">English</a> ·
  <a href="README.tr.md">Türkçe</a> ·
  <a href="README.ko.md">한국어</a> ·
  <a href="README.ru.md">Русский</a>
</p>

Water 用来启动 SCP: Secret Laboratory 专用服务器。它把服务器放成 `WaterSL`，修补 `Assembly-CSharp.dll`，然后留下控制台。

先装好 [.NET 10 SDK](https://dot.net) 和 Steam 上的专用服务器。游戏本体不在这个仓库里。

```bat
Start-Water.bat
```

第一次启动会问语言。目录里已经有 `WaterSL.exe` 时，下一问是端口。配置写在 `%APPDATA%\Water\water.json`。`WATER_HOME` 用来换这个目录。

`exit` 关闭 Water。其他行都发给游戏。

插件是引用 `WaterLib` 的 `net472` DLL，放在 `%APPDATA%\Water\plugins`。

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
