# Návrh GUI – role 1

Základní vizuální podklad: [30 návrhů obrazovek](HypertenzeApp_GUI_30_nahledu_tym.docx). Následující specifikace je aktualizace návrhu, nikoli tvrzení o implementaci.

## Navigace
Přehled | Historie | Životospráva | Od lékaře. Nastavení je dostupné z přehledu.

| Obrazovka | Obsah |
|---|---|
| Přihlášení | Účet, heslo, MFA; bez hostovského režimu |
| Přehled | Poslední tlak/puls/čas, stav synchronizace, nové měření, průvodce a připomínky |
| Měření | Ruční vstup nebo BLE, validace, potvrzení lokálního uložení, dostupný příznak rytmu |
| Historie | Filtr období, graf, detail, stav synchronizace; pouze neodeslaný záznam lze smazat |
| Životospráva | Káva, energetické nápoje, slané jídlo, pohybová aktivita |
| Od lékaře | Individuální limity, doporučení, čas a stav přečtení; offline cache |
| Nastavení | Párování tlakoměru, oprávnění senzorů, připomínky, odhlášení |

Barvy hodnot vycházejí z individuálních limitů, nikoli pevných hranic; stav je vyjádřen také textem. Aplikace nevytváří diagnózy ani vlastní doporučení. Interní alerty se nezobrazují. Klinický postup při urgentní hodnotě a podrobnosti průvodce budou doplněny po konzultaci.

## GUI-31 – Pohybová aktivita
[Návrh obrazovky a alternativních stavů](GUI-31-aktivita.md). Číselné hodnoty jsou ilustrační.
