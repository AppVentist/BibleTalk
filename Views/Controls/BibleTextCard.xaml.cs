namespace BibleTalk.Views.Controls
{
    // Bewusst ohne eigene BindableProperties: die Karte übernimmt den
    // BindingContext des Elternelements. Dadurch funktioniert sie
    // unverändert in jedem DataTemplate, dessen Item von
    // BibleStepViewModel erbt.
    public partial class BibleTextCard : ContentView
    {
        public BibleTextCard()
        {
            InitializeComponent();
        }
    }
}
