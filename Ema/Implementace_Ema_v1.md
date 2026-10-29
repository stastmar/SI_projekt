# Implementace systému – návrh (Ema, role 3)

**Verze:** v1 – pracovní návrh k odsouhlasení týmem. **Projekt:** telemedicínský systém pro pacienty s hypertenzí. Tato kapitola popisuje plánovanou implementaci, nikoliv již existující software.

## 1. Výběr technologií

| Část | Navržená technologie | Zdůvodnění |
|---|---|---|
| WEB API | ASP.NET Core (C#), REST, JSON | Typované rozhraní, validace vstupů, podpora autentizace a automatizovaných testů. |
| Databáze | PostgreSQL | Relační vazby, transakce, integritní omezení, auditní data a podpora časových údajů. |
| Databázová vrstva | Entity Framework Core | Mapování entit, migrace schématu a transakční práce s databází. |
| Dokumentace API | OpenAPI (Swagger) | Jednotný kontrakt pro mobilní aplikaci, webový portál a backend. |
| Autentizace | OAuth 2.0 / OpenID Connect prostřednictvím identity provideru; MFA (TOTP) | Centralizovaná správa identit, tokenů a druhého faktoru; přesné řešení nutno schválit. |
| Webový portál | Technologie určí Sabi | Portál volá zabezpečené REST API, nepřistupuje přímo do DB. |
| Mobilní aplikace | React Native + Expo + TypeScript (návrh Maky) | Sdílený návrh pro mobilní platformy, lokální fronta při offline režimu. |
| Integrátor NIS | Samostatná serverová služba, HL7 FHIR přes HTTPS | Čte potřebná data přímo z DB s omezeným účtem a předává je NIS; konkrétní FHIR zdroje odsouhlasit s rolí 4. |
| Provoz | Linux, kontejnery Docker | Opakovatelné nasazení a oddělení služeb. |

**Poznámka:** Volba ASP.NET Core/PostgreSQL je doporučení pro roli 3, nikoli potvrzená týmová dohoda. Komponentový diagram je technologicky nezávislý; konkrétní technologie patří do této kapitoly.

## 2. Organizace WEB API

Backend se rozdělí na vrstvy:

1. **API/Controllers:** přijímají HTTP požadavky a vracejí odpovědi a stavové kódy.
2. **Application/Services:** kontrolují oprávnění, provádějí validaci, výpočty souhrnů a pravidla alertů.
3. **Domain:** obsahuje datové entity a obchodní pravidla (měření, pacient, lékař, alert).
4. **Infrastructure:** komunikuje s PostgreSQL, identitním systémem a auditním úložištěm.

Mobilní aplikace a portál používají pouze HTTPS API. Integrátor NIS podle zadání přistupuje k databázi přímo, avšak přes samostatný účet s minimálními potřebnými oprávněními. API integrátoru data do NIS neposílá.

## 3. Příjem měření a práce s databází

Při příjmu měření API ověří identitu a oprávnění pacienta, zvaliduje JSON a zkontroluje unikátní `measurementId`. Nové měření uloží v databázové transakci, podle odsouhlasených limitů případně vytvoří alert a označí záznam pro synchronizaci s NIS. Opakované odeslání stejného ID se stejnými daty nevytvoří duplicitu; stejné ID s odlišným obsahem vyvolá konflikt.

Lékařské opravy měření se zapisují společně s historií změn a informací o nutnosti opětovné synchronizace. Pacient může v portálu své záznamy prohlížet, nikoliv upravovat. Záznamy o soli a nápojích zadává pouze mobilní aplikací.

## 4. Bezpečnost a ochrana osobních údajů

- Veškerá komunikace klientů s API probíhá přes HTTPS/TLS.
- Ověření uživatele podporuje druhý faktor; tokeny mají omezenou platnost. Hesla se ukládají pouze ve formě bezpečných hashů, například Argon2id.
- Autorizace probíhá na každém endpointu: pacient vidí své údaje, lékař pouze přiřazené pacienty a administrátor jen funkce odpovídající jeho roli.
- Audit zachycuje významné přístupy, změny a exporty; historie změn zdravotních záznamů uchovává původní a nové hodnoty a identitu oprávněného pracovníka.
- Zálohy databáze se šifrují; tajné údaje se neukládají do Git repozitáře. Nastaví se retenční pravidla a omezení přístupu podle GDPR.
- Pro vývoj a demonstraci se používají syntetická, nikoliv reálná zdravotní data.

## 5. Softwarový proces

Tým použije iterativní postup inspirovaný Scrumem, přizpůsobený semestrálnímu projektu. Práce se rozdělí na malé úkoly podle komponent a společných rozhraní. V každé iteraci se nejprve dohodne datový kontrakt, poté vznikne návrh, diagramy a kontrola návazností. Změny se projdou týmovou revizí a zapracují se do společné dokumentace.

Doporučený postup: (1) sjednotit požadavky a aktéry, (2) schválit API kontrakt a ER model, (3) připravit diagramy, (4) specifikovat testovací scénáře, (5) zkontrolovat bezpečnost a integraci s NIS, (6) finálně sjednotit dokumentaci a prezentaci. Pro evidenci úkolů lze použít GitHub Issues a jednoduchou tabuli Kanban.

## 6. Verzovací systém a build server

**Verzovací systém:** Git, sdílený repozitář GitHub. Každá změna vzniká na samostatné větvi (např. `feature/api-navrh`), popisuje se v commitu a do hlavní větve se slučuje po kontrole ostatním členem týmu. Do repozitáře patří diagramy `.drawio`, dokumentace `.md`, případně zdrojový kód; nepatří do něj hesla, tokeny, reálné údaje pacientů ani lokální konfigurační tajemství.

**Build server / CI:** GitHub Actions. Při každém pull requestu by automaticky spouštěl sestavení backendu, statickou kontrolu, jednotkové testy a integrační testy proti izolované testovací databázi. Pro mobilní aplikaci lze samostatně spouštět TypeScript kontrolu a testy. Nasazení do testovacího prostředí by proběhlo až po úspěšných kontrolách; produkční nasazení by vyžadovalo schválení a bezpečné předání tajných údajů.

Ukázková posloupnost CI: `checkout → restore dependencies → build → test → report výsledků`. Konkrétní workflow bude možné vytvořit teprve po založení zdrojového kódu.

## 7. Vazba na testování a nasazení

Pro WEB API se plánují jednotkové testy validace a autorizace, integrační testy databázových transakcí a testy endpointů. Klíčové scénáře: odeslání měření, opakované odeslání bez duplicity, chybný vstup, nepovolený přístup k cizímu pacientovi, MFA, oprava s historií změn a označení pro synchronizaci s NIS. Nasazení počítá s odděleným testovacím a produkčním prostředím, zálohováním, monitoringem a řízenými databázovými migracemi.

## 8. Otevřené body k odsouhlasení

1. Potvrdit se Sabi, Maky a rolí 4 technologický stack, zejména backend a identity provider.
2. Schválit pravidla alertů, individuálních limitů a oprávnění administrátora.
3. Doplnit do ER návrhu v2 identifikátory pro offline synchronizaci aktivity/životosprávy a chybějící atributy alertů.
4. S rolí 4 dohodnout mapování na FHIR, způsob sledování synchronizace a předávání oprav.
5. Rozhodnout, zda se skutečně bude vytvářet prototyp, nebo pouze návrhová dokumentace dle zadání.

---
*Text je určen jako pracovní kapitola pro společnou dokumentaci. Nenahrazuje týmové schválení architektury.*
