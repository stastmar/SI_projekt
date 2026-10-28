# Funkční a nefunkční požadavky

## Aktéři
- **Pacient**: používá aplikaci, zadává údaje a kontroluje synchronizaci.
- **Tlakoměr BLE (externí zařízení)**: poskytuje identifikaci, spojení a hodnoty měření; pro MVP simulovaný.
- **WEB API (externí systém)**: autentizuje, přijímá data a potvrzuje uložení.

> Zadání požaduje nejméně dva aktéry a čtyři use casy pro každého aktéra v samostatném diagramu. Zde je pacient a externí tlakoměr; WEB API je třetí aktér. U senzoru jsou modelovány interakce s aplikací, nikoli lidské úkony.

## Funkční požadavky
| ID | Požadavek | Priorita | MVP |
|---|---|---|---|
| M-F01 | Pacient se přihlásí | vysoká | demo přihlášení |
| M-F02 | Pacient uloží krevní tlak, puls a čas | vysoká | ano |
| M-F03 | Pacient vidí historii a graf | vysoká | ano |
| M-F04 | Pacient zaznamená sůl a energetické nápoje | vysoká | ano |
| M-F05 | Pacient eviduje fyzickou aktivitu | střední | ručně |
| M-F06 | Záznamy fungují offline a později se synchronizují | vysoká | ano |
| M-F07 | Aplikace zobrazuje stav odeslání/chyby | vysoká | ano |
| M-F08 | Aplikace vyhledá a spáruje BLE tlakoměr | střední | simulace |
| M-F09 | Aplikace načte hodnoty ze spárovaného tlakoměru | střední | simulace |
| M-F10 | Pacient může zobrazit detaily záznamu | střední | ano |

## Nefunkční požadavky
- HTTPS/TLS; žádná přímá komunikace mobilní aplikace s DB.
- Autentizační tokeny ukládat do zabezpečeného úložiště zařízení, nikoli do prostého AsyncStorage.
- Lokální data o zdraví ukládat bezpečně; pro prototyp používat pouze syntetická data.
- Každé měření má jednoznačné ID a čas s časovým pásmem; opakovaný upload nesmí vytvářet duplicity.
- Srozumitelný stav `čeká / odesílá se / synchronizováno / chyba`.
- Aplikace nesmí sama diagnostikovat hypertenzi ani vydávat léčebná doporučení.
- Zpracování osobních údajů a retenční doby musí tým odsouhlasit s ohledem na GDPR.

## Use case scénáře
**UC-M01 Zaznamenat tlak:** pacient otevře formulář → vyplní systolický, diastolický tlak a puls → aplikace ověří formát a základní konzistenci → uloží lokálně → odešle při dostupné síti → zobrazí stav. Při chybě síťového spojení data zůstávají ve frontě.

**UC-M02 Zapsat životosprávu:** pacient vybere sůl / energetický nápoj → zadá množství či počet podle schválených jednotek → uloží → synchronizuje.

**UC-M03 Připojit tlakoměr:** pacient zahájí hledání → zařízení je nalezeno → pacient potvrdí párování → aplikace naváže spojení; při chybě nabídne opakování. V MVP jde o simulaci.

**UC-M04 Zobrazit historii:** pacient otevře historii → vidí uložená lokální měření a jejich synchronizační stav → vybere detail nebo graf.
