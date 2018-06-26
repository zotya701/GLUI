using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation
{
    public class Commands : IEnumerable<Command>
    {
        private Dictionary<string, Command> mCommands = new Dictionary<string, Command>();

        public void Add(Command command)
        {
            mCommands[command.Name] = command;
        }

        public Command this[string name]
        {
            get
            {
                return mCommands[name];
            }
            set
            {
                mCommands[name] = value;
            }
        }

        public IEnumerator<Command> GetEnumerator()
        {
            return mCommands.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
