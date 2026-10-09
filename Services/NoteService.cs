using BibleTalk.Models;

namespace BibleTalk.Services
{
    // Speichert Freitext-Notizen des Nutzers geräte-lokal.
    // Preferences reicht dafür völlig: es sind wenige kurze Texte, und sie
    // sollen ein App-Update überleben - mehr nicht.
    public interface INoteService
    {
        string Get(string key);
        void Set(string key, string value);

        // Baut den Schlüssel. Bewusst zentral, damit das Schema nur an
        // EINER Stelle steht - ändert es sich, sind alle Notizen weg.
        string BuildKey(Series series, Topic topic, string sectionId);
    }

    public class NoteService : INoteService
    {
        public string Get(string key) =>
            Preferences.Default.Get(key, string.Empty);

        public void Set(string key, string value)
        {
            // Leere Notiz nicht speichern, sondern entfernen - sonst
            // sammeln sich mit der Zeit hunderte leere Einträge an.
            if (string.IsNullOrWhiteSpace(value))
                Preferences.Default.Remove(key);
            else
                Preferences.Default.Set(key, value);
        }

        public string BuildKey(Series series, Topic topic, string sectionId) =>
            $"note:{series.Kind}:{series.Slug}:{topic.Number}:{sectionId}";
    }
}
