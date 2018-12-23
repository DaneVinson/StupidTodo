using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.ClientConsole
{
    public class TheBeer
    {
        public string Name { get; set; }
        public Brewery Brewery { get; set; }
        public double Abv { get; set; }
        public double Ibu { get; set; }
    }

    public class Brewery
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public double Rating { get; set; }
    }
}
