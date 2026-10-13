# English

Water is a CLI host. On first launch it asks for a language. If `WaterSL.exe` is already in the dedicated server folder, the next question is the game port.

## Run

Install the [.NET 10 SDK](https://dot.net) and the Steam dedicated server.

```bat
Start-Water.bat
```

Config: `%APPDATA%\Water\water.json` on Windows. Set `WATER_HOME` to move that folder. Put plugin DLLs in the `plugins` directory next to the config.

`exit` stops the host. Other lines are sent to the game console, including `tps`.

## Patch

The patcher:

- inserts the join, leave, disconnect, round-start, and round-end calls
- changes background `Thread.Sleep` values of 15, 20, 25, and 30 milliseconds to 1 millisecond on the network and console poll loops
- after `ServerStatic.set_ServerTickrate` sets the dedicated frame rate, sets `QualitySettings.vSyncCount` to 0 and `Time.fixedDeltaTime` to `1 / ServerTickrate`
- returns immediately from client visual `Update` methods when `ServerStatic.IsDedicated` is true, if the method does not reference the server

A second run does not apply the same edit again. `WaterSL_Data` links the game data files. `Managed` is a real copy, then patched. `ScriptingAssemblies.json` lists `WaterLib.dll` and `WaterSL.dll`.

## Session

The game runtime arms only when the environment variable `WATER_SESSION` is 32 characters. WaterServer sets that when it starts `WaterSL`. Launching `WaterSL.exe` by itself does not arm the hooks.
