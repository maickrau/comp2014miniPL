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
                throw new MiniPLException("Scope: Variable " + var.name + " assigned before definition");
            }
            if (values[var].type != val.type)
            {
                throw new MiniPLException("Scope: Assignment to " + var.name + " with invalid type");
            }
            if (constants.Contains(var))
            {
                throw new MiniPLException("Scope: Assignment to constant " + var.name);
            }
            values[var] = val;
        }
        public void define(Variable var, TypeName type, Expression val = null)
        {
            if (values.ContainsKey(var))
            {
                throw new MiniPLException("Scope: Variable " + var.name + " defined twice");
            }
            if (val != null && val.type != type.type)
            {
                throw new MiniPLException("Scope: Variable " + var.name + " defined with initial value of different type");
            }
            names[var.name] = var;
            var.type = type.type;
            values[var] = val;
        }
        public Variable getVar(String var)
        {
            if (!names.ContainsKey(var))
            {
                throw new MiniPLException("Scope: Can't find variable " + var);
            }
            return names[var];
        }
        public Expression getValue(String var)
        {
            if (!names.ContainsKey(var))
            {
                throw new MiniPLException("Scope: Can't find variable " + var);
            }
            return get(names[var]);
        }
        public Expression get(Variable var)
        {
            if (!values.ContainsKey(var))
            {
                throw new MiniPLException("Scope: Variable " + var.name + " read before definition");
            }
            return values[var];
        }
    }
}
