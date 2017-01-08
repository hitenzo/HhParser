using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HhParser;

namespace HhParserTest
{
    [TestClass]
    public class HandConverterTest
    {
        private string hand =
                @"PokerStars Hand #151250400702: Tournament #1526271285, $6.51+$0.49 USD Hold'em No Limit - Level III (20/40) - 2016/03/31 23:48:52 ET
Table '1526271285 1' 3-max Seat #2 is the button
Seat 1: kszwenk (1065 in chips) 
Seat 2: Kreuz10 (435 in chips) 
Kreuz10: posts small blind 20
kszwenk: posts big blind 40
*** HOLE CARDS ***
Kreuz10: calls 20
kszwenk: checks 
*** FLOP *** [9c 9s 3c]
kszwenk: checks 
Kreuz10: checks 
*** TURN *** [9c 9s 3c] [4h]
kszwenk: checks 
Kreuz10: checks 
*** RIVER *** [9c 9s 3c 4h] [9d]
kszwenk: checks 
Kreuz10: bets 40
kszwenk: folds 
Uncalled bet (40) returned to Kreuz10
Kreuz10 collected 80 from pot
Kreuz10: doesn't show hand 
*** SUMMARY ***
Total pot 80 | Rake 0 
Board [9c 9s 3c 4h 9d]
Seat 1: kszwenk (big blind) folded on the River
Seat 2: Kreuz10 (button) (small blind) collected (80)";


        [TestMethod]
        public void GetCardsTest()
        {
            HandConverter converter = new HandConverter(hand);
            List<string> expected = new List<string>();
            expected.Add("\r\n" + "[flop]" + "\r\n" + "Cards = " + "9c, 9s, 3c");
            expected.Add("\r\n" + "[turn]" + "\r\n" + "Cards = " + "4h");
            expected.Add("\r\n" + "[river]" + "\r\n" + "Cards = " + "9d");

            List<string> actual = converter.GetStreetCards();

            Assert.AreEqual(actual[0], expected[0]);
            Assert.AreEqual(actual[1], expected[1]);
            Assert.AreEqual(actual[2], expected[2]);
        }

        [TestMethod]
        public void GetBalancesTest()
        {
            HandConverter converter = new HandConverter(hand);
            string expected = "kszwenk 1065,Kreuz10 435";

            string actual = converter.GetPlayersBalances();

            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void GetUsernameTest()
        {
            HandConverter converter = new HandConverter(hand);
            string expected = "Kreuz10";

            string actual = converter.GetUserName(1);

            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void RemoveCardsTest()
        {
            HandConverter converter = new HandConverter(hand);
            string turn = @" [9c 9s 3c] [4h]
kszwenk: checks 
Kreuz10: checks ";
            string expected = @" 
kszwenk: checks 
Kreuz10: checks ";

            string actual = converter.RemoveCardsFromAction(turn);

            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void GetBlindsTest()
        {
            HandConverter converter = new HandConverter(hand);
            string expected = "20/40";

            string actual = converter.GetBlinds();

            Assert.AreEqual(actual, expected);
        }


    }
}
