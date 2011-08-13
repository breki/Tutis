using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace OMetaSharp
{
    public delegate void GenericStatement();

    /// <summary>
    /// Experiments with syntactic sugar.
    /// </summary>
    public static class Sugar
    {        
        // HACK
        public static OMetaList<HostExpression> Cons(params object[] items)
        {
            if (items == null)
            {
                return OMetaList<HostExpression>.Nil;
            }

            var l = new List<OMetaList<HostExpression>>();

            foreach (var o in items)
            {
                var asList = o as OMetaList<HostExpression>;
                if (asList != null)
                {
                    if (asList != OMetaList<HostExpression>.Nil)
                    {
                        l.Add(asList);
                    }
                }
                else
                {
                    l.Add(o.AsHostExpressionList());
                }
            }

            var result = OMetaList<HostExpression>.ConcatLists(l.ToArray());
            return result;

        }

        // HACK: This is here until the C# recognizer can be cleaned up a little bit to allow for Alessandro's "statement expressions"
        public static OMetaList<HostExpression> StatementCons(GenericStatement statement, params object[] items)
        {
            statement();
            return Cons(items);
        }

        public static OMetaList<HostExpression> Statement(Func<object> statement)
        {
            return statement().AsHostExpressionList();            
        }

        // HACK: Still yet another
        public static OMetaList<HostExpression> ConsWithFlatten(params object[] items)
        {
            if (items == null)
            {
                return OMetaList<HostExpression>.Nil;
            }

            var l = new List<OMetaList<HostExpression>>();

            foreach (var o in items)
            {
                var asList = o as OMetaList<HostExpression>;
                if (asList != null)
                {
                    if (asList != OMetaList<HostExpression>.Nil)
                    {
                        if (asList.IsSingleItem || asList.Any(innerList => innerList.IsSingleItem))
                        {
                            l.Add(asList);
                        }
                        else
                        {
                            foreach (OMetaList<HostExpression> currentList in asList)
                            {
                                l.Add(currentList);
                            }
                        }
                    }
                }
                else
                {
                    l.Add(o.AsHostExpressionList());
                }
            }

            var result = OMetaList<HostExpression>.ConcatLists(l.ToArray());
            return result;

        }
        
        // HACK: Another experiment
        public static OMetaList<HostExpression> HackedInnerConcat(string operation, OMetaList<HostExpression> fullList)
        {
            var l = new List<OMetaList<HostExpression>>();
            l.Add(operation.AsHostExpressionList());

            if ((fullList != null) && (fullList != OMetaList<HostExpression>.Nil))
            {
                if (fullList.Head.IsSingleItem)
                {
                    l.Add(fullList);
                }
                else
                {
                    foreach (var innerList in fullList)
                    {
                        l.Add(innerList);
                    }
                }
            }

            return OMetaList<HostExpression>.ConcatLists(l.ToArray());
        }

        public static OMetaList<HostExpression> Implode(params object[] itemsToImplode)
        {
            return Join("", itemsToImplode);
        }

        // HACK
        public static OMetaList<HostExpression> Join(string joinChars, params object[] itemsToImplode)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < itemsToImplode.Length; i++)
            {
                var asString = itemsToImplode[i] as string;
                if (asString != null)
                {
                    sb.Append(joinChars);
                    sb.Append(asString);
                    continue;
                }

                var asList = itemsToImplode[i] as OMetaList<HostExpression>;
                if (asList != null)
                {
                    foreach (OMetaList<HostExpression> currentItem in asList)
                    {
                        if (currentItem.IsSingleItem)
                        {
                            HostExpression currentItemExpr = currentItem.HeadFirstItem;
                            if (currentItemExpr.Is<string>())
                            {
                                sb.Append(joinChars);
                                sb.Append(currentItemExpr.As<string>());
                                continue;
                            }
                            else if (currentItemExpr.Is<char>())
                            {
                                sb.Append(joinChars);
                                sb.Append(currentItemExpr.As<char>());
                                continue;
                            }
                            else
                            {
                                Debug.Fail(":-(");
                            }
                        }
                        else
                        {
                            // HACK
                            sb.Append(joinChars);
                            sb.Append(Implode(currentItem).HeadFirstItem.As<string>());
                            break;
                        }
                    }
                }
                else
                {
                    Debug.Fail("");
                }
            }

            if (sb.Length > 0)
            {
                return sb.ToString().Substring(joinChars.Length).AsHostExpressionList();
            }
            else
            {
                return "".AsHostExpressionList();
            }
        }
    }
}
