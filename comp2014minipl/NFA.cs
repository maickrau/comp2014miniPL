using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace comp2014minipl
{
    public class NFA
    {
        private class Connection
        {
            public int start;
            public int end;
            public char character; // '\0' is epsilon
            public Connection(Connection second)
            {
                this.start = second.start;
                this.end = second.end;
                this.character = second.character;
            }
            public Connection(int start, int end, char character)
            {
                this.start = start;
                this.end = end;
                this.character = character;
            }
        }
        private int startState;
        private int numStates;
        private List<int> endStates;
        private List<Connection> connections;
        private List<List<Connection>> connectionPerState;
        private bool hasEpsilons;
        public void debugPrint()
        {
            System.Console.WriteLine(numStates);
            System.Console.WriteLine(startState);
            foreach (int i in endStates)
            {
                System.Console.Write("{0} ", i);
            }
            System.Console.WriteLine();
            foreach (Connection c in connections)
            {
                System.Console.WriteLine("{0} {1} {2}", c.start, c.end, c.character);
            }
        }
        public NFA(String str)
        {
            endStates = new List<int>();
            connections = new List<Connection>();
            startState = 0;
            for (int i = 0; i < str.Length; i++)
            {
                connections.Add(new Connection(i, i + 1, str[i]));
            }
            numStates = str.Length + 1;
            endStates.Add(str.Length);
            hasEpsilons = false;
            calculateConnectionPerState();
        }
        private int addState(bool endState)
        {
            int ret = numStates;
            numStates++;
            connectionPerState.Add(new List<Connection>());
            if (endState)
            {
                endStates.Add(ret);
            }
            return ret;
        }
        private void connect(int start, int end, char character)
        {
            Connection newConnection = new Connection(start, end, character);
            connections.Add(newConnection);
            connectionPerState[start].Add(newConnection);
        }
        private void calculateConnectionPerState()
        {
            connectionPerState = new List<List<Connection>>();
            for (int i = 0; i < numStates; i++)
            {
                connectionPerState.Add(new List<Connection>());
            }
            foreach (Connection c in connections)
            {
                connectionPerState[c.start].Add(c);
            }
        }
        public NFA(NFA second)
        {
            //we want to create a deep copy instead of modifying the original
            connections = second.connections.ConvertAll(c => new Connection(c));
            endStates = new List<int>(second.endStates);
            startState = second.startState;
            numStates = second.numStates;
            hasEpsilons = second.hasEpsilons;
            calculateConnectionPerState();
        }
        private void incrementIndices(int amount)
        {
            foreach (Connection c in connections)
            {
                c.start += amount;
                c.end += amount;
            }
            for (int i = 0; i < endStates.Count; i++)
            {
                endStates[i] += amount;
            }
            startState += amount;
            numStates += amount;
            calculateConnectionPerState();
        }
        public NFA or(NFA second)
        {
            NFA ret = new NFA(this);
            //add states
            ret.incrementIndices(second.numStates);
            foreach (int i in second.endStates)
            {
                ret.endStates.Add(i);
            }
            //add connections between newly added states
            foreach (Connection c in second.connections)
            {
                ret.connections.Add(new Connection(c));
            }
            //connect new start states with old start state
            foreach (Connection c in ret.connections)
            {
                if (c.start == second.startState)
                {
                    c.start = ret.startState;
                }
            }
            ret.hasEpsilons = hasEpsilons | second.hasEpsilons;
            ret.calculateConnectionPerState();
            return ret;
        }
        private void addConnectionToEndStates(int destination, char character)
        {
            foreach (int i in endStates)
            {
                connect(i, destination, character);
            }
        }
        public NFA closure()
        {
            NFA ret = new NFA(this);
            int oldStartState = ret.startState;
            int newStartState = ret.addState(false);
            ret.connect(newStartState, oldStartState, '\0');
            ret.addConnectionToEndStates(newStartState, '\0');
            ret.endStates.Add(newStartState);
            ret.startState = newStartState;
            ret.hasEpsilons = true;
            return ret;
        }
        public NFA conc(NFA second)
        {
            NFA ret = new NFA(this);
            ret.incrementIndices(second.numStates);
            List<int> oldEndStates = new List<int>(ret.endStates);
            int secondStartState = second.startState;
            foreach (Connection c in second.connections)
            {
                ret.connect(c.start, c.end, c.character);
            }
            ret.calculateConnectionPerState();
            ret.addConnectionToEndStates(secondStartState, '\0');
            foreach (int i in second.endStates)
            {
                ret.endStates.Add(i);
            }
            ret.hasEpsilons = true;
            foreach (int i in oldEndStates)
            {
                ret.endStates.Remove(i);
            }
            return ret;
        }
        public void deEpsilonate()
        {
            List<Connection> epsilons = new List<Connection>();
            foreach (Connection c in connections)
            {
                if (c.character == '\0')
                {
                    epsilons.Add(c);
                }
            }
            hasEpsilons = epsilons.Count != 0;
            while (hasEpsilons)
            {
                System.Console.WriteLine(epsilons.Count);
                List<Connection> newEpsilons = new List<Connection>();
                foreach (Connection c in epsilons)
                {
                    List<Connection> newConnections = new List<Connection>();
                    foreach (Connection c2 in connectionPerState[c.end])
                    {
                        Connection newConnection = new Connection(c.start, c2.end, c2.character);
                        if (c2.character == '\0')
                        {
                            newEpsilons.Add(newConnection);
                        }
                        if (endStates.Contains(c.end) && !endStates.Contains(c.start))
                        {
                            endStates.Add(c.start);
                        }
                        System.Console.WriteLine("Added {0} {1} {2}", c.start, c2.end, c2.character);
                        newConnections.Add(newConnection);
                    }
                    foreach (Connection c2 in newConnections)
                    {
                        connections.Add(c2);
                        connectionPerState[c2.start].Add(c2);
                    }
                    System.Console.WriteLine("Removed {0} {1} {2}", c.start, c.end, c.character);
                    connections.Remove(c);
                    connectionPerState[c.start].Remove(c);
                }
                epsilons = newEpsilons;
                if (epsilons.Count == 0)
                {
                    hasEpsilons = false;
                }
            }
        }
        public bool recognizes(String str)
        {
            deEpsilonate();
            List<int> currentStates = new List<int>();
            currentStates.Add(startState);
            for (int i = 0; i < str.Length; i++)
            {
                List<int> newStates = new List<int>();
                foreach (int s in currentStates)
                {
                    foreach (Connection c in connectionPerState[s])
                    {
                        if (c.character == str[i])
                        {
                            newStates.Add(c.end);
                        }
                    }
                }
                currentStates = newStates;
            }
            foreach (int i in currentStates)
            {
                if (endStates.Contains(i))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
