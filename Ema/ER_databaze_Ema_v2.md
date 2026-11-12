# Databáze telemedicínského systému – hypertenze

Pracovní návrh role 3 (Ema), verze 2. Navazuje na návrh Sabi a JSON Maky.

## Tabulky

### Role
`id PK`, `nazev UNIQUE`, `popis`, `opravneni_json`.

### Uzivatel
`id PK`, `role_id FK`, `email UNIQUE`, `hash_hesla`, `mfa_zapnuto`, `mfa_secret_sifrovany`, `aktivni`, `posledni_prihlaseni`.

### Pacient
`id PK`, `uzivatel_id FK`, `jmeno`, `prijmeni`, `datum_narozeni`, `nis_identifikator`.

### Lekar
`id PK`, `uzivatel_id FK`, `jmeno`, `prijmeni`, `specializace`, `icp`, `zastupce_lekar_id FK NULL → Lekar.id`, `ordinacni_doba`.

### PacientLekar
`id PK`, `pacient_id FK`, `lekar_id FK`, `platne_od`, `platne_do`, `aktivni`.

### Zarizeni
`id PK`, `pacient_id FK`, `typ`, `vyrobce`, `model`, `identifikator`.

### MereniTlaku
`id PK / measurementId`, `pacient_id FK`, `zarizeni_id FK NULL`, `measured_at`, `systolic`, `diastolic`, `pulse`, `source`, `nepravidelny_rytmus`, `opraveno`, `verze`.

### Aktivita
`id PK`, `pacient_id FK`, `datum`, `kroky`, `aktivni_minuty`, `zdroj`, `klientske_uuid UUID UNIQUE`.

### ZaznamZivotospravy
`id PK`, `pacient_id FK`, `typ` (energetický nápoj / káva / slané jídlo), `mnozstvi` (počet kusů; u soli 1–3 = málo / středně / hodně), `jednotka`, `cas_zaznamu`, `poznamka`, `klientske_uuid UUID UNIQUE`.

### LimitAlertu
`id PK`, `pacient_id FK`, `cil_sys`, `cil_dia`, `horni_sys`, `horni_dia`, `dolni_sys`, `dolni_dia`, `max_energeticke_napoje_tyden`, `platne_od`, `nastavil_lekar_id FK`, `urgentni_sys`, `urgentni_dia`, `dny_bez_mereni`.

### Alert
`id PK`, `pacient_id FK`, `mereni_id FK NULL`, `typ`, `zavaznost`, `stav`, `vytvoren_at`, `vyresil_lekar_id FK NULL`, `vyreseno_at NULL`, `poznamka NULL`, `eskalovano_at NULL`, `prideleno_lekar_id FK NULL`. Povolené stavy: **Vytvořen, Odeslán lékaři, Zobrazen, Řeší se, Vyřešen, Eskalován, Zrušen**.

### Doporuceni
`id PK`, `pacient_id FK`, `lekar_id FK`, `alert_id FK NULL`, `kategorie`, `text`, `vytvoreno_at`, `precteno_at NULL`.

### HistorieZmen
`id PK`, `tabulka`, `zaznam_id`, `sloupec`, `stara_hodnota`, `nova_hodnota`, `zmenil_uzivatel_id FK`, `zmeneno_at`, `duvod`.

### AuditLog
`id PK`, `uzivatel_id FK`, `pacient_id FK NULL`, `akce`, `endpoint`, `cas`, `ip_adresa`.

### NisSynchronizace
`id PK`, `mereni_id FK`, `stav`, `pokusy`, `posledni_pokus_at`, `synchronizovano_at`, `fhir_resource_id`, `chyba`. Stav: **čeká / odesláno / chyba**.

### NastaveniSystemu
`klic PK`, `hodnota`, `popis NULL`. Např. `eskalace_alertu_minuty = 120` (měnitelné administrátorem).

## Klíčové návrhové zásady

- `MereniTlaku.id` je UUID převzaté z `measurementId` mobilní aplikace a má unikátní omezení; opakované doručení stejného měření je idempotentní.
- `measured_at` se ukládá s časovým pásmem (UTC).
- Hesla se ukládají pouze jako bezpečné hashe; tajemství MFA musí být šifrované, nikoliv prostý text.
- Přístup k údajům pacienta se ověřuje podle role a vazby `PacientLekar`.
- Opravy měření probíhají transakčně se zápisem do `HistorieZmen` a označením pro novou synchronizaci.
- Nové i lékařem opravené měření vytvoří či aktualizuje synchronizační úlohu se stavem `čeká` ve stejné databázové transakci.
- `Pacient.nis_identifikator` je navržen jako identifikátor pro `FHIR Patient.identifier`.
- Integrátor používá samostatný DB účet: SELECT na `Pacient`, `Zarizeni`, `MereniTlaku` (případně `Aktivita` po schválení) a SELECT/UPDATE na `NisSynchronizace`; nepovoluje se plošný zápis do zdravotních dat.
- Urgentní alert se v ordinační době eskaluje po 2 hodinách bez převzetí, mimo ordinační dobu ihned. Čas nastavuje administrátor; ordinační dobu má každý lékař v `Lekar.ordinacni_doba`, převzetí = lékař klikne „Převzít“ (stav Řeší se).
- `NisSynchronizace` je pracovní návrh pro synchronizaci jednotlivých měření; integrátor má přímý, omezený přístup do databáze.
- `AuditLog` obsahuje přístupy k citlivým údajům a změny, s omezeným přístupem a retenční politikou.

## Rozhodnutí týmu (12. 11. 2026)

Odsouhlaseno všemi členkami týmu:

1. **Alerty:** typy *nad horním limitem*, *urgentní* (jedno měření ≥ urgentní limit, výchozí 180/110), *pod dolním limitem* a *3 dny bez měření*. Převzetí = lékař klikne „Převzít“ → stav *Řeší se*. Ordinační dobu má každý lékař vlastní (`Lekar.ordinacni_doba`).
2. **Mazání a opravy:** zdravotní záznamy se nemažou; opravit je smí jen lékař s povinným důvodem a zápisem do `HistorieZmen`. Pacient smí smazat jen dosud neodeslaný záznam v aplikaci.
3. **Audit:** zapisuje se přihlášení, každé čtení zdravotních dat (detail, graf, export – otevření grafu = 1 záznam), každá změna a každý odmítnutý přístup (403).
4. **Doporučení:** aplikace je stahuje přes `GET /pacienti/{id}/doporuceni`, uchovává pro offline zobrazení a při prvním zobrazení pacientem nastaví `precteno_at` (`PATCH /doporuceni/{id}`).
5. **Aktivita:** denní souhrn kroků a aktivních minut ze senzorů telefonu (`zdroj = telefon`). **Životospráva:** `typ` (energetický nápoj / káva / slané jídlo), `mnozstvi` (počet kusů; u soli málo / středně / hodně).
6. **Tlakoměr:** identifikátor z párování se uloží do `Zarizeni.identifikator`; aplikace ho posílá jako `deviceId`, API podle něj dohledá `zarizeni_id`.
7. **Integrátor:** mapování dle Role4-S/25_Integrator_FHIR.docx (Patient, Observation, LOINC); do NIS se posílá jen měření tlaku a pulsu; 5 pokusů (1, 5, 15 min), při výpadku > 1 h e-mail administrátorovi. Životospráva do NIS (FP-I09) – ověřit na konzultaci.
8. **Tabulky a endpointy:** odsouhlaseno 16 tabulek ER v2 a endpointy API v2. Vazba pacient–lékař se nemaže, ale ukončí (`platne_do`).
9. **Doba uchování dat (NFP-16):** zatím otevřené – zeptat se na konzultaci 12. 11.; data v NIS se řídí pravidly nemocnice.
## Změny v2 a co ještě odsouhlasit

Doplněno podle vzkazů Sabi z 29.–30. 10.: zástupce lékaře, systémová nastavení, urgentní limity, stavový cyklus alertů, informace o vyřešení, UUID offline záznamů a stav synchronizace. Otevřené body týmu jsou rozhodnuté (viz Rozhodnutí týmu). ER diagram v2 s datovými typy (PostgreSQL, všechna id UUID, časy TIMESTAMPTZ) je v ER_databaze_Ema_v2.drawio. Zbývá: rozsah FHIR přenosu (FP-I09) a retence dat (NFP-16) – ověřit na konzultaci. Původní soubor v1 zůstává zachován.
