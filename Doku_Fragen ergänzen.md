# BibleTalk – Weitere Frage in Klappbox (QuestionsPage) anlegen 

1. Models/Question.cs -> class Question -> einmal string ... + [JsonIgnore] Attribut setzen
2. ViewModels/QuestionsItemViewModel -> Eintrag in der "// Kopfzeile der Box //"
    - einmal als string + einmal als bool zur Anzeige, wenn Kontekt enthalten - ansonsten unsichtbar 
3. In der Views/QuestionsPage.xaml -> Label-Titel anlegen + Label-Binding


# BibleTalk - ShareLinks

1. ShareLinks > Services/AppLinks.cs -> Klasse "AppLinks" anpassen