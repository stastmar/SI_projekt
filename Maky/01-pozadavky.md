# Požadavky a případy užití – role 1

Jde o návrh pacientské mobilní aplikace, nikoli o dodávku funkčního softwaru. Jediným závazným katalogem je [společná tabulka požadavků](../spolecny/Pozadavky_spolecne.xlsx); zde jsou popsány scénáře mobilní části, nikoli další sada požadavků.

## Aktéři a scénáře
Pacient se přihlašuje s MFA, pořizuje měření, prohlíží historii, eviduje životosprávu, sleduje aktivitu, čte doporučení a nastavuje připomínky. BLE tlakoměr se účastní párování, spojení, přenosu měření a zjištění stavu. WEB API zajišťuje autentizaci, synchronizaci, historii, limity a doporučení. Telefonní senzory jsou součástí zařízení pacienta.

- Ruční měření: pacient zadá tlak, puls a čas; aplikace validuje a uloží záznam do lokální fronty. Po dostupnosti sítě jej odešle. Při výpadku zůstane čekající.
- BLE: pacient vybere kompatibilní tlakoměr, potvrdí párování, aplikace přijme měření a identifikátor zařízení; při chybě nabídne opakování nebo ruční vstup.
- Životospráva: pacient zadá kávu, energetický nápoj (počet kusů) nebo slané jídlo (málo/středně/hodně), čas a případnou poznámku.
- Aktivita: aplikace po udělení oprávnění načte denní kroky a aktivní minuty z telefonu; nedostupnost dat nezamění za nulu.
- Historie: pacient vybere období a detail či graf; vidí stav synchronizace a individuální limity. Smazat může pouze neodeslaný lokální záznam; serverové opravy provádí lékař s historií změn.
- Doporučení: pacient čte text lékaře i offline; první zobrazení se potvrdí API po dostupnosti sítě.

Aplikace sama nediagnostikuje ani nevytváří léčebná doporučení. Interní alerty patří lékaři; pacient vidí obecnou výzvu ke kontaktu. Klinická pravidla čekající na konzultaci nejsou uzavřena.

Viz [diagramy](diagramy/README.md), [architektura](02-architektura.md) a [společná bezpečnostní kapitola](../spolecny/06_Bezpecnost_GDPR.docx).

## Trasování společných požadavků
FP-A01 přihlášení; A02 ruční měření; A03–A04 párování a přenos; A05–A06 offline a stav; A07 rychlý záznam; A08–A09 historie, graf a detail; A10 kroky a aktivní minuty; A11 doporučení a upozornění na nové doporučení; A12 průvodce (klid, manžeta, dvě měření); A13 připomínky ráno a večer; A14 kritické upozornění podle změnového znění; A15 příznak rytmu; A16 volitelné motivační prvky (série dnů bez energetického nápoje, týdenní souhrn). NFP-10: sůl a energetický nápoj nejvýše tři klepnutí. Káva je doplnění podle ER v2, nikoli změna společného katalogu.
