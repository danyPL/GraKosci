using GraKosci.Model;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GraKosci
{
    public partial class MainPage : ContentPage
    {
        List<Dice> dicesF = new();
        List<Dice> dicesS = new();
        int currentPlayer = 1; 
        bool rerollUsedF = false;
        bool rerollUsedS = false;
        Random rnd = new();

        public MainPage()
        {
            InitializeComponent();
            StartGame();
        }

        void StartGame()
        {
            dicesF = GenerateDices();
            dicesS = GenerateDices();

            rerollUsedF = false;
            rerollUsedS = false;
            currentPlayer = 1;

            DrawDiceGrids();
            UpdateScores();
        }

        List<Dice> GenerateDices()
        {
            return Enumerable.Range(0, 5)
                .Select(_ => new Dice { Value = rnd.Next(1, 7) })
                .ToList();
        }

        void DrawDiceGrids()
        {
            DrawDiceGrid(DiceGrid, dicesF, currentPlayer == 1);
            DrawDiceGrid(DiceGridS, dicesS, currentPlayer == 2);
        }

        void DrawDiceGrid(Grid grid, List<Dice> dices, bool active)
        {
            grid.Children.Clear();
            for (int i = 0; i < dices.Count; i++)
            {
                var dice = dices[i];
                var btn = new Button
                {
                    ImageSource = dice.ImagePath,
                    BackgroundColor = dice.IsSelected ? Colors.LightBlue : Colors.White,
                    CornerRadius = 10,
                    IsEnabled = active,
                    WidthRequest = 150,
                    HeightRequest= 150
                };

                int index = i;
                btn.Clicked += (s, e) =>
                {
                    dice.IsSelected = !dice.IsSelected;
                    DrawDiceGrids();
                };

                grid.Add(btn, i, 0);
            }
        }
        void OnPass(object sender, EventArgs e)
        {
            if (currentPlayer == 1)
            {
                rerollUsedF = true;
                currentPlayer = 2;
            }
            else if (currentPlayer == 2)
            {
                rerollUsedS = true;
                currentPlayer = 0;
            }

            DrawDiceGrids();
            UpdateScores();

            if (currentPlayer == 0)
            {
                int scoreF = CalcScore(dicesF);
                int scoreS = CalcScore(dicesS);

                if (scoreF > scoreS)
                    DisplayAlert("Wynik", "Gracz 1 wygrywa!", "OK");
                else if (scoreF < scoreS)
                    DisplayAlert("Wynik", "Gracz 2 wygrywa!", "OK");
                else
                    DisplayAlert("Wynik", "Remis!", "OK");
            }
        }

        void OnReroll(object sender, EventArgs e)
        {
            if (currentPlayer == 1 && rerollUsedF) return;
            if (currentPlayer == 2 && rerollUsedS) return;

            var dices = currentPlayer == 1 ? dicesF : dicesS;

            for (int i = 0; i < dices.Count; i++)
            {
                if (dices[i].IsSelected)
                {
                    dices[i] = new Dice { Value = rnd.Next(1, 7) };
                }
            }

            if (currentPlayer == 1)
            {
                rerollUsedF = true;
                currentPlayer = 2;
            }
            else
            {
                rerollUsedS = true;
                currentPlayer = 0; 
            }

            DrawDiceGrids();
            UpdateScores();

            if (currentPlayer == 0)
            {
                int scoreF = CalcScore(dicesF);
                int scoreS = CalcScore(dicesS);
                if (scoreF > scoreS)
                    DisplayAlert("Wynik", "Gracz 1 wygrywa!", "OK");
                else if (scoreF < scoreS)
                    DisplayAlert("Wynik", "Gracz 2 wygrywa!", "OK");
                else
                    DisplayAlert("Wynik", "Remis!", "OK");
            }
        }

        void OnRestart(object sender, EventArgs e)
        {
            StartGame();
        }

        void UpdateScores()
        {
            Score1.Text = $"Punktacja Gracza 1: {CalcScore(dicesF)}";
            Score2.Text = $"Punktacja Gracza 2: {CalcScore(dicesS)}";
        }

        int CalcScore(List<Dice> list)
        {
            var v = list.Select(d => d.Value).OrderBy(x => x).ToList();
            if (v.SequenceEqual(new[] { 1, 2, 3, 4, 5 }) || v.SequenceEqual(new[] { 2, 3, 4, 5, 6 }))
                return 50;

            var count = v.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
            int score = 0;

            foreach (var kvp in count.OrderByDescending(k => k.Value))
            {
                int val = kvp.Key;
                int c = kvp.Value;
                if (c == 5) score += 50;
                else if (c == 4) score += val * 5;
                else if (c == 3) score += val * 4;
                else if (c == 2) score += val * 3;
            }

            score += v.Sum();
            return score;
        }
    }
}
