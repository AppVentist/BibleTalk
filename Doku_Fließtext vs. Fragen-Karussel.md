Kurz eingeordnet: die Datei ist eine Serien-Datei (nicht ein einzelnes Thema).

title/period/coverImage > beschreiben die Serie, topics[] alles darunter. Sie wird über discipleship/index.json gefunden und dreistufig geladen (GitHub raw → lokaler Cache → gebündeltes Resources/Raw/).

Die Ebenen und wo sie landen

JSON > Model > Anzeige:
title, period, coverImage > Model/Series.cs > Kachel auf MainPage + Kopf der SeriesPage
topics[].number/title/memoryVerse/description/startingQuestion	> Model/Topic.cs > TopicPage.xaml
topics[].sections[]	> Model/LessonSection.cs > LessonPage.xaml (Fließtext + Notiz)
topics[].questions[] > Model/Question.cs > QuestionsPage.xaml (Fragen-Karussell)
passages	→ BibleReferenceParser → Luther-1912-JSON	BibleTextCard

Die Weiche sitzt im TopicItemViewModel: sobald sections gefüllt ist, öffnet die LessonPage – die questions desselben Themas werden dann nie angezeigt. In Thema 1 hast du beides; die questions dort sind aktuell toter Ballast (und e01_vertiefung dupliziert wörtlich die applicationQuestion). Entweder eins von beiden raus, oder du weißt, dass es Reserve ist.

Der empfindlichste Punkt: der Notizschlüssel
note:{Kind}:{Slug}:{topic.number}:{section.id}, wobei Slug aus dem Serientitel abgeleitet wird (nur Buchstaben/Ziffern, klein). Daraus folgt: Serientitel ändern, number umsortieren oder id anfassen löscht die Notizen der Nutzer stillschweigend. id muss nur innerhalb des Themas eindeutig sein – das Präfix e01_ ist trotzdem sinnvoll, weil man beim Kopieren sonst leicht Duplikate erzeugt.

showNote vs. notePrompt: ShowNote ist per Default true. Abschnitt 1 hat bewusst false. Setzt du weder das eine noch das andere, bekommst du ein Notizfeld ohne Beschriftung. Faustregel: entweder notePrompt angeben oder showNote: false.

Anlegen einer neuen Datei:
1. Datei unter Resources/Raw/<domain>/ ablegen und in die index.json der Domain eintragen – vergisst man das, existiert sie schlicht nicht.
2. Nach GitHub pushen. Solange die Remote-URL 404 liefert, siehst du dauerhaft den gebündelten Stand; lokal zusätzlich bin/obj löschen.
3. coverImage muss in Resources/Images liegen, echte PNG-Daten enthalten (AAPT2 prüft den Header) und darf im .csproj nicht per Remove ausgeschlossen sein.
4. Neue Felder im Model nie required – ein einziges Thema ohne das Feld lässt sonst die komplette Datei still ausfallen.
5. passages-Kürzel müssen zur Parserliste passen, Format Mt 16,13-20. Tippfehler im Kürzel = leere Bibelkarte, keine Fehlermeldung.
6. text[]: ein Eintrag = ein Absatz, kein Markdown.
7. Kommentare und trailing commas sind erlaubt, aber viele externe JSON-Validierer melden sie als Fehler – nicht irritieren lassen.