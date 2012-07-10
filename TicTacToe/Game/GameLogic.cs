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
    
    public class GameLogic{

        public static int MAX_SIZE = 3;
        public static int MINIMUN_TURNS_TO_WIN = 5;
        public static int MAX_TURNS = 9;

        public bool isAWinner(char[,] tablero)
        {
            Boolean ganador = false;
            if ((tablero[0,0] == tablero[1,1] && tablero[1,1] == tablero[2,2] && tablero[1,1] != ' ')
                || (tablero[0,2] == tablero[1,1] && tablero[1,1] == tablero[2,0] && tablero[1,1] != ' '))
            {
                ganador = true;
            }
            else
            {
                for (int i = 0; i < MAX_SIZE; i++)
                {
                    if ((tablero[i,0] == tablero[i,1] && tablero[i,1] == tablero[i,2] && tablero[i,0] != ' ')
                    || (tablero[0,i] == tablero[1,i] && tablero[1,i] == tablero[2,i] && tablero[0,i] != ' '))
                    {
                        ganador = true;
                    }
                }
            }
            return ganador;
        }
        
    }
}
