using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLAT.Resources;
using FLAT.SyntaxAnalisys.FormalGrammar;

namespace FLAT.SyntaxAnalisys.LR.Generation
{
    internal class ConfiguratingSetFamily
    {
        private IContextFreeGrammar grammar;

        private ISet<ConfiguratingSet> family;

        private ConfiguratingSet startConfigSet;


        public ConfiguratingSetFamily(IContextFreeGrammar grammar)
        {
            if (grammar == null)
            {
                throw new ArgumentNullException("grammar", Strings.Grammar + Strings.NullException);
            }

            this.grammar = grammar;
        }


        public ISet<ConfiguratingSet> Family
        {
            get
            {
                if (this.family == null)
                {
                    this.ComputeFamily();
                }

                return this.family;
            }
        }

        public IContextFreeGrammar ExtendedGrammar
        {
            get { return this.grammar; }
        }

        public ConfiguratingSet StartConfiguratingSet
        {
            get
            {
                if (this.startConfigSet == null)
                {
                    this.ComputeFamily();
                }

                return this.startConfigSet;
            }
        }


        private void ComputeFamily()
        {
            this.family = new HashSet<ConfiguratingSet>();

            var axiomConfig = this.grammar.ProductionsByLeft(this.grammar.Axiom).Select(prod => new Configuration(prod));

            this.startConfigSet = new ConfiguratingSet(this.grammar, axiomConfig);

            this.family.Add(this.startConfigSet);

            this.AddSuccessorsToFamilyRecursively(this.startConfigSet);
        }

        private void AddSuccessorsToFamilyRecursively(ConfiguratingSet configuratingSet)
        {
            if (configuratingSet.HasSuccessors())
            {
                var successors = configuratingSet.Successors;

                foreach (var key in successors.Keys.ToList())
                {
                    if (this.family.Contains(successors[key]))
                    {
                        successors[key] = this.family.First(configSet => configSet.Equals(successors[key]));
                    }
                    else
                    {
                        this.family.Add(successors[key]);

                        this.AddSuccessorsToFamilyRecursively(successors[key]);
                    }
                }
            }
        }
    }
}
