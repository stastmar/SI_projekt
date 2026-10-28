# Návrh GUI – obrazovky a navigace

## Spodní navigace
`Domů | Měření | Životospráva | Historie` a ikona `Nastavení`.

### 1. Domů
- Karta „Poslední měření“: tlak (mmHg), puls (tep/min), čas.
- Dlaždice „Zadat tlak“, „Zapsat nápoj“, „Zapsat sůl“.
- Stav synchronizace a poslední úspěšné odeslání.

### 2. Měření
- Formulář: systolický (mmHg), diastolický (mmHg), puls (tep/min), datum/čas.
- Přepínač `Ručně / Ze senzoru (simulace)`.
- Tlačítko „Uložit měření“, zpětná vazba po uložení.

### 3. Životospráva a aktivita
- Typ: `Sůl`, `Energetický nápoj`, `Aktivita`.
- Příslušné pole pro množství/počet nebo kroky/minuty.
- Datum a čas, potvrzení uložení.

### 4. Historie
- Seznam položek se stavem synchronizace, filtr podle typu a období.
- Graf tlaku (systolický/diastolický) podle času; bez diagnostických závěrů.
- Detail záznamu.

### 5. Nastavení / zařízení
- Testovací účet, URL testovacího serveru jen ve vývojové konfiguraci, odhlášení.
- Připojení tlakoměru (v MVP výslovně „Simulace BLE“).

## Textový wireframe
```text
┌──────────────────────────────┐
│ Můj tlak         ⚙ Nastavení │
│                              │
│ Poslední měření              │
│ 135 / 85 mmHg   Puls: 72     │
│ Dnes 09:30                   │
│                              │
│ [ + Zadat nové měření ]      │
│ [ + Zapsat nápoj ]           │
│ [ + Zapsat sůl ]             │
│                              │
│ Synchronizace: ✓ dokončeno   │
│                              │
│ Domů  Měření  Záznamy  Graf  │
└──────────────────────────────┘
```

Barvy: světlé pozadí, tmavý kontrastní text, modré akcenty; barva sama nesmí být jediným indikátorem chyby/stavu.
