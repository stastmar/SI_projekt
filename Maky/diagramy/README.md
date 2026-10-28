# Diagramy

Soubory `.mmd` lze otevřít v editoru Mermaid a importovat do draw.io (Insert → Advanced → Mermaid). Pro rychlý náhled na GitHubu:

## 01-use-case

```mermaid
flowchart LR
  P["Pacient"]:::actor
  S["BLE tlakoměr (externí)"]:::actor
  A["WEB API"]:::actor
  subgraph MOB["Pacientská mobilní aplikace"]
    U1([Přihlásit se])
    U2([Zadat měření ručně])
    U3([Zapsat sůl / nápoj])
    U4([Zobrazit historii])
    U5([Zaznamenat aktivitu])
    U6([Spárovat senzor])
    U7([Navázat spojení])
    U8([Převzít měření])
    U9([Zobrazit stav senzoru])
    U10([Synchronizovat záznamy])
  end
  P --- U1 & U2 & U3 & U4 & U5 & U6
  S --- U6 & U7 & U8 & U9
  A --- U1 & U4 & U10 & U3
  U2 --> U10
  U8 --> U10
  classDef actor fill:#eef4ff,stroke:#4f78ae,color:#18263c
```

## 02-stav-ble

```mermaid
stateDiagram-v2
  [*] --> Nepripojeno
  Nepripojeno --> Vyhledavani: Pacient zahájí hledání
  Vyhledavani --> Nalezeno: Zařízení nalezeno
  Vyhledavani --> Chyba: Timeout / oprávnění
  Nalezeno --> Parovani: Potvrzení pacienta
  Parovani --> Pripojeno: Párování úspěšné
  Parovani --> Chyba: Párování selhalo
  Pripojeno --> PrijemDat: Přichází měření
  PrijemDat --> Pripojeno: Hodnota převzata
  Pripojeno --> Nepripojeno: Odpojení
  Chyba --> Vyhledavani: Zkusit znovu
  Chyba --> Nepripojeno: Zrušit
```

## 03-stav-mereni-sync

```mermaid
stateDiagram-v2
  [*] --> Rozpracovano
  Rozpracovano --> Validace: Uložit
  Validace --> Rozpracovano: Neplatné hodnoty
  Validace --> CekaOffline: Lokálně uloženo
  CekaOffline --> Odesilani: Síť dostupná a přihlášení platné
  Odesilani --> Synchronizovano: Server potvrdil uložení
  Odesilani --> CekaOffline: Timeout / 5xx / offline
  Odesilani --> ChybaValidace: 400 / 422 / konflikt
  Odesilani --> VyžadujePrihlaseni: 401
  VyžadujePrihlaseni --> CekaOffline: Přihlášení obnoveno
  ChybaValidace --> Rozpracovano: Pacient opraví záznam
  Synchronizovano --> [*]
```

## 04-komponenty-mobil

```mermaid
flowchart TB
  subgraph Phone["Telefon pacienta"]
    UI["Obrazovky a navigace"]
    Forms["Formuláře / validace"]
    History["Historie / graf"]
    Store[("Lokální úložiště + fronta")]
    Sync["Synchronizační služba"]
    BLE["Adaptér senzoru / mock BLE"]
    Auth["Přihlášení + bezpečné tokeny"]
    UI --> Forms & History & Auth
    Forms --> Store
    BLE --> Forms
    History --> Store
    Store <--> Sync
    Auth --> Sync
  end
  Sensor["Tlakoměr BLE"] -. "BLE (pozdější fáze)" .-> BLE
  Sync <-- "HTTPS / JSON" --> API["WEB API – role 3"]
  API --> DB[("Databáze – role 3")]
  API --> Portal["Webový portál – role 2"]
  DB --> Integrator["Integrátor NIS – role 4"]
  Integrator --> NIS["NIS / HL7 FHIR"]
```
