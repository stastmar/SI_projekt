# Databáze telemedicínského systému – hypertenze

Pracovní návrh role 3 (Ema), verze 1. Navazuje na návrh Sabi a JSON Maky.

## Tabulky

### Role
`id PK`, `nazev UNIQUE`, `popis`, `opravneni_json`.

### Uzivatel
`id PK`, `role_id FK`, `email UNIQUE`, `hash_hesla`, `mfa_zapnuto`, `mfa_secret_sifrovany`, `aktivni`, `posledni_prihlaseni`.

### Pacient
`id PK`, `uzivatel_id FK`, `jmeno`, `prijmeni`, `datum_narozeni`, `nis_identifikator`.

### Lekar
`id PK`, `uzivatel_id FK`, `jmeno`, `prijmeni`, `specializace`, `icp`.

### PacientLekar
`id PK`, `pacient_id FK`, `lekar_id FK`, `platne_od`, `platne_do`, `aktivni`.

### Zarizeni
`id PK`, `pacient_id FK`, `typ`, `vyrobce`, `model`, `identifikator`.

### MereniTlaku
`id PK / measurementId`, `pacient_id FK`, `zarizeni_id FK NULL`, `measured_at`, `systolic`, `diastolic`, `pulse`, `source`, `nepravidelny_rytmus`, `opraveno`, `verze`.

### Aktivita
`id PK`, `pacient_id FK`, `datum`, `kroky`, `aktivni_minuty`, `zdroj`.

### ZaznamZivotospravy
`id PK`, `pacient_id FK`, `typ`, `mnozstvi`, `jednotka`, `cas_zaznamu`, `poznamka`.

### LimitAlertu
`id PK`, `pacient_id FK`, `cil_sys`, `cil_dia`, `horni_sys`, `horni_dia`, `dolni_sys`, `dolni_dia`, `max_energeticke_napoje_tyden`, `platne_od`, `nastavil_lekar_id FK`.

### Alert
`id PK`, `pacient_id FK`, `mereni_id FK NULL`, `typ`, `zavaznost`, `stav`, `vytvoren_at`, `vyresil_lekar_id FK NULL`.

### Doporuceni
`id PK`, `pacient_id FK`, `lekar_id FK`, `alert_id FK NULL`, `kategorie`, `text`, `vytvoreno_at`, `precteno_at NULL`.

### HistorieZmen
`id PK`, `tabulka`, `zaznam_id`, `sloupec`, `stara_hodnota`, `nova_hodnota`, `zmenil_uzivatel_id FK`, `zmeneno_at`, `duvod`.

### AuditLog
`id PK`, `uzivatel_id FK`, `pacient_id FK NULL`, `akce`, `endpoint`, `cas`, `ip_adresa`.

### NisSynchronizace
`id PK`, `mereni_id FK`, `stav`, `pokusy`, `posledni_pokus_at`, `synchronizovano_at`, `fhir_resource_id`, `chyba`.

## Klíčové návrhové zásady

- `MereniTlaku.id` je UUID převzaté z `measurementId` mobilní aplikace a má unikátní omezení; opakované doručení stejného měření je idempotentní.
- `measured_at` se ukládá s časovým pásmem (UTC).
- Hesla se ukládají pouze jako bezpečné hashe; tajemství MFA musí být šifrované, nikoliv prostý text.
- Přístup k údajům pacienta se ověřuje podle role a vazby `PacientLekar`.
- Opravy měření probíhají transakčně se zápisem do `HistorieZmen` a označením pro novou synchronizaci.
- `NisSynchronizace` je pracovní návrh pro synchronizaci jednotlivých měření; integrátor má přímý, omezený přístup do databáze.
- `AuditLog` obsahuje přístupy k citlivým údajům a změny, s omezeným přístupem a retenční politikou.

## Otevřené otázky pro tým

1. Sabi: potvrdit pravidla alertů, mazání/oprav a rozsah auditu.
2. Maky: potvrdit strukturu aktivity a záznamů životosprávy a způsob identifikace zařízení.
3. Integrátor: potvrdit FHIR mapování, rozsah synchronizovaných záznamů a strategii opakovaných pokusů.
4. Společně: odsouhlasit konečnou sadu tabulek a endpointů.