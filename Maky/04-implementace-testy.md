# Implementační plán a testy

## Doporučená technologie (návrh, ne týmové rozhodnutí)
- React Native + Expo + TypeScript.
- Expo Router nebo React Navigation; vybrat jednu variantu.
- Lokální úložiště: SQLite (pro zdravotní data ověřit možnosti šifrování; prototyp pouze syntetická data).
- HTTP klient: `fetch` + centrální API modul.
- BLE až po volbě kompatibilního tlakoměru a případně vývojového buildu (Expo Go nemusí pro potřebný nativní BLE modul stačit).

## Struktura budoucího kódu
```text
mobile/
  app/                 # obrazovky a navigace
  src/components/      # znovupoužitelné UI
  src/domain/          # typy Measurement, LifestyleEntry, ActivityEntry
  src/storage/         # lokální úložiště a fronta
  src/api/             # klient a synchronizace
  src/devices/         # rozhraní senzoru + mock implementace
  src/auth/            # přihlášení a bezpečné uložení tokenu
  src/config/          # konfigurace prostředí (bez tajných údajů)
```

## Postup implementace
1. Projekt Expo, navigace a statické obrazovky.
2. Formuláře + lokální validace + ukládání syntetických záznamů.
3. Historie a graf nad lokálními daty.
4. Fronta synchronizace a mock API odpovědi.
5. Dohoda o API kontraktu a připojení na testovací backend.
6. Simulátor tlakoměru, dokumentované rozhraní pro budoucí BLE.
7. Testy, demo scénář, screenshoty pro dokumentaci.

## Akceptační testy
| ID | Scénář | Očekávaný výsledek |
|---|---|---|
| T01 | Zadám 135/85, puls 72 | Záznam se objeví v historii |
| T02 | Vypnu internet a zadám tlak | Záznam se uloží jako čekající |
| T03 | Obnovím internet | Záznam se odešle a změní stav na synchronizováno |
| T04 | Opakuji odeslání stejného ID | Na serveru nevznikne duplicita |
| T05 | Zadám nečíselnou hodnotu | Formulář zobrazí chybu |
| T06 | Zadám energetický nápoj | Položka se zobrazí v historii životosprávy |
| T07 | Simuluji připojení senzoru | Zobrazí se stavy hledání/připojeno |
| T08 | Server vrátí 401 | Aplikace vyžádá nové přihlášení |
| T09 | Server vrátí 500 | Data zůstanou ve frontě a lze opakovat |
| T10 | Přihlásím se jiným účtem | Data předchozího účtu se nezobrazí |

## Co prokazuje „funkční prototyp“
Na telefonu lze zadat měření, zobrazit historii, zaznamenat životosprávu, vypnout síť a následně odeslat záznam do testovacího API. BLE lze pro demonstraci simulovat; omezení musí být přiznáno.
