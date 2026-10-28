# Architektura a datové toky

## Hranice odpovědností
**Mobilní aplikace:** obrazovky, lokální validace vstupu, lokální fronta, historie, komunikace BLE, HTTPS klient. **WEB API:** autentizace, autorizační pravidla, serverová validace, ukládání do DB, generování alertů. **Portál:** zobrazení záznamů pacientovi/lékaři podle rolí. **Integrátor:** přímé čtení DB a synchronizace s NIS/FHIR (podle zadání).

## Tok měření
1. Pacient zadá měření nebo ho převezme ze senzoru.
2. Aplikace vytvoří `measurementId` (UUID), čas `measuredAt`, hodnoty a `source`.
3. Data se uloží do lokální fronty se stavem `pending`.
4. Při připojení aplikace odešle požadavek s přístupovým tokenem přes HTTPS.
5. API ověří uživatele, validuje záznam, idempotentně jej uloží a vrátí potvrzení.
6. Aplikace označí záznam jako `synced`; při chybě ho ponechá ve frontě.
7. Portál čte záznam přes API; integrátor načítá schválené záznamy z DB pro NIS.

## Lokální datové entity (návrh, ne serverové tabulky)
| Entita | Minimální pole |
|---|---|
| Measurement | measurementId, measuredAt, systolic, diastolic, pulse, source, syncStatus |
| LifestyleEntry | entryId, recordedAt, kind, amount, unit, syncStatus |
| ActivityEntry | activityId, recordedAt, activityType, durationMinutes, steps, syncStatus |
| PairedDevice | localDeviceId, displayName, connectionStatus, lastSeenAt |

`LifestyleEntry.kind`: `salt` nebo `energy_drink`. Přesné jednotky příjmu soli a energetických nápojů musí tým schválit (např. gramy odhadované soli vs. počet nápojů). Nepředstírat přesné nutriční hodnoty bez dat.

## Offline a řešení chyb
- Lokální zápis je první krok; bez internetu se záznam neztratí.
- Po obnovení spojení synchronizovat položky `pending`/`failed` s exponenciálním odstupem opakování.
- Stejné ID při opakovaném požadavku; API musí podporovat idempotenci.
- Chyba 401 → obnovit relaci nebo vyžádat přihlášení; 400/422 → ukázat chybu a neopakovat automaticky; 5xx/timeout → opakovat později.
- Mazání či úpravy již synchronizovaných záznamů vyžadují zvláštní pravidla (otevřený bod).

## Bezpečnost
HTTPS, autorizace na serveru, minimální lokální uchování, ochrana tokenů, oddělení demo/produkčního prostředí, žádná reálná pacientská data v repozitáři. Konkrétní šifrování lokální DB a retenční pravidla je nutné navrhnout před produkčním použitím.
