using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace SchuzeApp
{
    public partial class MainWindow : Window
    {
        private List<MemberVote> memberVotes = new List<MemberVote>();

        public string MemberName1 { get; } = "Jan Novák";
        public string MemberName2 { get; } = "Petr Svoboda";
        public string MemberName3 { get; } = "Eva Malá";
        public string MemberName4 { get; } = "Lukáš Dvořák";
        public string MemberName5 { get; } = "Anna Černá";

        public bool MemberPresent1 { get; set; } = false;
        public bool MemberPresent2 { get; set; } = false;
        public bool MemberPresent3 { get; set; } = false;
        public bool MemberPresent4 { get; set; } = false;
        public bool MemberPresent5 { get; set; } = false;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            InitializeMemberVotes();
        }

        private void InitializeMemberVotes()
        {
            memberVotes.Add(new MemberVote { MemberName = MemberName1, VotedYes = false, VotedNo = false });
            memberVotes.Add(new MemberVote { MemberName = MemberName2, VotedYes = false, VotedNo = false });
            memberVotes.Add(new MemberVote { MemberName = MemberName3, VotedYes = false, VotedNo = false });
            memberVotes.Add(new MemberVote { MemberName = MemberName4, VotedYes = false, VotedNo = false });
            memberVotes.Add(new MemberVote { MemberName = MemberName5, VotedYes = false, VotedNo = false });

            MemberVotes.ItemsSource = memberVotes;
        }

        // Získání textu z RichTextBoxu
        private string GetTextFromRichTextBox(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            return textRange.Text;
        }

        // Nastavení textu do RichTextBoxu
        private void SetTextToRichTextBox(RichTextBox rtb, string text)
        {
            TextRange textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            textRange.Text = text;
        }

        // Metoda pro změnu velikosti textu
        private void ChangeFontSize(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedSize = (comboBox.SelectedItem as ComboBoxItem).Content.ToString();
                if (double.TryParse(selectedSize, out double newSize))
                {
                    // Nastavení nové velikosti pro označený text
                    MeetingNotes.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, newSize);
                }
            }
        }

        private void StartVoting_Click(object sender, RoutedEventArgs e)
        {
            VotingPanel.Visibility = Visibility.Visible;
        }

        private void ConfirmVoting_Click(object sender, RoutedEventArgs e)
        {
            UpdateVotingSummary();
        }

        private void UpdateVotingSummary()
        {
            int yesVotes = memberVotes.Count(m => m.VotedYes);
            int noVotes = memberVotes.Count(m => m.VotedNo);
            VotingSummary.Text = $"Ano: {yesVotes}, Ne: {noVotes}";
        }

        // Metody pro minimalizaci a další akce
        private void MinimizeApp_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ExitFullscreen_Click(object sender, RoutedEventArgs e)
        {
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.WindowState = WindowState.Normal;
        }

        private void SaveMeeting_Click(object sender, RoutedEventArgs e)
        {
            // Uložení textu ze zápisu
            string meetingText = GetTextFromRichTextBox(MeetingNotes);
            // Logika pro uložení zápisu
            MessageBox.Show("Schůze uložena.");
        }

        private void LoadMeeting_Click(object sender, RoutedEventArgs e)
        {
            // Logika pro načtení schůze
            MessageBox.Show("Schůze načtena.");
        }

        private void PrintMeeting_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                FlowDocument fd = new FlowDocument
                {
                    ColumnWidth = 9999 // Zabrání sloupcování textu
                };

                // Záhlaví
                fd.Blocks.Add(new Paragraph(new Run("Zápis ze zasedání členské schůze")));
                fd.Blocks.Add(new Paragraph(new Run($"Datum konání: {MeetingDate.SelectedDate?.ToString("dd.MM.yyyy") ?? "N/A"}")));

                // Přítomní členové
                fd.Blocks.Add(new Paragraph(new Run("Přítomní členové:")));
                if (MemberPresent1) fd.Blocks.Add(new Paragraph(new Run($"{MemberName1}")));
                if (MemberPresent2) fd.Blocks.Add(new Paragraph(new Run($"{MemberName2}")));
                if (MemberPresent3) fd.Blocks.Add(new Paragraph(new Run($"{MemberName3}")));
                if (MemberPresent4) fd.Blocks.Add(new Paragraph(new Run($"{MemberName4}")));
                if (MemberPresent5) fd.Blocks.Add(new Paragraph(new Run($"{MemberName5}")));

                // Získání formátovaného textu z RichTextBoxu
                TextRange richText = new TextRange(MeetingNotes.Document.ContentStart, MeetingNotes.Document.ContentEnd);
                fd.Blocks.Add(new Paragraph(new Run("Zápis:")));
                fd.Blocks.Add(new Paragraph(new Run(richText.Text))); // Použití formátovaného textu

                // Výsledek hlasování
                fd.Blocks.Add(new Paragraph(new Run("Výsledek hlasování:")));
                fd.Blocks.Add(new Paragraph(new Run(VotingSummary.Text)));

                // Podpisy
                fd.Blocks.Add(new Paragraph(new Run("Podpisy přítomných členů:")));
                if (MemberPresent1) fd.Blocks.Add(new Paragraph(new Run($"{MemberName1}: ____________________")));
                if (MemberPresent2) fd.Blocks.Add(new Paragraph(new Run($"{MemberName2}: ____________________")));
                if (MemberPresent3) fd.Blocks.Add(new Paragraph(new Run($"{MemberName3}: ____________________")));
                if (MemberPresent4) fd.Blocks.Add(new Paragraph(new Run($"{MemberName4}: ____________________")));
                if (MemberPresent5) fd.Blocks.Add(new Paragraph(new Run($"{MemberName5}: ____________________")));

                // Tisk dokumentu
                IDocumentPaginatorSource idpSource = fd;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Zápis Členské Schůze");
            }
        }


    }

    public class MemberVote
    {
        public string MemberName { get; set; }
        public bool VotedYes { get; set; }
        public bool VotedNo { get; set; }
    }
}
