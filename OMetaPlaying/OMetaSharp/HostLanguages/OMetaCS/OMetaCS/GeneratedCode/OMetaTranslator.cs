using System;

namespace OMetaSharp
{
    public class OMetaTranslator : Parser<HostExpression>
    {
        public virtual bool Trans(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> t = null;
            OMetaList<HostExpression> ans = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Form(
                        delegate(OMetaStream<HostExpression> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <HostExpression> modifiedStream3)
                        {
                            modifiedStream3 = inputStream3;
                            if(!MetaRules.Apply(
                                delegate(OMetaStream<HostExpression> inputStream4, out OMetaList<HostExpression> result4, out OMetaStream <HostExpression> modifiedStream4)
                                {
                                    modifiedStream4 = inputStream4;
                                    if(!MetaRules.Apply(Anything, modifiedStream4, out result4, out modifiedStream4))
                                    {
                                        return MetaRules.Fail(out result4, out modifiedStream4);
                                    }
                                    t = result4;
                                    if(!MetaRules.ApplyWithArgs(Apply, modifiedStream4, out result4, out modifiedStream4, (t).AsHostExpressionList()))
                                    {
                                        return MetaRules.Fail(out result4, out modifiedStream4);
                                    }
                                    ans = result4;
                                    return MetaRules.Success();
                                }, modifiedStream3, out result3, out modifiedStream3))
                            {
                                return MetaRules.Fail(out result3, out modifiedStream3);
                            }
                            return MetaRules.Success();
                        }
                    , modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    result2 = ( ans ).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool Success(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            modifiedStream = inputStream;
            result = ( CodeGen.Create()
            			  .WriteLine("return MetaRules.Success();")
            			 ).AsHostExpressionList();
            return MetaRules.Success();
        }
        public virtual bool Fail(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            modifiedStream = inputStream;
            result = ( CodeGen.Create()
             		   .WriteLine("{")
             		   .PushIndent()
            	  	   .Write("return MetaRules.Fail(out ")
            		   .WriteLeveledVar("result")
            		   .Write(", out ")
            		   .WriteLeveledVar("modifiedStream")
            		   .WriteLineEnd(");")  
            		   .PopIndent()
            		   .WriteLine("}")	 
            		  ).AsHostExpressionList();
            return MetaRules.Success();
        }
        public virtual bool ResultAndOutputUsage(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            modifiedStream = inputStream;
            result = ( CodeGen.Create()
            					       .Write(", out ")
            					       .WriteLeveledVar("result")
            					       .Write(", out ")
            					       .WriteLeveledVar("modifiedStream")					       
            					      ).AsHostExpressionList();
            return MetaRules.Success();
        }
        public virtual bool Act(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> expr = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    expr = result2;
                    result2 = ( CodeGen.Create()
                    			    .Write()
                    			    .WriteLeveledVar("result")
                    			    .Write(" = (", expr, ").AsHostExpressionList()")
                    			    .WriteLineEnd(";")  
                    			   ).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool App(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> rule = null;
            OMetaList<HostExpression> f = null;
            OMetaList<HostExpression> o = null;
            OMetaList<HostExpression> args = null;
            modifiedStream = inputStream;
            if(!MetaRules.Or(modifiedStream, out result, out modifiedStream,
            delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                if(!MetaRules.Apply(
                    delegate(OMetaStream<HostExpression> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <HostExpression> modifiedStream3)
                    {
                        modifiedStream3 = inputStream3;
                        if(!MetaRules.ApplyWithArgs(Token, modifiedStream3, out result3, out modifiedStream3, ("Super").AsHostExpressionList()))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        if(!MetaRules.Apply(Anything, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        rule = result3;
                        if(!MetaRules.Apply(Fail, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        f = result3;
                        if(!MetaRules.Apply(ResultAndOutputUsage, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        o = result3;
                        result3 = ( CodeGen.Create()
                        					   	        .Write("if(!MetaRules.Apply(__baseRule__, ")
                        					   	        .WriteLeveledVar("modifiedStream")
                        					   	        .Write(o)					   	    
                        					   	        .WriteLineEnd("))")
                        					   	        .Write(f)
                        					   	       ).AsHostExpressionList();
                        return MetaRules.Success();
                    }, modifiedStream2, out result2, out modifiedStream2))
                {
                    return MetaRules.Fail(out result2, out modifiedStream2);
                }
                return MetaRules.Success();
            }
            ,delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                if(!MetaRules.Apply(
                    delegate(OMetaStream<HostExpression> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <HostExpression> modifiedStream3)
                    {
                        modifiedStream3 = inputStream3;
                        if(!MetaRules.Apply(Anything, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        rule = result3;
                        if(!MetaRules.Many1(
                            delegate(OMetaStream<HostExpression> inputStream4, out OMetaList<HostExpression> result4, out OMetaStream <HostExpression> modifiedStream4)
                            {
                                modifiedStream4 = inputStream4;
                                if(!MetaRules.Apply(Anything, modifiedStream4, out result4, out modifiedStream4))
                                {
                                    return MetaRules.Fail(out result4, out modifiedStream4);
                                }
                                return MetaRules.Success();
                            }
                        , modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        args = result3;
                        if(!MetaRules.Apply(Fail, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        f = result3;
                        if(!MetaRules.Apply(ResultAndOutputUsage, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        o = result3;
                        result3 = ( CodeGen.Create()
                        					   	        .Write("if(!MetaRules.ApplyWithArgs(", rule, ", ")
                        					   	        .WriteLeveledVar("modifiedStream")
                        					   	        .Write(o)					 
                        					   	        .Do(
                        					   				cg => {
                        					   						foreach(var currentArg in args)
                        					   						{					   							
                        					   							cg.Write(", (", currentArg, ").AsHostExpressionList()");
                        					   						}
                        					   					  }
                        					   	        )  	    
                        					   	        .WriteLineEnd("))")
                        					   	        .Write(f)
                        					   	       ).AsHostExpressionList();
                        return MetaRules.Success();
                    }, modifiedStream2, out result2, out modifiedStream2))
                {
                    return MetaRules.Fail(out result2, out modifiedStream2);
                }
                return MetaRules.Success();
            }
            ,delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                if(!MetaRules.Apply(
                    delegate(OMetaStream<HostExpression> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <HostExpression> modifiedStream3)
                    {
                        modifiedStream3 = inputStream3;
                        if(!MetaRules.Apply(Anything, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        rule = result3;
                        if(!MetaRules.Apply(Fail, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        f = result3;
                        if(!MetaRules.Apply(ResultAndOutputUsage, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        o = result3;
                        result3 = ( CodeGen.Create()
                        					   	        .Write("if(!MetaRules.Apply(", rule, ", ")
                        					   	        .WriteLeveledVar("modifiedStream")
                        					   	        .Write(o)					   	    
                        					   	        .WriteLineEnd("))")
                        					   	        .Write(f)
                        					   	       ).AsHostExpressionList();
                        return MetaRules.Success();
                    }, modifiedStream2, out result2, out modifiedStream2))
                {
                    return MetaRules.Fail(out result2, out modifiedStream2);
                }
                return MetaRules.Success();
            }
            ))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool Set(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> n = null;
            OMetaList<HostExpression> v = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    n = result2;
                    if(!MetaRules.Apply(Trans, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    v = result2;
                    result2 = ( CodeGen.Create()
                    						   .Write(v)
                    						   .Write(n, " = ")
                    						   .WriteLeveledVar("result")
                    						   .WriteLineEnd(";") 
                    						 ).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool InputDecl(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            modifiedStream = inputStream;
            result = ( CodeGen.Create()
            	            .Write("(")
            	            .Write("OMetaStream<", Get<string>("CodeType"), "> ")
            	            .WriteLeveledVar("inputStream")
            				.Write(", out OMetaList<HostExpression> ")
            				.WriteLeveledVar("result")
            				.Write(", out OMetaStream <", Get<string>("CodeType"), "> ")
            				.WriteLeveledVar("modifiedStream")
            				.WriteLineEnd(")")
            			   ).AsHostExpressionList();
            return MetaRules.Success();
        }
        public virtual bool DelegateDeclaration(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> id = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(InputDecl, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    id = result2;
                    result2 = ( CodeGen.Create()
                    			   	     .WriteLine("delegate", id)
                    					).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool InitialModSet(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            modifiedStream = inputStream;
            result = ( CodeGen.Create()
            					.WriteLeveledVar("modifiedStream")
            					.Write(" = ")
            					.WriteLeveledVar("inputStream")		 
            					.WriteLine(";")
            				   ).AsHostExpressionList();
            return MetaRules.Success();
        }
        public virtual bool And(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> xs = null;
            OMetaList<HostExpression> y = null;
            OMetaList<HostExpression> suc = null;
            OMetaList<HostExpression> delDecl = null;
            OMetaList<HostExpression> ims = null;
            OMetaList<HostExpression> resultUsage = null;
            OMetaList<HostExpression> f = null;
            modifiedStream = inputStream;
            if(!MetaRules.Or(modifiedStream, out result, out modifiedStream,
            delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                if(!MetaRules.Apply(
                    delegate(OMetaStream<HostExpression> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <HostExpression> modifiedStream3)
                    {
                        modifiedStream3 = inputStream3;
                        if(!MetaRules.Many(
                            delegate(OMetaStream<HostExpression> inputStream4, out OMetaList<HostExpression> result4, out OMetaStream <HostExpression> modifiedStream4)
                            {
                                modifiedStream4 = inputStream4;
                                if(!MetaRules.ApplyWithArgs(NotLast, modifiedStream4, out result4, out modifiedStream4, ("Trans").AsHostExpressionList()))
                                {
                                    return MetaRules.Fail(out result4, out modifiedStream4);
                                }
                                return MetaRules.Success();
                            }
                        , modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        xs = result3;
                        if(!MetaRules.Apply(Trans, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        y = result3;
                        if(!MetaRules.Apply(Success, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        suc = result3;
                        if(!MetaRules.Apply(DelegateDeclaration, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        delDecl = result3;
                        if(!MetaRules.Apply(InitialModSet, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        ims = result3;
                        if(!MetaRules.Apply(ResultAndOutputUsage, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        resultUsage = result3;
                        if(!MetaRules.Apply(Fail, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        f = result3;
                        result3 = ( CodeGen.Create()
                        	           .WriteLine("if(!MetaRules.Apply(")
                        	           .PushVariableScope()
                        	           .PushIndent()
                        	           .WriteLine(delDecl)	           
                        	           .WriteLine("{")	  
                        	           .PushIndent()        			   	           
                        	           .Write(ims)
                        			   .Do(
                        				   cg => {
                        							foreach(var currentTrans in xs)
                        							{
                        								cg.Write(currentTrans);
                        							}
                        							cg.Write(y);
                        							cg.Write(suc);
                        						 })	
                        			   .PopIndent()		   		   
                        			   .Write("}")
                        			   .PopVariableScope()
                        			   .PopIndent()
                        			   .Write(", ")
                        			   .WriteLeveledVar("modifiedStream")
                        			   .WriteLineEnd(resultUsage, "))")
                        			   .Write(f)
                        			  ).AsHostExpressionList();
                        return MetaRules.Success();
                    }, modifiedStream2, out result2, out modifiedStream2))
                {
                    return MetaRules.Fail(out result2, out modifiedStream2);
                }
                return MetaRules.Success();
            }
            ,delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                result2 = ( CodeGen.Create() ).AsHostExpressionList();
                return MetaRules.Success();
            }
            ))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool MetaApplyFormat(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> x = null;
            OMetaList<HostExpression> u = null;
            OMetaList<HostExpression> f = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(TransFn, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    x = result2;
                    if(!MetaRules.Apply(ResultAndOutputUsage, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    u = result2;
                    if(!MetaRules.Apply(Fail, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    f = result2;
                    result2 = ( CodeGen.Create()
                    				     .WriteLine("if(!MetaRules.{0}(")
                      					 .PushIndent()
                    					 .Write(x)
                    					 .PopIndent()
                    					 .Write(", ")
                    					 .WriteLeveledVar("modifiedStream")
                    					 .Write(u)
                    					 .WriteLine("))")
                    					 .Write(f)
                    				     ).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool Many(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> f = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(MetaApplyFormat, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    f = result2;
                    result2 = ( CodeGen.Format(f, "Many") ).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool Many1(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> f = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(MetaApplyFormat, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    f = result2;
                    result2 = ( CodeGen.Format(f, "Many1") ).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool Not(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> f = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(MetaApplyFormat, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    f = result2;
                    result2 = ( CodeGen.Format(f, "Not") ).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool Lookahead(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> f = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(MetaApplyFormat, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    f = result2;
                    result2 = ( CodeGen.Format(f, "Lookahead") ).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool Form(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> f = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(MetaApplyFormat, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    f = result2;
                    result2 = ( CodeGen.Format(f, "Form") ).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool Or(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> xs = null;
            OMetaList<HostExpression> f = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Many(
                        delegate(OMetaStream<HostExpression> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <HostExpression> modifiedStream3)
                        {
                            modifiedStream3 = inputStream3;
                            if(!MetaRules.Apply(TransFn, modifiedStream3, out result3, out modifiedStream3))
                            {
                                return MetaRules.Fail(out result3, out modifiedStream3);
                            }
                            return MetaRules.Success();
                        }
                    , modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    xs = result2;
                    if(!MetaRules.Apply(Fail, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    f = result2;
                    result2 = ( CodeGen.Create().Do(
                    					    cg => {
                    								cg.Write("if(!MetaRules.Or(")
                    								  .WriteLeveledVar("modifiedStream")
                    								  .Write(", out ")
                    								  .WriteLeveledVar("result")
                    								  .Write(", out ")
                    								  .WriteLeveledVar("modifiedStream")
                    								  .WriteLine(",");
                    								var moreThanOne = false;
                    								foreach(var l in xs)
                    								{
                    									if(moreThanOne)
                    									{
                    										cg.Write(",");
                    									}									
                    									cg.Write(l);
                    									moreThanOne = true;
                    								}
                    								cg.WriteLine("))")
                    								  .Write(f);
                    							  })
                    				      ).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool Pred(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> expr = null;
            OMetaList<HostExpression> f = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    expr = result2;
                    if(!MetaRules.Apply(Fail, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    f = result2;
                    result2 = ( CodeGen.Create()
                    				 .WriteLine("if(!(", expr, "))")				 
                    				 .Write(f)
                    				 .WriteLeveledVar("result")
                    				 .WriteLine(" = true.AsHostExpressionList();")
                    				).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool Rule(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> name = null;
            OMetaList<HostExpression> over = null;
            OMetaList<HostExpression> id = null;
            OMetaList<HostExpression> ims = null;
            OMetaList<HostExpression> ls = null;
            OMetaList<HostExpression> body = null;
            OMetaList<HostExpression> s = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    name = result2;
                    if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    over = result2;
                    if(!MetaRules.Apply(InputDecl, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    id = result2;
                    if(!MetaRules.Apply(InitialModSet, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    ims = result2;
                    if(!MetaRules.Apply(Locals, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    ls = result2;
                    if(!MetaRules.Apply(Trans, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    body = result2;
                    if(!MetaRules.Apply(Success, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    s = result2;
                    result2 = ( CodeGen.Create()
                    			    .WriteLine("public ", over.As<bool>() ? "override" : "virtual", " bool ", name, id)
                    			    .WriteLine("{")
                    			    .PushIndent()	
                    			    .Do(cg=> {
                    							if(over.As<bool>())
                    							{
                    								cg.WriteLine("Rule<", Get<string>("CodeType"), "> __baseRule__ = base.", name.As<string>(), ";");
                    							}
                    						 })								    
                    			    .Write(ls, ims,  body, s)				   
                    			    .PopIndent()
                    			    .WriteLine("}")
                    			   ).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool Locals(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> ls = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    ls = result2;
                    result2 = ( CodeGen.Create().Do(
                    				   cg => {
                    							foreach(var l in ls.As<VariableSet>())
                    							{
                    								cg.WriteLine("OMetaList<HostExpression> ", l, " = null;");
                    							}
                    						 }
                    				  )							
                    				).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool Grammar(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> usings = null;
            OMetaList<HostExpression> ns = null;
            OMetaList<HostExpression> name = null;
            OMetaList<HostExpression> gtd = null;
            OMetaList<HostExpression> sName = null;
            OMetaList<HostExpression> btd = null;
            OMetaList<HostExpression> rules = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    usings = result2;
                    if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    ns = result2;
                    if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    name = result2;
                    if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    gtd = result2;
                    result2 = (Sugar.StatementCons(
                    								() => {
                    										var codeType = gtd[0].As<string>();
                    										if(string.IsNullOrEmpty(codeType))
                    										{
                    											codeType = "char";
                    										}
                    										Set("CodeType", codeType);
                    										
                    										var defaultType = gtd[1].As<string>();
                    										if(!string.IsNullOrEmpty(defaultType))
                    										{
                    											Set("DefaultType", defaultType);
                    										}
                    									  }, true)).AsHostExpressionList();
                    if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    sName = result2;
                    if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    btd = result2;
                    if(!MetaRules.Many(
                        delegate(OMetaStream<HostExpression> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <HostExpression> modifiedStream3)
                        {
                            modifiedStream3 = inputStream3;
                            if(!MetaRules.Apply(Trans, modifiedStream3, out result3, out modifiedStream3))
                            {
                                return MetaRules.Fail(out result3, out modifiedStream3);
                            }
                            return MetaRules.Success();
                        }
                    , modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    rules = result2;
                    result2 = ( CodeGen.Create()
                    						   .Do(
                    						   cg => {
                    									foreach(var u in usings)
                    									{
                    										cg.WriteLine(u.As<string>());
                    									}
                    								 }
                    						   )
                    					  	   .WriteLine()						
                    						   .WriteLine("namespace " + ns.As<string>())
                    						   .WriteLine("{")								     
                    						   .PushIndent()
                    						   .Write("public class ", name)						   
                    						   .Write(" : ", sName)						   	
                    						   .Do(
                    							cg => {
                    								    var baseType = btd.As<string>();
                    								    if(!string.IsNullOrEmpty(baseType))
                    								    {
                    										cg.Write("<" + baseType + ">");
                    								    }									
                    								  }
                    						   )
                    						   .WriteLine()
                    						   .WriteLine("{")
                    						   .PushIndent()
                    						   .Do(
                    						    cg => {
                    									foreach(var currentRule in rules)
                    									{
                    										cg.Write(currentRule);									
                    									}
                    								  }
                    						   )						   
                    						   .PopIndent()
                    						   .WriteLine("}")								     
                    						   .PopIndent()
                    						   .WriteLine("}")
                    						  ).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool TransFn(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> x = null;
            OMetaList<HostExpression> s = null;
            OMetaList<HostExpression> ims = null;
            OMetaList<HostExpression> deldecl = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(Trans, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    x = result2;
                    if(!MetaRules.Apply(Success, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    s = result2;
                    if(!MetaRules.Apply(InitialModSet, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    ims = result2;
                    if(!MetaRules.Apply(DelegateDeclaration, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    deldecl = result2;
                    result2 = ( CodeGen.Create()
                    				        .PushVariableScope()
                    						.WriteLine(deldecl)
                    						.WriteLine("{")
                    						.PushIndent()						
                    						.Write(ims, x, s)						
                    						.PopIndent()
                    						.WriteLine("}")
                    						.PopVariableScope()
                    					   ).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
    }
}
