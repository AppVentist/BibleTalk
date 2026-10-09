using System.Text.RegularExpressions;
using BibleTalk.Models;

namespace BibleTalk.Services
{
    // Zerlegt Angaben wie "1Kor 8,1-3", "1. Korinther 8,1-3.7",
    // "Joh 3,16", "Ps 23" oder "46 8:1-3" in Buch, Kapitel und Verse.
    public static partial class BibleReferenceParser
    {
        // Buch = entweder eine reine Zahl (Buchnummer, gefolgt von Leerzeichen)
        // oder ein Name, der mit 1-5 beginnen darf ("1. Korinther").
        // Kapitel und Verse werden durch Komma (deutsch) oder Doppelpunkt getrennt.
        [GeneratedRegex(
            @"^(?<book>\d{1,2}(?=\s)|(?:[1-5]\s*\.?\s*)?[^\d]+?)\s*(?<chapter>\d+)\s*(?:[,:]\s*(?<verses>[\d\s\-–.,;]+))?$",
            RegexOptions.CultureInvariant)]
        private static partial Regex ReferencePattern();

        // Einzelner Versbereich: "7" oder "7-12"
        [GeneratedRegex(@"(?<from>\d+)\s*(?:[-–]\s*(?<to>\d+))?", RegexOptions.CultureInvariant)]
        private static partial Regex RangePattern();

        // Liefert null, wenn die Angabe nicht gedeutet werden kann.
        // Bewusst kein Exception-Wurf: eine unsaubere Angabe in einer
        // Content-JSON darf nicht die ganze Seite kippen.
        public static BibleReference? Parse(string? reference)
        {
            if (string.IsNullOrWhiteSpace(reference))
                return null;

            var match = ReferencePattern().Match(reference.Trim());
            if (!match.Success)
                return null;

            var bookToken = match.Groups["book"].Value.Trim();

            // Reine Zahl = direkte Buchnummer, sonst über die Namenstabelle
            var bookNumber = int.TryParse(bookToken, out var direct)
                ? direct
                : BibleBooks.GetNumber(bookToken);

            if (bookNumber is < 1 or > BibleBooks.Count)
                return null;

            if (!int.TryParse(match.Groups["chapter"].Value, out var chapter) || chapter < 1)
                return null;

            return new BibleReference
            {
                BookNumber = bookNumber,
                BookName = BibleBooks.GetName(bookNumber),
                Chapter = chapter,
                VerseRanges = ParseRanges(match.Groups["verses"].Value)
            };
        }

        private static IReadOnlyList<(int From, int To)> ParseRanges(string verses)
        {
            if (string.IsNullOrWhiteSpace(verses))
                return [];

            var ranges = new List<(int From, int To)>();

            foreach (Match range in RangePattern().Matches(verses))
            {
                if (!int.TryParse(range.Groups["from"].Value, out var from))
                    continue;

                var to = int.TryParse(range.Groups["to"].Value, out var parsedTo) ? parsedTo : from;

                if (to < from)
                    (from, to) = (to, from);

                ranges.Add((from, to));
            }

            return ranges;
        }
    }
}
