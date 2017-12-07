using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatchScheduling
{
    class Program
    {
        // The description of the match scheduling program is taken from :
        // https://link.springer.com/chapter/10.1007/978-1-4419-8702-0_7
        //
        // Suppose we have n players, labelled 1, 2, 3, ... n
        // We also suppose that n is even. If there are an
        // odd number of players, then we ad a "bye", i.e.
        // an extra, non-existent player, in order to make
        // the total number even. A player who is to play 
        // a "bye" on a given round does not play in that
        // round. 
        //
        // Each player is to play all of the other (n - 1)
        // players. Hence there will be (n - 1) rounds.
        //
        // Assignment : use congruence modulo to design 
        // the tournament schedule.
        //
        // Note :
        // For round r, and players x and y :
        //
        // x >= 1
        // y <= (n - 1)
        // x != y
        //
        // Also, x will play y if 
        // x + y ≡ r (mod n - 1).
        //
        // And :
        // if x + x ≡ r (mod n - 1) then x plays player n.
        //        

        // The Player class is designed to keep track of the 
        // of whether an individual Player instance has 
        // been assigned an opponent.
        //
        // The Player class also has member methods to help 
        // determine whether an individual Player instance
        // can be paired with another Player for a specific
        // round.
        //
        private class Player
        {
            public Player(int iPlayerNumber, int iOpponent)
            {
                this.PlayerNumber = iPlayerNumber;
                this.Opponent = iOpponent;
            }

            public int PlayerNumber
            {
                get
                {
                    return iPlayerNumber;
                }

                set
                {
                    iPlayerNumber = value;
                }
            }

            public int Opponent
            {
                get
                {
                    return iOpponentNumber;
                }

                set
                {
                    iOpponentNumber = value;
                }
            }

            // Note : the following implementation for IsMatchedToPlayerN() is wrong.
            // The one below is correct.
            // 
            // The reason why the implementation is wrong :
            // Note that the principle for determining whether a player
            // x is to play the last player (player n) in round r is :
            //
            // if x + x ≡ r (mod n - 1) then x plays player n.
            //
            // Note that (x + x) must be congruent to r modulo n-1.
            // This does not mean that (x + x) % (n - 1) equals r
            // (although sometimes it does).
            // 
            /*public bool IsMatchedToPlayerN(int iTotalNumberOfPlayers, int iRound)
            {
                int iTest = PlayerNumber * 2;
                int iRemainder = iTest % (iTotalNumberOfPlayers - 1);

                if (iRemainder == iRound)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }*/

            // If the total number of players is N,
            // IsMatchedToPlayerN() determines whether an individual 
            // Player is to be paired with Player N (i.e. 
            // the last player).
            //
            // E.g. if N==10, IsMatchedToPlayerN() determines
            // whether an individual Player instance is to play
            // Player 10.
            //
            public bool IsMatchedToPlayerN(int iTotalNumberOfPlayers, int iRound)
            {
                return Program.IsCongruentModulo(PlayerNumber * 2, iRound, iTotalNumberOfPlayers - 1);
            }

            // Note : the following implementation for IsMatchingPlayerInRound() be wrong.
            // The one below is correct.
            //
            // The reason why the implementation is wrong :
            // The principle of determining whether x will play y is :
            //
            // x will play y if 
            // x + y ≡ r (mod n - 1).
            // 
            // Note that (x + y) must be congruent to r modulo n-1.
            // This does not mean that (x + y) % (n - 1) equals r
            // (although sometimes it does).
            /*public bool IsMatchingPlayerInRound(int iTotalNumberOfPlayers, int iRound, Player Opponent)
            {
                int iTest = PlayerNumber + Opponent.PlayerNumber;
                int iRemainder = iTest % (iTotalNumberOfPlayers - 1);

                if (iRemainder == iRound)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }*/

            // IsMatchingPlayerInRound() determines whether an individual Player instance
            // is to be paired with a specific opponent (i.e. the "Opponent" parameter)
            // for a specific round.
            public bool IsMatchingPlayerInRound(int iTotalNumberOfPlayers, int iRound, Player Opponent)
            {
                return Program.IsCongruentModulo(PlayerNumber + Opponent.PlayerNumber, iRound, iTotalNumberOfPlayers - 1);
            }

            private int iPlayerNumber; // 0 - means not assigned yet.
            private int iOpponentNumber; // 0 - means not assigned yet.
        }

        // IsCongruentModulo() determines whether 2 numbers are Congruent
        // Modulo with respect to iModulo.
        //
        // It is based on the simple principle that if A ≡ B (mod C), then :
        //
        // 1. If the remainder of A divided by C is R1.
        // 2. And if the remainder of B divided by C is R2.
        // 3. R1 = R2.
        // 
        // In other words, when A is divided by C, the remainder is the same 
        // as that of B divided by C.
        static bool IsCongruentModulo(int iNumber1, int iNumber2, int iModulo)
        {
            int iRemainder1 = iNumber1 % iModulo;
            int iRemainder2 = iNumber2 % iModulo;

            if (iRemainder1 == iRemainder2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // For a specific Player instance, SearchForOpponent() will determine
        // the Player's opponent for a specific round iRound.
        static void SearchForOpponent(Player player, int iTotalNumberOfPlayers, int iRound)
        {
            int iCurrentPlayerNumber = player.PlayerNumber;

            if (player.Opponent != 0)
            {
                // Current player already has opponent.
                return;
            }

            // First check whether the current Player is to play Player N (i.e. the last Player).
            if (player.IsMatchedToPlayerN(iTotalNumberOfPlayers, iRound) == true)
            {
                player.Opponent = iTotalNumberOfPlayers;
                m_Players[iTotalNumberOfPlayers - 1].Opponent = iCurrentPlayerNumber;
                return;
            }

            // So the current Player is not to play Player N.
            // We go through every other player to search for 
            // the appropriate opponent.
            for (int i = iCurrentPlayerNumber; i < iTotalNumberOfPlayers; i++)
            {
                if (m_Players[i].Opponent == 0)
                {
                    if (player.IsMatchingPlayerInRound(iTotalNumberOfPlayers, iRound, m_Players[i]) == true)
                    {
                        player.Opponent = i + 1;
                        m_Players[i].Opponent = iCurrentPlayerNumber;
                        break;
                    }
                }
            }
        }

        // InitializePlayers() initializes the m_Players array
        // to the total number of players.
        //
        // The method then sets up the initial conditions
        // for each individual Player instance in the m_Players
        // array.
        static void InitializePlayers()
        {
            m_Players = new Player[iTotalNumberOfPlayers];

            for (int i = 0; i < iTotalNumberOfPlayers; i++)
            {
                m_Players[i] = new Player(i + 1, 0);
            }
        }

        // InitializeBye() initializes the "bye" player
        // in the event that iTotalNumberOfPlayers is 
        // an odd number.
        static void InitializeBye()
        {
            if ((iTotalNumberOfPlayers % 2) == 0)
            {
                // If iTotalNumberOfPlayers is even,
                // set the "bye" to 0.
                m_iBye = 0;
            }
            else
            {
                // If iTotalNumberOfPlayers is odd,
                // we increment iTotalNumberOfPlayers by 1
                // (thus making it even) and then set
                // the "bye" to the new value of iTotalNumberOfPlayers.
                iTotalNumberOfPlayers++;
                m_iBye = iTotalNumberOfPlayers;
            }
        }

        // ReinitializeOpponents() re-initializes each individual 
        // Player's opponent to no one. This is important at the
        // start of each round.
        static void ReinitializeOpponents()
        {
            for (int i = 0; i < iTotalNumberOfPlayers; i++)
            {
                m_Players[i].Opponent = 0;
            }
        }

        // DoScheduling() begins the match scheduling process.
        static void DoScheduling()
        {
            for (int iRound = 1; iRound < iTotalNumberOfPlayers; iRound++)
            {
                // At the start of each round, we re-initialize 
                // each Player instance's opponent to 0.
                ReinitializeOpponents();

                for (int i = 0; i < iTotalNumberOfPlayers; i++)
                {
                    SearchForOpponent(m_Players[i], iTotalNumberOfPlayers, iRound);
                }

                // Now display each Player's opponent for the current round.
                Console.Write("Round {0:D} : ", iRound);

                for (int i = 0; i < iTotalNumberOfPlayers; i++)
                {
                    // If the current Player is a "bye", skip it.
                    // Recall that a "bye" is a non-existent player.
                    if (m_Players[i].PlayerNumber == m_iBye)
                    {
                        continue;
                    }

                    // If the current Player's opponent is a "bye", skip it.
                    // Recall that a "bye" is a non-existent player.
                    if (m_Players[i].Opponent == m_iBye)
                    {
                        continue;
                    }

                    Console.Write("{0:D} vs {1:D} ", m_Players[i].PlayerNumber, m_Players[i].Opponent);
                }

                Console.WriteLine();
            }
        }

        static void Main(string[] args)
        {
            InitializeBye();
            InitializePlayers();
            DoScheduling();
        }

        private static Player[] m_Players = null;
        private static int m_iBye = 0;
        private static int iTotalNumberOfPlayers = 8;
    }
}