using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Game
{
    public class GraphicInterface : StackPanel
    {
        //Constants
        public static int GRID_SIZE = 150;
        public static int ELLIPSE_SIZE = 120;
        public static int REDUCED_GRID_SIZE = 120;
        public static int REDUCED_ELLIPSE_SIZE = 90;
        public static SolidColorBrush PLAYER_1 = new SolidColorBrush {Color = Colors.Red};
        public static SolidColorBrush PLAYER_2 = new SolidColorBrush {Color = Colors.Blue};
        public static SolidColorBrush NO_COLOR = new SolidColorBrush {Color = Colors.Transparent};

        private Button btnNewGame;
        private TextBlock tbHeaderTitle;
        private TextBlock tbHeaderCurrentPlayer;
        private Grid gridHeaderTokens;
        private Grid gridBoard;
        private int actualTurn = 0;
        private GameLogic logic;
        private bool isWinner = false;
        private char[,] logicBoard = new char[GameLogic.MAX_SIZE, GameLogic.MAX_SIZE];
        private Ellipse tokenActualPlayer;

        public GraphicInterface()
        {
            logic = new GameLogic();
            initializeGridBoard();            
            setEllipses();
            setHeaderPlayers();
            setGridHeader();

            btnNewGame = new Button()
            {
                Content = "New game"
            };
            btnNewGame.Tap += startNewGame;

            tbHeaderTitle = new TextBlock()
            {
                Text = "TicTacToe by @FerPeralesM",
                FontSize = 34,         
                TextAlignment = TextAlignment.Center       
            };

            this.Children.Add(tbHeaderTitle);
            this.Children.Add(tbHeaderCurrentPlayer);
            this.Children.Add(gridHeaderTokens);
            this.Children.Add(gridBoard);
            this.Children.Add(btnNewGame); 
        }

        private void setHeaderPlayers()
        {
            tbHeaderCurrentPlayer = new TextBlock()
            {
                Text = "PLAYER 1        ACTUAL      PLAYER 2  ",
                FontSize = 26,
                TextAlignment = TextAlignment.Center
            };
        }

        private void setGridHeader()
        {
            gridHeaderTokens = new Grid();
            for (int i = 0; i < 3; i++)
            {
                gridHeaderTokens.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(GRID_SIZE),
                });
            }

            Ellipse tokenPlayerOne = new Ellipse()
            {
                Fill = PLAYER_1,
                Width = REDUCED_ELLIPSE_SIZE,
                Height = REDUCED_ELLIPSE_SIZE,

            };
            tokenPlayerOne.SetValue(Grid.ColumnProperty, 0);

            Ellipse tokenPlayerTwo = new Ellipse()
            {
                Fill = PLAYER_2,
                Width = REDUCED_ELLIPSE_SIZE,
                Height = REDUCED_ELLIPSE_SIZE,
            };
            tokenPlayerTwo.SetValue(Grid.ColumnProperty, 2);

            tokenActualPlayer = new Ellipse(){
                Fill = getActualTurn(actualTurn),
                Width = REDUCED_ELLIPSE_SIZE,
                Height = REDUCED_ELLIPSE_SIZE,
            };
            tokenActualPlayer.SetValue(Grid.ColumnProperty, 1);

            gridHeaderTokens.Children.Add(tokenPlayerOne);
            gridHeaderTokens.Children.Add(tokenActualPlayer);
            gridHeaderTokens.Children.Add(tokenPlayerTwo);
        }

        private void setEllipses()
        {
            for (int i = 0; i < GameLogic.MAX_SIZE; i++)
            {
                for (int j = 0; j < GameLogic.MAX_SIZE; j++)
                {
                    Ellipse ellipse = new Ellipse();
                    ellipse.Fill = NO_COLOR;
                    ellipse.Width = ELLIPSE_SIZE;
                    ellipse.Height = ELLIPSE_SIZE;
                    ellipse.SetValue(Grid.ColumnProperty, i);
                    ellipse.SetValue(Grid.RowProperty, j);
                    ellipse.Tap += detectTapInGrid;
                    gridBoard.Children.Add(ellipse);
                    logicBoard[i, j] = ' ';
                }
            }
        }

        private void initializeGridBoard()
        {
            gridBoard = new Grid();
            for (int i = 0; i < GameLogic.MAX_SIZE; i++)
            {
                gridBoard.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(GRID_SIZE),  
                });
               
                gridBoard.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(GRID_SIZE)
                });
            }
            gridBoard.ShowGridLines = true;
            gridBoard.Background = NO_COLOR;
        }

        public SolidColorBrush getActualTurn(int value)
        {
            return value % 2 == 0 ? PLAYER_1 : PLAYER_2;
        }

        public void detectTapInGrid(object sender, GestureEventArgs e)
        {
            Ellipse tempTappedItem = sender as Ellipse;
            int tappedColumn = Grid.GetColumn(tempTappedItem);
            int tappedRow = Grid.GetRow(tempTappedItem);

            tokenActualPlayer.Fill = getActualTurn(actualTurn+1);

            if (!isWinner)
            {
                if (isPositionValid(tappedColumn, tappedRow))
                {
                    setToken(tappedColumn, tappedRow, tempTappedItem);
                    actualTurn++;
                }
            }


            isWinner = logic.isAWinner(logicBoard);
            if (canSomeoneWin())
            {
                if (isWinner)
                {
                    MessageBox.Show("Player " + (actualTurn % 2 == 0 ? 2 : 1) + " wins");
                }
                else
                {
                    if (isPositionValid(tappedColumn, tappedRow))
                    {
                        setToken(tappedColumn, tappedRow, tempTappedItem);
                        actualTurn++;
                    }
                }
            }
            else
            {
                //Cannot be a winner in less than GameLogic.MINIMUN_TURNS_TO_WIN
            }
        }

        private void setToken(int column, int row, Ellipse tempTappedItem)
        {
            tempTappedItem.Fill = getActualTurn(actualTurn);            
            logicBoard[row, column] = actualTurn % 2 == 0 ? 'x' : 'o';
        }

        private bool isPositionValid(int column, int row)
        {
            return logicBoard[row,column] == ' ';            
        }

        private bool canSomeoneWin()
        {
            return actualTurn >= GameLogic.MINIMUN_TURNS_TO_WIN;
        }

        void startNewGame(object sender, GestureEventArgs e)
        {
            MessageBoxResult result;
            String message;
            if (isWinner || actualTurn == GameLogic.MAX_TURNS)
            {
                message = "Do you want to play again?";
            }
            else
            {
                message = "Do you want to restart the current game?";
            }

            result = MessageBox.Show(message, "TicTacToe", MessageBoxButton.OKCancel);            
            if (MessageBoxResult.OK == result)
            {
                restartGame();
            }
        }

        private void restartGame()
        {
            actualTurn = 0;
            isWinner = false;
            gridBoard.Children.Clear();
            setEllipses();
            tokenActualPlayer.Fill = getActualTurn(actualTurn);
        }
    }
}
