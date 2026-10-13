# Plugins

A plugin is a class library that targets `net472` and references `WaterLib`. Put the DLL in the `plugins` folder beside `water.json`.

```csharp
using Water;

public sealed class Announce : WaterPlugin
{
    public override string Name => "Announce";
    public override string Author => "you";

    protected override void OnEnabled()
    {
        PlayerEvents.Joined += OnJoin;
        PlayerEvents.Left += OnLeft;
        RoundEvents.Started += OnRound;
        RegisterCommand("announce", "log a line", line => Log(line));
    }

    protected override void OnDisabled()
    {
        PlayerEvents.Joined -= OnJoin;
    }

    private void OnJoin(PlayerJoinInfo player) => Log(player.Name + " joined");
    private void OnLeft(PlayerJoinInfo player) => Log(player.Name + " left");
    private void OnRound(RoundInfo round) => Log(round.Phase);
}
```

`PlayerEvents` and `RoundEvents` run inside the game process, after WaterServer has started `WaterSL`. The host process loads the same folder so a plugin command can register on the CLI. Keep the plugin free of per-tick reflection over every player. That path was removed because it sat on the game thread.

`WaterPlayers.TryGet` reads the players announced through the join event. It is not a live transform snapshot.
