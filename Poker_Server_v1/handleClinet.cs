using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;


namespace Poker_Server_v1
{
    class HandleClient
    {
        public TcpClient clientSocket;
        public GameDealer gamed;
        public string clNo; //number of the client
        private string serverResponse = null;

        public void startClient(TcpClient inClientSocket, string clineNo, GameDealer gamed)
        {
            this.clientSocket = inClientSocket;
            this.clNo = clineNo;
            this.gamed = gamed;
            //Thread ctThread = new Thread(doChat);
            if (clineNo == "1")
            {
                Thread ctThread1 = new Thread(() => startGame1(gamed));
                ctThread1.Start();
            }
            else if (clineNo == "2")
            {
                Thread ctThread2 = new Thread(() => startGame2(gamed));
                ctThread2.Start();
                
            }
            
            else
            {
                Console.WriteLine("2 clients allready connected, please reset the server");
            }
            
        }      
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void startGame1(GameDealer gamed)
        {
            bool flag = true;
            int requestCount = 0;
            byte[] bytesFrom = new byte[10025];
            string dataFromClient = null;
            Byte[] sendBytes = null;
           
            //string rCount = null;
            requestCount = 0;

            string[] ClientMessage = new string[1];

            while ((flag))
            {
                try
                {
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                    dataFromClient = readFromClient(bytesFrom, networkStream);
                    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                    ClientMessage = dataFromClient.Split('-');
                    if (ClientMessage[0] == "name")
                    {
                        gamed.playerOne = ClientMessage[1];
                    }

                    while (gamed.playerTwo.ToString() == "no_name") ; //stay here while player2 didnt change his name


                    Console.WriteLine(" Client1>> " + "client " + clNo + " name is: " + ClientMessage[1]);

                    serverResponse = "READY-" + gamed.playerTwo.ToString(); //send ready to the client
                    writeToClient(networkStream, sendBytes, serverResponse);
                    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~Console Output~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                    Console.WriteLine(" Client1>> Client1 get the message: " + serverResponse ); 
           
                    //waiting for answer "start game" from client1:
                    dataFromClient = readFromClient(bytesFrom, networkStream);
                    gamed.player1Start = true;
                    while (gamed.player2Start == false) ; //stay here while player2 clicked on start button
                    Console.WriteLine(" Client1>>  cliked on Start Button");
                    serverResponse = "START-p1-";
                    writeToClient(networkStream, sendBytes, serverResponse); //send start to client
                    //delay
                    var t = Task.Run(async delegate
                    {
                        await Task.Delay(1000);
                    });
                    t.Wait();
                                                                                   //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
             label1:
                    sharePlayerCards(networkStream, sendBytes, "1"); //the func send 7 to the client
            
                    //now we geting player1 bet ~~~~~~~~~~~Round 1~~~~~~~~~~~~~~~~~~
                    dataFromClient = readFromClient(bytesFrom, networkStream); //get the bet from player 1 (check,fold,raise)
                    string[] dataparts = dataFromClient.Split('-');
                    if (dataparts[0] == "RAISE")
                    {
                        gamed.player1Bet = dataparts[0];  //saving the bet of player 1 (check,call,fold,raise)
                        gamed.player1RaiseScore = dataparts[1];  //saving the bet value of player 1
                        gamed.player1Pot = dataparts[2];  //saving the pot of player 1
                        gamed.tablePot = (Int32.Parse(gamed.tablePot) + Int32.Parse(gamed.player1RaiseScore)).ToString(); //saving the tablePot
                       // gamed.round = 0;
                        //dataFromClient have 3 parts: 1.RAISE 2.rais sum 3. pot of the player
                        Console.WriteLine(" Client1>> Server Current Data is(player 1 first move) : table pot,player 1 pot,player 2 pot \n" + gamed.tablePot + " " + gamed.player1Pot + " " + gamed.player2Pot);

                    }
                    else if(dataparts[0] == "FOLD")
                    {
                        //update pot ???
                        //we need to send new cards here
                        gamed.player1Bet = dataparts[0];
                        gamed.createCards = true;  //to get new Cards!!~
                        gamed.player2Pot = (Int32.Parse(gamed.tablePot) + Int32.Parse(gamed.player2Pot)).ToString();  
                        gamed.tablePot = "0";
                        goto label1;
                    }
                    else if (dataparts[0] == "CHECK")
                    {
                        gamed.player1Bet = dataFromClient;  //saving the bet
                        Console.WriteLine(" Client1>> Server Current Data is(player 1 first move) : table pot,player 1 pot,player 2 pot \n" + gamed.tablePot + " " + gamed.player1Pot + " " + gamed.player2Pot);
                    }

                                  //////wait for player 2 move/////////
                    while (gamed.player2Bet.ToString() == "no_bet") ; //wait for player 2 move
                    Console.WriteLine(" Client1>> the bet of player2 is:" + gamed.player2Bet);

                              //FIRST Round #player 2 turn (he alredy played)
                    if (gamed.player2Bet.ToString() == "CHECK")
                    {
                        serverResponse = "1-" + "CHECK-" + "FLOP" + "-$";
                        Console.WriteLine(" Client1>> get Check from player 2 And nowFlop...");
                        writeToClient(networkStream, sendBytes, serverResponse);
                        gamed.player1Bet = "no_bet";
                        gamed.player2Bet = "no_bet";
                    }
                    else if(gamed.player2Bet.ToString() == "FOLD")
                    {       
                        serverResponse = gamed.player2Bet + "-1" + "-$";
                        writeToClient(networkStream, sendBytes, serverResponse);
                        Thread.Sleep(200);
                        gamed.player2Bet = "no_bet";
                        gamed.tablePot = "0";
                        goto label1;
                    }
                    else if (gamed.player2Bet.ToString() == "CALL")
                    {
                        serverResponse = "1-" + "CALL-" +  "FLOP" + "-" + gamed.player2Pot + "-" + gamed.player1RaiseScore +  "-$";
                        Console.WriteLine(" Client1>> get CALL from player 2 and Now Flop... + pot of player2 is: " + gamed.player2Pot + "the raise of player 1 was :" + gamed.player1RaiseScore);
                        writeToClient(networkStream, sendBytes, serverResponse);
                        gamed.player1Bet = "no_bet";
                        gamed.player2Bet = "no_bet";
                        gamed.player1RaiseScore = "no_bet"; 
                    }
                    else if (gamed.player2Bet.ToString() == "RAISE")
                    {
                            
                    }
                    /********************************************************************************
                     * ******************************************************************************
                     * ******************************************************************************/
                     
                    ////////////////////#FLOP-End  ----> TURN now!!! New Round#//////////////
                     dataFromClient = readFromClient(bytesFrom, networkStream); //get the bet from player 1 (check,fold,raise)
                     dataparts = dataFromClient.Split('-');
                    if (dataparts[0] == "RAISE")
                    {
                        gamed.player1Bet = dataparts[0];  //saving the bet of player 1 (check,call,fold,raise)
                        gamed.player1RaiseScore = dataparts[1];  //saving the bet value of player 1
                        gamed.player1Pot = dataparts[2];  //saving the pot of player 1
                        gamed.tablePot = (Int32.Parse(gamed.tablePot) + Int32.Parse(gamed.player1RaiseScore)).ToString();
                        Console.WriteLine(" Client1>> Server Current Data is(Second move) : table pot,player 1 pot,player 2 pot \n" + gamed.tablePot + " " + gamed.player1Pot + " " + gamed.player2Pot);

                    }
                    else if (dataparts[0] == "FOLD")
                    {
                        gamed.player1Bet = dataparts[0];
                        gamed.player2Pot = (Int32.Parse(gamed.tablePot) + Int32.Parse(gamed.player2Pot)).ToString();
                        gamed.tablePot = "0";
                        gamed.createCards = true;  //to get new Cards!!~
                        goto label1;
                    }
                    else if (dataparts[0] == "CHECK")
                    {
                        gamed.player1Bet = dataFromClient;  //saving the bet
                        Console.WriteLine(" Client1>> Server Current Data is(player 1 second move) : table pot,player 1 pot,player 2 pot \n" + gamed.tablePot + " " + gamed.player1Pot + " " + gamed.player2Pot);
                    }
                                //////wait for player 2 move/////////
                    while (gamed.player2Bet.ToString() == "no_bet") ; //wait for player 2 move
                    Console.WriteLine(" Client1>> the bet of player2 is:" + gamed.player2Bet);

                    //Second Round #player 2 turn (he alredy played)
                    if (gamed.player2Bet.ToString() == "CHECK")
                    {
                        serverResponse = "1-" + "CHECK-" + "TURN" + "-$";
                        Console.WriteLine(" Client1>> sending Turn...");
                        writeToClient(networkStream, sendBytes, serverResponse);
                        gamed.player1Bet = "no_bet";
                        gamed.player2Bet = "no_bet";
                    }
                    else if (gamed.player2Bet.ToString() == "FOLD")
                    {
                        serverResponse = gamed.player2Bet + "-1" + "-$";
                        writeToClient(networkStream, sendBytes, serverResponse);
                        Thread.Sleep(200);
                        gamed.player2Bet = "no_bet";
                        goto label1;
                    }
                    else if (gamed.player2Bet.ToString() == "CALL")
                    {
                        serverResponse = "1-" + "CALL-" + "TURN" + "-" + gamed.player2Pot + "-" + gamed.player1RaiseScore + "-$";
                        Console.WriteLine(" Client1>> sending Turn... + pot of player2 is: " + gamed.player2Pot + "the raise of player 1 was :" + gamed.player1RaiseScore);
                        writeToClient(networkStream, sendBytes, serverResponse);
                        gamed.player1Bet = "no_bet";
                        gamed.player2Bet = "no_bet";
                        gamed.player1RaiseScore = "no_bet"; // need to reset here because we send it to player1 and
                    }

                    /********************************************************************************
 * ******************************************************************************
 * ******************************************************************************/

                    ////////////////////#TURN-End  ----> RIVER now!!! New and Last Round#//////////////
                    dataFromClient = readFromClient(bytesFrom, networkStream); //get the bet from player 1 (check,fold,raise)
                    dataparts = dataFromClient.Split('-');
                    if (dataparts[0] == "RAISE")
                    {
                        gamed.player1Bet = dataparts[0];  //saving the bet of player 1 (check,call,fold,raise)
                        gamed.player1RaiseScore = dataparts[1];  //saving the bet value of player 1
                        gamed.player1Pot = dataparts[2];  //saving the pot of player 1
                        gamed.tablePot = (Int32.Parse(gamed.tablePot) + Int32.Parse(gamed.player1RaiseScore)).ToString();
                        //dataFromClient have 3 parts: 1.RAISE 2.rais sum 3. pot of the player
                        Console.WriteLine(" Client1>> Server Current Data is : table pot,player 1 pot,player 2 pot \n" + gamed.tablePot + " " + gamed.player1Pot + " " + gamed.player2Pot);

                    }
                    else if (dataparts[0] == "FOLD")
                    {
                        gamed.player1Bet = dataparts[0];
                        gamed.player2Pot = (Int32.Parse(gamed.tablePot) + Int32.Parse(gamed.player2Pot)).ToString();
                        gamed.tablePot = "0";
                        gamed.createCards = true;  //to get new Cards!!~
                        goto label1;
                    }
                    else if (dataparts[0] == "CHECK")
                    {
                        gamed.player1Bet = dataFromClient;  //saving the bet
                        Console.WriteLine(" Client1>> Server Current Data is(player 1 thired move) : table pot,player 1 pot,player 2 pot \n" + gamed.tablePot + " " + gamed.player1Pot + " " + gamed.player2Pot);
                    }
                    //////wait for player 2 move/////////
                    while (gamed.player2Bet.ToString() == "no_bet") ; //wait for player 2 move
                    Console.WriteLine(" Client1>> the bet of player2 is:" + gamed.player2Bet);

                    //Second Round #player 2 turn (he alredy played)
                    if (gamed.player2Bet.ToString() == "CHECK")
                    {
                        serverResponse = "1-" + "CHECK-" + "RIVER" + "-$";
                        Console.WriteLine(" Client1>> sending River...");
                        writeToClient(networkStream, sendBytes, serverResponse);
                        gamed.player1Bet = "no_bet";
                        gamed.player2Bet = "no_bet";
                    }
                    else if (gamed.player2Bet.ToString() == "FOLD")
                    {
                        serverResponse = gamed.player2Bet + "-1" + "-$";
                        writeToClient(networkStream, sendBytes, serverResponse);
                        Thread.Sleep(200);
                        gamed.player2Bet = "no_bet";
                        goto label1;
                    }
                    else if (gamed.player2Bet.ToString() == "CALL")
                    {
                        serverResponse = "1-" + "CALL-" + "RIVER" + "-" + gamed.player2Pot + "-" + gamed.player1RaiseScore + "-$";
                        Console.WriteLine(" Client1>> sending River... + pot of player2 is: " + gamed.player2Pot + "the raise of player 1 was :" + gamed.player1RaiseScore);
                        writeToClient(networkStream, sendBytes, serverResponse);
                        gamed.player1Bet = "no_bet";
                        gamed.player2Bet = "no_bet";
                        gamed.player1RaiseScore = "no_bet"; // need to reset here because we send it to player1 and
                    }

                    /********************************************************************************
                     * ******************************************************************************
                     * ******************************************************************************/

                    ////////////////////#RIVER End  ----> Last Bet now!!!/////////////
                    dataFromClient = readFromClient(bytesFrom, networkStream); //get the bet from player 1 (check,fold,raise)
                    dataparts = dataFromClient.Split('-');
                    if (dataparts[0] == "RAISE")
                    {
                        gamed.player1Bet = dataparts[0];  //saving the bet of player 1 (check,call,fold,raise)
                        gamed.player1RaiseScore = dataparts[1];  //saving the bet value of player 1
                        gamed.player1Pot = dataparts[2];  //saving the pot of player 1
                        gamed.tablePot = (Int32.Parse(gamed.tablePot) + Int32.Parse(gamed.player1RaiseScore)).ToString();
                        Console.WriteLine(" Client1>> Server Current Data is : table pot,player 1 pot,player 2 pot \n" + gamed.tablePot + " " + gamed.player1Pot + " " + gamed.player2Pot);

                    }
                    else if (dataparts[0] == "FOLD")
                    {
                        gamed.player1Bet = dataparts[0];
                        gamed.player2Pot = (Int32.Parse(gamed.tablePot) + Int32.Parse(gamed.player2Pot)).ToString();
                        gamed.tablePot = "0";
                        gamed.createCards = true;  //to get new Cards!!~
                        goto label1;
                    }
                    else if (dataparts[0] == "CHECK")
                    {
                        gamed.player1Bet = dataFromClient;  //saving the bet
                        Console.WriteLine(" Client1>> Server Current Data is(player 1 4th move) : table pot,player 1 pot,player 2 pot \n" + gamed.tablePot + " " + gamed.player1Pot + " " + gamed.player2Pot);
                    }
                    //////wait for player 2 move/////////
                    while (gamed.player2Bet.ToString() == "no_bet") ; //wait for player 2 move
                    Console.WriteLine(" Client1>> the bet of player2 is:" + gamed.player2Bet);

                    //Last Bet Final Round #player 2 turn (he alredy played)
                    if (gamed.player2Bet.ToString() == "CHECK")
                    {
                        string player2Card1 = gamed.p2c1.ToString();
                        string player2Card2 = gamed.p2c2.ToString();

                        while (gamed.winPlayer == "no_name") ;
                        // delay for player two to make new cards for next round
                        var t1 = Task.Run(async delegate
                        {
                            await Task.Delay(300);
                        });
                        t1.Wait();
                        if (gamed.winPlayer == "DRAW")
                        {
                            Console.WriteLine(" Client2>> Draw ! ");
                        }
                        else
                        {
                            Console.WriteLine(" Client2>> " + gamed.winPlayer + " win with " + gamed.winHandRank);
                        }
                        string tbl = gamed.tableCardsArray();
                        string card1 = gamed.getCardsArray(1);
                        int handRank = Hand.CalculateHand(card1,tbl);
                        serverResponse = "1-" + "CHECK-" + "WINNER-" + "0-" + gamed.winPlayer + "-" + gamed.player2Pot + "-" + gamed.winHandRank + "-" + gamed.gameEnd + "-" + player2Card1 + "-" + player2Card2 + "-" + handRank + "-$";
                        Console.WriteLine(" Client1>> sending the Winner of the Round: "+gamed.winPlayer);
                        writeToClient(networkStream, sendBytes, serverResponse);


                                                // check if the game end
                        if (gamed.player1Pot == "0" || gamed.player2Pot == "0")
                        {
                            //get if player 1 want to play new round
                            dataFromClient = readFromClient(bytesFrom, networkStream); //geting if player2 want to play new game
                            dataparts = dataFromClient.Split('-');
                            gamed.newGamePlayer1 = dataparts[0];

                            while (gamed.newGamePlayer2.ToString() == "no_answer") ;
                            if (gamed.newGamePlayer1 == "ENDGAME" || gamed.newGamePlayer2 == "ENDGAME")
                            {
                                serverResponse = "ENDGAME";
                                writeToClient(networkStream, sendBytes, serverResponse);
                                Console.WriteLine(" Client1>> the game over");
                                while (true) ;
                            }
                            gamed.newGamePlayer2 = "no_answer";
                        }
                        Thread.Sleep(200);
                        gamed.winPlayer = "no_name"; //reset winplayer
                        gamed.winHandRank = "no_rank";
                        gamed.player1Bet = "no_bet";
                        gamed.player2Bet = "no_bet";
                        gamed.player1RaiseScore = "no_bet";
                        gamed.gameEnd = "notEndGame";
                        gamed.tablePot = "0";
                        goto label1;
                    }
                    else if (gamed.player2Bet.ToString() == "FOLD")
                    {
                        serverResponse = gamed.player2Bet + "-1" + "-$";
                        writeToClient(networkStream, sendBytes, serverResponse);
                        Thread.Sleep(200);
                        gamed.winPlayer = "no_name"; //reset winplayer
                        gamed.winHandRank = "no_rank";
                        gamed.player1Bet = "no_bet";
                        gamed.player2Bet = "no_bet";
                        gamed.player1RaiseScore = "no_bet";
                        gamed.tablePot = "0";
                        goto label1;
                    }


                    else if (gamed.player2Bet.ToString() == "CALL")
                    {
                        string player2Card1 = gamed.p2c1.ToString();
                        string player2Card2 = gamed.p2c2.ToString();

                        while (gamed.winPlayer == "no_name") ;
                        // delay for player two, to make new cards for next round
                        var t1 = Task.Run(async delegate
                        {
                            await Task.Delay(300);
                        });
                        t1.Wait();

                        Console.WriteLine(" Client1>> Server Current Data is(final move : befor reset) : table pot,player 1 pot,player 2 pot \n" + gamed.tablePot + " " + gamed.player1Pot + " " + gamed.player2Pot);
                        string tbl = gamed.tableCardsArray();
                        string card1 = gamed.getCardsArray(1);
                        int handRank = Hand.CalculateHand(card1, tbl);
                        serverResponse = "1-" + "CALL-" + "WINNER" + "-" + gamed.player1RaiseScore + "-" + gamed.winPlayer + "-" + gamed.player2Pot + "-" + gamed.winHandRank + "-" + gamed.gameEnd + "-" + player2Card1 + "-" + player2Card2 +"-"+ handRank + "-$";
                        Console.WriteLine(" Client1>> sending the Winner of the Round... + pot of player2 is: " + gamed.player2Pot + "the raise of player 1 was :" + gamed.player1RaiseScore);
                        writeToClient(networkStream, sendBytes, serverResponse);
                                                // check if the game end
                        if (gamed.player1Pot == "0" || gamed.player2Pot == "0")
                        {
                            //get if player 1 want to play new round
                            dataFromClient = readFromClient(bytesFrom, networkStream); //geting if player2 want to play new game
                            dataparts = dataFromClient.Split('-');
                            gamed.newGamePlayer1 = dataparts[0];
                            while (gamed.newGamePlayer2.ToString() == "no_answer") ;
                            if (gamed.newGamePlayer1 == "ENDGAME" || gamed.newGamePlayer2 == "ENDGAME")
                            {
                                serverResponse = "ENDGAME";
                                writeToClient(networkStream, sendBytes, serverResponse);
                                Console.WriteLine(" Client1>> the game over");
                                while (true) ;
                            }
                            gamed.newGamePlayer2 = "no_answer";
                        }
                        Thread.Sleep(200);
                        gamed.winPlayer = "no_name"; //reset winplayer
                        gamed.winHandRank = "no_rank";
                        gamed.player1Bet = "no_bet";
                        gamed.player2Bet = "no_bet";
                        gamed.player1RaiseScore = "no_bet";
                        gamed.tablePot = "0";
                        gamed.gameEnd = "notEndGame";
                        Console.WriteLine(" Client1>> Server Current Data is(final move : after reset) : table pot,player 1 pot,player 2 pot \n" + gamed.tablePot + " " + gamed.player1Pot + " " + gamed.player2Pot);
                        goto label1;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" >> Client "+ clNo+" Disconnected");
                    Console.WriteLine(" >> Client " + clNo +" "+ ex);
                    flag = false;
                }
            }
        }
        //for the first 2 cards:
        private void sharePlayerCards(NetworkStream networkStream, byte[] sendBytes, string turn)
        {
            string cardToSend="";
            //the first time the function is working, the cards of the table are created:
            //only for the first time
            if(gamed.createCards==true)
            {
                //create all the game cards
                Deck deck = new Deck();
                gamed.c3 = deck.getCardOut(); //1 card out from the deck
                gamed.c4 = deck.getCardOut(); //1 card out from the deck
                gamed.c5 = deck.getCardOut(); //1 card out from the deck
                gamed.c6 = deck.getCardOut(); //1 card out from the deck
                gamed.c7 = deck.getCardOut(); //1 card out from the deck

                gamed.p1c1 = deck.getCardOut(); //Cards for PLayer 1
                gamed.p1c2 = deck.getCardOut(); //Cards for PLayer 1
                gamed.p2c1 = deck.getCardOut(); //Cards for PLayer 2
                gamed.p2c2 = deck.getCardOut(); //Cards for PLayer 2

                gamed.createCards = false;
            }

            if (Int32.Parse(turn) == 1)
            {
                cardToSend = "CARDS-1-" + gamed.p1c1.ToString() + "-" + gamed.p1c2.ToString() + "-"
               + turn.ToString() + "-" + gamed.c3.ToString() + "-" + gamed.c4.ToString() + "-"
               + gamed.c5.ToString() + "-" + gamed.c6.ToString() + "-" + gamed.c7.ToString() + "-$"; //prepare the string to be send
            }
            else if (Int32.Parse(turn) == 2)
            {


                cardToSend = "CARDS-1-" + gamed.p2c1.ToString() + "-" + gamed.p2c2.ToString() + "-"
                + turn.ToString() + "-" + gamed.c3.ToString() + "-" + gamed.c4.ToString() + "-"
                + gamed.c5.ToString() + "-" + gamed.c6.ToString() + "-" + gamed.c7.ToString() + "-$"; //prepare the string to be send
            }

            writeToClient(networkStream, sendBytes, cardToSend); //sending data to client
            Console.WriteLine(" Client" + turn + ">> Client get the cards");  //+cardToSend ); 

        }
        private void writeToClient(NetworkStream networkStream, byte[] sendBytes,string serverResponse)
        {
            sendBytes = Encoding.ASCII.GetBytes(serverResponse);
            networkStream.Write(sendBytes, 0, sendBytes.Length); //sendig 
            networkStream.Flush(); //clean the buffer
        }
        private string readFromClient(byte[] bytesFrom, NetworkStream networkStream)
        {
            string dataFromClient;
            networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
            dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
            dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
            return dataFromClient;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ 
        private void startGame2(GameDealer gamed)
        {
            bool flag = true;
            int requestCount = 0;
            byte[] bytesFrom = new byte[10025];
            string dataFromClient = null;
            Byte[] sendBytes = null;
            string serverResponse = null;
            //string rCount = null;
            requestCount = 0;
            bool localFlag = true;

            string[] ClientMessage = new string[1];

            while ((flag))
            {
                try
                {
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    dataFromClient = readFromClient(bytesFrom, networkStream);
                    ClientMessage = dataFromClient.Split('-');
                    if (ClientMessage[0] == "name")
                    {
                        gamed.playerTwo = ClientMessage[1];
                    }
                    while (gamed.playerOne.ToString() == "no_name") ;

                    Console.WriteLine(" Client2>> " + "client " + clNo + " name is: " + ClientMessage[1]);


                    serverResponse = "READY-" + gamed.playerOne.ToString(); //the string to be send
                    writeToClient(networkStream, sendBytes, serverResponse);
                    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~Console Output~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                    Console.WriteLine(" Client2>> Client1 get the message: " + serverResponse ); //~~

                    //waiting for answer "start game" from client1:
                    dataFromClient = readFromClient(bytesFrom, networkStream);
                    gamed.player2Start = true;
                    while (gamed.player1Start == false) ; //stay here while player2 clicked on start button
                    Console.WriteLine(" Client2>>  cliked on Start Button");
                    serverResponse = "START-p2-";
                    writeToClient(networkStream, sendBytes, serverResponse);
                    var t = Task.Run(async delegate
                    {
                        await Task.Delay(1000);
                    });
                    t.Wait();

                                                                 

             label2:
                    sharePlayerCards(networkStream, sendBytes, "2"); //the func send 2 cards from the table to client
              
                    // now sending the bet of player1  to player2
                    while (gamed.player1Bet.ToString() == "no_bet") ; //wait for player 1 move
                    Console.WriteLine(" Client2>> the bet of player1 is:" + gamed.player1Bet);
                    //
                    if (gamed.player1Bet == "RAISE")
                    {
                        serverResponse = gamed.player1Bet +"-2"+ "-" + gamed.player1RaiseScore + "-" + gamed.player1Pot + "-$"; //sending the "Raise" + value + pot
                        writeToClient(networkStream, sendBytes, serverResponse);
                    }
                    else if (gamed.player1Bet == "CALL") //can't because its the first round!
                    {
                        serverResponse = gamed.player1Bet + "-2" + "-$";
                        writeToClient(networkStream, sendBytes, serverResponse);

                    }
                    else if (gamed.player1Bet == "FOLD")
                    {
                        serverResponse = gamed.player1Bet + "-2" + "-$";
                        writeToClient(networkStream, sendBytes, serverResponse);
                        Thread.Sleep(200);
                        gamed.player1Bet = "no_bet";
                        goto label2;
                    }
                    else if (gamed.player1Bet == "CHECK")
                    {
                        serverResponse = gamed.player1Bet + "-2" + "-$";
                        writeToClient(networkStream, sendBytes, serverResponse);
                    }

                    gamed.player1Bet = "no_bet"; //reset the bet status

                                 //get the bet of player 2
                    dataFromClient = readFromClient(bytesFrom, networkStream); 
                    string[] dataparts = dataFromClient.Split('-');

                    if (dataparts[0] == "CHECK")
                    {
                        gamed.player2Bet = dataFromClient;  //saving the bet
                        serverResponse = "2-" + "CHECK-" + "FLOP" + "-$";
                        Console.WriteLine(" Client2>> sending Flop...");
                        writeToClient(networkStream, sendBytes, serverResponse);
                        Console.WriteLine(" Client2>> Server Current Data is : table pot,player 1 pot,player 2 pot \n" + gamed.tablePot + " " + gamed.player1Pot + " " + gamed.player2Pot);
                    }
                    else if (dataparts[0] == "FOLD")
                    {
                        gamed.player2Bet = dataFromClient;  //saving the bet
                        gamed.player1Bet = "no_bet"; //reset the bet status
                        gamed.player1Pot = (Int32.Parse(gamed.tablePot) + Int32.Parse(gamed.player1Pot)).ToString();
                        gamed.tablePot = "0";
                        gamed.createCards = true;  //to get new Cards!!~
                        goto label2;
                    }
                    else if (dataparts[0] == "CALL")
                    {
                        gamed.player2Bet = dataparts[0];
                        gamed.player2Pot = (Int32.Parse(gamed.player2Pot) - Int32.Parse(gamed.player1RaiseScore)).ToString();
                        gamed.tablePot = (Int32.Parse(gamed.tablePot) + Int32.Parse(gamed.player1RaiseScore)).ToString();
                        Console.WriteLine(" Client2>> Server Current Data is : table pot,player 1 pot,player 2 pot \n" + gamed.tablePot + " " + gamed.player1Pot + " " + gamed.player2Pot);
                        serverResponse = "2-" + "CALL-" +  "FLOP" + "-" + gamed.player1RaiseScore + "-$";
                        Console.WriteLine(" Client2>> sending Flop... + table pot is: " + gamed.player1RaiseScore);
                        writeToClient(networkStream, sendBytes, serverResponse);

                    }
                    else if(dataparts[0] == "RAISE")
                    {
                        
                    }

                    /********************************************************************************
                     * ******************************************************************************
                     * ******************************************************************************/

                    ////////////////////#FLOP-End   New Round#//////////////
                    while (gamed.player1Bet.ToString() == "no_bet") ; //wait for player 1 move
                    Console.WriteLine(" Client2>> the bet of player1 is:" + gamed.player1Bet);
                    //
                    if (gamed.player1Bet == "RAISE")
                    {
                        serverResponse = gamed.player1Bet + "-2" + "-" + gamed.player1RaiseScore + "-" + gamed.player1Pot + "-$"; //sending the "Raise" + value + pot
                        writeToClient(networkStream, sendBytes, serverResponse);
                    }
                    else if (gamed.player1Bet == "CALL") //can't because player 1 cant play with Call Button now
                    {
                        serverResponse = gamed.player1Bet + "-2" + "-$";
                        writeToClient(networkStream, sendBytes, serverResponse);

                    }
                    else if (gamed.player1Bet == "FOLD")
                    {
                        serverResponse = gamed.player1Bet + "-2" + "-$";
                        writeToClient(networkStream, sendBytes, serverResponse);
                        Thread.Sleep(200);
                        gamed.player1Bet = "no_bet";
                        goto label2;
                    }
                    else if (gamed.player1Bet == "CHECK")
                    {
                        serverResponse = gamed.player1Bet + "-2" + "-$";
                        writeToClient(networkStream, sendBytes, serverResponse);
                    }

                    gamed.player1Bet = "no_bet"; //reset the bet status

                              //get the bet of player 2
                     dataFromClient = readFromClient(bytesFrom, networkStream);
                     dataparts = dataFromClient.Split('-');

                    if (dataparts[0] == "CHECK")
                    {
                        gamed.player2Bet = dataFromClient;  //saving the bet
                        serverResponse = "2-" + "CHECK-" + "TURN" + "-$";
                        Console.WriteLine(" Client2>> sending Turn...");
                        writeToClient(networkStream, sendBytes, serverResponse);
                        Console.WriteLine(" Client2>> Server Current Data is : table pot,player 1 pot,player 2 pot \n" + gamed.tablePot + " " + gamed.player1Pot + " " + gamed.player2Pot);
                    }
                    else if (dataparts[0] == "FOLD")
                    {
                        gamed.player2Bet = dataFromClient;  //saving the bet
                        gamed.player1Bet = "no_bet"; //reset the bet status
                        gamed.player1Pot = (Int32.Parse(gamed.tablePot) + Int32.Parse(gamed.player1Pot)).ToString();
                        gamed.tablePot = "0";
                        gamed.createCards = true;  //to get new Cards!!~
                        goto label2;
                    }
                    else if (dataparts[0] == "CALL")
                    {
                        gamed.player2Bet = dataparts[0];
                        gamed.player2Pot = (Int32.Parse(gamed.player2Pot) - Int32.Parse(gamed.player1RaiseScore)).ToString();
                        gamed.tablePot = (Int32.Parse(gamed.tablePot) + Int32.Parse(gamed.player1RaiseScore)).ToString();
                        Console.WriteLine(" Client2>> Server Current Data is : table pot,player 1 pot,player 2 pot \n" + gamed.tablePot + " " + gamed.player1Pot + " " + gamed.player2Pot);
                        serverResponse = "2-" + "CALL-" + "TURN" + "-" + gamed.player1RaiseScore + "-$";
                        Console.WriteLine(" Client2>> sending Turn... + table pot is: " + gamed.player1RaiseScore);
                        writeToClient(networkStream, sendBytes, serverResponse);

                    }
                    else if (dataparts[0] == "RAISE")
                    {

                    }
                    /********************************************************************************
                     * ******************************************************************************
                     * ******************************************************************************/

                    ////////////////////#TURN-End  ----> RIVER now!!! Final Bet #//////////////

                    while (gamed.player1Bet.ToString() == "no_bet") ; //wait for player 1 move
                    Console.WriteLine(" Client2>> the bet of player1 is:" + gamed.player1Bet);
                    //
                    if (gamed.player1Bet == "RAISE")
                    {
                        serverResponse = gamed.player1Bet + "-2" + "-" + gamed.player1RaiseScore + "-" + gamed.player1Pot + "-$"; //sending the "Raise" + value + pot
                        writeToClient(networkStream, sendBytes, serverResponse);
                    }
                    else if (gamed.player1Bet == "CALL") //can't because player 1 cant play with Call Button now
                    {
                        serverResponse = gamed.player1Bet + "-2" + "-$";
                        writeToClient(networkStream, sendBytes, serverResponse);

                    }
                    else if (gamed.player1Bet == "FOLD")
                    {
                        serverResponse = gamed.player1Bet + "-2" + "-$";
                        writeToClient(networkStream, sendBytes, serverResponse);
                        Thread.Sleep(200);
                        gamed.player1Bet = "no_bet";
                        goto label2;
                    }
                    else if (gamed.player1Bet == "CHECK")
                    {
                        serverResponse = gamed.player1Bet + "-2" + "-$";
                        writeToClient(networkStream, sendBytes, serverResponse);
                    }

                    gamed.player1Bet = "no_bet"; //reset the bet status

                    //get the bet of player 2
                    dataFromClient = readFromClient(bytesFrom, networkStream);
                    dataparts = dataFromClient.Split('-');

                    if (dataparts[0] == "CHECK")
                    {
                        gamed.player2Bet = dataFromClient;  //saving the bet
                        serverResponse = "2-" + "CHECK-" + "RIVER" + "-$";
                        Console.WriteLine(" Client2>> sending River...");
                        writeToClient(networkStream, sendBytes, serverResponse);
                        Console.WriteLine(" Client2>> Server Current Data is : table pot,player 1 pot,player 2 pot \n" + gamed.tablePot + " " + gamed.player1Pot + " " + gamed.player2Pot);
                    }
                    else if (dataparts[0] == "FOLD")
                    {
                        gamed.player2Bet = dataFromClient;  //saving the bet
                        gamed.player1Bet = "no_bet"; //reset the bet status
                        gamed.player1Pot = (Int32.Parse(gamed.tablePot) + Int32.Parse(gamed.player1Pot)).ToString();
                        gamed.tablePot = "0";
                        gamed.createCards = true;  //to get new Cards!!~
                        goto label2;
                    }
                    else if (dataparts[0] == "CALL")
                    {
                        gamed.player2Bet = dataparts[0];
                        gamed.player2Pot = (Int32.Parse(gamed.player2Pot) - Int32.Parse(gamed.player1RaiseScore)).ToString();
                        gamed.tablePot = (Int32.Parse(gamed.tablePot) + Int32.Parse(gamed.player1RaiseScore)).ToString();
                        Console.WriteLine(" Client2>> Server Current Data is : table pot,player 1 pot,player 2 pot \n" + gamed.tablePot + " " + gamed.player1Pot + " " + gamed.player2Pot);
                        serverResponse = "2-" + "CALL-" + "RIVER" + "-" + gamed.player1RaiseScore + "-$";
                        Console.WriteLine(" Client2>> sending River... + table pot is: " + gamed.player1RaiseScore);
                        writeToClient(networkStream, sendBytes, serverResponse);
                    }
                    else if (dataparts[0] == "RAISE")
                    {

                    }
                    /********************************************************************************
                     * ******************************************************************************
                     * ******************************************************************************/

                    ////////////////////#TURN-End  ----> RIVER now!!! Final Bet #//////////////
                    while (gamed.player1Bet.ToString() == "no_bet") ; //wait for player 1 move
                    Console.WriteLine(" Client2>> the bet of player1 is:" + gamed.player1Bet);
                    //
                    if (gamed.player1Bet == "RAISE")
                    {
                        serverResponse = gamed.player1Bet + "-2" + "-" + gamed.player1RaiseScore + "-" + gamed.player1Pot + "-$"; //sending the "Raise" + value + pot
                        writeToClient(networkStream, sendBytes, serverResponse);
                    }
                    else if (gamed.player1Bet == "CALL") //can't because player 1 cant play with Call Button now
                    {
                        serverResponse = gamed.player1Bet + "-2" + "-$";
                        writeToClient(networkStream, sendBytes, serverResponse);

                    }
                    else if (gamed.player1Bet == "FOLD")
                    {
                        serverResponse = gamed.player1Bet + "-2" + "-$";
                        writeToClient(networkStream, sendBytes, serverResponse);
                        Thread.Sleep(200);
                        gamed.player1Bet = "no_bet";
                        goto label2;
                    }
                    else if (gamed.player1Bet == "CHECK")
                    {
                        serverResponse = gamed.player1Bet + "-2" + "-$";
                        writeToClient(networkStream, sendBytes, serverResponse);
                    }

                    gamed.player1Bet = "no_bet"; //reset the bet status

                    //get the bet of player 2
                    dataFromClient = readFromClient(bytesFrom, networkStream);
                    dataparts = dataFromClient.Split('-');

                    if (dataparts[0] == "CHECK")
                    {
                        gamed.player2Bet = dataFromClient;  //saving the bet
                        string p1 = gamed.playerOne.ToString();
                        string p2 = gamed.playerTwo.ToString();
                        string tbl = gamed.tableCardsArray();
                        string card1 = gamed.getCardsArray(1);
                        string card2 = gamed.getCardsArray(2);
                        gamed.winHandRank = Hand.getRankOfHand(card1, card2, tbl);
                        gamed.winPlayer = Hand.CalculateWin(p1, card1, p2, card2, tbl);
                        // update server data only from here (Client2) because he faster
                        if (gamed.winPlayer == "DRAW")
                        {
                            Console.WriteLine(" Client2>> Draw ! ");
                            gamed.player2Pot = (Int32.Parse(gamed.player2Pot) + (Int32.Parse(gamed.tablePot) / 2)).ToString();
                            gamed.player1Pot = (Int32.Parse(gamed.player1Pot) + (Int32.Parse(gamed.tablePot) / 2)).ToString();
                            Console.WriteLine(" Client2>> Server Current Data after the DRAW : table pot,player 1 pot,player 2 pot \n" + gamed.tablePot + " " + gamed.player1Pot + " " + gamed.player2Pot);
                            gamed.tablePot = "0";
                            tbl = gamed.tableCardsArray();
                            card1 = gamed.getCardsArray(1);
                            int handRank = Hand.CalculateHand(card1, tbl);
                            if (gamed.player1Pot == "0" || gamed.player2Pot == "0") //check if the game end
                            {
                                gamed.gameEnd = "endGame";
                            }
                            serverResponse = "2-" + "CHECK-" + "WINNER-" + "0-" + gamed.winPlayer + "-" + gamed.player1Pot + "-" + gamed.winHandRank + "-" + gamed.gameEnd + "-" + gamed.p1c1.ToString() + "-" + gamed.p1c2.ToString() + "-" + handRank + "-$";
                            Console.WriteLine(" Client2>> sending the Winner of the Round..." + gamed.winPlayer);
                            writeToClient(networkStream, sendBytes, serverResponse);
                        }
                        else if (gamed.winPlayer == "1") //player 1 win
                        {
                            Console.WriteLine(" Client2>> " + gamed.winPlayer + " win with " + gamed.winHandRank);
                            gamed.player1Pot = (Int32.Parse(gamed.player1Pot) + Int32.Parse(gamed.tablePot)).ToString();
                            gamed.tablePot = "0";
                            Console.WriteLine(" Client2>> Server Current Data after the player 1 won: table pot,player 1 pot,player 2 pot \n" + gamed.tablePot + " " + gamed.player1Pot + " " + gamed.player2Pot);
                            tbl = gamed.tableCardsArray();
                            card1 = gamed.getCardsArray(1);
                            int handRank = Hand.CalculateHand(card1, tbl);
                            if (gamed.player1Pot == "0" || gamed.player2Pot == "0") //check if the game end
                            {
                                gamed.gameEnd = "endGame";
                            }
                            serverResponse = "2-" + "CHECK-" + "WINNER-" + "0-" + gamed.winPlayer + "-" + gamed.player1Pot + "-" + gamed.winHandRank + "-" + gamed.gameEnd + "-" + gamed.p1c1.ToString() + "-" + gamed.p1c2.ToString() +"-" + handRank+"-$";
                            Console.WriteLine(" Client2>> sending the Winner of the Round..." + gamed.winPlayer);
                            writeToClient(networkStream, sendBytes, serverResponse);
                        }
                        else if (gamed.winPlayer == "2") //player 2 win
                        {
                            Console.WriteLine(" Client2>> " + gamed.winPlayer + " win with " + gamed.winHandRank);
                            gamed.player2Pot = (Int32.Parse(gamed.player2Pot) + Int32.Parse(gamed.tablePot)).ToString();
                            gamed.tablePot = "0";
                            Console.WriteLine(" Client2>> Server Current Data after the player 2 won : table pot,player 1 pot,player 2 pot \n" + gamed.tablePot + " " + gamed.player1Pot + " " + gamed.player2Pot);
                            tbl = gamed.tableCardsArray();
                            card1 = gamed.getCardsArray(1);
                            int handRank = Hand.CalculateHand(card1, tbl);
                            if (gamed.player1Pot == "0" || gamed.player2Pot == "0") //check if the game end
                            {
                                gamed.gameEnd = "endGame";
                            }
                            serverResponse = "2-" + "CHECK-" + "WINNER-" + "0-" + gamed.winPlayer + "-" + gamed.player1Pot + "-" + gamed.winHandRank + "-" + gamed.gameEnd + "-" + gamed.p1c1.ToString() + "-" + gamed.p1c2.ToString() +"-"+ handRank + "-$";
                            Console.WriteLine(" Client2>> sending the Winner of the Round..." + gamed.winPlayer);
                            writeToClient(networkStream, sendBytes, serverResponse);
                        }

                        if (gamed.player1Pot == "0" || gamed.player2Pot == "0") //check if the game end
                        {
                            gamed.gameEnd = "endGame";
                            tbl = gamed.tableCardsArray();
                            card1 = gamed.getCardsArray(1);
                            int handRank = Hand.CalculateHand(card1, tbl);
                            serverResponse = "2-" + "CHECK-" + "WINNER-" + "0-" + gamed.winPlayer + "-" + gamed.player1Pot + "-" + gamed.winHandRank + "-" + gamed.gameEnd + "-" + gamed.p1c1.ToString() + "-" + gamed.p1c2.ToString() +"-"+ handRank + "-$";
                            Console.WriteLine(" Client2>> sending the Winner of the Round..." + gamed.winPlayer);
                            writeToClient(networkStream, sendBytes, serverResponse);
                            //get if player 2 want to play new round
                            dataFromClient = readFromClient(bytesFrom, networkStream); //geting if player2 want to play new game
                            dataparts = dataFromClient.Split('-');
                            gamed.newGamePlayer2 = dataparts[0];
                            gamed.tablePot = "0";
                            gamed.player1Pot = "1000";
                            gamed.player2Pot = "1000";
                            while (gamed.newGamePlayer1.ToString() == "no_answer") ;
                            if (gamed.newGamePlayer1 == "ENDGAME" || gamed.newGamePlayer2 == "ENDGAME")
                            {
                                serverResponse = "ENDGAME";
                                writeToClient(networkStream, sendBytes, serverResponse);
                                Console.WriteLine(" Client2>> the game over");
                                while (true) ;
                            }
                            gamed.newGamePlayer1 = "no_answer";
                        }


                        Thread.Sleep(200);
                        gamed.createCards = true;  //to get new Cards!!~
                        goto label2;
                    }
                    else if (dataparts[0] == "FOLD")
                    {
                        gamed.player2Bet = dataFromClient;  //saving the bet
                        gamed.player1Bet = "no_bet"; //reset the bet status
                        gamed.player1Pot = (Int32.Parse(gamed.tablePot) + Int32.Parse(gamed.player1Pot)).ToString();
                        gamed.tablePot = "0";
                        gamed.createCards = true;  //to get new Cards!!~
                        goto label2;
                    }
                    else if (dataparts[0] == "CALL")
                    {
                        gamed.player2Bet = dataparts[0];
                        gamed.tablePot = (Int32.Parse(gamed.tablePot) + Int32.Parse(gamed.player1RaiseScore)).ToString();
                        gamed.player2Pot = (Int32.Parse(gamed.player2Pot) - Int32.Parse(gamed.player1RaiseScore)).ToString();
                        Console.WriteLine(" Client2>> Server Current Data is : table pot,player 1 pot,player 2 pot \n" + gamed.tablePot + " " + gamed.player1Pot + " " + gamed.player2Pot);

                        string p1 = gamed.playerOne.ToString();
                        string p2 = gamed.playerTwo.ToString();
                        string tbl = gamed.tableCardsArray();
                        string card1 = gamed.getCardsArray(1);
                        string card2 = gamed.getCardsArray(2);
                        gamed.winHandRank = Hand.getRankOfHand(card1, card2, tbl);
                        gamed.winPlayer = Hand.CalculateWin(p1, card1, p2, card2, tbl);
                        // update server data only from here (Client2) because he faster
                        if (gamed.winPlayer == "DRAW")
                        {
                            Console.WriteLine(" Client2>> Draw ! ");
                            Console.WriteLine(" befor draw" + gamed.player1Pot + " " + gamed.player2Pot); //delete  this (only for check)
                            gamed.player2Pot = (Int32.Parse(gamed.player2Pot) + (Int32.Parse(gamed.tablePot) / 2)).ToString();
                            gamed.player1Pot = (Int32.Parse(gamed.player1Pot) + (Int32.Parse(gamed.tablePot) / 2)).ToString();
                            Console.WriteLine(" after draw" + gamed.player1Pot + " " + gamed.player2Pot);//delete  this (only for check)
                            gamed.tablePot = "0";
                            Console.WriteLine(" Client2>> Server Current Data after the DRAW : table pot,player 1 pot,player 2 pot \n" + gamed.tablePot + " " + gamed.player1Pot + " " + gamed.player2Pot);
                            tbl = gamed.tableCardsArray();
                            card1 = gamed.getCardsArray(1);
                            int handRank = Hand.CalculateHand(card1, tbl);
                            if (gamed.player1Pot == "0" || gamed.player2Pot == "0")
                            {
                                gamed.gameEnd = "endGame";
                            }
                            serverResponse = "2-" + "CALL-" + "WINNER" + "-" + gamed.player1RaiseScore + "-" + gamed.winPlayer + "-" + gamed.player1Pot + "-" + gamed.winHandRank + "-" + gamed.gameEnd + "-" + gamed.p1c1.ToString() + "-" + gamed.p1c2.ToString() +"-"+ handRank+ "-$";
                            Console.WriteLine(" Client2>> sending the Winner of the Round... + table pot is: " + gamed.player1RaiseScore);
                            writeToClient(networkStream, sendBytes, serverResponse);

                        }
                        else if (gamed.winPlayer == "1") //player 1 win
                        {
                            Console.WriteLine(" Client2>> " + gamed.winPlayer + " win with " + gamed.winHandRank);
                            gamed.player1Pot = (Int32.Parse(gamed.player1Pot) + Int32.Parse(gamed.tablePot)).ToString();
                            gamed.tablePot = "0";
                            Console.WriteLine(" Client2>> Server Current Data after the player 1 won: table pot,player 1 pot,player 2 pot \n" + gamed.tablePot + " " + gamed.player1Pot + " " + gamed.player2Pot);
                            tbl = gamed.tableCardsArray();
                            card1 = gamed.getCardsArray(1);
                            int handRank = Hand.CalculateHand(card1, tbl);
                            if (gamed.player1Pot == "0" || gamed.player2Pot == "0")
                            {
                                gamed.gameEnd = "endGame";
                            }
                            serverResponse = "2-" + "CALL-" + "WINNER" + "-" + gamed.player1RaiseScore + "-" + gamed.winPlayer + "-" + gamed.player1Pot + "-" + gamed.winHandRank + "-" + gamed.gameEnd + "-" + gamed.p1c1.ToString() + "-" + gamed.p1c2.ToString() +"-"+ handRank + "-$";
                            Console.WriteLine(" Client2>> sending the Winner of the Round... + table pot is: " + gamed.player1RaiseScore);
                            writeToClient(networkStream, sendBytes, serverResponse);
                        }
                        else if (gamed.winPlayer == "2") //player 2 win
                        {
                            Console.WriteLine(" Client2>> " + gamed.winPlayer + " win with " + gamed.winHandRank);
                            gamed.player2Pot = (Int32.Parse(gamed.player2Pot) + Int32.Parse(gamed.tablePot)).ToString();
                            gamed.tablePot = "0";
                            Console.WriteLine(" Client2>> Server Current Data after the player 2 won : table pot,player 1 pot,player 2 pot \n" + gamed.tablePot + " " + gamed.player1Pot + " " + gamed.player2Pot);
                            tbl = gamed.tableCardsArray();
                            card1 = gamed.getCardsArray(1);
                            int handRank = Hand.CalculateHand(card1, tbl);
                            if (gamed.player1Pot == "0" || gamed.player2Pot == "0")
                            {
                                gamed.gameEnd = "endGame";
                            }
                            serverResponse = "2-" + "CALL-" + "WINNER" + "-" + gamed.player1RaiseScore + "-" + gamed.winPlayer + "-" + gamed.player1Pot + "-" + gamed.winHandRank + "-" + gamed.gameEnd + "-" + gamed.p1c1.ToString() + "-" + gamed.p1c2.ToString() +"-"+ handRank + "-$";
                            Console.WriteLine(" Client2>> sending the Winner of the Round... + table pot is: " + gamed.player1RaiseScore);
                            writeToClient(networkStream, sendBytes, serverResponse);
                        }
                        // check if the game end
                        if (gamed.player1Pot == "0" || gamed.player2Pot == "0")
                        {
                            gamed.gameEnd = "endGame";
                            tbl = gamed.tableCardsArray();
                            card1 = gamed.getCardsArray(1);
                            int handRank = Hand.CalculateHand(card1, tbl);
                            serverResponse = "2-" + "CALL-" + "WINNER" + "-" + gamed.player1RaiseScore + "-" + gamed.winPlayer + "-" + gamed.player1Pot + "-" + gamed.winHandRank + "-" + gamed.gameEnd + "-" + gamed.p1c1.ToString() + "-" + gamed.p1c2.ToString() +"-"+ handRank + "-$";
                            Console.WriteLine(" Client2>> sending the Winner of the Round... + table pot is: " + gamed.player1RaiseScore);
                            writeToClient(networkStream, sendBytes, serverResponse);
                            //get if player 2 want to play new round
                            dataFromClient = readFromClient(bytesFrom, networkStream); //geting if player2 want to play new game
                            dataparts = dataFromClient.Split('-');
                            gamed.newGamePlayer2 = dataparts[0];
                            gamed.tablePot = "0";
                            gamed.player1Pot = "1000";
                            gamed.player2Pot = "1000";
                            while (gamed.newGamePlayer1.ToString() == "no_answer") ;
                            if (gamed.newGamePlayer1 == "ENDGAME" || gamed.newGamePlayer2 == "ENDGAME")
                            {
                                serverResponse = "ENDGAME";
                                writeToClient(networkStream, sendBytes, serverResponse);
                                Console.WriteLine(" Client2>> the game over");
                                while (true) ;
                            }
                            gamed.newGamePlayer1 = "no_answer";
                        }

                        Thread.Sleep(200);
                        gamed.createCards = true;  //to get new Cards!!~
                        goto label2;

                    }
                    else if (dataparts[0] == "RAISE")
                    {

                    }




                }
                catch (Exception ex)
                {
                    Console.WriteLine(" >> Client " + clNo + " Disconnected");
                    Console.WriteLine(" >> Client " + clNo + " " + ex);
                    flag = false;
                }
            }
        }
     
    }
}
