using System;

namespace Tic_tac_toe
{
    public class Gamestate
    {
        public Player[,] GameGrid { get; private set; }
        public Player CurrentPlayer { get; private set; }
        public int TurnsPassed { get; private set; }
        public bool GameOver { get; private set; }

        public event Action<int, int> MoveMade;
        public event Action<Gameresult> GameEnded;
        public event Action GameRestarted;

        public Gamestate()
        {
            GameGrid = new Player[3, 3];
            CurrentPlayer = Player.X;
            TurnsPassed = 0;
            GameOver = false;
        }
        private bool CanMakeMove(int r, int c)
        {
            return !GameOver && GameGrid[r, c] == Player.None;
        }

        private bool IsGridFull()
        {
            return TurnsPassed == 9;
        }

        private void SwitchPlayer()
        {
            CurrentPlayer = CurrentPlayer == Player.X ? Player.O : Player.X;
        }

        private bool AreSquaresMarked((int, int)[] squares, Player player)
        {
            foreach((int r, int c) in squares)
            {
                if (GameGrid[r, c] != player)
                {
                    return false;
                }
            } 
            return true;
        }


        private bool DidMoveWin(int r, int c, out Wininfo? winInfo)
        {
            (int, int)[] row = new[] { (r, 0), (r, 1), (r, 2) };
            (int, int)[] col = new[] { (0, c), (1, c), (2, c) };
            (int, int)[] mainDiag = new[] { (0, 0), (1, 1), (2, 2) };
            (int, int)[] antiDiag = new[] { (0, 2), (1, 1), (2, 0) };

            if (AreSquaresMarked(row, CurrentPlayer))
            {
                winInfo = new Wininfo { Type = Win.Row, Number = r };
                return true;
            } 
            if (AreSquaresMarked(col, CurrentPlayer))
            {
                winInfo = new Wininfo { Type = Win.Column, Number = c };
                return true;
            }

            if (AreSquaresMarked(mainDiag, CurrentPlayer))
            {
                winInfo = new Wininfo { Type = Win.AntiDiagonal };
                return true;
            }

            winInfo = null;
            return false;
        }

        private bool DidMoveEndGame(int r, int c, out Gameresult? gameResult)
        {
            if (DidMoveWin(r, c, out Wininfo winInfo))
            {
                gameResult = new Gameresult { Winner = CurrentPlayer, Wininfo = winInfo };
                return true;
            }

            if (IsGridFull())
            {
                gameResult = new Gameresult { Winner = Player.None };
                return true;
            }

            gameResult = null;
            return false;
        }

        public void MakeMove(int r, int c)
        {
            if (!CanMakeMove(r, c))
            {
                return;
            }

            GameGrid[r, c] = CurrentPlayer;
            TurnsPassed++;

            if(DidMoveEndGame(r, c, out Gameresult gameResult))
            {
                GameOver = true;
                MoveMade?.Invoke(r, c);
                GameEnded?.Invoke(gameResult);

            }
            else
            {
                SwitchPlayer();
                MoveMade?.Invoke(r, c);
            }
        }

        public void Reset()
        {
            GameGrid = new Player[3, 3];
            CurrentPlayer = Player.X;
            TurnsPassed = 0;
            GameOver = false;
            GameRestarted?.Invoke();
        }
    }
}
