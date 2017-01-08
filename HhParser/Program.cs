using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HhParser
{
    internal class Program
    {
        private const string DestinationFolder = @"C:\.net projects\HhParser\HhParser\HhParser.IntegrationTests";

        private static void Main(string[] args)
        {
            var files = Directory.GetFiles(@"C:\Users\kuite\Desktop\testsuite_2.3\testcases\source");
            var controller = new MainController();
            var handWriter = new HandWriter();
            Console.WriteLine("Choose position for user: 1-small blind, 2-big blind, 3-button");
            var input = Console.ReadLine();
            var position = int.Parse(input);

            foreach (var file in files)
            {
                Dictionary<string, PokerHand> parsedHands = controller.Parse(file, position);
                foreach (KeyValuePair<string, PokerHand> hand in parsedHands)
                {
                    handWriter.Save(hand.Value, hand.Key);
                }
            }
        }
    }
}
