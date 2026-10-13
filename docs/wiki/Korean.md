# 한국어

Water는 CLI 호스트입니다. 처음 실행하면 언어를 묻습니다. 데디케이티드 서버 폴더에 `WaterSL.exe`가 있으면 다음 질문은 게임 포트입니다.

## 실행

[.NET 10 SDK](https://dot.net)와 Steam 데디케이티드 서버가 필요합니다.

```bat
Start-Water.bat
```

Windows 설정은 `%APPDATA%\Water\water.json`입니다. `WATER_HOME`으로 폴더를 옮깁니다. 플러그인 DLL은 설정 옆 `plugins`에 둡니다.

`exit`는 호스트를 종료합니다. 다른 줄은 게임 콘솔로 갑니다. `tps`도 그렇습니다.

## 패치

패치는 다음을 합니다.

- 입장, 퇴장, 연결 종료, 라운드 시작, 라운드 종료 호출을 넣습니다
- 네트워크와 콘솔 폴링 루프의 15, 20, 25, 30밀리초 백그라운드 `Thread.Sleep`을 1밀리초로 바꿉니다
- `ServerStatic.set_ServerTickrate`가 데디케이티드 프레임 속도를 쓴 뒤 `QualitySettings.vSyncCount`를 0, `Time.fixedDeltaTime`을 `1 / ServerTickrate`로 둡니다
- 메서드가 서버를 건드리지 않으면 `ServerStatic.IsDedicated`가 참일 때 클라이언트 시각 `Update`에서 바로 반환합니다

두 번째 실행은 같은 수정을 다시 쓰지 않습니다. `WaterSL_Data`는 게임 데이터 파일에 연결됩니다. `Managed`는 실제 복사본이며 그다음 패치됩니다. `ScriptingAssemblies.json`에 `WaterLib.dll`과 `WaterSL.dll`이 들어갑니다.

## 세션

게임 런타임은 `WATER_SESSION`이 32자일 때만 켜집니다. WaterServer가 `WaterSL`을 시작할 때 이 값을 넣습니다. `WaterSL.exe`만 실행하면 후크는 동작하지 않습니다.
