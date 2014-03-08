using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace comp2014minipl
{
    class Scope
    {
        private Dictionary<String, Variable> names;
        private Dictionary<Variable, Expression> values;
        private HashSet<Variable> constants;
        public Scope()
        {
            values = new Dictionary<Variable, Expression>();
            names = new Dictionary<String, Variable>();
            constants = new HashSet<Variable>();
        }
        public bool hasVariable(Variable var)
        {
            return values.ContainsKey(var);
        }
        public void setConstant(Variable var, bool isConstant)
        {
            if (isConstant)
            {
                constants.Add(var);
            }
            else
            {
                constants.Remove(var);
            }
        }
        public void set(Variable var, Expression val)
        {
            values[var] = val;
        }
        public void define(Variable var, TypeName type, Expression val = null)
        {
            names[var.name] = var;
            var.type = type.type;
            values[var] = val;
        }
        public Variable getVar(String var)
        {
            return names[var];
        }
        public Expression getValue(String var)
        {
            return get(names[var]);
        }
        public Expression get(Variable var)
        {
            return values[var];
        }
    }
}
