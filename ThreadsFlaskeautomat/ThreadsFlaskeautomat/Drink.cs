using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsFlaskeautomat
{
    //Super class for the Drink objects.
    abstract class Drink
    {
        protected int runnerNumber;
        public string Name { get; private set; }
        public int RunnerNumber { get { return this.runnerNumber; } set { this.runnerNumber = value; } }

        public Drink(string name, int id)
        {
            this.Name = name;
            this.runnerNumber = id;
        }

    }
}
