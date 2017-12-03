using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker_Server_v1
{
    class Deck 
    {
        
        public Card[] deck = new Card[52];

        public  Card getCardOut()
        {
            Random rnd = new Random();
            int drawCard;
            Card randomCard;
            drawCard = rnd.Next(0,52);

            if (deck[drawCard] == null)
            {
                while (deck[drawCard] == null)
                {
                    drawCard = rnd.Next(0, 52);
                    randomCard = deck[drawCard];
                }
            }

            randomCard = deck[drawCard];
            deck[drawCard] = null;

            return randomCard;

        }
        public Deck()
        {
        //  Card[] deck = new Card[52];
                     //CLUB
           Card aceClub = new Card(Card.Suit.club, Card.Number.Ace);
           deck[0] = aceClub;

           Card twoClub = new Card(Card.Suit.club, Card.Number.Two);
           deck[1] = twoClub;

           Card threeClub = new Card(Card.Suit.club, Card.Number.Three);
           deck[2] = threeClub;

           Card fourClub = new Card(Card.Suit.club, Card.Number.Four);
           deck[3] = fourClub;

           Card fiveClub = new Card(Card.Suit.club, Card.Number.Five);
           deck[4] = fiveClub;

           Card sixClub = new Card(Card.Suit.club, Card.Number.Six);
           deck[5] = sixClub;

           Card sevenClub = new Card(Card.Suit.club, Card.Number.Seven);
           deck[6] = sevenClub;

           Card eightClub = new Card(Card.Suit.club, Card.Number.Eight);
           deck[7] = eightClub;

           Card nineClub = new Card(Card.Suit.club, Card.Number.Nine);
           deck[8] = nineClub;

           Card tenClub = new Card(Card.Suit.club, Card.Number.Ten);
           deck[9] = tenClub;

           Card jackClub = new Card(Card.Suit.club, Card.Number.Jack);
           deck[10] = jackClub;

           Card queenClub = new Card(Card.Suit.club, Card.Number.Queen);
           deck[11] = queenClub;

           Card kingClub = new Card(Card.Suit.club, Card.Number.King);
           deck[12] = kingClub;

                                //diamond
           Card aceDiamond = new Card(Card.Suit.diamond, Card.Number.Ace);
           deck[13] = aceDiamond;

           Card twoDiamond = new Card(Card.Suit.diamond, Card.Number.Two);
           deck[14] = twoDiamond;

           Card threeDiamond = new Card(Card.Suit.diamond, Card.Number.Three);
           deck[15] = threeDiamond;

           Card fourDiamond = new Card(Card.Suit.diamond, Card.Number.Four);
           deck[16] = fourDiamond;

           Card fiveDiamond = new Card(Card.Suit.diamond, Card.Number.Five);
           deck[17] = fiveDiamond;

           Card sixDiamond = new Card(Card.Suit.diamond, Card.Number.Six);
           deck[18] = sixDiamond;

           Card sevenDiamond = new Card(Card.Suit.diamond, Card.Number.Seven);
           deck[19] = sevenDiamond;

           Card eightDiamond = new Card(Card.Suit.diamond, Card.Number.Eight);
           deck[20] = eightDiamond;

           Card nineDiamond = new Card(Card.Suit.diamond, Card.Number.Nine);
           deck[21] = nineDiamond;

           Card tenDiamond = new Card(Card.Suit.diamond, Card.Number.Ten);
           deck[22] = tenDiamond;

           Card jackDiamond = new Card(Card.Suit.diamond, Card.Number.Jack);
           deck[23] = jackDiamond;

           Card queenDiamond = new Card(Card.Suit.diamond, Card.Number.Queen);
           deck[24] = queenDiamond;

           Card kingDiamond = new Card(Card.Suit.diamond, Card.Number.King);
           deck[25] = kingDiamond;


                                       // heart
           Card aceHeart = new Card(Card.Suit.heart, Card.Number.Ace);
           deck[26] = aceHeart;

           Card twoHeart = new Card(Card.Suit.heart, Card.Number.Two);
           deck[27] = twoHeart;

           Card threeHeart = new Card(Card.Suit.heart, Card.Number.Three);
           deck[28] = threeHeart;

           Card fourHeart = new Card(Card.Suit.heart, Card.Number.Four);
           deck[29] = fourHeart;

           Card fiveHeart = new Card(Card.Suit.heart, Card.Number.Five);
           deck[30] = fiveHeart;

           Card sixHeart = new Card(Card.Suit.heart, Card.Number.Six);
           deck[31] = sixHeart;

           Card sevenHeart = new Card(Card.Suit.heart, Card.Number.Seven);
           deck[32] = sevenHeart;

           Card eightHeart = new Card(Card.Suit.heart, Card.Number.Eight);
           deck[33] = eightHeart;

           Card nineHeart = new Card(Card.Suit.heart, Card.Number.Nine);
           deck[34] = nineHeart;

           Card tenHeart = new Card(Card.Suit.heart, Card.Number.Ten);
           deck[35] = tenHeart;

           Card jackHeart = new Card(Card.Suit.heart, Card.Number.Jack);
           deck[36] = jackHeart;

           Card queenHeart = new Card(Card.Suit.heart, Card.Number.Queen);
           deck[37] = queenHeart;

           Card kingHeart = new Card(Card.Suit.heart, Card.Number.King);
           deck[38] = kingHeart;


                  // spade
           Card aceSpade = new Card(Card.Suit.spade, Card.Number.Ace);
           deck[39] = aceSpade;

           Card twoSpade = new Card(Card.Suit.spade, Card.Number.Two);
           deck[40] = twoSpade;

           Card threeSpade = new Card(Card.Suit.spade, Card.Number.Three);
           deck[41] = threeSpade;

           Card fourSpade = new Card(Card.Suit.spade, Card.Number.Four);
           deck[42] = fourSpade;

           Card fiveSpade = new Card(Card.Suit.spade, Card.Number.Five);
           deck[43] = fiveSpade;

           Card sixSpade = new Card(Card.Suit.spade, Card.Number.Six);
           deck[44] = sixSpade;

           Card sevenSpade = new Card(Card.Suit.spade, Card.Number.Seven);
           deck[45] = sevenSpade;

           Card eightSpade = new Card(Card.Suit.spade, Card.Number.Eight);
           deck[46] = eightSpade;

           Card nineSpade = new Card(Card.Suit.spade, Card.Number.Nine);
           deck[47] = nineSpade;

           Card tenSpade = new Card(Card.Suit.spade, Card.Number.Ten);
           deck[48] = tenSpade;

           Card jackSpade = new Card(Card.Suit.spade, Card.Number.Jack);
           deck[49] = jackSpade;

           Card queenSpade = new Card(Card.Suit.spade, Card.Number.Queen);
           deck[50] = queenSpade;

           Card kingSpade = new Card(Card.Suit.spade, Card.Number.King);
           deck[51] = kingSpade;

        }



    }
}
