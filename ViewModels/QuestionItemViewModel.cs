using BibleTalk.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BibleTalk.ViewModels
{
    // Wrapper um Question: ergänzt den Auf-/Zuklapp-Zustand,
    // den das reine Daten-Model nicht kennen soll
    public partial class QuestionItemViewModel : ObservableObject
    {
        private readonly Question _question;

        public QuestionItemViewModel(Question question, int number)
        {
            _question = question;
            DisplayLabel = string.IsNullOrWhiteSpace(question.Label)
                ? $"Frage {number}"
                : question.Label;
        }

        // Kopfzeile der Box: Label aus der JSON oder "Frage N"
        public string DisplayLabel { get; }

        // Können je Fragenblock einen oder mehrere Einträge enthalten
        public IReadOnlyList<string> BibleQuestions => _question.BibleQuestion;
        public bool HasBibleQuestion => _question.HasBibleQuestion;
        public IReadOnlyList<string> ApplicationQuestions => _question.ApplicationQuestion;
        public bool HasApplicationQuestion => _question.HasApplicationQuestion;
        public IReadOnlyList<string> AddedQuestions => _question.AddedQuestions?
            .Where(q => !string.IsNullOrWhiteSpace(q))
            .ToList() ?? [];
        public bool HasAddedQuestions => _question.HasAddedQuestions;
        public string? BibleReference => _question.BibleReference;
        public bool HasBibleReference => _question.HasBibleReference;
        public string? BibleText => _question.BibleText;
        public bool HasBibleText => _question.HasBibleText;

        [ObservableProperty]
        private bool isExpanded;

        [RelayCommand]
        private void Toggle() => IsExpanded = !IsExpanded;
    }
}
