<p align="center">
  <img src="water-logo.svg" width="72" alt="Water">
</p>

<p align="center">
  <a href="README.md">English</a> ·
  <a href="README.tr.md">Türkçe</a> ·
  <a href="README.ru.md">Русский</a> ·
  <a href="README.zh-CN.md">中文</a>
</p>

Water는 SCP: Secret Laboratory 데디케이티드 서버를 띄웁니다. 서버를 `WaterSL`로 두고 `Assembly-CSharp.dll`을 패치한 뒤 콘솔을 남깁니다.

먼저 [.NET 10 SDK](https://dot.net)와 Steam 데디케이티드 서버가 설치되어 있어야 합니다. 게임 파일은 이 저장소에 없습니다.

```bat
Start-Water.bat
```

처음 실행하면 언어를 묻습니다. 폴더에 `WaterSL.exe`가 있으면 다음은 포트입니다. 설정은 `%APPDATA%\Water\water.json`에 저장됩니다. `WATER_HOME`으로 그 폴더를 옮깁니다.

`exit`는 Water를 종료합니다. 그 외의 줄은 게임으로 갑니다.

플러그인은 `WaterLib`를 참조하는 `net472` DLL입니다. 위치는 `%APPDATA%\Water\plugins`입니다.

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
