# Návrh implementace a testování

Odevzdává se dokumentace. Budoucí implementace může využít .NET MAUI/C# pro Android a iOS podle [návrhu Emy](../Ema/Implementace_Ema_v2.md); technologie není vydávána za definitivně schválenou.

## Budoucí vrstvy a postup
GUI a navigace, doménové modely, lokální chráněné úložiště, BLE a pohybové adaptéry, autentizace, API klient a synchronizační služba. Nejprve potvrdit kontrakt a oprávnění, následně navrhnout úložiště a rozhraní zařízení, implementovat obrazovky, integraci a automatizované testy. CI je plánována v GitHub Actions; žádné sestavení ani běh aplikace není součástí tohoto výstupu.

## Návrh testovacích scénářů
Níže jsou doplňkové scénáře role 1, nikoli nový závazný katalog. Vazbu na A-01–A-03 a BT-03 je nutné potvrdit podle [společného testování](../Sabi/17_Testovani.docx).

| Scénář | Očekávaný výsledek |
|---|---|
| Ruční měření a BLE | Validní údaje, správný zdroj a identifikátor; při odpojení nabídka ručního vstupu |
| Offline měření | Trvalá fronta, stav čeká, po návratu sítě potvrzení |
| Opakování UUID | 200 bez duplicity; změněný obsah 409 |
| Chybný vstup | Validace a 422 bez nekonečného opakování |
| Životospráva | Všechny tři typy a správné jednotky |
| Aktivita | Denní kroky a minuty; chybějící data nejsou nula |
| Relace a oprávnění | 401 obnova relace; 403 zastavení; žádná data jiného účtu |
| Výpadek API | Fronta zachová záznam, opakování s odstupem |
| Doporučení offline | Cache a potvrzení prvního přečtení po návratu sítě |
| Mazání | Jen prokazatelně neodeslaný lokální záznam |
| Přístupnost | Textový indikátor stavu, čitelnost a ovládání |

Nyní lze kontrolovat konzistenci dokumentace, kontraktů, scénářů a GUI. Funkční, integrační, bezpečnostní a zařízeníové testy jsou plánované, nikoli provedené.
