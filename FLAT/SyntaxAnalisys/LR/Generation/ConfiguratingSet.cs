using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLAT.Resources;
using FLAT.SyntaxAnalisys.FormalGrammar;

namespace FLAT.SyntaxAnalisys.LR.Generation
{
    internal class ConfiguratingSet : IEquatable<ConfiguratingSet>
    {
        private IContextFreeGrammar grammar;

        private ISet<Configuration> closure;

        private ISet<Configuration> coreConfigurations;

        private IDictionary<Symbol, ConfiguratingSet> successors;


        public ConfiguratingSet(IContextFreeGrammar grammar, IEnumerable<Configuration> configurations)
        {
            if (grammar == null)
            {
                throw new ArgumentNullException("grammar", Strings.Grammar + Strings.NullException);
            }

            if (configurations == null)
            {
                throw new ArgumentNullException("configurations", Strings.Configurations + Strings.NullException);
            }

            if (configurations.Count() == 0)
            {
                throw new ArgumentException("No configs in set.", Strings.Configurations + Strings.EmptyException);
            }

            this.grammar = grammar;

            this.coreConfigurations = new HashSet<Configuration>(configurations);
        }

        public ConfiguratingSet(IContextFreeGrammar grammar, params Configuration[] configurations)
            : this(grammar, (IEnumerable<Configuration>)configurations) { }


        public IEnumerable<Configuration> CoreConfigurations
        {
            get { return this.coreConfigurations; }
        }

        public ISet<Configuration> Closure
        {
            get
            {
                if (this.closure == null)
                {
                    this.ComputeClosure();
                }

                return this.closure;
            }
        }

        public IDictionary<Symbol, ConfiguratingSet> Successors
        {
            get
            {
                if (this.successors == null)
                {
                    this.ComputeSuccessors();
                }

                return this.successors;
            }
        }


        public bool Equals(ConfiguratingSet other)
        {
            if (other == null)
            {
                return false;
            }

            if (other == this)
            {
                return true;
            }

            return this.coreConfigurations.SetEquals(other.CoreConfigurations);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var other = obj as ConfiguratingSet;
            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.coreConfigurations.Aggregate(0, (acc, config) => acc + config.GetHashCode());
        }

        private void ComputeClosure()
        {
            this.closure = new HashSet<Configuration>();

            var queue = new Queue<Configuration>(this.coreConfigurations);
            while (queue.Any())
            {
                var config = queue.Dequeue();
                this.closure.Add(config);

                var marked = config.MarkedSymbol;
                if (marked != null && marked.IsNonterminal())
                {
                    var query = grammar.ProductionsByLeft((Nonterminal)marked).Select(prod => new Configuration(prod));

                    foreach (var newConfig in query)
                    {
                        if (!queue.Contains(newConfig) && !this.closure.Contains(newConfig))
                        {
                            queue.Enqueue(newConfig);
                        }
                    }
                }
            }
        }

        private ISet<Configuration> GetEquivalentConfigs(Configuration configuration)
        {
            if (!configuration.IsMarkAtEnd())
            {
                var leftNontermsOfProdsInClosure = this.closure.Select(c => c.Production.Left).Distinct();
                if (configuration.MarkedSymbol.IsNonterminal()
                        && !leftNontermsOfProdsInClosure.Contains(configuration.MarkedSymbol))
                {
                    var tempSet = new HashSet<Configuration>();
                    foreach (var productionDescribedMarkedNonterm in
                            this.grammar.ProductionsByLeft((Nonterminal)configuration.MarkedSymbol))
                    {
                        var tempConfig = new Configuration(productionDescribedMarkedNonterm);
                        tempSet.Add(tempConfig);

                        var equivalentsOfTempConfig = this.GetEquivalentConfigs(tempConfig);
                        if (equivalentsOfTempConfig != null)
                        {
                            tempSet.UnionWith(equivalentsOfTempConfig);
                        }
                    }

                    return tempSet;
                }
            }

            return null;
        }

        private void ComputeSuccessors()
        {
            this.successors = new Dictionary<Symbol, ConfiguratingSet>();
            foreach (var group in this.Closure.GroupBy(config => config.MarkedSymbol))
            {
                if (group.Key != null)
                {
                    var tempConfigSet = new HashSet<Configuration>();
                    foreach (var config in group)
                    {
                        var tempConfig = new Configuration(config);
                        tempConfig.MoveMarkToNextSymbol();
                        tempConfigSet.Add(tempConfig);
                    }
                    this.successors.Add(group.Key, new ConfiguratingSet(this.grammar, tempConfigSet));
                }
            }
        }

        public bool HasSuccessors()
        {
            return this.Successors.Count != 0;
        }
    }
}
