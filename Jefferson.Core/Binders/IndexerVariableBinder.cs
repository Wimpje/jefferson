﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Jefferson.Binders
{
   /// <summary>
   /// Binds variables by looking up an indexer in the current context.
   /// </summary>
   public class IndexerVariableBinder : IVariableBinder
   {
      protected readonly IDictionary<String, Type> mTypeDeclarations;

      public IndexerVariableBinder(IDictionary<String, Type> variableTypeDeclarations)
      {
         Ensure.NotNull(variableTypeDeclarations, "variableTypeDeclarations");
         mTypeDeclarations = variableTypeDeclarations;
      }

      private PropertyInfo _GetIndexer(Expression currentContext)
      {
         // Todo: we could look for e.g. things like Chars... i.e. the correctly marked indexer.
         var indexer = currentContext.Type.GetProperty("Item");
         if (indexer == null)
            throw SyntaxException.Create("Context type '{0}' declares variables but provides no indexer to obtain them.", currentContext.Type.FullName);
         return indexer;
      }

      public System.Linq.Expressions.Expression BindVariable(Expression currentContext, String name)
      {
         Type varType;
         if (!mTypeDeclarations.TryGetValue(name, out varType)) return null;

         var indexer = _GetIndexer(currentContext);
         return Expression.Convert(Expression.MakeIndex(currentContext, indexer, new[] { Expression.Constant(name) }), varType);
      }

      public Expression BindVariableToValue(Expression currentContext, String name, Expression value)
      {
         mTypeDeclarations[name] = value.Type;

         var indexer = _GetIndexer(currentContext);
         var indexExpr = Expression.Property(currentContext, indexer, new[] { Expression.Constant(name) });
         return Expression.Assign(indexExpr, Expression.Convert(value, indexExpr.Type));
      }

      public Expression UnbindVariable(Expression currentContext, String name)
      {
         // todo: error incorrect
         if (!mTypeDeclarations.ContainsKey(name))
            throw SyntaxException.Create(null, "Variable '{0}' cannot be unset because it has not been set.", name);
         mTypeDeclarations.Remove(name);
         return Expression.Constant("");
      }
   }
}
