using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HhParser
{
    public class HandConverter
    {
        private string _handSource;
        private string _userName;
        private string _balances;
        private Random _rnd;
        private List<string> _cards;
        private string _randomHand;
        private List<string> _sortedActions;
        private string _rawBlinds;
        private string _sbPlayer;
        private string _bbPlayer;
        

        public HandConverter(string handSource, int position)
        {
            _handSource = handSource;
            _balances = GetPlayersBalances();
            _userName = GetUserName(position);
            _cards = GetStreetCards();
            _rnd = new Random();
            _randomHand = GetRandomHand();
            _sortedActions = GetSortedPlayerActions();
            _rawBlinds = GetBlinds();
        }

        public HandConverter(string handSource)
        {
            _handSource = handSource;
        }

        public List<string> GetStreetActions()
        {
            var streets = new List<string>();
            var handAction = ExtractFromString(_handSource, "*** HOLE CARDS **", "*** SUMMARY ***").First();

            string preflopAction = string.Empty;
            string flopAction = string.Empty;
            string turnAction = string.Empty;
            string riverAction = string.Empty;
            bool containsFlop = handAction.Contains("*** FLOP ***");
            bool containsTurn = handAction.Contains("*** TURN ***");
            bool containsRiver = handAction.Contains("*** RIVER ***");
            int flopIndex = handAction.IndexOf("*** FLOP ***", StringComparison.Ordinal);
            int turnIndex = handAction.IndexOf("*** TURN ***", StringComparison.Ordinal);
            int riverIndex = handAction.IndexOf("*** RIVER ***", StringComparison.Ordinal);

            try
            {
                preflopAction = ExtractFromString(handAction, "*", "*** FLOP ***").First();
                streets.Add(preflopAction);
            }
            catch (Exception)
            {
                preflopAction = handAction;
                preflopAction = preflopAction.Remove(0, 1);
                streets.Add(preflopAction);
            }

            try
            {
                flopAction = ExtractFromString(handAction, "*** FLOP ***", "*** TURN ***").First();
                flopAction = RemoveCardsFromAction(flopAction);
                streets.Add(flopAction);
            }
            catch (Exception)
            {
                int length = "*** FLOP ***".Length;
                if (!containsFlop)
                {
                    flopAction = "";
                    streets.Add(flopAction);
                }
                if (!containsTurn && containsFlop)
                {
                    flopAction = handAction.Substring(flopIndex + length);
                    streets.Add(flopAction);
                }
            }

            try
            {
                turnAction = ExtractFromString(handAction, "*** TURN ***", "*** RIVER ***").First();
                turnAction = RemoveCardsFromAction(turnAction);
                streets.Add(turnAction);
            }
            catch (Exception)
            {
                int length = "*** TURN ***".Length;
                if (!containsRiver && containsTurn)
                {
                    turnAction = handAction.Substring(turnIndex + length);
                    streets.Add(turnAction);
                }
                else
                {
                    turnAction = "";
                    streets.Add(turnAction);
                }
            }

            if (containsRiver)
            {
                int length = "*** RIVER ***".Length;
                riverAction = handAction.Substring(riverIndex + length);
                int showDown = riverAction.IndexOf("*** SHOW DOWN ***");
                if (showDown != -1)
                {
                    riverAction = riverAction.Remove(showDown);
                }
                riverAction = RemoveCardsFromAction(riverAction);
                streets.Add(riverAction);
            }
            else
            {
                riverAction = "";
                streets.Add(riverAction);
            }
            return streets;
        }

        public string GetFormatHandAction()
        {
            var blinds = _rawBlinds.Split('/');
            var bb = blinds[1];
            var sb = blinds[0];

            var handString = string.Format(@"[table]
sblind = {0}
bblind = {1}
ante = 0
gtype = FL
network = Everest
tournament = False
balances = {2}

[preflop]
Hand = {3}
Actions = {4}
{5}
{6}
{7}
{8}
{9}
{10}", sb, bb, _balances, _randomHand, _sortedActions[0], _cards[0], _sortedActions[1], _cards[1], _sortedActions[2], _cards[2], _sortedActions[3]);

            return handString;
        }

        public string RemoveCardsFromAction(string actionString)
        {
            int firstSquareBracket = actionString.IndexOf("[");
            int lastSquareBracket = actionString.LastIndexOf("]");
            string dealtCardsToRemove = actionString.Substring(firstSquareBracket, lastSquareBracket);

            int index = actionString.IndexOf(dealtCardsToRemove);

            actionString = (index < 0) ? actionString :
                actionString.Remove(index, dealtCardsToRemove.Length);

            return actionString;
        }

        public List<string> GetSortedPlayerActions()
        {
            List<string> streetActionFomrated = new List<string>() { "", "", "", "" };
            List<string> streets = GetStreetActions();

            for (var i = 0; i < 4; i++)
            {
                var streetLines = streets[i].Split(new string[] { "\r\n" }, StringSplitOptions.None);
                for (var j = 0; j < streetLines.Count(); j++)
                //                foreach (var actionLine in streetLines)
                {
                    string actionLine = streetLines[j];
                    if (actionLine.Contains(':') && !actionLine.Contains("doesn"))
                    {
                        actionLine = actionLine.Replace(Environment.NewLine, " " + Environment.NewLine + " ");
                        var actionPlayerName = actionLine.Substring(0, actionLine.IndexOf(':'));
                        actionPlayerName = Regex.Replace(actionPlayerName, @"\s+", "");

                        String playerAction = ActionFactory.GetAction(actionLine);

                        if (_userName.Equals(actionPlayerName))
                        {
                            playerAction = "can FCKRA do C";
                        }

                        streetActionFomrated[i] += string.Format(", {0} {1}", actionPlayerName, playerAction);
                    }
                }
                if (streetActionFomrated[i] != "" && i > 0)
                {
                    streetActionFomrated[i] = streetActionFomrated[i].Remove(0, 2);
                    streetActionFomrated[i] = "Actions = " + streetActionFomrated[i];
                }
                else if (streetActionFomrated[i] != "" && i == 0)
                {
                    streetActionFomrated[i] = _sbPlayer + " S, " + _bbPlayer + " B, " + streetActionFomrated[i].Remove(0, 2);
                }
            }
            foreach (string street in streetActionFomrated)
            {
                if (street.Length > 1)
                {
                    street.Remove(street.Length - 1);
                }
            }
            return streetActionFomrated;
        }

        public string GetBlinds()
        {
            var rawBlinds = ExtractFromString(_handSource, "(", ")").First();
            return rawBlinds;
        }

        public string GetPlayersBalances()
        {
            var preflopState = ExtractFromString(_handSource, "the button", "*** HOLE CARDS").First();
            var playersInfo = ExtractFromString(preflopState, "Seat ", " in chips)");

            var players = new List<Tuple<int, string, int>>();
            foreach (var i in playersInfo)
            {
                var seat = Int32.Parse(i[0].ToString());
                var playerName = ExtractFromString(i, ": ", " (").First();
                playerName = Regex.Replace(playerName, @"\s+", "");
                var playerStack = i.Substring(i.LastIndexOf('(') + 1);

                players.Add(new Tuple<int, string, int>(seat, playerName, Int32.Parse(playerStack)));
            }

            var balances = string.Empty;
            foreach (var player in players)
            {
                balances += string.Format("{0} {1},", player.Item2, player.Item3);
            }
            balances = balances.Remove(balances.Length - 1);
            return balances;
        }

        private string GetRandomHand()
        {
            var cardsSymbols = "AKQJT98765432";
            var colors = "chds";

            var h1 = string.Format("{0}{1}", cardsSymbols.Substring(_rnd.Next(0, 12), 1), colors.Substring(_rnd.Next(0, 4), 1));
            var h2 = string.Format("{0}{1}", cardsSymbols.Substring(_rnd.Next(0, 13), 1), colors.Substring(_rnd.Next(0, 4), 1));

            while (h1.Equals(h2))
            {
                h2 = string.Format("{0}{1}", cardsSymbols.Substring(_rnd.Next(0, 13), 1), colors.Substring(_rnd.Next(0, 4), 1));
            }

            var hand = string.Format("{0}, {1}", h1, h2);

            return hand;
        }

        public List<string> GetStreetCards()
        {
            var cardsHolder = new List<string>();

            var flopLength = "*** FLOP *** [".Length;
            var turnLength = "*** TURN *** [".Length;
            var riverLength = "*** RIVER *** [".Length;

            string flopCards = string.Empty;
            string turnCards = string.Empty;
            string riverCards = string.Empty;

            int flopIndex;
            int turnIndex;
            int riverIndex;
            int turnCardIndex;
            int riverCardIndex;

            flopIndex = _handSource.IndexOf("*** FLOP *** [");
            flopCards = _handSource.Substring(flopIndex + flopLength, 8);

            turnIndex = _handSource.IndexOf("*** TURN *** [");
            turnCardIndex = _handSource.IndexOf("[", turnIndex + turnLength + 1);
            turnCards = _handSource.Substring(turnCardIndex + 1, 2);

            riverIndex = _handSource.IndexOf("*** RIVER *** [");
            riverCardIndex = _handSource.IndexOf("[", riverIndex + riverLength + 1);
            riverCards = _handSource.Substring(riverCardIndex + 1, 2);

            flopCards = Regex.Replace(flopCards, "\\s", ", ");

            flopCards = "\r\n" + "[flop]" + "\r\n" + "Cards = " + flopCards;
            turnCards = "\r\n" + "[turn]" + "\r\n" + "Cards = " + turnCards;
            riverCards = "\r\n" + "[river]" + "\r\n" + "Cards = " + riverCards;

            if (flopIndex == -1)
            {
                flopCards = "";
                turnCards = "";
                riverCards = "";
            }
            if (turnIndex == -1)
            {
                turnCards = "";
                riverCards = "";
            }
            if (riverIndex == -1)
            {
                riverCards = "";
            }

            cardsHolder.Add(flopCards);
            cardsHolder.Add(turnCards);
            cardsHolder.Add(riverCards);
            return cardsHolder;
        }

        public string GetUserName(int position)
        {
            string userName = string.Empty;
            var preflopState = ExtractFromString(_handSource, "button", "*** HOLE CARDS ***").First();
            var preflopStateLines = preflopState.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            List<string> players = new List<string>();
            string sbPlayer = string.Empty;
            string bbPlayer = string.Empty;
            string buPlayer = string.Empty;

            for (var i = preflopStateLines.Length - 1; i >= 0; i--)
            {
                if (preflopStateLines[i].Contains(":") && preflopStateLines[i].Contains("big blind"))
                {
                    bbPlayer = preflopStateLines[i].Substring(0, preflopStateLines[i].IndexOf(":"));
                    _bbPlayer = bbPlayer;
                }
                if (preflopStateLines[i].Contains(":") && preflopStateLines[i].Contains("small blind"))
                {
                    sbPlayer = preflopStateLines[i].Substring(0, preflopStateLines[i].IndexOf(":"));
                    _sbPlayer = sbPlayer;
                }
                if (preflopStateLines[i].Contains("Seat") && preflopStateLines[i].Contains(":"))
                {
                    var player = ExtractFromString(preflopStateLines[i], ":", "(").First();
                    player = player.Remove(0, 1);
                    player = player.Remove(player.Length - 1, 1);
                    players.Add(player);
                    if (player != bbPlayer && player != sbPlayer)
                    {
                        buPlayer = player;
                    }
                }
            }

            if (players.Count < 3)
            {
                buPlayer = sbPlayer;
            }

            if (position == 1)
            {
                userName = sbPlayer;
            }
            else if (position == 2)
            {
                userName = bbPlayer;
            }
            else if (position == 3)
            {
                userName = buPlayer;
            }
            return userName;
        }

        public static List<string> ExtractFromString(string text, string startString, string endString)
        {
            List<string> matched = new List<string>();
            int indexStart = 0, indexEnd = 0;
            bool exit = false;
            while (!exit)
            {
                indexStart = text.IndexOf(startString);
                indexEnd = text.IndexOf(endString);
                if (indexStart != -1 && indexEnd != -1)
                {
                    matched.Add(text.Substring(indexStart + startString.Length,
                        indexEnd - indexStart - startString.Length));
                    text = text.Substring(indexEnd + endString.Length);
                }
                else
                    exit = true;
            }
            return matched;
        }
    }
}
