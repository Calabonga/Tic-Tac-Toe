using System;
using System.Collections.Generic;
using System.Linq;

namespace Calabonga.TicTac.Web
{
    public class GameInfo
    {
        private readonly List<int> _p1Cells = new List<int>();
        private readonly List<int> _p2Cells = new List<int>();

        public DateTime StartedAt { get; set; }

        public ConnectedUser Player1 { get; set; }

        public ConnectedUser Player2 { get; set; }

        public string Winner { get; set; }

        public bool Finished
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Winner);
            }
        }

        public NextMove CanMove(int index, int value)
        {
            var currentCells = value == 1 ? _p1Cells : _p2Cells;

            currentCells.Add(index);

            var isWinner =
                (from first in currentCells
                 from second in currentCells.Where(o => o > first)
                 from third in currentCells.Where(o => o > second)
                 where (third - second) == (second - first) &&
                       (third / 3 - second / 3) == (second / 3 - first / 3)
                 select first).Any();

            if (isWinner) return new NextMove { CanMove = false, HasWinner = true };

            var result = _p1Cells.Count + _p2Cells.Count <= 8;
            return new NextMove { CanMove = result, HasWinner = false };
        }
    }

    public class NextMove
    {
        public bool CanMove { get; set; }

        public bool HasWinner { get; set; }
    }
}