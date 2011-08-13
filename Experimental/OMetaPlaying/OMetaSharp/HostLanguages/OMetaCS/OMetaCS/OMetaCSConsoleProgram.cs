using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMetaSharp;

namespace OMetaSharp.OMetaCS
{
    public class OMetaCSConsoleProgram<TGrammar, TGrammarInput> 
        : OMetaConsoleProgram<TGrammar, 
                              TGrammarInput,
                              OMetaParser, 
                              OMetaOptimizer,
                              OMetaTranslator>
        where TGrammar : OMeta<TGrammarInput>, new()
    {
        public OMetaCSConsoleProgram()
            : base(parser => parser.Grammar,
                   optimizer => optimizer.OptimizeGrammar,
                   translator => translator.Trans)
        {
        }
    }

    public class OMetaCSConsoleProgram<TGrammar>
        : OMetaCSConsoleProgram<TGrammar, char>
          where TGrammar : OMeta<char>, new()
    {        
    }
}
