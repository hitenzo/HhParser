using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HhParser
{
    public static class ActionFactory
    {
        public static string GetAction(string actionString)
        {
            if (actionString.Contains("calls"))
            {
                return "C";
            }
            if (actionString.Contains("bets"))
            {
                return "B";
            }
            if (actionString.Contains("folds"))
            {
                return "F";
            }
            if (actionString.Contains("checks"))
            {
                return "K";
            }
            if (actionString.Contains("raises"))
            {
                return "R";
            }
            if (actionString.Contains("all-in"))
            {
                return "A";
            }
            if (actionString.Contains("mucks"))
            {
                return "F";
            }
            throw new WrongGameFormatException("Cos się popsulo i nie było mnie słychać więc powtórzę jeszcze raz");
        }
    }

    class WrongGameFormatException : Exception
    {
        public WrongGameFormatException() : base() { }
        public WrongGameFormatException(string message) : base(message) { }
    }

}
