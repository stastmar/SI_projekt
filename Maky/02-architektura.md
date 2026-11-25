# Architektura a datové toky – role 1

Mobilní klient bude obsahovat GUI, validaci, adaptér BLE, adaptér pohybových senzorů, zabezpečené lokální úložiště, autentizaci a synchronizační službu. Komunikuje pouze s WEB API přes HTTPS/JSON; nikdy přímo s DB ani NIS. Integrátor čte DB a předává do NIS tlak a puls. Viz [API v2](../Ema/WEB_API_Ema_v2.md), [ER v2](../Ema/ER_databaze_Ema_v2.md) a [společný DFD](../spolecny/DFD_tok_dat.drawio).

## Zařízení
Aktuálně podporovaný Android/iOS, BLE pro kompatibilní tlakoměr, zabezpečené úložiště a internet pro synchronizaci. Pohybová aktivita využije dostupné systémové rozhraní kroků a aktivních minut; konkrétní platformní API a model tlakoměru se ověří před implementací. Při chybě BLE je možný ruční vstup. Chybějící oprávnění či senzor neznamenají nulovou aktivitu.

## Data
| Lokální entita | Obsah |
|---|---|
| Měření | measurementId (UUID), measuredAt (čas s pásmem), systolic, diastolic, pulse, source, deviceId, irregularRhythm, lokální syncStatus |
| Aktivita | klientské UUID, datum, kroky, aktivní minuty, zdroj telefon, syncStatus |
| Životospráva | klientské UUID, typ (káva / energetický nápoj / slané jídlo), množství, jednotka, čas, poznámka, syncStatus |
| Zařízení | identifikátor z párování, název, stav spojení |
| Doporučení | ID, text lékaře, čas vytvoření, stav přečtení a čekající potvrzení |

Názvy JSON polí aktivity a životosprávy a pravidla aktualizace denního souhrnu musí potvrdit role 3; ER v2 obsahuje klientská UUID. U ručního měření bez zařízení je FK v ER nullable; přesnou reprezentaci deviceId a neznámého příznaku rytmu v API je nutné potvrdit.

## Synchronizace
Po validaci se záznam trvale uloží lokálně. POST /api/v1/pacienti/{id}/mereni s tokenem použije stejné UUID a obsah při opakování. 201 nebo 200 potvrzuje uložení; timeout/5xx znamená opakování s odstupem; 401 obnovu relace, 403 zastavení a kontrolu oprávnění, 422 opravu vstupu, 409 konflikt k vyřešení bez automatického přepsání. Při nejistém doručení nelze záznam považovat za bezpečně neodeslaný. Pacient nesmí měnit potvrzené záznamy.

Aktivita a životospráva se odesílají na /pacienti/{id}/aktivity a /zivotosprava. Historie, graf, souhrn, limity a doporučení se načítají přes API v2. Doporučení se uchovají offline; při prvním zobrazení se odešle PATCH /doporuceni/{id}, případně z fronty. Lokální cache se oddělí podle účtu.

## DFD a bezpečnost
F1: tlak, puls, identifikátor a dostupný příznak rytmu z BLE; ruční vstup životosprávy od pacienta. F2: měření, aktivita, životospráva a potvrzení přečtení přes HTTPS s tokenem. F3: potvrzení uložení, historie, limity a doporučení. Upřesnění společného DFD předat týmu.
Bezpečnost, GDPR a audit se řídí [společnou kapitolou](../spolecny/06_Bezpecnost_GDPR.docx); tokeny a zdravotní cache musí být chráněné, logy bez citlivých údajů. Retence NFP-16 zůstává otevřená.
