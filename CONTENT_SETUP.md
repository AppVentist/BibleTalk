# BibleTalk – Inhalte über GitHub pflegen

## Dateistruktur

Die Inhalte liegen unter `Resources/Raw/quarterlies/` – **eine Datei pro Heft**
plus ein Index, der bestimmt, welche Hefte in welcher Reihenfolge erscheinen:

```
Resources/Raw/quarterlies/
  index.json       <- Liste der Hefte (Reihenfolge = Anzeige-Reihenfolge)
  2026_q1.json     <- Philipper & Kolosser (JAN-MÄR)
  2026_q2.json     <- Im Glauben wachsen (APR-JUN)
  2026_q3.json     <- Die Korintherbriefe (JUL-SEP)
```

## Neues Heft anlegen

1. Neue Datei `quarterlies/2026_q4.json` anlegen (bestehende Datei als Vorlage kopieren).
2. In `index.json` einen Eintrag ergänzen – ganz oben, wenn das Heft zuerst
   angezeigt werden soll:

```json
{
  "quarterlies": [
    { "id": "2026_q4", "file": "quarterlies/2026_q4.json" },
    { "id": "2026_q3", "file": "quarterlies/2026_q3.json" }
  ]
}
```

Hefte ohne Themen (`"topics": []`) werden in der App automatisch ausgeblendet –
man kann eine Datei also schon vorbereiten, bevor sie sichtbar wird.

## Einmalige Einrichtung (GitHub)

1. Auf github.com ein **öffentliches** Repository anlegen.
2. Den Ordner `quarterlies/` mit allen JSON-Dateien hochladen.
3. Die raw-Basis-URL in `Services/QuarterlyService.cs` bei der Konstante
   `RemoteBaseUrl` eintragen, z. B.:
   `https://raw.githubusercontent.com/<benutzername>/<repo>/main/Resources/Raw/`
   (Die App hängt daran `quarterlies/index.json` bzw. die Heft-Dateien an.)

## Wöchentliche Pflege

- Die betreffende Heft-Datei direkt auf GitHub bearbeiten (Stift-Symbol) und committen.
- Die App lädt beim nächsten Start automatisch den neuen Stand.
- Ohne Internet zeigt die App den zuletzt geladenen Stand (Cache), beim allerersten
  Start ohne Internet die mitgelieferten Fallback-Dateien.
- Ist eine einzelne Heft-Datei fehlerhaft, werden die übrigen Hefte trotzdem angezeigt.

## JSON-Struktur einer Heft-Datei

```json
{
  "title": "Die Korintherbriefe",
  "period": "JUL | AUG | SEP 2026",
  "coverImage": "cover3.jpg",
  "topics": [
    {
      "number": 1,
      "title": "Thema 1",
      "memoryVerse": "…",
      "description": "…",
      "startingQuestion": "…",
      "questions": []
    }
  ]
}
```

Hinweise:
- `coverImage` verweist auf Bilder, die in der App unter `Resources/Images` liegen.
  Neue Cover erfordern daher ein App-Update. Alternativ kann hier eine komplette
  Bild-URL stehen (z. B. ebenfalls aus dem GitHub-Repo) – MAUI lädt `Image.Source`
  auch von URLs.
- Nach dem Bearbeiten die Datei z. B. auf jsonlint.com prüfen – ein Tippfehler
  (fehlendes Komma) macht die Datei ungültig; die App fällt dann für dieses Heft
  auf den Cache zurück.
- `//`-Kommentare sind in den JSON-Dateien erlaubt (z. B. um Themen vorzubereiten).
