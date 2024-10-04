using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Toolkit.Mvvm.Input;
using Puzzle.BL.Enums;
using Puzzle.BL.Interfaces;

namespace Puzzle.App.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ISolver solver;
        private readonly IBoard board;
        private readonly ICardMover cardMover;
        private bool isSolvingInProgress;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainViewModel(ISolver solver, 
            IBoardBuilder boardBuilder,
            ICardMover cardMover)
        {
            boardBuilder.SetDefaultSquareCards();
            board = boardBuilder.GetBoard();
            this.solver = solver;
            this.cardMover = cardMover;
            LoadImages();
            SolvePuzzleCmd = new RelayCommand(SolvePuzzle);
            SetRandomPuzzleArrangementCmd = new RelayCommand(SetRandomPuzzleArrangement);
        }

        public ObservableCollection<BitmapImage> Images { get; set; } = new();
        public ICommand SolvePuzzleCmd { get; set; }
        public ICommand SetRandomPuzzleArrangementCmd { get; set; }
        public bool IsSolvingInProgress 
        { 
            get => isSolvingInProgress; 
            set
            {
                isSolvingInProgress = value;
                OnPropertyChanged(nameof(IsSolvingInProgress));
            }
        }

        private void SetRandomPuzzleArrangement()
        {
            try
            {
                cardMover.MoveCardsToRandomPositions(board);
                RearrangeBoard(board);
                
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error while generating random puzzle arrangement: {e.Message}");
            }
        }

        private async void SolvePuzzle()
        {
            try
            {
                IsSolvingInProgress = true;
                var isSolved = await solver.SolveBoard(board);
                RearrangeBoard(board);
                if (!isSolved)
                {
                    MessageBox.Show("Could not be solved.");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error while solving: {e.Message}");
            }
            finally
            {
                IsSolvingInProgress = false;
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
            var numberOfImages = board.RowCount * board.ColumnCount;
            for (var i = 1; i <= numberOfImages; i++)
            {
                var bitmap = new BitmapImage(new Uri(Path.GetFullPath($".\\Resources\\Images\\card{i}.png")));
                Images.Add(bitmap);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}