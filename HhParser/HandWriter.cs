using System.Collections.Generic;
using System.IO;

namespace HhParser
{
    public class HandWriter
    {
        public void Save(PokerHand hand, string destination)
        {
            File.WriteAllText(destination, hand.Action);
        }
    }
}