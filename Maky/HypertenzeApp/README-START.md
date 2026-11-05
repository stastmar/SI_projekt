# HypertenzeApp – Android prototyp

Upraveno z dodaného .NET MAUI projektu. Cílí pouze na `net10.0-android`, takže při restore nevyžaduje iOS.

## Co funguje v návrhu
- Jedna obrazovka pro ruční zadání krevního tlaku a pulsu.
- Kontrola základních rozsahů hodnot.
- Lokální uložení do MAUI Preferences a zobrazení posledních 20 měření.
- Nevyžaduje API ani databázi. **Není určeno pro klinické použití.**

## Instalace / spuštění
1. Zálohuj původní `E:\Projekty\HypertenzeApp` a nahraď ji složkou z ZIPu.
2. Otevři `HypertenzeApp.csproj` ve Visual Studio 2026.
3. Ověř Android SDK a JDK. Android SDK zatím na E: nebylo nalezeno; samotný MAUI workload nestačí.
4. V terminálu projektu spusť `dotnet restore -p:TargetFramework=net10.0-android` a následně sestavení až po instalaci SDK.
5. Spusť na Android telefonu s USB debugging.

## Důležité
- `Preferences` jsou vhodné jen pro malý demonstrační prototyp. Pro více dat a bezpečné zdravotní záznamy přejděte na lokální databázi, šifrování a autentizované API.
- Synchronizace s webovým portálem ani BLE tlakoměr **zatím nejsou implementovány**.
- Na C: bylo jen 3,22 GB volného místa: před instalací Android SDK uvolni místo a nastav jeho cestu na E:.
