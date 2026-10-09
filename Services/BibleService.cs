using System.Collections.Concurrent;
using System.Text.Json;
using BibleTalk.Models;

namespace BibleTalk.Services
{
    public interface IBibleService
    {
        // Anzeigename der Übersetzung, z. B. für die Quellenangabe
        string TranslationName { get; }

        // Löst eine Angabe wie "1Kor 8,1-3" auf. null = nicht gefunden.
        Task<BiblePassage?> GetPassageAsync(string? reference, CancellationToken token = default);
    }

    // Liest die gebündelte Übersetzung aus Resources/Raw/bibles/<name>/<buchnr>.json.
    // Bewusst EINE Datei pro Buch: so muss nie die komplette Bibel im Speicher
    // liegen, sondern nur die tatsächlich aufgeschlagenen Bücher.
    public class BibleService : IBibleService
    {
        private const string TranslationFolder = "luther1912";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        // Geladene Bücher bleiben im Speicher - ein Buch wird pro Sitzung
        // typischerweise mehrfach gebraucht
        private readonly ConcurrentDictionary<int, BibleBookFile?> _books = new();
        private readonly SemaphoreSlim _loadLock = new(1, 1);

        public string TranslationName => "Luther 1912";

        public async Task<BiblePassage?> GetPassageAsync(string? reference, CancellationToken token = default)
        {
            var parsed = BibleReferenceParser.Parse(reference);
            if (parsed is null)
                return null;

            var book = await GetBookAsync(parsed.BookNumber, token);
            if (book is null)
                return null;

            if (!book.Chapters.TryGetValue(parsed.Chapter.ToString(), out var chapter))
                return null;

            var verses = SelectVerses(chapter, parsed.VerseRanges);
            if (verses.Count == 0)
                return null;

            return new BiblePassage
            {
                // Buchname aus der Datei, damit Anzeige und Text zusammenpassen
                Title = string.IsNullOrWhiteSpace(book.Name)
                    ? parsed.Display
                    : parsed.Display.Replace(parsed.BookName, book.Name),
                Verses = verses
            };
        }

        private static List<BibleVerse> SelectVerses(
            Dictionary<string, string> chapter,
            IReadOnlyList<(int From, int To)> ranges)
        {
            var verses = new List<BibleVerse>();

            // Ohne Versangabe: ganzes Kapitel in Versreihenfolge
            if (ranges.Count == 0)
            {
                foreach (var entry in chapter.OrderBy(e => ToNumber(e.Key)))
                    verses.Add(new BibleVerse { Number = ToNumber(entry.Key), Text = entry.Value });

                return verses;
            }

            foreach (var (from, to) in ranges)
            {
                for (var number = from; number <= to; number++)
                {
                    // Fehlende Verse still überspringen: manche Kapitel haben
                    // Lücken oder zusammengezogene Verse
                    if (chapter.TryGetValue(number.ToString(), out var text))
                        verses.Add(new BibleVerse { Number = number, Text = text });
                }
            }

            return verses;
        }

        private static int ToNumber(string key) => int.TryParse(key, out var value) ? value : 0;

        private async Task<BibleBookFile?> GetBookAsync(int bookNumber, CancellationToken token)
        {
            if (_books.TryGetValue(bookNumber, out var cached))
                return cached;

            await _loadLock.WaitAsync(token);
            try
            {
                // Zweite Prüfung: ein paralleler Aufruf könnte das Buch
                // inzwischen geladen haben
                if (_books.TryGetValue(bookNumber, out cached))
                    return cached;

                var book = await LoadBookAsync(bookNumber, token);
                _books[bookNumber] = book;
                return book;
            }
            finally
            {
                _loadLock.Release();
            }
        }

        private static async Task<BibleBookFile?> LoadBookAsync(int bookNumber, CancellationToken token)
        {
            var path = $"bibles/{TranslationFolder}/{bookNumber}.json";

            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync(path);
                return await JsonSerializer.DeserializeAsync<BibleBookFile>(stream, JsonOptions, token);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Bibelbuch {bookNumber} nicht ladbar: {ex}");
                return null;
            }
        }
    }
}
