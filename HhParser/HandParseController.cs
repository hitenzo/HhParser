using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HhParser
{
    public class HandParseController
    {
        private string _handSource;
        private string _userName;
        private string _balances;
        private Random _rnd;
        private List<string> _cards;

        public PokerHand Parse(string handSource, int position, int? desiredPlayersCount = null)
        {
            HandConverter converter = new HandConverter(handSource, position);
            string handString = converter.GetFormatHandAction();
            var hand = new PokerHand { Action = handString };
            return hand;
        }
    }
}