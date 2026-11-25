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

## Vazba na testy Sabi – ověřeno v 17_Testovani.docx
| ID | Kritérium návrhu |
|---|---|
| A-01 | 135/85, puls 72, uložit a zobrazit v historii |
| A-02 | Offline fronta, po připojení odeslat a označit synchronizováno |
| A-03 | Energetický nápoj do 3 klepnutí a zobrazení v historii |
| S-01/S-02 | Stejné UUID a obsah: 200 bez duplicity; jiný obsah: 409 |
| BT-01 | Cizí pacient: 403 bez dat, audit na serveru |
| BT-03 | 150 požadavků/min, od 101. 429; offline 185/115, výzva do 2 s, po připojení alert v portálu. Testovací limit musí být nastaven lékařem tak, aby hodnota byla kritická; nejde o univerzální klinickou hranici. |
| BT-05 | API validace dle BP-06, TLS 1.0 odmítnuto |
Pouze syntetická data (NFP-14). Jde o kontrolu návrhu, nikoli provedené funkční testy. Test BT-03 je třeba sladit se změnovým zněním BP-13 společně se Sabi.
