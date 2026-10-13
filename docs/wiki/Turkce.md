# Türkçe

Water bir komut satırı hostudur. İlk açılışta dil sorar. Dedicated server klasöründe `WaterSL.exe` varsa sonraki soru oyun portudur.

## Çalıştırma

[.NET 10 SDK](https://dot.net) ve Steam dedicated server kurulumu gerekir.

```bat
Start-Water.bat
```

Ayarlar Windows'ta `%APPDATA%\Water\water.json`. Klasörü taşımak için `WATER_HOME`. Eklenti DLL'leri ayarın yanındaki `plugins` klasörüne konur.

`exit` hostu kapatır. Diğer satırlar oyun konsoluna gider. `tps` de buna dahildir.

## Yama

Yama şunları yapar:

- giriş, çıkış, bağlantı kopması, raunt başlangıcı ve raunt bitişi çağrılarını ekler
- ağ ve konsol yoklama döngülerindeki 15, 20, 25 ve 30 milisaniyelik arka plan `Thread.Sleep` değerlerini 1 milisaniye yapar
- `ServerStatic.set_ServerTickrate` dedicated kare hızını yazdıktan sonra `QualitySettings.vSyncCount` değerini 0, `Time.fixedDeltaTime` değerini `1 / ServerTickrate` yapar
- metot sunucuya dokunmuyorsa, `ServerStatic.IsDedicated` doğru olduğunda istemci görsel `Update` metotlarından hemen döner

İkinci çalıştırma aynı değişikliği yeniden yazmaz. `WaterSL_Data` oyun veri dosyalarına bağlanır. `Managed` gerçek kopyadır, sonra yamalanır. `ScriptingAssemblies.json` içine `WaterLib.dll` ve `WaterSL.dll` yazılır.

## Oturum

Oyun çalışma zamanı yalnızca `WATER_SESSION` 32 karakter olduğunda açılır. Bunu `WaterSL` başlarken WaterServer yazar. `WaterSL.exe` tek başına açılırsa kancalar çalışmaz.
