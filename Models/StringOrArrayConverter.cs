using System.Text.Json;
using System.Text.Json.Serialization;

namespace BibleTalk.Models
{
    // Erlaubt in der JSON beide Schreibweisen für dasselbe Feld:
    //
    //   "bibleQuestion": "Eine einzelne Frage"
    //   "bibleQuestion": [ "Erste Frage", "Zweite Frage" ]
    //
    // Dadurch bleiben alle bestehenden Dateien gültig, und neue Inhalte
    // können bei Bedarf mehrere Fragen pro Block enthalten.
    public class StringOrArrayConverter : JsonConverter<List<string>>
    {
        public override List<string> Read(
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                // Einzelner String -> Liste mit einem Eintrag
                case JsonTokenType.String:
                    var single = reader.GetString();
                    return string.IsNullOrWhiteSpace(single) ? [] : [single];

                // Fehlender Wert -> leere Liste (kein Fehler)
                case JsonTokenType.Null:
                    return [];

                // Array -> alle nicht-leeren Einträge übernehmen
                case JsonTokenType.StartArray:
                    var list = new List<string>();
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                            return list;

                        if (reader.TokenType == JsonTokenType.String)
                        {
                            var value = reader.GetString();
                            if (!string.IsNullOrWhiteSpace(value))
                                list.Add(value);
                        }
                        else
                        {
                            throw new JsonException(
                                "Innerhalb des Arrays sind nur Textwerte erlaubt.");
                        }
                    }
                    throw new JsonException("Array wurde nicht korrekt beendet.");

                default:
                    throw new JsonException(
                        $"Erwartet wurde ein Text oder ein Array von Texten, gefunden: {reader.TokenType}.");
            }
        }

        public override void Write(
            Utf8JsonWriter writer, List<string> value, JsonSerializerOptions options)
        {
            // Ein einzelner Eintrag wird wieder als schlichter String geschrieben
            if (value.Count == 1)
            {
                writer.WriteStringValue(value[0]);
                return;
            }

            writer.WriteStartArray();
            foreach (var item in value)
                writer.WriteStringValue(item);
            writer.WriteEndArray();
        }
    }
}
