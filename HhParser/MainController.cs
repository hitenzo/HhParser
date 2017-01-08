using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HhParser
{
    public class MainController
    {
        public Dictionary<string, PokerHand> Parse(string sourcefilePath, int position)
        {
            var hands = new Dictionary<string, PokerHand>();
            var content = File.ReadAllText(sourcefilePath);
            var rawHands = content.Split(new string[] { "PokerStars Hand #" }, StringSplitOptions.None);
            var destinationPath = @"C:\.net projects\HhParser\HhParser\HhParser.IntegrationTests\";
            var parseController = new HandParseController();

            foreach (var hand in rawHands)
            {
                if (hand.Length > 30)
                {
                    PokerHand converted = parseController.Parse(hand, position);
                    string fileName = GetHandNumber(hand) + ".txt";
                    string path = String.Concat(destinationPath, "parsed\\", fileName);
                    
                    hands.Add(path, converted);
                }
            }

            return hands;
        }

        private string GetHandNumber(string handSource)
        {
            var handNumber = handSource.Substring(0, 12);
            handNumber = Regex.Replace(handNumber, @"\s+", "");
            return handNumber;
        }
    }
}
