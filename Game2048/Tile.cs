using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2048
{
    public class Tile
    {
        public int Value { get; set; }

        public bool IsEmpty => Value == 0;

        public Tile(int value = 0)
        {
            Value = value;
        }
    }
}
