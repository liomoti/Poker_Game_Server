using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker_Server_v1
{
   static class Hand
    {
        public static string[] cardsArray ;
        public static string[] cardsSuiteArray ;

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~Calculate Hand~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public static string CalculateWin(string p1Name, string cardA, string p2Name, string cardB, string tableCards)
        {
            int hand1 = CalculateHand(cardA, tableCards);
            int hand2 = CalculateHand(cardB, tableCards);
            if (hand1 > hand2)
                return "1";
            else if (hand1 < hand2)
                return "2";
            else if (hand1 == hand2)
                return "DRAW";
            else
            return "error";
        }

        public static string getRankOfHand(string cardA, string cardB, string tableCards)
        {
            string[] HandStrangth = new string[11];
            HandStrangth[1] = "High Card";
            HandStrangth[2] = "Pair";
            HandStrangth[3] = "Two Pairs";
            HandStrangth[4] = "Three Of A Kind";
            HandStrangth[5] = "Straight";
            HandStrangth[6] = "Flush";
            HandStrangth[7] = "FullHouse";
            HandStrangth[8] = "Four Of A Kind";
            HandStrangth[9] = "Straight Flush";
            HandStrangth[10] = "Royal Straight Flush";
            int hand1 = CalculateHand(cardA, tableCards);
            int hand2 = CalculateHand(cardB, tableCards);

            if (hand1 > hand2)
                return HandStrangth[hand1];
            else if (hand1 < hand2)
                return HandStrangth[hand2];
            return HandStrangth[hand1];
        }

        //the func get 7 cards
        //the func return rank of the hand in this order:
        /*
         * 10=RoyalStraightFlush
         * 9=straightFlush
         * 8=fourOfAKind
         * 7=fullHouse
         * 6=Flush
         * 5=Straight
         * 4=threeOfAKind
         * 3=twoPairs
         * 2=Pair
         * 1= there is no hand!
         */
        public static int CalculateHand(string cardP, string tableCards)
        {
            //string alldata = cardP + "-" + tableCards;
            string alldata = string.Concat(cardP, "-", tableCards);
            cardsArray = alldata.Split('-');
            cardsSuiteArray = alldata.Split('-');
            AddNumToCards();
            AddSuiteToCards();

            if (RoyalStraightFlush())
                return 10;
            else if (StraightFlush())
                return 09;
            else if (FourOfAKind())
                return 08;
            else if (FullHouse())
                return 07;
            else if (Flush())
                return 06;
            else if (Straight())
                return 05;
            else if (ThreeOfAKind())
                return 04;
            else if (TwoPairs())
                return 03;
            else if (Pair())
                return 02;
            else
                return 01;
                

        }

        //the func get string represent 7 cards,
        //the func adding the number of the cards as the first char of each card
        public static void AddNumToCards()
        {
            for (int i = 0; i < 7; i++)
            {
                if (cardsArray[i].Substring(0, 3) == "Ace")   //------>>>ace
                    cardsArray[i] = string.Concat("01", cardsArray[i]);
                if (cardsArray[i].Substring(0, 3) == "Two")   //------>>>two
                    cardsArray[i] = string.Concat("02", cardsArray[i]);
                if (cardsArray[i].Substring(0, 5) == "Three")  //------>>>three
                    cardsArray[i] = string.Concat("03", cardsArray[i]);
                if (cardsArray[i].Substring(0, 4) == "Four")   //------>>>four
                    cardsArray[i] = string.Concat("04", cardsArray[i]);
                if (cardsArray[i].Substring(0, 4) == "Five")    //------>>>five
                    cardsArray[i] = string.Concat("05", cardsArray[i]);
                if (cardsArray[i].Substring(0, 3) == "Six")  //------>>>six
                    cardsArray[i] = string.Concat("06", cardsArray[i]);
                if (cardsArray[i].Substring(0, 5) == "Seven")    //------>>>seven
                    cardsArray[i] = string.Concat("07", cardsArray[i]);
                if (cardsArray[i].Substring(0, 5) == "Eight")    //------>>>eight
                    cardsArray[i] = string.Concat("08", cardsArray[i]);
                if (cardsArray[i].Substring(0, 4) == "Nine")    //------>>>nine
                    cardsArray[i] = string.Concat("09", cardsArray[i]);
                if (cardsArray[i].Substring(0, 3) == "Ten")     //------>>>ten
                    cardsArray[i] = string.Concat("10", cardsArray[i]);
                if (cardsArray[i].Substring(0, 4) == "Jack")     //------>>>jack
                    cardsArray[i] = string.Concat("11", cardsArray[i]);
                if (cardsArray[i].Substring(0, 5) == "Queen")    //------>>>queen
                    cardsArray[i] = string.Concat("12", cardsArray[i]);
                if (cardsArray[i].Substring(0, 4) == "King")    //------>>>king
                    cardsArray[i] = string.Concat("13", cardsArray[i]);
            }
        }
        //the func get string represent 7 cards,
        //the func return the high card
        //example: twoClub-fiveDiamond-kingDiamond-sixHeart-queenSpade-fourHeart-threeDiamond
        public static int HighCard()
        {
            int[] newcards = new int[14]; //int array
            for (int i = 0; i < 7; i++)
            {
                newcards[int.Parse(cardsArray[i].Substring(0, 2))]++; //filling the array
            }
            if (newcards[1] == 1) //if we have Ace 
                return 14;
            else //if we dont have Ace
            {
                for (int i = 13; i > 1; i--)
                {
                    if (newcards[i] > 0) //return the high card, from king to 2
                    {
                        return i;
                    }
                }
            }
            return 0;
        }
        //the func get string represent 7 cards,
        //the func return true if we have pair
        public static bool Pair()
        {
            int[] newcards = new int[14]; //int array
            for (int i = 0; i < 7; i++)
            {
                newcards[int.Parse(cardsArray[i].Substring(0, 2))]++; //filling the array
            }
            for (int i = 1; i < 14; i++)
            {
                if (newcards[i] >= 2)
                    return true;
            }
            return false;
        }
        //the func get string represent 7 cards,
        //the func return true if we have 2 pairs
        public static bool TwoPairs()
        {
            int counter = 0;
            int[] newcards = new int[14]; //int array
            for (int i = 0; i < 7; i++)
            {
                newcards[int.Parse(cardsArray[i].Substring(0, 2))]++; //filling the array
            }
            for (int i = 1; i < 14; i++)
            {
                if (newcards[i] >= 2)
                    counter++; //counting pairs
            }
            if (counter >= 2) //if we found more than 1 pair
                return true;
            return false;
        }
        //the func get string represent 7 cards,
        //the func return true if we have 3 of a kind
        public static bool ThreeOfAKind()
        {
            int counter = 0;
            int[] newcards = new int[14]; //int array
            for (int i = 0; i < 7; i++)
            {
                newcards[int.Parse(cardsArray[i].Substring(0, 2))]++; //filling the array
            }
            for (int i = 1; i < 14; i++)
            {
                if (newcards[i] == 3)
                    counter++; //counting 3 of a kind
            }
            if (counter != 0) //if we found 3 of a kind
                return true;
            return false;
        }
        //the func get string represent 7 cards,
        //the func return true if we have 4 of a kind
        public static bool FourOfAKind()
        {
            int counter = 0;
            int[] newcards = new int[14]; //int array
            for (int i = 0; i < 7; i++)
            {
                newcards[int.Parse(cardsArray[i].Substring(0, 2))]++; //filling the array
            }
            for (int i = 1; i < 14; i++)
            {
                if (newcards[i] == 4)
                    counter++; //counting 4 of a kind
            }
            if (counter != 0) //if we found 4 of a kind
                return true;
            return false;
        }
        //the func get string represent 7 cards,
        //the func return true if we have FullHouse
        public static bool FullHouse()
        {
            int counter2 = 0, counter3 = 0;
            int[] newcards = new int[14]; //int array
            for (int i = 0; i < 7; i++)
            {
                newcards[int.Parse(cardsArray[i].Substring(0, 2))]++; //filling the array
            }
            for (int i = 1; i < 14; i++)
            {
                if (newcards[i] >= 3)
                {
                    counter3++; //counting 3 
                }
                if (newcards[i] == 2)
                {
                    counter2++; //counting pair
                }
            }
            if ((counter2 != 0 && counter3 != 0) || counter3 > 1) //if we found FullHouse
                return true;
            return false;
        }
        //the func get string represent 7 cards,
        //the func adding the suite of the cards as the first char of each card
        public static void AddSuiteToCards()
        {
            for (int i = 0; i < 7; i++)
            {
                if (cardsSuiteArray[i].Substring(0, 3) == "Ace")   //------>>>ace
                    cardsSuiteArray[i] = string.Concat(cardsSuiteArray[i].Substring(4, 1), cardsSuiteArray[i]);
                if (cardsSuiteArray[i].Substring(0, 3) == "Two")   //------>>>two
                    cardsSuiteArray[i] = string.Concat(cardsSuiteArray[i].Substring(4, 1), cardsSuiteArray[i]);
                if (cardsSuiteArray[i].Substring(0, 5) == "Three")  //------>>>three
                    cardsSuiteArray[i] = string.Concat(cardsSuiteArray[i].Substring(6, 1), cardsSuiteArray[i]);
                if (cardsSuiteArray[i].Substring(0, 4) == "Four")   //------>>>four
                    cardsSuiteArray[i] = string.Concat(cardsSuiteArray[i].Substring(5, 1), cardsSuiteArray[i]);
                if (cardsSuiteArray[i].Substring(0, 4) == "Five")    //------>>>five
                    cardsSuiteArray[i] = string.Concat(cardsSuiteArray[i].Substring(5, 1), cardsSuiteArray[i]);
                if (cardsSuiteArray[i].Substring(0, 3) == "Six")  //------>>>six
                    cardsSuiteArray[i] = string.Concat(cardsSuiteArray[i].Substring(4, 1), cardsSuiteArray[i]);
                if (cardsSuiteArray[i].Substring(0, 5) == "Seven")    //------>>>seven
                    cardsSuiteArray[i] = string.Concat(cardsSuiteArray[i].Substring(6, 1), cardsSuiteArray[i]);
                if (cardsSuiteArray[i].Substring(0, 5) == "Eight")    //------>>>eight
                    cardsSuiteArray[i] = string.Concat(cardsSuiteArray[i].Substring(6, 1), cardsSuiteArray[i]);
                if (cardsSuiteArray[i].Substring(0, 4) == "Nine")    //------>>>nine
                    cardsSuiteArray[i] = string.Concat(cardsSuiteArray[i].Substring(5, 1), cardsSuiteArray[i]);
                if (cardsSuiteArray[i].Substring(0, 3) == "Ten")     //------>>>ten
                    cardsSuiteArray[i] = string.Concat(cardsSuiteArray[i].Substring(4, 1), cardsSuiteArray[i]);
                if (cardsSuiteArray[i].Substring(0, 4) == "Jack")     //------>>>jack
                    cardsSuiteArray[i] = string.Concat(cardsSuiteArray[i].Substring(5, 1), cardsSuiteArray[i]);
                if (cardsSuiteArray[i].Substring(0, 5) == "Queen")    //------>>>queen
                    cardsSuiteArray[i] = string.Concat(cardsSuiteArray[i].Substring(6, 1), cardsSuiteArray[i]);
                if (cardsSuiteArray[i].Substring(0, 4) == "King")    //------>>>king
                    cardsSuiteArray[i] = string.Concat(cardsSuiteArray[i].Substring(5, 1), cardsSuiteArray[i]);
            }
        }
        //the func get string represent 7 cards,
        //the func return true if we have Flush
        public static bool Flush()
        {
            int cntClub = 0, cntDiamond = 0, cntHeart = 0, cntSpade = 0;
            for (int i = 0; i < 7; i++)
            {
                if (cardsSuiteArray[i].Substring(0, 1) == "d")
                    cntDiamond++;
                if (cardsSuiteArray[i].Substring(0, 1) == "c")
                    cntClub++;
                if (cardsSuiteArray[i].Substring(0, 1) == "h")
                    cntHeart++;
                if (cardsSuiteArray[i].Substring(0, 1) == "s")
                    cntSpade++;
            }
            if (cntDiamond == 5 || cntClub == 5 || cntHeart == 5 || cntSpade == 5) //if we found Flush!
                return true;
            return false;
        }
        //the func get string represent 7 cards,
        //the func return true if we have Straight
        public static bool Straight()
        {
            int counter = 1, counterSt = 0;
            int[] newcards = new int[14]; //int array
            for (int i = 0; i < 7; i++)
            {
                newcards[int.Parse(cardsArray[i].Substring(0, 2))] = int.Parse(cardsArray[i].Substring(0, 2)); //filling the array
            }
            for (int i = 2; i < 14; i++)
            {
                if (newcards[i - 1] + 1 == (newcards[i])) //if the current card and the last are Followers cards
                {
                    counter++;
                    if (counter == 5) //we found Straight!
                        counterSt++;
                }
                else
                    counter = 1; //reset the counter
            }
            if (counterSt > 0) //if we found Straight
                return true;
            return false;
        }
        //the func get string represent 7 cards,
        //the func return true if we have StraightFlush
        public static bool StraightFlush()
        {
            int counter = 1, counterStS = 0;
            int[] newcards = new int[14]; //int array for the numbers
            string[] newsuite = new string[14]; //string array for suites
            for (int i = 0; i < 7; i++)
            {
                newcards[int.Parse(cardsArray[i].Substring(0, 2))] = int.Parse(cardsArray[i].Substring(0, 2)); //filling the array
                newsuite[int.Parse(cardsArray[i].Substring(0, 2))] = cardsSuiteArray[i].Substring(cardsSuiteArray[i].Length - 1, 1);
                //newsuite key: b=Club d=Diamond t=Heart e=Spade
            }
            for (int i = 2; i < 14; i++)
            {
                if (newcards[i - 1] + 1 == newcards[i] && (newsuite[i - 1] == newsuite[i])) //if the current card and the last are Followers cards and have the same suite
                {
                    counter++;
                    if (counter == 5) //we found StraightFlush!
                        counterStS++;
                }
                else
                    counter = 1; //reset the counter
            }
            if (counterStS > 0) //if we found StraightFlush
                return true;
            return false;
        }
        //the func get string represent 7 cards,
        //the func return true if we have RoyalStraightFlush
        public static bool RoyalStraightFlush()
        {
            bool flag = true;
            int counter = 1, counterStS = 0, counterRSF = 0;
            int[] newcards = new int[14]; //int array for the numbers
            string[] newsuite = new string[14]; //string array for suites
            for (int i = 0; i < 7; i++)
            {
                newcards[int.Parse(cardsArray[i].Substring(0, 2))] = int.Parse(cardsArray[i].Substring(0, 2)); //filling the array
                newsuite[int.Parse(cardsArray[i].Substring(0, 2))] = cardsSuiteArray[i].Substring(cardsSuiteArray[i].Length - 1, 1);
                //newsuite key: b=Club d=Diamond t=Heart e=Spade
            }
            for (int i = 2; i < 14; i++)
            {
                if (newcards[i - 1] + 1 == newcards[i] && (newsuite[i - 1] == newsuite[i])) //if the current card and the last are Followers cards and have the same suite
                {
                    if (flag)
                    {
                        counterRSF = newcards[i - 1];
                        flag = false;
                    }
                    counter++;
                    counterRSF += newcards[i]; //checking sum of the cards for Royal
                    if (counter == 5 && counterRSF == 55) //we found RoyalStraightFlush!
                    {
                        counterStS++;
                    }
                }
                else
                {
                    counter = 1; //reset the counter
                    counterRSF = 0; //reset
                    flag = true;
                }
            }
            if (counterStS > 0) //if we found RoyalStraightFlush
                return true;
            return false;
        }
    }
}
