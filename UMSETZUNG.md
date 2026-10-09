# Neue BibleTalk-Startseite

Die Arbeitskopie wurde aus BibleTalk_codex.zip erstellt. Die Standardausgabe verwendet weiterhin die vorhandenen Cover und Inhalte.

## Umsetzung
- Kompaktere Heftcover, danach Weiterlesen und drei Gesprächskarten.
- Weiterlesen speichert Serien-ID, Serienbereich und Themennummer lokal. Die Karte erscheint erst nach dem ersten geöffneten Thema und wird bei der Rückkehr auf die Startseite aktualisiert.
- Alle Themen entdecken: Suche nach Titel, Bibelstelle oder Serie, dazu ein Kategorienfilter. Bestehende Jüngerschafts- und Hauskreisreihen bleiben hier erreichbar; ihre Notizschlüssel bleiben unverändert.
- Drei neue Gespräche mit Einstieg, Bibeltext, Beobachtungsfragen, Verständnisfragen und Anwendung: Sorgen, Vergebung und Mut.
- Neue Inhalte unter Resources/Raw/conversations. Der vorhandene ContentService verwendet auch hier Remote → Cache → mitgelieferte Dateien. Für Inhaltsupdates diese Dateien später in das vorhandene Inhaltsrepository übernehmen.
- Helles und dunkles Farbschema; die Navigation bleibt bei Home, Feedback und Einstellungen.

## Manuelle Prüfung auf einem Gerät
1. App erstmals öffnen: Standardausgabe und drei Gesprächskarten prüfen; Weiterlesen ist noch ausgeblendet.
2. Gespräch öffnen, durch Fragen gehen und eine Notiz schreiben. Zur Startseite zurückkehren: Weiterlesen zeigt das Gespräch.
3. App neu starten: Weiterlesen und Notiz bleiben erhalten.
4. Alle Themen entdecken öffnen: nach „Vergeben“ und „Josua“ suchen, danach Kategorien wechseln und eine bestehende Reihe öffnen.
5. Hell und Dunkel in Einstellungen prüfen, ebenso lange Titel und vergrößerte Systemschrift.
6. App offline neu starten und die mitgelieferten Gespräche öffnen. Herunterziehen aktualisiert die Startseite.

Die Mockups dienen als Gestaltungsvorlage. Die Umsetzung nutzt native MAUI-Komponenten und die vorhandenen Projektbilder.

## Automatischer Prüfstand
Android-Build (net10.0-android): erfolgreich, 0 Fehler. Bestehende Nullable-Warnungen in AppShell/FeedbackPage und NU1900 (NuGet-Sicherheitsprüfung nicht erreichbar). Neue XAML-Dateien und JSON-Inhalte geprüft. Kein Laufzeit- oder Sichttest auf einem Gerät durchgeführt.
