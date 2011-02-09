
using System;

namespace Parser.Coco {



public class Parser {
	public const int _EOF = 0;
	public const int _literal = 1;
	public const int _tagName = 2;
	public const int _tagNameQuoted = 3;
	public const int _functionName = 4;
	public const int maxT = 9;

	const bool T = true;
	const bool x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

public LabelDefinition ParsedLabelDefinition
    {
        get
        {
            return parsedLabelDefinition;
        }
    }

    private LabelDefinition parsedLabelDefinition;



	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void Label() {
		LabelDefinition labelDef; 
		LabelDefinition(out labelDef);
		parsedLabelDefinition = labelDef; 
	}

	void LabelDefinition(out LabelDefinition labelDef) {
		labelDef = new LabelDefinition(); ILabelPart part; 
		if (la.kind == 5) {
			Get();
		} else if (StartOf(1)) {
			LabelPart(out part);
			labelDef.AddPart(part); 
			while (StartOf(1)) {
				LabelPart(out part);
				labelDef.AddPart(part); 
			}
		} else SynErr(10);
	}

	void LabelPart(out ILabelPart part) {
		part = null; 
		if (la.kind == 1) {
			LiteralText(out part);
		} else if (la.kind == 2 || la.kind == 3) {
			TagReference(out part);
		} else if (la.kind == 4) {
			FunctionCall(out part);
		} else SynErr(11);
	}

	void LiteralText(out ILabelPart part) {
		Expect(1);
		part = new Literal(t.val.Substring(1, t.val.Length-2)); 
	}

	void TagReference(out ILabelPart part) {
		part = null; 
		if (la.kind == 2) {
			Get();
			part = new TagReference(t.val); 
		} else if (la.kind == 3) {
			Get();
			part = new TagReference(t.val.Substring(2, t.val.Length - 4)); 
		} else SynErr(12);
	}

	void FunctionCall(out ILabelPart part) {
		FunctionCall func = new FunctionCall(); 
		Expect(4);
		func.FunctionName = t.val.Substring(1); 
		Expect(6);
		while (StartOf(2)) {
			FunctionArgs(func);
		}
		part = func; 
		Expect(7);
	}

	void FunctionArgs(FunctionCall func) {
		FunctionArgument arg; 
		FunctionArg(out arg);
		func.AddArgument(arg); 
		while (la.kind == 8) {
			Get();
			FunctionArg(out arg);
			func.AddArgument(arg); 
		}
	}

	void FunctionArg(out FunctionArgument arg) {
		LabelDefinition labelDef; 
		LabelDefinition(out labelDef);
		arg = new FunctionArgument (labelDef); 
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		Label();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x},
		{x,T,T,T, T,x,x,x, x,x,x},
		{x,T,T,T, T,T,x,x, x,x,x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "literal expected"; break;
			case 2: s = "tagName expected"; break;
			case 3: s = "tagNameQuoted expected"; break;
			case 4: s = "functionName expected"; break;
			case 5: s = "\"@none\" expected"; break;
			case 6: s = "\"(\" expected"; break;
			case 7: s = "\")\" expected"; break;
			case 8: s = "\",\" expected"; break;
			case 9: s = "??? expected"; break;
			case 10: s = "invalid LabelDefinition"; break;
			case 11: s = "invalid LabelPart"; break;
			case 12: s = "invalid TagReference"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public virtual void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public virtual void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public virtual void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
}