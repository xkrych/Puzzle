using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Toolkit.Mvvm.Input;
using Puzzle.BL.Enums;
using Puzzle.BL.Interfaces;

namespace Puzzle.App.ViewModels
{
    public class MainViewModel
    {
        private readonly ISolver solver;

        public MainViewModel(ISolver solver)
        {
            this.solver = solver;
            LoadImages();
            SolvePuzzleCmd = new RelayCommand(SolvePuzzle);
        }

        public ObservableCollection<BitmapImage> Images { get; set; } = new();
        public ICommand SolvePuzzleCmd { get; set; }

        private async void SolvePuzzle()
        {
            try
            {
                var isSolved = await solver.SolveBoard();
                RearrangeBoard(solver.Board);
                if (!isSolved)
                {
                    MessageBox.Show("Could not be solved.");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error while solving: {e.Message}");
            }
        }

        private void RearrangeBoard(IBoard board)
        {
            Images.Clear();

            for (var i = 0; i < board.RowCount; i++)
            {
                for (var j = 0; j < board.ColumnCount; j++)
                {
                    var card = board.Cards[i, j];
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(Path.GetFullPath($".\\Resources\\Images\\card{card.Id}.png"));
                    bitmap.Rotation = GetCardRotation(card);
                    bitmap.EndInit();
                    Images.Add(bitmap);
                }
            }
        }

        private Rotation GetCardRotation(ICard card)
        {
            switch (card.GetCardRightRotation())
            {
                case CardRightRotation.Rotate90:
                    return Rotation.Rotate90;
                case CardRightRotation.Rotate180:
                    return Rotation.Rotate180;
                case CardRightRotation.Rotate270:
                    return Rotation.Rotate270;
                default:
                    return Rotation.Rotate0;
            }
        }

        private void LoadImages()
        {
            for (var i = 1; i <= 9; i++)
            {
                var bitmap = new BitmapImage(new Uri(Path.GetFullPath($".\\Resources\\Images\\card{i}.png")));
                Images.Add(bitmap);
            }
        }
    }
}