using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker_Server_v1
{
   class GameDealer
    {
        private string player1 ="no_name";
        private string player2 ="no_name";
        public string player1Bet = "no_bet";
        public string player2Bet = "no_bet";
        public string player1RaiseScore = "no_bet";
        public string player2RaiseScore = "no_bet";
        public string player1Pot = "1000";
        public string player2Pot = "1000";
        public string tablePot = "0";
        public string winPlayer = "no_name";
        public string winHandRank = "no_rank";
        public string gameEnd = "notEndGame";
        public string newGamePlayer1 = "no_answer"; //the game end. and the user want to play again
        public string newGamePlayer2 = "no_answer"; //the game end. and the user want to play again
        public bool player1Start = false;
        public bool player2Start = false; 
        public bool createCards = true;
        public Card p1c1, p1c2, p2c1, p2c2;
        public Card c3, c4, c5, c6, c7;

        public string playerOne
    {
        get
        {
            return player1;
        }
        set
        {
            player1 = value;
        }
    }
        public string playerTwo
    {
        get
        {
            return player2;
        }
        set
        {
            player2 = value;
        }
    }
        public string playerOneBet
        {
            get
            {
                return player1Bet;
            }
            set
            {
                player1Bet = value;
            }
        }
        public string playerTwoBet
        {
            get
            {
                return player2Bet;
            }
            set
            {
                player2Bet = value;
            }
        }
        public string getCardsArray(int player)
        {
            string cardsArray = "";
            if (player == 1)
            {
                cardsArray = p1c1.ToString() + "-"+ p1c2.ToString();

            }
            else
            {
                cardsArray = p2c1.ToString() + "-" + p2c2.ToString();
            }
            return cardsArray;
        }
        public string tableCardsArray()
        {
            string tableCardsArray = "";
            tableCardsArray = c3.ToString() + "-" + c4.ToString() + "-" + c5.ToString() + "-" + c6.ToString() + "-" + c7.ToString();
            return tableCardsArray;
        }

 
    }
}
