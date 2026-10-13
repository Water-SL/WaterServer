<p align="center">
  <img src="water-logo.svg" width="72" alt="Water">
</p>

<p align="center">
  <a href="README.md">English</a> ·
  <a href="README.ko.md">한국어</a> ·
  <a href="README.ru.md">Русский</a> ·
  <a href="README.zh-CN.md">中文</a>
</p>

Water, SCP: Secret Laboratory dedicated server'ını çalıştırır. Sunucuyu `WaterSL` olarak kurar, `Assembly-CSharp.dll` dosyasını yamalar ve seni konsola bırakır.

Önce [.NET 10 SDK](https://dot.net) ve Steam'deki dedicated server kurulu olmalı. Oyunun kendisi bu repoda yok.

```bat
Start-Water.bat
```

İlk açılış dili sorar. Klasörde `WaterSL.exe` varsa sonraki soru porttur. Ayar `%APPDATA%\Water\water.json` dosyasına yazılır. `WATER_HOME` bu klasörü taşır.

`exit` Water'ı kapatır. Başka her satır oyuna gider.

Eklenti, `WaterLib`'e referans veren bir `net472` DLL'dir. Yeri `%APPDATA%\Water\plugins`.

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
