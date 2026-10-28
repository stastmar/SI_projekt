# Údržba a distribuce aktualizací – role 1

## Mobilní aplikace
- Změny spravovat v Gitu, verzovat aplikaci a evidovat změny v changelogu.
- Testovat na podporovaných verzích Android/iOS před vydáním.
- Distribuce: testovací build pro spolužáky; případná produkční distribuce přes oficiální obchody.
- Aktualizace JS/UI lze při vhodné konfiguraci distribuovat OTA (např. Expo Updates); změny nativních modulů, oprávnění či BLE knihoven vyžadují nový binární build.
- U změn API udržovat kompatibilitu klienta a serveru, případně verzovat endpointy.
- Zálohování a retenční pravidla serveru zajišťuje role 3; aplikace má bezpečně zpracovat neodeslaná data.

## Firmware tlakoměru
- Prototyp nepředpokládá vlastní aktualizaci firmwaru zdravotnického zařízení.
- Pokud výrobce poskytne oficiální postup a podporu OTA, aktualizaci navrhnout až pro konkrétní model a dokumentovat riziko přerušení spojení.
- Bez dokumentovaného protokolu zařízení firmware neaktualizovat.

## Provozní postup
1. Nahlášení chyby a reprodukční kroky bez osobních zdravotních údajů.
2. Oprava ve větvi, code review, testy.
3. Vydání testovací verze, ověření synchronizace a kompatibility API.
4. Vydání nové verze a sledování anonymizovaných provozních chyb se souhlasem a v souladu s GDPR.
