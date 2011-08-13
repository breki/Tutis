using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace OMetaSharp
{
    [Flags]
    public enum OMetaConsoleOptions
    {
        None = 0,
        CompileGrammars        = 1,
        PerformTests           = 2,
        Interactive            = 4,
        InteractiveWithSamples = Interactive | 8,
        Default = InteractiveWithSamples
    }
    public class OMetaConsoleProgram<TGrammar, 
                                     TGrammarInput,
                                     TParser, 
                                     TOptimizer,
                                     TTranslator> 
        : OMetaConsoleProgram, 
          IOMetaConsoleProgram 
        where TGrammar : OMeta<TGrammarInput>, new()
        where TParser : OMeta<char>, new() 
        where TOptimizer : Parser<HostExpression>, new()
        where TTranslator : Parser<HostExpression>, new()
    {
        private readonly Func<TParser, Rule<char>> m_TopParserRuleFetcher;
        private readonly Func<TOptimizer, Rule<HostExpression>> m_TopOptimizerRuleFetcher;
        private readonly Func<TTranslator, Rule<HostExpression>> m_TopTranslatorRuleFetcher;
        private ConsoleInput m_Input = new ConsoleInput();

        public OMetaConsoleProgram(Func<TParser, Rule<char>> topParserRuleFetcher,
                                   Func<TOptimizer, Rule<HostExpression>> topOptimizerRuleFetcher,
                                   Func<TTranslator, Rule<HostExpression>> topTranslatorRuleFetcher)
        {
            m_TopParserRuleFetcher = topParserRuleFetcher;
            m_TopOptimizerRuleFetcher = topOptimizerRuleFetcher;
            m_TopTranslatorRuleFetcher = topTranslatorRuleFetcher;
        }

        protected virtual Func<TGrammar, Rule<TGrammarInput>> DefaultGrammarRuleFetcher { get; set; }

        protected ConsoleInput Input
        {
            get
            {
                return m_Input;
            }
        }

        public virtual void PerformTests()
        {
        }

        public virtual void AddSamples()
        {
        }

        public virtual void CompileGrammars()
        {
            // We want to make the default case simple and have it
            // "magically" just work.    
            var gg = Grammars.Find<TGrammar>();
            var generatedCode = Grammars.ParseGrammarThenOptimizeThenTranslate<TGrammar, 
                                                                               TParser, 
                                                                               TOptimizer, 
                                                                               TTranslator>
                (m_TopParserRuleFetcher, 
                 m_TopOptimizerRuleFetcher, 
                 m_TopTranslatorRuleFetcher);

            Grammars.WriteGeneratedCode<TGrammar>(generatedCode, ".cs");
        }
                
        public void AssertResult(object input, object output)
        {
            var actualInput = CreateInputStream(input);
            TGrammar grammar = new TGrammar();            
            var result = Grammars.ParseWith<TGrammar, TGrammarInput>(actualInput, DefaultGrammarRuleFetcher);
                        
            if(!result.As<string>().Equals(output.ToString()))
            {
                throw new OMetaException("");
            }
        }

        protected OMetaStream<TGrammarInput> CreateInputStream(object input)
        {
            Type inputType = typeof(TGrammarInput);
            OMetaStream<TGrammarInput> actualInput = null;

            if (inputType == typeof(char))
            {
                actualInput = new StringStream(input as string) as OMetaStream<TGrammarInput>;
                Debug.Assert(actualInput != null);
            }
            else
            {
                if (!typeof(TGrammarInput).IsAssignableFrom(input.GetType()))
                {
                    input = Convert.ChangeType(input, typeof(TGrammarInput));
                }

                actualInput = new OMetaStream<TGrammarInput>((TGrammarInput)input);
            }

            return actualInput;
        }

        protected void DisplaySampleMessage()
        {
            if (!Input.HasQueuedInput)
            {
                return;
            }

            Console.WriteLine("Sample input:");
        }

        protected void AddSample(params object[] sampleCommands)
        {
            foreach (var command in sampleCommands)
            {
                Input.Add(command.ToString());
            }
        }
               
        public virtual void StartInteractiveSession()
        {
            // For now, provide isolation across statements
            // This will obviously need changed to support environments.

            DisplayWelcomeMessage();
                         
            while (true)
            {
                var prompt = string.Format("{0}> ", typeof(TGrammar).Name);
                Console.Write(prompt);
                var input = Input.Next();

                try
                {
                    var inputStream = CreateInputStream(input);
                    Execute(inputStream);                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occurred: " + ex.Message);
                }                
            }
        }

        protected virtual void Execute(OMetaStream<TGrammarInput> inputStream)
        {
            var result = Grammars.ParseWith<TGrammar, TGrammarInput>(inputStream, DefaultGrammarRuleFetcher);
            Console.WriteLine(result);
        }

        protected virtual void DisplayWelcomeMessage()
        {
            var welcomeMessage = string.Format("{0} Interactive Mode", typeof(TGrammar).Name);
            var quitInstructions = string.Format("Press Ctrl+C to exit");

            Console.WriteLine(welcomeMessage);
            Console.WriteLine(quitInstructions);
            Console.WriteLine();
            DisplaySampleMessage();
        }
    }   
    
    public class OMetaConsoleProgram
    {
        public static void Run<TProgram>(OMetaConsoleOptions options) where TProgram : OMetaConsoleProgram
        {
            TProgram program = Activator.CreateInstance<TProgram>();
            var internalProgram = program as IOMetaConsoleProgram;
            Debug.Assert(internalProgram != null);

            if ((options & OMetaConsoleOptions.CompileGrammars) != 0)
            {
                internalProgram.CompileGrammars();
            }

            if ((options & OMetaConsoleOptions.PerformTests) != 0)
            {
                internalProgram.PerformTests();
            }

            if ((options & OMetaConsoleOptions.InteractiveWithSamples) == OMetaConsoleOptions.InteractiveWithSamples)
            {
                internalProgram.AddSamples();
            }

            if ((options & OMetaConsoleOptions.Interactive) != 0)
            {
                internalProgram.StartInteractiveSession();
            }
        }

        public static void Run<TProgram>() where TProgram : OMetaConsoleProgram
        {
            Run<TProgram>(OMetaConsoleOptions.Default);
        }
    }

    public class ConsoleInput
    {
        private readonly Queue<string> m_Input = new Queue<string>();

        public void Add(string inputToEnqueue)
        {
            m_Input.Enqueue(inputToEnqueue);
        }

        public bool HasQueuedInput
        {
            get
            {
                return m_Input.Count > 0;
            }
        }

        public string Next()
        {
            if (!HasQueuedInput)
            {
                return Console.ReadLine();
            }

            string queuedInput = m_Input.Dequeue();
            // User didn't type it, so act like they did.

            Console.WriteLine(queuedInput);
            return queuedInput;
        }
    }

    public interface IOMetaConsoleProgram : ICompileGrammars, IPerformTests
    {   
        void StartInteractiveSession();
        void AddSamples();
    }

    public interface ICompileGrammars
    {
        void CompileGrammars();
    }

    public interface IPerformTests
    {
        void PerformTests();
    }
}
