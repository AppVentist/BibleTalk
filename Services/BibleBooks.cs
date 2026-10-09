using System.Text;

namespace BibleTalk.Services
{
    // Zuordnung Buchnummer <-> deutscher Name, plus die gängigen Abkürzungen.
    // Damit kannst du in den Content-JSONs "1Kor", "1. Korinther" oder
    // "1 Kor" schreiben - alles landet bei Buch 46.
    public static class BibleBooks
    {
        // Reihenfolge = Buchnummer 1..66
        private static readonly string[][] Names =
        [
            ["1. Mose", "1Mo", "1Mos", "Gen", "Genesis"],
            ["2. Mose", "2Mo", "2Mos", "Ex", "Exodus"],
            ["3. Mose", "3Mo", "3Mos", "Lev", "Levitikus"],
            ["4. Mose", "4Mo", "4Mos", "Num", "Numeri"],
            ["5. Mose", "5Mo", "5Mos", "Dtn", "Dt", "Deuteronomium"],
            ["Josua", "Jos"],
            ["Richter", "Ri"],
            ["Rut", "Ruth"],
            ["1. Samuel", "1Sam", "1Sa"],
            ["2. Samuel", "2Sam", "2Sa"],
            ["1. Könige", "1Kön", "1Kge", "1Kg"],
            ["2. Könige", "2Kön", "2Kge", "2Kg"],
            ["1. Chronik", "1Chr", "1Ch"],
            ["2. Chronik", "2Chr", "2Ch"],
            ["Esra", "Esr"],
            ["Nehemia", "Neh"],
            ["Ester", "Esther", "Est"],
            ["Hiob", "Ijob", "Hi", "Job"],
            ["Psalm", "Psalmen", "Ps"],
            ["Sprüche", "Sprichwörter", "Spr"],
            ["Prediger", "Kohelet", "Pred", "Koh"],
            ["Hoheslied", "Hohelied", "Hld", "Hl"],
            ["Jesaja", "Jes"],
            ["Jeremia", "Jer"],
            ["Klagelieder", "Klgl", "Klag"],
            ["Hesekiel", "Ezechiel", "Hes", "Ez"],
            ["Daniel", "Dan"],
            ["Hosea", "Hos"],
            ["Joel", "Joe"],
            ["Amos", "Am"],
            ["Obadja", "Obd", "Ob"],
            ["Jona", "Jon"],
            ["Micha", "Mi", "Mich"],
            ["Nahum", "Nah"],
            ["Habakuk", "Hab"],
            ["Zefanja", "Zephanja", "Zef", "Zeph"],
            ["Haggai", "Hag"],
            ["Sacharja", "Sach"],
            ["Maleachi", "Mal"],
            ["Matthäus", "Matth", "Matt", "Mt"],
            ["Markus", "Mark", "Mk", "Mr"],
            ["Lukas", "Luk", "Lk"],
            ["Johannes", "Joh", "Jh"],
            ["Apostelgeschichte", "Apg", "Act"],
            ["Römer", "Röm", "Rö", "Rom"],
            ["1. Korinther", "1Kor", "1Ko"],
            ["2. Korinther", "2Kor", "2Ko"],
            ["Galater", "Gal"],
            ["Epheser", "Eph"],
            ["Philipper", "Phil", "Php"],
            ["Kolosser", "Kol"],
            ["1. Thessalonicher", "1Thess", "1Thes", "1Th"],
            ["2. Thessalonicher", "2Thess", "2Thes", "2Th"],
            ["1. Timotheus", "1Tim", "1Ti"],
            ["2. Timotheus", "2Tim", "2Ti"],
            ["Titus", "Tit"],
            ["Philemon", "Phlm", "Phm"],
            ["Hebräer", "Hebr", "Heb"],
            ["Jakobus", "Jak"],
            ["1. Petrus", "1Petr", "1Pet", "1Pt"],
            ["2. Petrus", "2Petr", "2Pet", "2Pt"],
            ["1. Johannes", "1Joh", "1Jo"],
            ["2. Johannes", "2Joh", "2Jo"],
            ["3. Johannes", "3Joh", "3Jo"],
            ["Judas", "Jud", "Jd"],
            ["Offenbarung", "Offb", "Off", "Apk", "Apok"],
        ];

        // Normalisierte Abkürzung -> Buchnummer
        private static readonly Dictionary<string, int> Lookup = BuildLookup();

        public const int Count = 66;

        public static string GetName(int bookNumber) =>
            bookNumber is >= 1 and <= Count ? Names[bookNumber - 1][0] : string.Empty;

        // Liefert 0, wenn der Name unbekannt ist
        public static int GetNumber(string name)
        {
            var key = Normalize(name);
            return key.Length > 0 && Lookup.TryGetValue(key, out var number) ? number : 0;
        }

        private static Dictionary<string, int> BuildLookup()
        {
            var lookup = new Dictionary<string, int>(StringComparer.Ordinal);

            for (var i = 0; i < Names.Length; i++)
            {
                foreach (var alias in Names[i])
                {
                    var key = Normalize(alias);

                    // Erste Nennung gewinnt - so kann eine Abkürzung nicht
                    // versehentlich von einem späteren Buch überschrieben werden
                    if (key.Length > 0)
                        lookup.TryAdd(key, i + 1);
                }
            }

            return lookup;
        }

        // "1. Korinther" / "1 Kor" / "1Kor" -> "1kor..." bzw. "1korinther"
        // Umlaute werden ausgeschrieben, damit "Röm" und "Roem" gleich landen.
        private static string Normalize(string value)
        {
            var builder = new StringBuilder(value.Length);

            foreach (var c in value.ToLowerInvariant())
            {
                switch (c)
                {
                    case 'ä': builder.Append("ae"); break;
                    case 'ö': builder.Append("oe"); break;
                    case 'ü': builder.Append("ue"); break;
                    case 'ß': builder.Append("ss"); break;
                    default:
                        if (char.IsLetterOrDigit(c))
                            builder.Append(c);
                        break;
                }
            }

            return builder.ToString();
        }
    }
}
