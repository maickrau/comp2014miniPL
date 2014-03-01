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
        private abstract class Connection
        {
            public int start;
            public int end;
            public abstract Connection copy();
            public abstract bool accepts(char ch);
            public void increment(int amount)
            {
                start += amount;
                end += amount;
            }
            public abstract String debugString();
        }
        private class EpsilonConnection : Connection
        {
            public EpsilonConnection(int start, int end)
            {
                this.start = start;
                this.end = end;
            }
            public override Connection copy()
            {
                return new EpsilonConnection(start, end);
            }
            public override bool accepts(char ch)
            {
                return true;
            }
            public override string debugString()
            {
                return new StringBuilder().Append(start).Append(" ").Append(end).Append(" epsilon").ToString();
            }
        }
        private class ConnectionExactCharacter : Connection
        {
            private char character; // '\0' is epsilon
            public ConnectionExactCharacter(ConnectionExactCharacter second)
            {
                this.start = second.start;
                this.end = second.end;
                this.character = second.character;
            }
            public ConnectionExactCharacter(int start, int end, char character)
            {
                this.start = start;
                this.end = end;
                this.character = character;
            }
            public override Connection copy()
            {
                return new ConnectionExactCharacter(start, end, character);
            }
            public override bool accepts(char c)
            {
                return c == character;
            }
            public override string debugString()
            {
                return new StringBuilder().Append(start).Append(" ").Append(end).Append(" ").Append(character).ToString();
            }
        }

        private int startState;
        private int numStates;
        private List<Tuple<int, int>> currentStates;
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
                System.Console.WriteLine(c.debugString());
            }
        }
        public NFA(List<char> range)
        {
            endStates = new List<int>();
            connections = new List<Connection>();
            startState = 0;
            foreach (char c in range)
            {
                connections.Add(new ConnectionExactCharacter(0, 1, c));
            }
            numStates = 2;
            endStates.Add(1);
            hasEpsilons = false;
            calculateConnectionPerState();
        }
        public NFA(String str)
        {
            endStates = new List<int>();
            connections = new List<Connection>();
            startState = 0;
            for (int i = 0; i < str.Length; i++)
            {
                connections.Add(new ConnectionExactCharacter(i, i + 1, str[i]));
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
        private void connect(int start, int end, Connection connectionType)
        {
            Connection newConnection = connectionType.copy();
            newConnection.start = start;
            newConnection.end = end;
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
            connections = second.connections.ConvertAll(c => c.copy());
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
                c.increment(amount);
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
                ret.connections.Add(c.copy());
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
        private void addConnectionToEndStates(int destination, Connection connectionType)
        {
            foreach (int i in endStates)
            {
                connect(i, destination, connectionType);
            }
        }
        public NFA closure()
        {
            NFA ret = new NFA(this);
            int oldStartState = ret.startState;
            int newStartState = ret.addState(false);
            ret.connect(newStartState, oldStartState, new EpsilonConnection(0, 0));
            ret.addConnectionToEndStates(newStartState, new EpsilonConnection(0, 0));
            ret.endStates.Add(newStartState);
            ret.startState = newStartState;
            ret.hasEpsilons = true;
            return ret;
        }
        public NFA maybe()
        {
            NFA ret = new NFA(this);
            ret.endStates.Add(ret.startState);
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
                ret.connections.Add(c.copy());
            }
            ret.calculateConnectionPerState();
            ret.addConnectionToEndStates(secondStartState, new EpsilonConnection(0, 0));
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
                if (c is EpsilonConnection)
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
                        Connection newConnection = c2.copy();
                        newConnection.start = c.start;
                        if (newConnection is EpsilonConnection)
                        {
                            newEpsilons.Add(newConnection);
                        }
                        if (endStates.Contains(c.end) && !endStates.Contains(c.start))
                        {
                            endStates.Add(c.start);
                        }
                        System.Console.WriteLine("Added {0} {1}", c.start, c2.end);
                        newConnections.Add(newConnection);
                    }
                    foreach (Connection c2 in newConnections)
                    {
                        connections.Add(c2);
                        connectionPerState[c2.start].Add(c2);
                    }
                    System.Console.WriteLine("Removed {0} {1}", c.start, c.end);
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
        public void startRecognizing()
        {
            deEpsilonate();
            currentStates = new List<Tuple<int, int>>();
            currentStates.Add(new Tuple<int, int>(startState, 0));
        }
        public void alsoRecognizeFromHere()
        {
            currentStates.Add(new Tuple<int, int>(startState, 0));
        }
        public int recognizesCurrent()
        {
            foreach (Tuple<int, int> i in currentStates)
            {
                if (endStates.Contains(i.Item1))
                {
                    return i.Item2;
                }
            }
            return -1;
        }
        public void takeCharacter(char ch)
        {
            List<Tuple<int, int>> newStates = new List<Tuple<int, int>>();
            foreach (Tuple<int, int> s in currentStates)
            {
                foreach (Connection c in connectionPerState[s.Item1])
                {
                    if (c.accepts(ch))
                    {
                        newStates.Add(new Tuple<int, int>(c.end, s.Item2+1));
                    }
                }
            }
            currentStates = newStates;
        }
        public bool recognizes(String str)
        {
            startRecognizing();
            for (int i = 0; i < str.Length; i++)
            {
                takeCharacter(str[i]);
            }
            return recognizesCurrent() > -1;
        }
    }
}
