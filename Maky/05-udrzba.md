# Údržba a distribuce aktualizací – role 1

Kapitola popisuje budoucí provoz pacientské aplikace. Cílem údržby je zachovat dostupnost, bezpečnost, kompatibilitu a správnost přenosu zdravotních dat bez ztráty čekajících záznamů.

## Druhy údržby
**Korektivní:** opravy chyb formulářů, pádů, BLE spojení, synchronizace a nesprávného zobrazení. Hlášení obsahuje verzi aplikace, OS, typ zařízení a reprodukční postup bez zdravotních údajů. Oprava projde revizí a regresními testy.
**Adaptivní:** reakce na nové verze Android/iOS, změny oprávnění, BLE protokolů, pravidel obchodů a API. Před vydáním se ověří kompatibilita podporovaných zařízení a starších klientů.
**Preventivní:** pravidelná kontrola závislostí, bezpečnostních aktualizací, expirace certifikátů, kapacity lokální fronty a obnovy relace. Sleduje se funkčnost synchronizace a migrací úložiště.
**Perfektivní:** zlepšování čitelnosti, přístupnosti, rychlosti, spotřeby baterie a použitelnosti podle zpětné vazby pacientů a lékařů. Změny klinických pravidel vyžadují týmové schválení.

## Monitoring a odpovědnosti
Mobilní část sleduje pády, četnost chyb spojení, stáří čekajících záznamů a úspěšnost synchronizace. Provozní metriky nesmí obsahovat zdravotní hodnoty, tokeny ani identifikátory pacientů bez schváleného právního a bezpečnostního rámce. Pacient vidí stav a poslední úspěšnou synchronizaci. Role 3 zajišťuje monitoring API/DB, zálohy a obnovu serveru; role 4 integrátor a NIS. Incident se předává odpovědné roli, zaznamená se příčina, náprava a ověření.

## Vydávání aktualizací
Zdrojové změny se verzují v Gitu, kontrolují pull requestem a evidují v changelogu. Budoucí CI sestaví a otestuje aplikaci; před distribucí se ověří přihlášení, BLE, offline fronta, migrace lokálního úložiště a kompatibilita API. Produkční distribuce se plánuje přes Google Play a App Store. Nativní změny vyžadují nové podepsané vydání. Postupné nasazení umožní sledovat chyby a zastavit další distribuci.

## Servisní okna a návrat verze
Serverové zásahy se koordinují s rolemi 3 a 4, plánují mimo vytížené období a oznamují uživatelům. Při nedostupnosti API aplikace zachová lokální frontu a zobrazí stav. Před migrací serveru se ověří záloha a plán obnovy; změny API musí být zpětně kompatibilní nebo verzované. Po zásahu se ověří autentizace, přenos a absence duplicit. Při závažné chybě se zastaví rollout, obnoví kompatibilní serverová verze a vydá opravená mobilní verze; návrat nesmí poškodit lokální data. Obchody nemusí umožnit okamžitý downgrade, proto je důležitá kompatibilita a oprava směrem vpřed.

## Firmware tlakoměru
Aktualizace pouze oficiálním postupem výrobce pro konkrétní podporovaný model. Před zahájením ověřit napájení, spojení a podmínky výrobce; při chybě postupovat podle jeho návodu. Vlastní firmware ani neověřený OTA mechanismus se nenavrhuje. Po aktualizaci ověřit párování a přenos měření.

Bezpečnost a retence se řídí společnou politikou; konkrétní retenční doba NFP-16 zůstává otevřená. V této fázi jde o plán údržby, nikoli doklad skutečného provozu.
