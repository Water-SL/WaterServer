# 中文

Water 是命令行宿主。第一次启动时询问语言。专用服务器目录里已经有 `WaterSL.exe` 时，下一个问题是游戏端口。

## 运行

需要 [.NET 10 SDK](https://dot.net) 和 Steam 上的专用服务器。

```bat
Start-Water.bat
```

Windows 配置文件是 `%APPDATA%\Water\water.json`。用 `WATER_HOME` 可以改这个目录。插件 DLL 放在配置旁边的 `plugins` 目录。

`exit` 关闭宿主。其他行会送到游戏控制台，包括 `tps`。

## 补丁

补丁会：

- 插入加入、离开、断开、回合开始和回合结束调用
- 把网络和控制台轮询循环里 15、20、25、30 毫秒的后台 `Thread.Sleep` 改成 1 毫秒
- 在 `ServerStatic.set_ServerTickrate` 写好专用服务器帧率之后，把 `QualitySettings.vSyncCount` 设为 0，把 `Time.fixedDeltaTime` 设为 `1 / ServerTickrate`
- 当 `ServerStatic.IsDedicated` 为真且方法不接触服务器时，客户端视觉 `Update` 立即返回

第二次运行不会再次写入同一处修改。`WaterSL_Data` 链接游戏数据文件。`Managed` 是真实副本，然后再打补丁。`ScriptingAssemblies.json` 会列入 `WaterLib.dll` 和 `WaterSL.dll`。

## 会话

只有 `WATER_SESSION` 为 32 个字符时，游戏运行时才会武装。WaterServer 在启动 `WaterSL` 时写入这个值。单独启动 `WaterSL.exe` 不会启用钩子。
