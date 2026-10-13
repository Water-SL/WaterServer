<p align="center">
  <img src="water-logo.svg" width="72" alt="Water">
</p>

<p align="center">
  <a href="README.md">English</a> ·
  <a href="README.tr.md">Türkçe</a> ·
  <a href="README.ko.md">한국어</a> ·
  <a href="README.zh-CN.md">中文</a>
</p>

Water запускает выделенный сервер SCP: Secret Laboratory. Ставит его как `WaterSL`, патчит `Assembly-CSharp.dll` и оставляет консоль.

Сначала нужны [.NET 10 SDK](https://dot.net) и выделенный сервер из Steam. Самой игры в этом репозитории нет.

```bat
Start-Water.bat
```

Первый запуск спрашивает язык. Если в папке уже есть `WaterSL.exe`, дальше спрашивает порт. Настройки пишутся в `%APPDATA%\Water\water.json`. `WATER_HOME` переносит эту папку.

`exit` закрывает Water. Любая другая строка уходит в игру.

Плагин — это DLL `net472` со ссылкой на `WaterLib`. Папка: `%APPDATA%\Water\plugins`.

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
