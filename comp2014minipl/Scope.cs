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
            if (!values.ContainsKey(var))
            {
                throw new Exception("Scope: Variable assigned before definition");
            }
            if (values[var].type != val.type)
            {
                throw new Exception("Scope: Assignment with invalid type");
            }
            if (constants.Contains(var))
            {
                throw new Exception("Scope: Assignment to a constant");
            }
            values[var] = val;
        }
        public void define(Variable var, TypeName type, Expression val = null)
        {
            if (values.ContainsKey(var))
            {
                throw new Exception("Scope: Variable defined twice");
            }
            if (val != null && val.type != type.type)
            {
                throw new Exception("Scope: Variable defined with initial value of different type");
            }
            values[var] = val;
            values[var].type = type.type;
        }
        public Variable getVar(String var)
        {
            if (!names.ContainsKey(var))
            {
                throw new Exception("Scope: Can't find variable");
            }
            return names[var];
        }
        public Expression getValue(String var)
        {
            return get(new Variable(var));
        }
        public Expression get(Variable var)
        {
            if (!values.ContainsKey(var))
            {
                throw new Exception("Scope: Variable read before definition");
            }
            return values[var];
        }
    }
}
