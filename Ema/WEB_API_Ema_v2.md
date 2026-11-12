# WEB API – návrh rozhraní (Ema, role 3)

**Verze:** v2, pracovní návrh k odsouhlasení. **Systém:** telemedicína pro hypertenzi. **Základ URL:** `/api/v1`. Klienti (mobilní aplikace a portál) komunikují pouze přes HTTPS; integrátor NIS čte databázi přímo, nikoliv přes API.

## Aktéři a oprávnění
- **Mobilní aplikace** (technický aktér): přihlášení, odeslání tlaku, odeslání aktivity, odeslání životosprávy, čtení historie a doporučení.
- **Webový portál** (technický aktér): přihlášení/MFA, čtení zdravotních údajů, opravy měření, správa alertů, doporučení a uživatelů.
- **Pacient, lékař, administrátor** jsou uživatelské role; přístup k endpointům se kontroluje podle role a vazby `PacientLekar`.

## Endpointy
| Metoda a cesta | Účel | Oprávnění / poznámka |
|---|---|---|
| POST `/auth/login` | Ověření hesla, zahájení přihlášení | Veřejný, omezení pokusů |
| POST `/auth/mfa/verify` | Ověření druhého faktoru a vydání tokenů | Přihlášený v rozpracované MFA relaci |
| POST `/auth/refresh` | Obnova přístupového tokenu | Platný obnovovací token |
| POST `/pacienti/{id}/mereni` | Příjem měření tlaku | Pacient ve vlastní mobilní aplikaci |
| GET `/pacienti/{id}/mereni?od=&do=` | Historie měření | Pacient / přiřazený lékař |
| PATCH `/mereni/{id}` | Oprava měření s důvodem | Přiřazený lékař; audit + historie změn |
| POST `/pacienti/{id}/aktivity` | Příjem fyzické aktivity | Pacient ve vlastní aplikaci |
| POST `/pacienti/{id}/zivotosprava` | Příjem soli a nápojů | Pacient ve vlastní aplikaci |
| GET `/pacienti/{id}/graf?od=&do=` | Data pro grafy | Pacient / přiřazený lékař |
| GET `/pacienti/{id}/souhrn?tyden=` | Týdenní souhrn vypočtený API | Pacient / přiřazený lékař |
| GET `/pacienti/{id}/export?format=pdf|csv` | Export dat | Pacient / přiřazený lékař; audit |
| GET `/pacienti/{id}` | Detail pacienta | Pacient / přiřazený lékař |
| GET `/lekari/{id}/pacienti?razeni=riziko` | Seznam přiřazených pacientů | Daný lékař |
| GET, PUT `/pacienti/{id}/limity` | Zobrazení a nastavení limitů | Čtení pacient/lékař, změna lékař |
| GET `/pacienti/{id}/alerty` | Seznam interních alertů | Pouze přiřazený lékař; pacient vidí jen obecnou výzvu ke kontaktu s lékařem (návrh) |
| PATCH `/alerty/{id}` | Změna stavu alertu | Přiřazený lékař |
| GET `/pacienti/{id}/doporuceni` | Seznam doporučení | Pacient / přiřazený lékař |
| POST `/pacienti/{id}/doporuceni` | Nové doporučení | Přiřazený lékař |
| PATCH `/doporuceni/{id}` | Označení jako přečtené | Pacient / přiřazený lékař dle pravidel |
| GET, POST, PATCH `/uzivatele` | Správa uživatelů | Administrátor; PATCH `/uzivatele/{id}` |
| PUT `/uzivatele/{id}/role` | Změna role | Administrátor |
| POST `/pacienti/{id}/lekari` | Přiřazení lékaře | Administrátor |
| PATCH `/pacienti/{id}/lekari/{vazbaId}` | Ukončení vazby pacient–lékař (vyplní `platne_do`, vazba se nemaže) | Administrátor |
| GET `/audit?uzivatel=&pacient=&od=&do=` | Auditní log | Administrátor s omezeným přístupem |

## Příklad příjmu měření
`POST /api/v1/pacienti/{id}/mereni`

```json
{
  "measurementId": "c38ef61d-11a1-4f72-a57e-589e9eb39fd4",
  "measuredAt": "2026-10-28T09:30:00Z",
  "systolic": 135,
  "diastolic": 85,
  "pulse": 72,
  "source": "manual",
  "deviceId": "DEVICE-001",
  "irregularRhythm": false
}
```

**Odpověď při prvním přijetí:** `201 Created`, např. `{"id":"c38ef61d-11a1-4f72-a57e-589e9eb39fd4","status":"ulozeno"}`. **Opakované zaslání totožného `measurementId` a obsahu:** `200 OK` s existujícím ID (bez duplicity). **Stejné ID, jiný obsah:** `409 Conflict`. Chybné hodnoty: `422 Unprocessable Content`; nepřihlášený klient: `401`; nedostatečné oprávnění: `403`. Server ověřuje konzistenci a rozsahy hodnot, nepředstírá lékařskou diagnózu.

## Zpracování a konzistence
1. Ověřit token a právo zapisovat pro daného pacienta.
2. Zvalidovat formát, čas, UUID a měřené hodnoty; `measuredAt` ukládat jako čas s časovou zónou.
3. V transakci zkontrolovat unikátní `measurementId`, uložit měření a případně vytvořit alert podle schválených individuálních limitů.
4. Vytvořit nebo aktualizovat záznam `NisSynchronizace` pro pozdější přenos integrátorem; API samo FHIR neposílá.
5. Vrátit stav aplikaci. Při výpadku sítě klient opakuje požadavek se stejným ID.
6. Oprava lékařem: změna měření, zápis `HistorieZmen` a označení pro opakovanou synchronizaci proběhnou v jedné transakci.

## Urgentní alerty a eskalace (doplnění v2)

Interní stavy alertu: `Vytvořen`, `Odeslán lékaři`, `Zobrazen`, `Řeší se`, `Vyřešen`, `Eskalován`, `Zrušen`. Při urgentním alertu, který lékař nepřevezme, systém v ordinační době po 2 hodinách předá alert jeho zástupci (`Lekar.zastupce_lekar_id`); mimo ordinační dobu ihned. Interval se spravuje v `NastaveniSystemu` administrátorem. API musí evidovat čas vyřešení, poznámku a audit změny stavu. Převzetí = lékař klikne „Převzít“ (stav `Řeší se`); ordinační dobu má každý lékař v `Lekar.ordinacni_doba` (odsouhlaseno 12. 11.).

Pacientovi se podle návrhu Sabi **nezobrazují interní alerty**, pouze výzva ke kontaktu s lékařem. Odsouhlaseno týmem 12. 11. – sedí s portálem, aplikací i maticí oprávnění.

## Integrátor NIS (doplnění v2)

Při vytvoření **i opravě** měření API transakčně založí nebo aktualizuje `NisSynchronizace` se stavem `čeká`. Integrátor samostatně odesílá FHIR zprávy a stav mění na `odesláno` nebo `chyba`. `Pacient.nis_identifikator` slouží jako navržený identifikátor pro FHIR `Patient.identifier`. Integrátor má oddělený DB účet s minimálními právy; do NIS se posílá jen měření tlaku a pulsu; přenos životosprávy (FP-I09) se ověří na konzultaci.

## Bezpečnost
HTTPS/TLS, krátce platné přístupové tokeny, bezpečně uložené obnovovací tokeny, MFA (např. TOTP), bezpečné hashování hesel (Argon2id), autorizace u každého požadavku, audit přístupu k citlivým datům a exportů. Logy nesmí obsahovat hesla, MFA tajemství ani celé zdravotní záznamy. Retence, přístupová práva a šifrování záloh se upřesní v týmové bezpečnostní politice.

## Návaznost na ER v2 a otevřené otázky
- `MereniTlaku.id` odpovídá `measurementId` z JSON Maky. Pro aktivitu a životosprávu **doporučujeme přidat klientské UUID**, aby i jejich offline opakované odeslání bylo idempotentní; ER v2 tato pole obsahuje.
- Sabi v Excelu navrhuje navíc `urgentni_sys`, `urgentni_dia`, `dny_bez_mereni` v `LimitAlertu` a `vyreseno`, `poznamka` v `Alert`; **ER v2 je doplňuje jako návrh k odsouhlasení**.
- **Rozhodnuto týmem 12. 11.** (podrobně v ER_databaze_Ema_v2.md, Rozhodnutí týmu): pravidla alertů a převzetí, mazání a opravy, rozsah auditu (vč. grafů), doporučení v aplikaci (offline + `precteno_at`), identifikace tlakoměru přes `deviceId`, FHIR mapování a opakování synchronizace dle role 4, ukončení vazby pacient–lékař místo mazání.
- Otevřené: přenos životosprávy do NIS (FP-I09) a doba uchování dat (NFP-16) – ověřit na konzultaci.
- Endpointy jsou návrh kontraktu, nikoliv implementované funkční služby.
