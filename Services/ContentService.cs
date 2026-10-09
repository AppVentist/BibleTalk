using System.Text.Json;
using BibleTalk.Models;
using Microsoft.Maui.Networking;

namespace BibleTalk.Services
{
    public interface IContentService
    {
        // indexPath z. B. "quarterlies/index.json"
        Task<IReadOnlyList<Series>> GetSeriesAsync(string indexPath, SeriesKind kind);
    }

    // Die Inhalte sind auf mehrere Dateien verteilt:
    //   quarterlies/index.json    -> Liste der Hefte (Reihenfolge = Anzeige-Reihenfolge)
    //   quarterlies/2026_q1.json  -> ein Heft pro Datei
    //   quarterlies/2026_q2.json
    //   ...
    //
    // Jede Datei wird in dieser Reihenfolge geladen:
    // 1. GitHub (raw-URL)  -> bei Erfolg lokal cachen
    // 2. Lokaler Cache     -> falls offline oder GitHub nicht erreichbar
    // 3. Mitgelieferte Fallback-Datei (Resources/Raw/quarterlies/...)
    public class ContentService : IContentService
    {
        // Hinweis: Das Repository muss PUBLIC sein, sonst liefert die URL 404
        // und die App faellt auf Cache bzw. mitgelieferte Dateien zurueck.
        private const string RemoteBaseUrl =
            "https://raw.githubusercontent.com/IT-BW/BibleTalk/master/Resources/Raw/";


        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip, // //-Kommentare in der JSON erlauben
            AllowTrailingCommas = true
        };

        private readonly HttpClient _httpClient;

        public ContentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Liefert false bei Flugmodus, ausgeschaltetem WLAN/Mobilfunk usw.
        // "Verbunden, aber ohne Internet" (Hotel-Portal) laesst sich damit
        // nicht erkennen - dafuer greift weiterhin der Timeout des HttpClient.
        private static bool HasInternet =>
            Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

        public async Task<IReadOnlyList<Series>> GetSeriesAsync(string indexPath, SeriesKind kind)
        {
            // Zuerst den Index laden - er bestimmt, welche Hefte es gibt
            // und in welcher Reihenfolge sie angezeigt werden.
            var index = await LoadFileAsync<SeriesIndex>(indexPath);
            if (index is null || index.Series.Count == 0)
                return [];

            // Dann jedes Heft aus seiner eigenen Datei laden - parallel.
            // Seriell lagen bei schlechter Verbindung so viele Timeouts
            // hintereinander, wie es Dateien gibt; die Startseite stand
            // entsprechend lange still.
            var loaded = await Task.WhenAll(
                index.Series.Select(entry => LoadFileAsync<Series>(entry.File)));

            // Reihenfolge aus dem Index beibehalten.
            // Eine fehlerhafte Datei blockiert nicht die anderen Hefte.
            var result = new List<Series>();
            for (var i = 0; i < index.Series.Count; i++)
            {
                var entry = index.Series[i];
                var series = loaded[i];

                if (series is not null)
                {
                    // Id aus dem Index setzen - sie ist die Grundlage der
                    // Notiz-Schluessel und muss stabil bleiben, auch wenn der
                    // Titel der Serie spaeter korrigiert wird.
                    series.Id = entry.Id;
                    series.Kind = kind;
                    result.Add(series);
                }
                else
                    System.Diagnostics.Debug.WriteLine(
                        $"Serie '{entry.Id}' ({entry.File}) konnte nicht geladen werden.");
            }

            return result;
        }

        // Lädt eine einzelne Datei: Remote -> Cache -> App-Paket.
        // relativePath z. B. "quarterlies/2026_q1.json"
        private async Task<T?> LoadFileAsync<T>(string relativePath) where T : class
        {
            var cachePath = Path.Combine(FileSystem.AppDataDirectory, relativePath);

            // 1. Versuch: aktuelle Inhalte von GitHub laden.
            // Ohne Netzverbindung wird der Versuch uebersprungen, statt in
            // den Timeout zu laufen.
            if (HasInternet)
            {
                try
                {
                    var json = await _httpClient.GetStringAsync(RemoteBaseUrl + relativePath);
                    var data = Deserialize<T>(json, relativePath);
                    if (data is not null)
                    {
                        // Cache erst schreiben, wenn die Datei gültig ist
                        Directory.CreateDirectory(Path.GetDirectoryName(cachePath)!);
                        await File.WriteAllTextAsync(cachePath, json);
                        return data;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"Remote-Laden fehlgeschlagen ({relativePath}): {ex.Message}");
                }
            }

            // 2. Versuch: letzter erfolgreich geladener Stand (Cache)
            try
            {
                if (File.Exists(cachePath))
                {
                    var data = Deserialize<T>(await File.ReadAllTextAsync(cachePath), relativePath);
                    if (data is not null)
                        return data;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Cache-Laden fehlgeschlagen ({relativePath}): {ex.Message}");
            }

            // 3. Fallback: mitgelieferte Datei aus dem App-Paket
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync(relativePath);
                using var reader = new StreamReader(stream);
                return Deserialize<T>(await reader.ReadToEndAsync(), relativePath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Fallback-Laden fehlgeschlagen ({relativePath}): {ex.Message}");
                return null;
            }
        }

        private static T? Deserialize<T>(string json, string relativePath) where T : class
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json, JsonOptions);
            }
            catch (JsonException ex)
            {
                // Pfad und Zeile mit ausgeben - sonst faellt eine einzelne
                // fehlerhafte Frage nur als komplett fehlendes Heft auf.
                System.Diagnostics.Debug.WriteLine(
                    $"JSON ungueltig in '{relativePath}' " +
                    $"(Zeile {ex.LineNumber}, Position {ex.BytePositionInLine}): {ex.Message}");
                return null;
            }
        }

        // Wurzelobjekt von series/index.json
        private sealed class SeriesIndex
        {
            public List<SeriesIndexEntry> Series { get; set; } = [];
        }

        private sealed class SeriesIndexEntry
        {
            public string Id { get; set; } = "";
            public string File { get; set; } = "";
        }
    }
}
