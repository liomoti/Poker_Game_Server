using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker_Server_v1
{
    class Card
    {

      public enum Suit 
        {
            heart,
            spade,
            diamond,
            club
        }
        public enum Number : int
        {
            Ace = 1, //14???
            Two = 2,
            Three = 3,
            Four = 4,
            Five = 5,
            Six = 6,
            Seven = 7,
            Eight = 8,
            Nine = 9,
            Ten = 10,
            Jack = 11,
            Queen = 12,
            King = 13,
        }
         public Suit suit;
         public Number number;

         public Card(Suit suit, Number number)
          {
              this.suit = suit;
              this.number = number;
          }

         public override string ToString()
         {
             return
              this.number.ToString() + " " + this.suit.ToString();
         }
    }
}
