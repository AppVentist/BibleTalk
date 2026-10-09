using BibleTalk.Models;
using BibleTalk.ViewModels;

namespace BibleTalk.Services
{
    // Löst die "passages"-Angaben einer Menge von Schritten in echten Bibeltext
    // auf. Lag früher im QuestionsViewModel - jetzt geteilt, damit die
    // Jüngerschaft (und später der Hauskreis) den Cache-Mechanismus nicht
    // nachbauen müssen.
    public interface IBibleTextResolver
    {
        Task LoadAsync(IEnumerable<BibleStepViewModel> steps);
    }

    public class BibleTextResolver : IBibleTextResolver
    {
        private readonly IBibleService _bibleService;

        public BibleTextResolver(IBibleService bibleService)
        {
            _bibleService = bibleService;
        }

        public async Task LoadAsync(IEnumerable<BibleStepViewModel> steps)
        {
            var withPassages = steps.Where(s => s.Passages.Count > 0).ToList();
            if (withPassages.Count == 0)
                return;

            foreach (var step in withPassages)
                step.IsLoadingBibleText = true;

            // Mehrere Schritte teilen sich oft denselben Block, deshalb wird
            // pro Stelle nur einmal nachgeschlagen.
            var cache = new Dictionary<string, BiblePassage>(StringComparer.OrdinalIgnoreCase);

            try
            {
                foreach (var step in withPassages)
                {
                    var resolved = new List<BiblePassage>();

                    foreach (var reference in step.Passages)
                    {
                        if (!cache.TryGetValue(reference, out var passage))
                        {
                            var loaded = await _bibleService.GetPassageAsync(reference);

                            // Unbekannte Angabe still überspringen - der Rest
                            // der Seite soll trotzdem funktionieren
                            if (loaded is null)
                            {
                                System.Diagnostics.Debug.WriteLine(
                                    $"Bibelstelle nicht auflösbar: \"{reference}\"");
                                continue;
                            }

                            cache[reference] = passage = loaded;
                        }

                        resolved.Add(passage);
                    }

                    step.ApplyPassages(resolved, _bibleService.TranslationName);
                }
            }
            catch (Exception ex)
            {
                // Der Aufrufer startet diese Methode bewusst ohne await.
                // Eine Ausnahme waere hier sonst niemand mehr zugeordnet und
                // wuerde als unbeobachtete Task-Ausnahme enden.
                System.Diagnostics.Debug.WriteLine($"Bibeltexte laden fehlgeschlagen: {ex}");
            }
            finally
            {
                foreach (var step in withPassages)
                    step.IsLoadingBibleText = false;
            }
        }
    }
}
