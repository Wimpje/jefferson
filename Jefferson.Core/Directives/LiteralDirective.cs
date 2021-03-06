﻿using Jefferson.Output;
using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Jefferson.Directives
{
   /// <summary>
   /// Implements #literal directive which simply outputs the source it contains.
   /// NOTE: when used with ReplaceDeep, the body of the #literal directive will be evaluated in a second run.
   /// NOTE (ii): cannot be nested currently
   /// </summary>
   [DebuggerDisplay("#{Name}")]
   public class LiteralDirective : IDirective
   {
      public virtual String Name
      {
         get { return "literal"; }
      }

      public virtual String[] ReservedWords
      {
         get { return null; }
      }

      public virtual Expression Compile(Parsing.TemplateParserContext parserContext, String arguments, String source)
      {
         if (!String.IsNullOrWhiteSpace(arguments)) throw parserContext.SyntaxError(0, "#literal directive does not take arguments");
         if (source == null) throw parserContext.SyntaxError(0, "#literal directive may not be empty");
         return Expression.Call(parserContext.Output, Utils.GetMethod<IOutputWriter>(b => b.Write("")), Expression.Constant(source));
      }
   }
}
