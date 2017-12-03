using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker_Server_v1
{
    public class Player
    {
        public string name;
        public int score;

        public Player (string name , int score)
        {
            this.name = name;
            this.score = score;
        }

        public int Score
        {
            get
            {
                return score;
            }

        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }


    }
}
