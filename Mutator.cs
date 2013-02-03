//-----------------------------------------------------------------------------
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the Microsoft Public License.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableContracts;
using System.Diagnostics.Contracts;

//^ using Microsoft.Contracts;

namespace Microsoft.Cci.MutableCodeModel {

  /// <summary>
  /// A class that traverses a mutable code and metadata model in depth first, left to right order,
  /// rewriting each mutable node it visits by updating the node's children with recursivly rewritten nodes.
  /// </summary>
  public class CodeRewriter : MetadataRewriter {

    /// <summary>
    /// A class that traverses a mutable code and metadata model in depth first, left to right order,
    /// rewriting each mutable node it visits by updating the node's children with recursivly rewritten nodes.
    /// </summary>
    /// <param name="host">An object representing the application that is hosting this rewriter. It is used to obtain access to some global
    /// objects and services such as the shared name table and the table for interning references.</param>
    /// <param name="copyAndRewriteImmutableReferences">
    /// If true, the rewriter replaces frozen or immutable references with shallow copies.
    /// Mutable method definitions that are being used as method references are considered to be "frozen" and so also
    /// get copied.
    /// </param>
    public CodeRewriter(IMetadataHost host, bool copyAndRewriteImmutableReferences = false)
      : base(host, copyAndRewriteImmutableReferences) {
      this.dispatchingVisitor = new Dispatcher() { rewriter = this };
    }

    Dispatcher dispatchingVisitor;
    class Dispatcher : MetadataVisitor, ICodeVisitor {

      internal CodeRewriter rewriter;
      internal object result;

      public void Visit(IAddition addition) {
        this.result = this.rewriter.Rewrite(addition);
      }

      public void Visit(IAddressableExpression addressableExpression) {
        this.result = this.rewriter.Rewrite(addressableExpression);
      }

      public void Visit(IAddressDereference addressDereference) {
        this.result = this.rewriter.Rewrite(addressDereference);
      }

      public void Visit(IAddressOf addressOf) {
        this.result = this.rewriter.Rewrite(addressOf);
      }

      public void Visit(IAnonymousDelegate anonymousDelegate) {
        this.result = this.rewriter.Rewrite(anonymousDelegate);
      }

      public void Visit(IArrayIndexer arrayIndexer) {
        this.result = this.rewriter.Rewrite(arrayIndexer);
      }

      public void Visit(IAssertStatement assertStatement) {
        this.result = this.rewriter.Rewrite(assertStatement);
      }

      public void Visit(IAssignment assignment) {
        this.result = this.rewriter.Rewrite(assignment);
      }

      public void Visit(IAssumeStatement assumeStatement) {
        this.result = this.rewriter.Rewrite(assumeStatement);
      }

      public void Visit(IBitwiseAnd bitwiseAnd) {
        this.result = this.rewriter.Rewrite(bitwiseAnd);
      }

      public void Visit(IBitwiseOr bitwiseOr) {
        this.result = this.rewriter.Rewrite(bitwiseOr);
      }

      public void Visit(IBlockExpression blockExpression) {
        this.result = this.rewriter.Rewrite(blockExpression);
      }

      public void Visit(IBlockStatement block) {
        this.result = this.rewriter.Rewrite(block);
      }

      public void Visit(IBreakStatement breakStatement) {
        this.result = this.rewriter.Rewrite(breakStatement);
      }

      public void Visit(IBoundExpression boundExpression) {
        this.result = this.rewriter.Rewrite(boundExpression);
      }

      public void Visit(ICastIfPossible castIfPossible) {
        this.result = this.rewriter.Rewrite(castIfPossible);
      }

      public void Visit(ICatchClause catchClause) {
        this.result = this.rewriter.Rewrite(catchClause);
      }

      public void Visit(ICheckIfInstance checkIfInstance) {
        this.result = this.rewriter.Rewrite(checkIfInstance);
      }

      public void Visit(ICompileTimeConstant constant) {
        this.result = this.rewriter.Rewrite(constant);
      }

      public void Visit(IConversion conversion) {
        this.result = this.rewriter.Rewrite(conversion);
      }

      public void Visit(IConditional conditional) {
        this.result = this.rewriter.Rewrite(conditional);
      }

      public void Visit(IConditionalStatement conditionalStatement) {
        this.result = this.rewriter.Rewrite(conditionalStatement);
      }

      public void Visit(IContinueStatement continueStatement) {
        this.result = this.rewriter.Rewrite(continueStatement);
      }

      public void Visit(ICopyMemoryStatement copyMemoryBlock) {
        this.result = this.rewriter.Rewrite(copyMemoryBlock);
      }

      public void Visit(ICreateArray createArray) {
        this.result = this.rewriter.Rewrite(createArray);
      }

      public void Visit(ICreateDelegateInstance createDelegateInstance) {
        this.result = this.rewriter.Rewrite(createDelegateInstance);
      }

      public void Visit(ICreateObjectInstance createObjectInstance) {
        this.result = this.rewriter.Rewrite(createObjectInstance);
      }

      public void Visit(IDebuggerBreakStatement debuggerBreakStatement) {
        this.result = this.rewriter.Rewrite(debuggerBreakStatement);
      }

      public void Visit(IDefaultValue defaultValue) {
        this.result = this.rewriter.Rewrite(defaultValue);
      }

      public void Visit(IDivision division) {
        this.result = this.rewriter.Rewrite(division);
      }

      public void Visit(IDoUntilStatement doUntilStatement) {
        this.result = this.rewriter.Rewrite(doUntilStatement);
      }

      public void Visit(IDupValue dupValue) {
        this.result = this.rewriter.Rewrite((DupValue)dupValue);
      }

      public void Visit(IEmptyStatement emptyStatement) {
        this.result = this.rewriter.Rewrite((EmptyStatement)emptyStatement);
      }

      public void Visit(IEquality equality) {
        this.result = this.rewriter.Rewrite(equality);
      }

      public void Visit(IExclusiveOr exclusiveOr) {
        this.result = this.rewriter.Rewrite(exclusiveOr);
      }

      public void Visit(IExpressionStatement expressionStatement) {
        this.result = this.rewriter.Rewrite(expressionStatement);
      }

      public void Visit(IFillMemoryStatement fillMemoryStatement) {
        this.result = this.rewriter.Rewrite(fillMemoryStatement);
      }

      public void Visit(IForEachStatement forEachStatement) {
        this.result = this.rewriter.Rewrite((ForEachStatement)forEachStatement);
      }

      public void Visit(IForStatement forStatement) {
        this.result = this.rewriter.Rewrite(forStatement);
      }

      public void Visit(IGotoStatement gotoStatement) {
        this.result = this.rewriter.Rewrite(gotoStatement);
      }

      public void Visit(IGotoSwitchCaseStatement gotoSwitchCaseStatement) {
        this.result = this.rewriter.Rewrite(gotoSwitchCaseStatement);
      }

      public void Visit(IGetTypeOfTypedReference getTypeOfTypedReference) {
        this.result = this.rewriter.Rewrite(getTypeOfTypedReference);
      }

      public void Visit(IGetValueOfTypedReference getValueOfTypedReference) {
        this.result = this.rewriter.Rewrite(getValueOfTypedReference);
      }

      public void Visit(IGreaterThan greaterThan) {
        this.result = this.rewriter.Rewrite(greaterThan);
      }

      public void Visit(IGreaterThanOrEqual greaterThanOrEqual) {
        this.result = this.rewriter.Rewrite(greaterThanOrEqual);
      }

      public void Visit(ILabeledStatement labeledStatement) {
        this.result = this.rewriter.Rewrite((LabeledStatement)labeledStatement);
      }

      public void Visit(ILeftShift leftShift) {
        this.result = this.rewriter.Rewrite(leftShift);
      }

      public void Visit(ILessThan lessThan) {
        this.result = this.rewriter.Rewrite(lessThan);
      }

      public void Visit(ILessThanOrEqual lessThanOrEqual) {
        this.result = this.rewriter.Rewrite(lessThanOrEqual);
      }

      public void Visit(ILocalDeclarationStatement localDeclarationStatement) {
        this.result = this.rewriter.Rewrite(localDeclarationStatement);
      }

      public void Visit(ILockStatement lockStatement) {
        this.result = this.rewriter.Rewrite(lockStatement);
      }

      public void Visit(ILogicalNot logicalNot) {
        this.result = this.rewriter.Rewrite(logicalNot);
      }

      public void Visit(IMakeTypedReference makeTypedReference) {
        this.result = this.rewriter.Rewrite(makeTypedReference);
      }

      public void Visit(IMethodCall methodCall) {
        this.result = this.rewriter.Rewrite(methodCall);
      }

      public void Visit(IModulus modulus) {
        this.result = this.rewriter.Rewrite(modulus);
      }

      public void Visit(IMultiplication multiplication) {
        this.result = this.rewriter.Rewrite(multiplication);
      }

      public void Visit(INamedArgument namedArgument) {
        this.result = this.rewriter.Rewrite(namedArgument);
      }

      public void Visit(INotEquality notEquality) {
        this.result = this.rewriter.Rewrite((NotEquality)notEquality);
      }

      public void Visit(IOldValue oldValue) {
        this.result = this.rewriter.Rewrite(oldValue);
      }

      public void Visit(IOnesComplement onesComplement) {
        this.result = this.rewriter.Rewrite(onesComplement);
      }

      public void Visit(IOutArgument outArgument) {
        this.result = this.rewriter.Rewrite(outArgument);
      }

      public void Visit(IPointerCall pointerCall) {
        this.result = this.rewriter.Rewrite(pointerCall);
      }

      public void Visit(IPopValue popValue) {
        this.result = this.rewriter.Rewrite(popValue);
      }

      public void Visit(IPushStatement pushStatement) {
        this.result = this.rewriter.Rewrite(pushStatement);
      }

      public void Visit(IRefArgument refArgument) {
        this.result = this.rewriter.Rewrite(refArgument);
      }

      public void Visit(IResourceUseStatement resourceUseStatement) {
        this.result = this.rewriter.Rewrite(resourceUseStatement);
      }

      public void Visit(IReturnValue returnValue) {
        this.result = this.rewriter.Rewrite(returnValue);
      }

      public void Visit(IRethrowStatement rethrowStatement) {
        this.result = this.rewriter.Rewrite(rethrowStatement);
      }

      public void Visit(IReturnStatement returnStatement) {
        this.result = this.rewriter.Rewrite(returnStatement);
      }

      public void Visit(IRightShift rightShift) {
        this.result = this.rewriter.Rewrite(rightShift);
      }

      public void Visit(IRuntimeArgumentHandleExpression runtimeArgumentHandleExpression) {
        this.result = this.rewriter.Rewrite(runtimeArgumentHandleExpression);
      }

      public void Visit(ISizeOf sizeOf) {
        this.result = this.rewriter.Rewrite(sizeOf);
      }

      public void Visit(IStackArrayCreate stackArrayCreate) {
        this.result = this.rewriter.Rewrite(stackArrayCreate);
      }

      public void Visit(ISubtraction subtraction) {
        this.result = this.rewriter.Rewrite(subtraction);
      }

      public void Visit(ISwitchCase switchCase) {
        this.result = this.rewriter.Rewrite(switchCase);
      }

      public void Visit(ISwitchStatement switchStatement) {
        this.result = this.rewriter.Rewrite(switchStatement);
      }

      public void Visit(ITargetExpression targetExpression) {
        this.result = this.rewriter.Rewrite(targetExpression);
      }

      public void Visit(IThisReference thisReference) {
        this.result = this.rewriter.Rewrite(thisReference);
      }

      public void Visit(IThrowStatement throwStatement) {
        this.result = this.rewriter.Rewrite(throwStatement);
      }

      public void Visit(ITryCatchFinallyStatement tryCatchFilterFinallyStatement) {
        this.result = this.rewriter.Rewrite(tryCatchFilterFinallyStatement);
      }

      public void Visit(ITokenOf tokenOf) {
        this.result = this.rewriter.Rewrite(tokenOf);
      }

      public void Visit(ITypeOf typeOf) {
        this.result = this.rewriter.Rewrite(typeOf);
      }

      public void Visit(IUnaryNegation unaryNegation) {
        this.result = this.rewriter.Rewrite(unaryNegation);
      }

      public void Visit(IUnaryPlus unaryPlus) {
        this.result = this.rewriter.Rewrite(unaryPlus);
      }

      public void Visit(IVectorLength vectorLength) {
        this.result = this.rewriter.Rewrite(vectorLength);
      }

      public void Visit(IWhileDoStatement whileDoStatement) {
        this.result = this.rewriter.Rewrite(whileDoStatement);
      }

      public void Visit(IYieldBreakStatement yieldBreakStatement) {
        this.result = this.rewriter.Rewrite(yieldBreakStatement);
      }

      public void Visit(IYieldReturnStatement yieldReturnStatement) {
        this.result = this.rewriter.Rewrite(yieldReturnStatement);
      }

    }

    /// <summary>
    /// Rewrites the given addition.
    /// </summary>
    /// <param name="addition"></param>
    public virtual IExpression Rewrite(IAddition addition) {
      Contract.Requires(addition != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableAddition = addition as Addition;
      if (mutableAddition == null) return addition;
      this.RewriteChildren(mutableAddition);
      return mutableAddition;
    }

    /// <summary>
    /// Rewrites the given addressable expression.
    /// </summary>
    /// <param name="addressableExpression"></param>
    public virtual IAddressableExpression Rewrite(IAddressableExpression addressableExpression) {
      Contract.Requires(addressableExpression != null);
      Contract.Ensures(Contract.Result<IAddressableExpression>() != null);

      var mutableAddressableExpression = addressableExpression as AddressableExpression;
      if (mutableAddressableExpression == null) return addressableExpression;
      this.RewriteChildren(mutableAddressableExpression);
      return mutableAddressableExpression;
    }

    /// <summary>
    /// Rewrites the given address dereference expression.
    /// </summary>
    /// <param name="addressDereference"></param>
    public virtual IExpression Rewrite(IAddressDereference addressDereference) {
      Contract.Requires(addressDereference != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableAddressDereference = addressDereference as AddressDereference;
      if (mutableAddressDereference == null) return addressDereference;
      this.RewriteChildren(mutableAddressDereference);
      return mutableAddressDereference;
    }

    /// <summary>
    /// Rewrites the given AddressOf expression.
    /// </summary>
    /// <param name="addressOf"></param>
    public virtual IExpression Rewrite(IAddressOf addressOf) {
      Contract.Requires(addressOf != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableAddressOf = addressOf as AddressOf;
      if (mutableAddressOf == null) return addressOf;
      this.RewriteChildren(mutableAddressOf);
      return mutableAddressOf;
    }

    /// <summary>
    /// Rewrites the given anonymous delegate expression.
    /// </summary>
    /// <param name="anonymousDelegate"></param>
    public virtual IExpression Rewrite(IAnonymousDelegate anonymousDelegate) {
      Contract.Requires(anonymousDelegate != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableAnonymousDelegate = anonymousDelegate as AnonymousDelegate;
      if (mutableAnonymousDelegate == null) return anonymousDelegate;
      this.RewriteChildren(mutableAnonymousDelegate);
      return mutableAnonymousDelegate;
    }

    /// <summary>
    /// Rewrites the given array indexer expression.
    /// </summary>
    /// <param name="arrayIndexer"></param>
    public virtual IExpression Rewrite(IArrayIndexer arrayIndexer) {
      Contract.Requires(arrayIndexer != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableArrayIndexer = arrayIndexer as ArrayIndexer;
      if (mutableArrayIndexer == null) return arrayIndexer;
      this.RewriteChildren(mutableArrayIndexer);
      return mutableArrayIndexer;
    }

    /// <summary>
    /// Rewrites the given assert statement.
    /// </summary>
    /// <param name="assertStatement"></param>
    public virtual IStatement Rewrite(IAssertStatement assertStatement) {
      Contract.Requires(assertStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableAssertStatement = assertStatement as AssertStatement;
      if (mutableAssertStatement == null) return assertStatement;
      this.RewriteChildren(mutableAssertStatement);
      return mutableAssertStatement;
    }

    /// <summary>
    /// Rewrites the given assignment expression.
    /// </summary>
    /// <param name="assignment"></param>
    public virtual IExpression Rewrite(IAssignment assignment) {
      Contract.Requires(assignment != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableAssignment = assignment as Assignment;
      if (mutableAssignment == null) return assignment;
      this.RewriteChildren(mutableAssignment);
      return mutableAssignment;
    }

    /// <summary>
    /// Rewrites the given assume statement.
    /// </summary>
    /// <param name="assumeStatement"></param>
    public virtual IStatement Rewrite(IAssumeStatement assumeStatement) {
      Contract.Requires(assumeStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableAssumeStatement = assumeStatement as AssumeStatement;
      if (mutableAssumeStatement == null) return assumeStatement;
      this.RewriteChildren(mutableAssumeStatement);
      return mutableAssumeStatement;
    }

    /// <summary>
    /// Rewrites the given bitwise and expression.
    /// </summary>
    /// <param name="binaryOperation"></param>
    public virtual IExpression Rewrite(IBinaryOperation binaryOperation) {
      Contract.Requires(binaryOperation != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      binaryOperation.Dispatch(this.dispatchingVisitor);
      return (IBinaryOperation)this.dispatchingVisitor.result;
    }

    /// <summary>
    /// Rewrites the given bitwise and expression.
    /// </summary>
    /// <param name="bitwiseAnd"></param>
    public virtual IExpression Rewrite(IBitwiseAnd bitwiseAnd) {
      Contract.Requires(bitwiseAnd != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableBitwiseAnd = bitwiseAnd as BitwiseAnd;
      if (mutableBitwiseAnd == null) return bitwiseAnd;
      this.RewriteChildren(mutableBitwiseAnd);
      return mutableBitwiseAnd;
    }

    /// <summary>
    /// Rewrites the given bitwise or expression.
    /// </summary>
    /// <param name="bitwiseOr"></param>
    public virtual IExpression Rewrite(IBitwiseOr bitwiseOr) {
      Contract.Requires(bitwiseOr != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableBitwiseOr = bitwiseOr as BitwiseOr;
      if (mutableBitwiseOr == null) return bitwiseOr;
      this.RewriteChildren(mutableBitwiseOr);
      return mutableBitwiseOr;
    }

    /// <summary>
    /// Rewrites the given block expression.
    /// </summary>
    /// <param name="blockExpression"></param>
    public virtual IExpression Rewrite(IBlockExpression blockExpression) {
      Contract.Requires(blockExpression != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableBlockExpression = blockExpression as BlockExpression;
      if (mutableBlockExpression == null) return blockExpression;
      this.RewriteChildren(mutableBlockExpression);
      return mutableBlockExpression;
    }

    /// <summary>
    /// Rewrites the given statement block.
    /// </summary>
    /// <param name="block"></param>
    public virtual IBlockStatement Rewrite(IBlockStatement block) {
      Contract.Requires(block != null);
      Contract.Ensures(Contract.Result<IBlockStatement>() != null);

      var mutableBlockStatement = block as BlockStatement;
      if (mutableBlockStatement == null) return block;
      this.RewriteChildren(mutableBlockStatement);
      return mutableBlockStatement;
    }

    /// <summary>
    /// Rewrites the given bound expression.
    /// </summary>
    /// <param name="boundExpression"></param>
    public virtual IExpression Rewrite(IBoundExpression boundExpression) {
      Contract.Requires(boundExpression != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableBoundExpression = boundExpression as BoundExpression;
      if (mutableBoundExpression == null) return boundExpression;
      this.RewriteChildren(mutableBoundExpression);
      return mutableBoundExpression;
    }

    /// <summary>
    /// Rewrites the given break statement.
    /// </summary>
    /// <param name="breakStatement"></param>
    public virtual IStatement Rewrite(IBreakStatement breakStatement) {
      Contract.Requires(breakStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableBreakStatement = breakStatement as BreakStatement;
      if (mutableBreakStatement == null) return breakStatement;
      this.RewriteChildren(mutableBreakStatement);
      return mutableBreakStatement;
    }

    /// <summary>
    /// Rewrites the cast-if-possible expression.
    /// </summary>
    /// <param name="castIfPossible"></param>
    public virtual IExpression Rewrite(ICastIfPossible castIfPossible) {
      Contract.Requires(castIfPossible != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableCastIfPossible = castIfPossible as CastIfPossible;
      if (mutableCastIfPossible == null) return castIfPossible;
      this.RewriteChildren(mutableCastIfPossible);
      return mutableCastIfPossible;
    }

    /// <summary>
    /// Rewrites the given catch clause.
    /// </summary>
    /// <param name="catchClause"></param>
    public virtual ICatchClause Rewrite(ICatchClause catchClause) {
      Contract.Requires(catchClause != null);
      Contract.Ensures(Contract.Result<ICatchClause>() != null);

      var mutableCatchClause = catchClause as CatchClause;
      if (mutableCatchClause == null) return catchClause;
      this.RewriteChildren(mutableCatchClause);
      return mutableCatchClause;
    }

    /// <summary>
    /// Rewrites the given check-if-instance expression.
    /// </summary>
    /// <param name="checkIfInstance"></param>
    public virtual IExpression Rewrite(ICheckIfInstance checkIfInstance) {
      Contract.Requires(checkIfInstance != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableCheckIfInstance = checkIfInstance as CheckIfInstance;
      if (mutableCheckIfInstance == null) return checkIfInstance;
      this.RewriteChildren(mutableCheckIfInstance);
      return mutableCheckIfInstance;
    }

    /// <summary>
    /// Rewrites the given compile time constant.
    /// </summary>
    /// <param name="constant"></param>
    public virtual ICompileTimeConstant Rewrite(ICompileTimeConstant constant) {
      Contract.Requires(constant != null);
      Contract.Ensures(Contract.Result<ICompileTimeConstant>() != null);

      var mutableCompileTimeConstant = constant as CompileTimeConstant;
      if (mutableCompileTimeConstant == null) return constant;
      this.RewriteChildren(mutableCompileTimeConstant);
      return mutableCompileTimeConstant;
    }

    /// <summary>
    /// Rewrites the given conditional expression.
    /// </summary>
    /// <param name="conditional"></param>
    public virtual IExpression Rewrite(IConditional conditional) {
      Contract.Requires(conditional != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableConditional = conditional as Conditional;
      if (mutableConditional == null) return conditional;
      this.RewriteChildren(mutableConditional);
      return mutableConditional;
    }

    /// <summary>
    /// Rewrites the given conditional statement.
    /// </summary>
    /// <param name="conditionalStatement"></param>
    public virtual IStatement Rewrite(IConditionalStatement conditionalStatement) {
      Contract.Requires(conditionalStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableConditionalStatement = conditionalStatement as ConditionalStatement;
      if (mutableConditionalStatement == null) return conditionalStatement;
      this.RewriteChildren(mutableConditionalStatement);
      return mutableConditionalStatement;
    }

    /// <summary>
    /// Rewrites the given continue statement.
    /// </summary>
    /// <param name="continueStatement"></param>
    public virtual IStatement Rewrite(IContinueStatement continueStatement) {
      Contract.Requires(continueStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableContinueStatement = continueStatement as ContinueStatement;
      if (mutableContinueStatement == null) return continueStatement;
      this.RewriteChildren(mutableContinueStatement);
      return mutableContinueStatement;
    }

    /// <summary>
    /// Rewrites the given conversion expression.
    /// </summary>
    /// <param name="conversion"></param>
    public virtual IExpression Rewrite(IConversion conversion) {
      Contract.Requires(conversion != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableConversion = conversion as Conversion;
      if (mutableConversion == null) return conversion;
      this.RewriteChildren(mutableConversion);
      return mutableConversion;
    }

    /// <summary>
    /// Rewrites the given copy memory statement.
    /// </summary>
    /// <param name="copyMemoryStatement"></param>
    public virtual IStatement Rewrite(ICopyMemoryStatement copyMemoryStatement) {
      Contract.Requires(copyMemoryStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableCopyMemoryStatement = copyMemoryStatement as CopyMemoryStatement;
      if (mutableCopyMemoryStatement == null) return copyMemoryStatement;
      this.RewriteChildren(mutableCopyMemoryStatement);
      return mutableCopyMemoryStatement;
    }

    /// <summary>
    /// Rewrites the given array creation expression.
    /// </summary>
    /// <param name="createArray"></param>
    public virtual IExpression Rewrite(ICreateArray createArray) {
      Contract.Requires(createArray != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableCreateArray = createArray as CreateArray;
      if (mutableCreateArray == null) return createArray;
      this.RewriteChildren(mutableCreateArray);
      return mutableCreateArray;
    }

    /// <summary>
    /// Rewrites the anonymous object creation expression.
    /// </summary>
    /// <param name="createDelegateInstance"></param>
    public virtual IExpression Rewrite(ICreateDelegateInstance createDelegateInstance) {
      Contract.Requires(createDelegateInstance != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableCreateDelegateInstance = createDelegateInstance as CreateDelegateInstance;
      if (mutableCreateDelegateInstance == null) return createDelegateInstance;
      this.RewriteChildren(mutableCreateDelegateInstance);
      return mutableCreateDelegateInstance;
    }

    /// <summary>
    /// Rewrites the given constructor call expression.
    /// </summary>
    /// <param name="createObjectInstance"></param>
    public virtual IExpression Rewrite(ICreateObjectInstance createObjectInstance) {
      Contract.Requires(createObjectInstance != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableCreateObjectInstance = createObjectInstance as CreateObjectInstance;
      if (mutableCreateObjectInstance == null) return createObjectInstance;
      this.RewriteChildren(mutableCreateObjectInstance);
      return mutableCreateObjectInstance;
    }

    /// <summary>
    /// Rewrites the given debugger break statement.
    /// </summary>
    /// <param name="debuggerBreakStatement"></param>
    public virtual IStatement Rewrite(IDebuggerBreakStatement debuggerBreakStatement) {
      Contract.Requires(debuggerBreakStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableDebuggerBreakStatement = debuggerBreakStatement as DebuggerBreakStatement;
      if (mutableDebuggerBreakStatement == null) return debuggerBreakStatement;
      this.RewriteChildren(mutableDebuggerBreakStatement);
      return mutableDebuggerBreakStatement;
    }

    /// <summary>
    /// Rewrites the given defalut value expression.
    /// </summary>
    /// <param name="defaultValue"></param>
    public virtual IExpression Rewrite(IDefaultValue defaultValue) {
      Contract.Requires(defaultValue != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableDefaultValue = defaultValue as DefaultValue;
      if (mutableDefaultValue == null) return defaultValue;
      this.RewriteChildren(mutableDefaultValue);
      return mutableDefaultValue;
    }

    /// <summary>
    /// Rewrites the given division expression.
    /// </summary>
    /// <param name="division"></param>
    public virtual IExpression Rewrite(IDivision division) {
      Contract.Requires(division != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableDivision = division as Division;
      if (mutableDivision == null) return division;
      this.RewriteChildren(mutableDivision);
      return mutableDivision;
    }

    /// <summary>
    /// Rewrites the given do until statement.
    /// </summary>
    /// <param name="doUntilStatement"></param>
    public virtual IStatement Rewrite(IDoUntilStatement doUntilStatement) {
      Contract.Requires(doUntilStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableDoUntilStatement = doUntilStatement as DoUntilStatement;
      if (mutableDoUntilStatement == null) return doUntilStatement;
      this.RewriteChildren(mutableDoUntilStatement);
      return mutableDoUntilStatement;
    }

    /// <summary>
    /// Rewrites the given dup value expression.
    /// </summary>
    /// <param name="dupValue"></param>
    public virtual IExpression Rewrite(IDupValue dupValue) {
      Contract.Requires(dupValue != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableDupValue = dupValue as DupValue;
      if (mutableDupValue == null) return dupValue;
      this.RewriteChildren(mutableDupValue);
      return mutableDupValue;
    }

    /// <summary>
    /// Rewrites the given empty statement.
    /// </summary>
    /// <param name="emptyStatement"></param>
    public virtual IStatement Rewrite(IEmptyStatement emptyStatement) {
      Contract.Requires(emptyStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableEmptyStatement = emptyStatement as EmptyStatement;
      if (mutableEmptyStatement == null) return emptyStatement;
      this.RewriteChildren(mutableEmptyStatement);
      return mutableEmptyStatement;
    }

    /// <summary>
    /// Rewrites the given equality expression.
    /// </summary>
    /// <param name="equality"></param>
    public virtual IExpression Rewrite(IEquality equality) {
      Contract.Requires(equality != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableEquality = equality as Equality;
      if (mutableEquality == null) return equality;
      this.RewriteChildren(mutableEquality);
      return mutableEquality;
    }

    /// <summary>
    /// Rewrites the given exclusive or expression.
    /// </summary>
    /// <param name="exclusiveOr"></param>
    public virtual IExpression Rewrite(IExclusiveOr exclusiveOr) {
      Contract.Requires(exclusiveOr != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableExclusiveOr = exclusiveOr as ExclusiveOr;
      if (mutableExclusiveOr == null) return exclusiveOr;
      this.RewriteChildren(mutableExclusiveOr);
      return mutableExclusiveOr;
    }

    /// <summary>
    /// Rewrites the given expression.
    /// </summary>
    /// <param name="expression"></param>
    public virtual IExpression Rewrite(IExpression expression) {
      Contract.Requires(expression != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      expression.Dispatch(this.dispatchingVisitor);
      return (IExpression)this.dispatchingVisitor.result;
    }

    /// <summary>
    /// Rewrites the given expression statement.
    /// </summary>
    /// <param name="expressionStatement"></param>
    public virtual IStatement Rewrite(IExpressionStatement expressionStatement) {
      Contract.Requires(expressionStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableExpressionStatement = expressionStatement as ExpressionStatement;
      if (mutableExpressionStatement == null) return expressionStatement;
      this.RewriteChildren(mutableExpressionStatement);
      return mutableExpressionStatement;
    }

    /// <summary>
    /// Rewrites the given fill memory statement.
    /// </summary>
    /// <param name="fillMemoryStatement"></param>
    public virtual IStatement Rewrite(IFillMemoryStatement fillMemoryStatement) {
      Contract.Requires(fillMemoryStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableFillMemoryStatement = fillMemoryStatement as FillMemoryStatement;
      if (mutableFillMemoryStatement == null) return fillMemoryStatement;
      this.RewriteChildren(mutableFillMemoryStatement);
      return mutableFillMemoryStatement;
    }

    /// <summary>
    /// Rewrites the given foreach statement.
    /// </summary>
    /// <param name="forEachStatement"></param>
    public virtual IStatement Rewrite(IForEachStatement forEachStatement) {
      Contract.Requires(forEachStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableForEachStatement = forEachStatement as ForEachStatement;
      if (mutableForEachStatement == null) return forEachStatement;
      this.RewriteChildren(mutableForEachStatement);
      return mutableForEachStatement;
    }

    /// <summary>
    /// Rewrites the given for statement.
    /// </summary>
    /// <param name="forStatement"></param>
    public virtual IStatement Rewrite(IForStatement forStatement) {
      Contract.Requires(forStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableForStatement = forStatement as ForStatement;
      if (mutableForStatement == null) return forStatement;
      this.RewriteChildren(mutableForStatement);
      return mutableForStatement;
    }

    /// <summary>
    /// Rewrites the given get type of typed reference expression.
    /// </summary>
    /// <param name="getTypeOfTypedReference"></param>
    public virtual IExpression Rewrite(IGetTypeOfTypedReference getTypeOfTypedReference) {
      Contract.Requires(getTypeOfTypedReference != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableGetTypeOfTypedReference = getTypeOfTypedReference as GetTypeOfTypedReference;
      if (mutableGetTypeOfTypedReference == null) return getTypeOfTypedReference;
      this.RewriteChildren(mutableGetTypeOfTypedReference);
      return mutableGetTypeOfTypedReference;
    }

    /// <summary>
    /// Rewrites the given get value of typed reference expression.
    /// </summary>
    /// <param name="getValueOfTypedReference"></param>
    public virtual IExpression Rewrite(IGetValueOfTypedReference getValueOfTypedReference) {
      Contract.Requires(getValueOfTypedReference != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableGetValueOfTypedReference = getValueOfTypedReference as GetValueOfTypedReference;
      if (mutableGetValueOfTypedReference == null) return getValueOfTypedReference;
      this.RewriteChildren(mutableGetValueOfTypedReference);
      return mutableGetValueOfTypedReference;
    }

    /// <summary>
    /// Rewrites the given goto statement.
    /// </summary>
    /// <param name="gotoStatement"></param>
    public virtual IStatement Rewrite(IGotoStatement gotoStatement) {
      Contract.Requires(gotoStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableGotoStatement = gotoStatement as GotoStatement;
      if (mutableGotoStatement == null) return gotoStatement;
      this.RewriteChildren(mutableGotoStatement);
      return mutableGotoStatement;
    }

    /// <summary>
    /// Rewrites the given goto switch case statement.
    /// </summary>
    /// <param name="gotoSwitchCaseStatement"></param>
    public virtual IStatement Rewrite(IGotoSwitchCaseStatement gotoSwitchCaseStatement) {
      Contract.Requires(gotoSwitchCaseStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableGotoSwitchCaseStatement = gotoSwitchCaseStatement as GotoSwitchCaseStatement;
      if (mutableGotoSwitchCaseStatement == null) return gotoSwitchCaseStatement;
      this.RewriteChildren(mutableGotoSwitchCaseStatement);
      return mutableGotoSwitchCaseStatement;
    }

    /// <summary>
    /// Rewrites the given greater-than expression.
    /// </summary>
    /// <param name="greaterThan"></param>
    public virtual IExpression Rewrite(IGreaterThan greaterThan) {
      Contract.Requires(greaterThan != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableGreaterThan = greaterThan as GreaterThan;
      if (mutableGreaterThan == null) return greaterThan;
      this.RewriteChildren(mutableGreaterThan);
      return mutableGreaterThan;
    }

    /// <summary>
    /// Rewrites the given greater-than-or-equal expression.
    /// </summary>
    /// <param name="greaterThanOrEqual"></param>
    public virtual IExpression Rewrite(IGreaterThanOrEqual greaterThanOrEqual) {
      Contract.Requires(greaterThanOrEqual != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableGreaterThanOrEqual = greaterThanOrEqual as GreaterThanOrEqual;
      if (mutableGreaterThanOrEqual == null) return greaterThanOrEqual;
      this.RewriteChildren(mutableGreaterThanOrEqual);
      return mutableGreaterThanOrEqual;
    }

    /// <summary>
    /// Rewrites the given labeled statement.
    /// </summary>
    /// <param name="labeledStatement"></param>
    public virtual IStatement Rewrite(ILabeledStatement labeledStatement) {
      Contract.Requires(labeledStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableLabeledStatement = labeledStatement as LabeledStatement;
      if (mutableLabeledStatement == null) return labeledStatement;
      this.RewriteChildren(mutableLabeledStatement);
      return mutableLabeledStatement;
    }

    /// <summary>
    /// Rewrites the given left shift expression.
    /// </summary>
    /// <param name="leftShift"></param>
    public virtual IExpression Rewrite(ILeftShift leftShift) {
      Contract.Requires(leftShift != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableLeftShift = leftShift as LeftShift;
      if (mutableLeftShift == null) return leftShift;
      this.RewriteChildren(mutableLeftShift);
      return mutableLeftShift;
    }

    /// <summary>
    /// Rewrites the given less-than expression.
    /// </summary>
    /// <param name="lessThan"></param>
    public virtual IExpression Rewrite(ILessThan lessThan) {
      Contract.Requires(lessThan != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableLessThan = lessThan as LessThan;
      if (mutableLessThan == null) return lessThan;
      this.RewriteChildren(mutableLessThan);
      return mutableLessThan;
    }

    /// <summary>
    /// Rewrites the given less-than-or-equal expression.
    /// </summary>
    /// <param name="lessThanOrEqual"></param>
    public virtual IExpression Rewrite(ILessThanOrEqual lessThanOrEqual) {
      Contract.Requires(lessThanOrEqual != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableLessThanOrEqual = lessThanOrEqual as LessThanOrEqual;
      if (mutableLessThanOrEqual == null) return lessThanOrEqual;
      this.RewriteChildren(mutableLessThanOrEqual);
      return mutableLessThanOrEqual;
    }

    /// <summary>
    /// Rewrites the given local declaration statement.
    /// </summary>
    /// <param name="localDeclarationStatement"></param>
    public virtual IStatement Rewrite(ILocalDeclarationStatement localDeclarationStatement) {
      Contract.Requires(localDeclarationStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableLocalDeclarationStatement = localDeclarationStatement as LocalDeclarationStatement;
      if (mutableLocalDeclarationStatement == null) return localDeclarationStatement;
      this.RewriteChildren(mutableLocalDeclarationStatement);
      return mutableLocalDeclarationStatement;
    }

    /// <summary>
    /// Rewrites the given lock statement.
    /// </summary>
    /// <param name="lockStatement"></param>
    public virtual IStatement Rewrite(ILockStatement lockStatement) {
      Contract.Requires(lockStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableLockStatement = lockStatement as LockStatement;
      if (mutableLockStatement == null) return lockStatement;
      this.RewriteChildren(mutableLockStatement);
      return mutableLockStatement;
    }

    /// <summary>
    /// Rewrites the given logical not expression.
    /// </summary>
    /// <param name="logicalNot"></param>
    public virtual IExpression Rewrite(ILogicalNot logicalNot) {
      Contract.Requires(logicalNot != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableLogicalNot = logicalNot as LogicalNot;
      if (mutableLogicalNot == null) return logicalNot;
      this.RewriteChildren(mutableLogicalNot);
      return mutableLogicalNot;
    }

    /// <summary>
    /// Rewrites the given make typed reference expression.
    /// </summary>
    /// <param name="makeTypedReference"></param>
    public virtual IExpression Rewrite(IMakeTypedReference makeTypedReference) {
      Contract.Requires(makeTypedReference != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableMakeTypedReference = makeTypedReference as MakeTypedReference;
      if (mutableMakeTypedReference == null) return makeTypedReference;
      this.RewriteChildren(mutableMakeTypedReference);
      return mutableMakeTypedReference;
    }

    /// <summary>
    /// Rewrites the the given method body.
    /// </summary>
    /// <param name="methodBody"></param>
    public override IMethodBody Rewrite(IMethodBody methodBody) {
      Contract.Ensures(Contract.Result<IMethodBody>() != null);
      var sourceBody = methodBody as ISourceMethodBody;
      if (sourceBody != null) return this.Rewrite(sourceBody);
      return base.Rewrite(methodBody);
    }

    /// <summary>
    /// Rewrites the given method call.
    /// </summary>
    /// <param name="methodCall"></param>
    public virtual IExpression Rewrite(IMethodCall methodCall) {
      Contract.Requires(methodCall != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableMethodCall = methodCall as MethodCall;
      if (mutableMethodCall == null) return methodCall;
      this.RewriteChildren(mutableMethodCall);
      return mutableMethodCall;
    }

    /// <summary>
    /// Rewrites the given modulus expression.
    /// </summary>
    /// <param name="modulus"></param>
    public virtual IExpression Rewrite(IModulus modulus) {
      Contract.Requires(modulus != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableModulus = modulus as Modulus;
      if (mutableModulus == null) return modulus;
      this.RewriteChildren(mutableModulus);
      return mutableModulus;
    }

    /// <summary>
    /// Rewrites the given multiplication expression.
    /// </summary>
    /// <param name="multiplication"></param>
    public virtual IExpression Rewrite(IMultiplication multiplication) {
      Contract.Requires(multiplication != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableMultiplication = multiplication as Multiplication;
      if (mutableMultiplication == null) return multiplication;
      this.RewriteChildren(mutableMultiplication);
      return mutableMultiplication;
    }

    /// <summary>
    /// Rewrites the given named argument expression.
    /// </summary>
    /// <param name="namedArgument"></param>
    public virtual IExpression Rewrite(INamedArgument namedArgument) {
      Contract.Requires(namedArgument != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableNamedArgument = namedArgument as NamedArgument;
      if (mutableNamedArgument == null) return namedArgument;
      this.RewriteChildren(mutableNamedArgument);
      return mutableNamedArgument;
    }

    /// <summary>
    /// Rewrites the given not equality expression.
    /// </summary>
    /// <param name="notEquality"></param>
    public virtual IExpression Rewrite(INotEquality notEquality) {
      Contract.Requires(notEquality != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableNotEquality = notEquality as NotEquality;
      if (mutableNotEquality == null) return notEquality;
      this.RewriteChildren(mutableNotEquality);
      return mutableNotEquality;
    }

    /// <summary>
    /// Rewrites the given old value expression.
    /// </summary>
    /// <param name="oldValue"></param>
    public virtual IExpression Rewrite(IOldValue oldValue) {
      Contract.Requires(oldValue != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableOldValue = oldValue as OldValue;
      if (mutableOldValue == null) return oldValue;
      this.RewriteChildren(mutableOldValue);
      return mutableOldValue;
    }

    /// <summary>
    /// Rewrites the given one's complement expression.
    /// </summary>
    /// <param name="onesComplement"></param>
    public virtual IExpression Rewrite(IOnesComplement onesComplement) {
      Contract.Requires(onesComplement != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableOnesComplement = onesComplement as OnesComplement;
      if (mutableOnesComplement == null) return onesComplement;
      this.RewriteChildren(mutableOnesComplement);
      return mutableOnesComplement;
    }

    /// <summary>
    /// Rewrites the given out argument expression.
    /// </summary>
    /// <param name="outArgument"></param>
    public virtual IExpression Rewrite(IOutArgument outArgument) {
      Contract.Requires(outArgument != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableOutArgument = outArgument as OutArgument;
      if (mutableOutArgument == null) return outArgument;
      this.RewriteChildren(mutableOutArgument);
      return mutableOutArgument;
    }

    /// <summary>
    /// Rewrites the given pointer call.
    /// </summary>
    /// <param name="pointerCall"></param>
    public virtual IExpression Rewrite(IPointerCall pointerCall) {
      Contract.Requires(pointerCall != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutablePointerCall = pointerCall as PointerCall;
      if (mutablePointerCall == null) return pointerCall;
      this.RewriteChildren(mutablePointerCall);
      return mutablePointerCall;
    }

    /// <summary>
    /// Rewrites the given pop value expression.
    /// </summary>
    /// <param name="popValue"></param>
    public virtual IExpression Rewrite(IPopValue popValue) {
      Contract.Requires(popValue != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutablePopValue = popValue as PopValue;
      if (mutablePopValue == null) return popValue;
      this.RewriteChildren(mutablePopValue);
      return mutablePopValue;
    }

    /// <summary>
    /// Rewrites the given push statement.
    /// </summary>
    /// <param name="pushStatement"></param>
    public virtual IStatement Rewrite(IPushStatement pushStatement) {
      Contract.Requires(pushStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutablePushStatement = pushStatement as PushStatement;
      if (mutablePushStatement == null) return pushStatement;
      this.RewriteChildren(mutablePushStatement);
      return mutablePushStatement;
    }

    /// <summary>
    /// Rewrites the given ref argument expression.
    /// </summary>
    /// <param name="refArgument"></param>
    public virtual IExpression Rewrite(IRefArgument refArgument) {
      Contract.Requires(refArgument != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableRefArgument = refArgument as RefArgument;
      if (mutableRefArgument == null) return refArgument;
      this.RewriteChildren(mutableRefArgument);
      return mutableRefArgument;
    }

    /// <summary>
    /// Rewrites the given resource usage statement.
    /// </summary>
    /// <param name="resourceUseStatement"></param>
    public virtual IStatement Rewrite(IResourceUseStatement resourceUseStatement) {
      Contract.Requires(resourceUseStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableResourceUseStatement = resourceUseStatement as ResourceUseStatement;
      if (mutableResourceUseStatement == null) return resourceUseStatement;
      this.RewriteChildren(mutableResourceUseStatement);
      return mutableResourceUseStatement;
    }

    /// <summary>
    /// Rewrites the rethrow statement.
    /// </summary>
    /// <param name="rethrowStatement"></param>
    public virtual IStatement Rewrite(IRethrowStatement rethrowStatement) {
      Contract.Requires(rethrowStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableRethrowStatement = rethrowStatement as RethrowStatement;
      if (mutableRethrowStatement == null) return rethrowStatement;
      this.RewriteChildren(mutableRethrowStatement);
      return mutableRethrowStatement;
    }

    /// <summary>
    /// Rewrites the return statement.
    /// </summary>
    /// <param name="returnStatement"></param>
    public virtual IStatement Rewrite(IReturnStatement returnStatement) {
      Contract.Requires(returnStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableReturnStatement = returnStatement as ReturnStatement;
      if (mutableReturnStatement == null) return returnStatement;
      this.RewriteChildren(mutableReturnStatement);
      return mutableReturnStatement;
    }

    /// <summary>
    /// Rewrites the given return value expression.
    /// </summary>
    /// <param name="returnValue"></param>
    public virtual IExpression Rewrite(IReturnValue returnValue) {
      Contract.Requires(returnValue != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableReturnValue = returnValue as ReturnValue;
      if (mutableReturnValue == null) return returnValue;
      this.RewriteChildren(mutableReturnValue);
      return mutableReturnValue;
    }

    /// <summary>
    /// Rewrites the given right shift expression.
    /// </summary>
    /// <param name="rightShift"></param>
    public virtual IExpression Rewrite(IRightShift rightShift) {
      Contract.Requires(rightShift != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableRightShift = rightShift as RightShift;
      if (mutableRightShift == null) return rightShift;
      this.RewriteChildren(mutableRightShift);
      return mutableRightShift;
    }

    /// <summary>
    /// Rewrites the given runtime argument handle expression.
    /// </summary>
    /// <param name="runtimeArgumentHandleExpression"></param>
    public virtual IExpression Rewrite(IRuntimeArgumentHandleExpression runtimeArgumentHandleExpression) {
      Contract.Requires(runtimeArgumentHandleExpression != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableRuntimeArgumentHandleExpression = runtimeArgumentHandleExpression as RuntimeArgumentHandleExpression;
      if (mutableRuntimeArgumentHandleExpression == null) return runtimeArgumentHandleExpression;
      this.RewriteChildren(mutableRuntimeArgumentHandleExpression);
      return mutableRuntimeArgumentHandleExpression;
    }

    /// <summary>
    /// Rewrites the given sizeof() expression.
    /// </summary>
    /// <param name="sizeOf"></param>
    public virtual IExpression Rewrite(ISizeOf sizeOf) {
      Contract.Requires(sizeOf != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableSizeOf = sizeOf as SizeOf;
      if (mutableSizeOf == null) return sizeOf;
      this.RewriteChildren(mutableSizeOf);
      return mutableSizeOf;
    }

    /// <summary>
    /// Rewrites the given stack array create expression.
    /// </summary>
    /// <param name="stackArrayCreate"></param>
    public virtual IExpression Rewrite(IStackArrayCreate stackArrayCreate) {
      Contract.Requires(stackArrayCreate != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableStackArrayCreate = stackArrayCreate as StackArrayCreate;
      if (mutableStackArrayCreate == null) return stackArrayCreate;
      this.RewriteChildren(mutableStackArrayCreate);
      return mutableStackArrayCreate;
    }

    /// <summary>
    /// Rewrites the the given source method body.
    /// </summary>
    /// <param name="sourceMethodBody"></param>
    public virtual ISourceMethodBody Rewrite(ISourceMethodBody sourceMethodBody) {
      Contract.Requires(sourceMethodBody != null);
      Contract.Ensures(Contract.Result<ISourceMethodBody>() != null);

      var mutableSourceMethodBody = sourceMethodBody as SourceMethodBody;
      if (mutableSourceMethodBody == null) return sourceMethodBody;
      this.RewriteChildren(mutableSourceMethodBody);
      return mutableSourceMethodBody;
    }

    /// <summary>
    /// Rewrites the specified statement.
    /// </summary>
    /// <param name="statement">The statement.</param>
    public virtual IStatement Rewrite(IStatement statement) {
      Contract.Requires(statement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      statement.Dispatch(this.dispatchingVisitor);
      return (IStatement)this.dispatchingVisitor.result;
    }

    /// <summary>
    /// Rewrites the given subtraction expression.
    /// </summary>
    /// <param name="subtraction"></param>
    public virtual IExpression Rewrite(ISubtraction subtraction) {
      Contract.Requires(subtraction != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableSubtraction = subtraction as Subtraction;
      if (mutableSubtraction == null) return subtraction;
      this.RewriteChildren(mutableSubtraction);
      return mutableSubtraction;
    }

    /// <summary>
    /// Rewrites the given switch case.
    /// </summary>
    /// <param name="switchCase"></param>
    public virtual ISwitchCase Rewrite(ISwitchCase switchCase) {
      Contract.Requires(switchCase != null);
      Contract.Ensures(Contract.Result<ISwitchCase>() != null);

      var mutableSwitchCase = switchCase as SwitchCase;
      if (mutableSwitchCase == null) return switchCase;
      this.RewriteChildren(mutableSwitchCase);
      return mutableSwitchCase;
    }

    /// <summary>
    /// Rewrites the given switch statement.
    /// </summary>
    /// <param name="switchStatement"></param>
    public virtual IStatement Rewrite(ISwitchStatement switchStatement) {
      Contract.Requires(switchStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableSwitchStatement = switchStatement as SwitchStatement;
      if (mutableSwitchStatement == null) return switchStatement;
      this.RewriteChildren(mutableSwitchStatement);
      return mutableSwitchStatement;
    }

    /// <summary>
    /// Rewrites the given target expression.
    /// </summary>
    /// <param name="targetExpression"></param>
    public virtual ITargetExpression Rewrite(ITargetExpression targetExpression) {
      Contract.Requires(targetExpression != null);
      Contract.Ensures(Contract.Result<ITargetExpression>() != null);

      var mutableTargetExpression = targetExpression as TargetExpression;
      if (mutableTargetExpression == null) return targetExpression;
      this.RewriteChildren(mutableTargetExpression);
      return mutableTargetExpression;
    }

    /// <summary>
    /// Rewrites the given this reference expression.
    /// </summary>
    /// <param name="thisReference"></param>
    public virtual IExpression Rewrite(IThisReference thisReference) {
      Contract.Requires(thisReference != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableThisReference = thisReference as ThisReference;
      if (mutableThisReference == null) return thisReference;
      this.RewriteChildren(mutableThisReference);
      return mutableThisReference;
    }

    /// <summary>
    /// Rewrites the throw statement.
    /// </summary>
    /// <param name="throwStatement"></param>
    public virtual IStatement Rewrite(IThrowStatement throwStatement) {
      Contract.Requires(throwStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableThrowStatement = throwStatement as ThrowStatement;
      if (mutableThrowStatement == null) return throwStatement;
      this.RewriteChildren(mutableThrowStatement);
      return mutableThrowStatement;
    }

    /// <summary>
    /// Rewrites the given tokenof() expression.
    /// </summary>
    /// <param name="tokenOf"></param>
    public virtual IExpression Rewrite(ITokenOf tokenOf) {
      Contract.Requires(tokenOf != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableTokenOf = tokenOf as TokenOf;
      if (mutableTokenOf == null) return tokenOf;
      this.RewriteChildren(mutableTokenOf);
      return mutableTokenOf;
    }

    /// <summary>
    /// Rewrites the try-catch-filter-finally statement.
    /// </summary>
    /// <param name="tryCatchFilterFinallyStatement"></param>
    public virtual IStatement Rewrite(ITryCatchFinallyStatement tryCatchFilterFinallyStatement) {
      Contract.Requires(tryCatchFilterFinallyStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableTryCatchFinallyStatement = tryCatchFilterFinallyStatement as TryCatchFinallyStatement;
      if (mutableTryCatchFinallyStatement == null) return tryCatchFilterFinallyStatement;
      this.RewriteChildren(mutableTryCatchFinallyStatement);
      return mutableTryCatchFinallyStatement;
    }

    /// <summary>
    /// Rewrites the given typeof() expression.
    /// </summary>
    /// <param name="typeOf"></param>
    public virtual IExpression Rewrite(ITypeOf typeOf) {
      Contract.Requires(typeOf != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableTypeOf = typeOf as TypeOf;
      if (mutableTypeOf == null) return typeOf;
      this.RewriteChildren(mutableTypeOf);
      return mutableTypeOf;
    }

    /// <summary>
    /// Rewrites the given unary negation expression.
    /// </summary>
    /// <param name="unaryNegation"></param>
    public virtual IExpression Rewrite(IUnaryNegation unaryNegation) {
      Contract.Requires(unaryNegation != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableUnaryNegation = unaryNegation as UnaryNegation;
      if (mutableUnaryNegation == null) return unaryNegation;
      this.RewriteChildren(mutableUnaryNegation);
      return mutableUnaryNegation;
    }

    /// <summary>
    /// Rewrites the given unary plus expression.
    /// </summary>
    /// <param name="unaryPlus"></param>
    public virtual IExpression Rewrite(IUnaryPlus unaryPlus) {
      Contract.Requires(unaryPlus != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableUnaryPlus = unaryPlus as UnaryPlus;
      if (mutableUnaryPlus == null) return unaryPlus;
      this.RewriteChildren(mutableUnaryPlus);
      return mutableUnaryPlus;
    }

    /// <summary>
    /// Rewrites the given vector length expression.
    /// </summary>
    /// <param name="vectorLength"></param>
    public virtual IExpression Rewrite(IVectorLength vectorLength) {
      Contract.Requires(vectorLength != null);
      Contract.Ensures(Contract.Result<IExpression>() != null);

      var mutableVectorLength = vectorLength as VectorLength;
      if (mutableVectorLength == null) return vectorLength;
      this.RewriteChildren(mutableVectorLength);
      return mutableVectorLength;
    }

    /// <summary>
    /// Rewrites the given while do statement.
    /// </summary>
    /// <param name="whileDoStatement"></param>
    public virtual IStatement Rewrite(IWhileDoStatement whileDoStatement) {
      Contract.Requires(whileDoStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableWhileDoStatement = whileDoStatement as WhileDoStatement;
      if (mutableWhileDoStatement == null) return whileDoStatement;
      this.RewriteChildren(mutableWhileDoStatement);
      return mutableWhileDoStatement;
    }

    /// <summary>
    /// Rewrites the given yield break statement.
    /// </summary>
    /// <param name="yieldBreakStatement"></param>
    public virtual IStatement Rewrite(IYieldBreakStatement yieldBreakStatement) {
      Contract.Requires(yieldBreakStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableYieldBreakStatement = yieldBreakStatement as YieldBreakStatement;
      if (mutableYieldBreakStatement == null) return yieldBreakStatement;
      this.RewriteChildren(mutableYieldBreakStatement);
      return mutableYieldBreakStatement;
    }

    /// <summary>
    /// Rewrites the given yield return statement.
    /// </summary>
    /// <param name="yieldReturnStatement"></param>
    public virtual IStatement Rewrite(IYieldReturnStatement yieldReturnStatement) {
      Contract.Requires(yieldReturnStatement != null);
      Contract.Ensures(Contract.Result<IStatement>() != null);

      var mutableYieldReturnStatement = yieldReturnStatement as YieldReturnStatement;
      if (mutableYieldReturnStatement == null) return yieldReturnStatement;
      this.RewriteChildren(mutableYieldReturnStatement);
      return mutableYieldReturnStatement;
    }

    /// <summary>
    /// Rewrites the given list of catch clauses.
    /// </summary>
    /// <param name="catchClauses"></param>
    public virtual List<ICatchClause> Rewrite(List<ICatchClause> catchClauses) {
      Contract.Requires(catchClauses != null);
      Contract.Ensures(Contract.Result<List<ICatchClause>>() != null);

      for (int i = 0, n = catchClauses.Count; i < n; i++)
        catchClauses[i] = this.Rewrite((CatchClause)catchClauses[i]);
      return catchClauses;
    }

    /// <summary>
    /// Rewrites the given list of expressions.
    /// </summary>
    /// <param name="expressions"></param>
    public virtual List<IExpression> Rewrite(List<IExpression> expressions) {
      Contract.Requires(expressions != null);
      Contract.Ensures(Contract.Result<List<IExpression>>() != null);

      for (int i = 0, n = expressions.Count; i < n; i++)
        expressions[i] = this.Rewrite(expressions[i]);
      return expressions;
    }

    /// <summary>
    /// Rewrites the given list of switch cases.
    /// </summary>
    /// <param name="switchCases"></param>
    public virtual List<ISwitchCase> Rewrite(List<ISwitchCase> switchCases) {
      Contract.Requires(switchCases != null);
      Contract.Ensures(Contract.Result<List<ISwitchCase>>() != null);

      for (int i = 0, n = switchCases.Count; i < n; i++)
        switchCases[i] = this.Rewrite((SwitchCase)switchCases[i]);
      return switchCases;
    }

    /// <summary>
    /// Rewrites the given list of statements.
    /// </summary>
    /// <param name="statements"></param>
    public virtual List<IStatement> Rewrite(List<IStatement> statements) {
      Contract.Requires(statements != null);
      Contract.Ensures(Contract.Result<List<IStatement>>() != null);

      var n = statements.Count;
      var j = 0;
      for (int i = 0; i < n; i++) {
        Contract.Assume(statements[i] != null);
        var s = this.Rewrite(statements[i]);
        if (s == CodeDummy.Block) continue;
        Contract.Assume(j <= i);
        statements[j++] = s;
      }
      if (j < n) statements.RemoveRange(j, n-j);
      return statements;
    }

    /// <summary>
    /// Rewrites the children of the given addition.
    /// </summary>
    /// <param name="addition"></param>
    public virtual void RewriteChildren(Addition addition) {
      Contract.Requires(addition != null);

      this.RewriteChildren((BinaryOperation)addition);
    }

    /// <summary>
    /// Rewrites the children of the given addressable expression.
    /// </summary>
    /// <param name="addressableExpression"></param>
    public virtual void RewriteChildren(AddressableExpression addressableExpression) {
      Contract.Requires(addressableExpression != null);

      this.RewriteChildren((Expression)addressableExpression);
      var local = addressableExpression.Definition as ILocalDefinition;
      if (local != null)
        addressableExpression.Definition = this.RewriteReference(local);
      else {
        var parameter = addressableExpression.Definition as IParameterDefinition;
        if (parameter != null)
          addressableExpression.Definition = this.RewriteReference(parameter);
        else {
          var fieldReference = addressableExpression.Definition as IFieldReference;
          if (fieldReference != null)
            addressableExpression.Definition = this.Rewrite(fieldReference);
          else {
            var arrayIndexer = addressableExpression.Definition as IArrayIndexer;
            if (arrayIndexer != null) {
              addressableExpression.Definition = this.Rewrite(arrayIndexer);
              return; //do not rewrite Instance again
            } else {
              var methodReference = addressableExpression.Definition as IMethodReference;
              if (methodReference != null)
                addressableExpression.Definition = this.Rewrite(methodReference);
              else {
                var expression = (IExpression)addressableExpression.Definition;
                addressableExpression.Definition = this.Rewrite(expression);
              }
            }
          }
        }
      }
      if (addressableExpression.Instance != null)
        addressableExpression.Instance = this.Rewrite(addressableExpression.Instance);
    }

    /// <summary>
    /// Rewrites the children of the given address dereference expression.
    /// </summary>
    /// <param name="addressDereference"></param>
    public virtual void RewriteChildren(AddressDereference addressDereference) {
      Contract.Requires(addressDereference != null);

      this.RewriteChildren((Expression)addressDereference);
      addressDereference.Address = this.Rewrite(addressDereference.Address);
    }

    /// <summary>
    /// Rewrites the children of the given AddressOf expression.
    /// </summary>
    /// <param name="addressOf"></param>
    public virtual void RewriteChildren(AddressOf addressOf) {
      Contract.Requires(addressOf != null);

      this.RewriteChildren((Expression)addressOf);
      addressOf.Expression = this.Rewrite((AddressableExpression)addressOf.Expression);
    }

    /// <summary>
    /// Rewrites the children of the given anonymous delegate expression.
    /// </summary>
    /// <param name="anonymousDelegate"></param>
    public virtual void RewriteChildren(AnonymousDelegate anonymousDelegate) {
      Contract.Requires(anonymousDelegate != null);

      this.RewriteChildren((Expression)anonymousDelegate);
      anonymousDelegate.Parameters = this.Rewrite(anonymousDelegate.Parameters);
      anonymousDelegate.Body = this.Rewrite((BlockStatement)anonymousDelegate.Body);
      anonymousDelegate.ReturnType = this.Rewrite(anonymousDelegate.ReturnType);
      if (anonymousDelegate.ReturnValueIsModified)
        anonymousDelegate.ReturnValueCustomModifiers =this.Rewrite(anonymousDelegate.ReturnValueCustomModifiers);
    }

    /// <summary>
    /// Rewrites the children of the given array indexer expression.
    /// </summary>
    /// <param name="arrayIndexer"></param>
    public virtual void RewriteChildren(ArrayIndexer arrayIndexer) {
      Contract.Requires(arrayIndexer != null);

      this.RewriteChildren((Expression)arrayIndexer);
      arrayIndexer.IndexedObject = this.Rewrite(arrayIndexer.IndexedObject);
      arrayIndexer.Indices = this.Rewrite(arrayIndexer.Indices);
    }

    /// <summary>
    /// Rewrites the children of the given assert statement.
    /// </summary>
    /// <param name="assertStatement"></param>
    public virtual void RewriteChildren(AssertStatement assertStatement) {
      Contract.Requires(assertStatement != null);

      this.RewriteChildren((Statement)assertStatement);
      assertStatement.Condition = this.Rewrite(assertStatement.Condition);
      if (assertStatement.Description != null)
        assertStatement.Description = this.Rewrite(assertStatement.Description);
    }

    /// <summary>
    /// Rewrites the children of the given assignment expression.
    /// </summary>
    /// <param name="assignment"></param>
    public virtual void RewriteChildren(Assignment assignment) {
      Contract.Requires(assignment != null);

      this.RewriteChildren((Expression)assignment);
      assignment.Target = this.Rewrite(assignment.Target);
      assignment.Source = this.Rewrite(assignment.Source);
    }

    /// <summary>
    /// Rewrites the children of the given assume statement.
    /// </summary>
    /// <param name="assumeStatement"></param>
    public virtual void RewriteChildren(AssumeStatement assumeStatement) {
      Contract.Requires(assumeStatement != null);

      this.RewriteChildren((Statement)assumeStatement);
      assumeStatement.Condition = this.Rewrite(assumeStatement.Condition);
      if (assumeStatement.Description != null)
        assumeStatement.Description = this.Rewrite(assumeStatement.Description);
    }

    /// <summary>
    /// Called from the type specific rewrite method to rewrite the common part of all binary operation expressions.
    /// </summary>
    public virtual void RewriteChildren(BinaryOperation binaryOperation) {
      Contract.Requires(binaryOperation != null);

      this.RewriteChildren((Expression)binaryOperation);
      binaryOperation.LeftOperand = this.Rewrite(binaryOperation.LeftOperand);
      binaryOperation.RightOperand = this.Rewrite(binaryOperation.RightOperand);
    }

    /// <summary>
    /// Rewrites the children of the given bitwise and expression.
    /// </summary>
    public virtual void RewriteChildren(BitwiseAnd bitwiseAnd) {
      Contract.Requires(bitwiseAnd != null);

      this.RewriteChildren((BinaryOperation)bitwiseAnd);
    }

    /// <summary>
    /// Rewrites the children of the given bitwise or expression.
    /// </summary>
    public virtual void RewriteChildren(BitwiseOr bitwiseOr) {
      Contract.Requires(bitwiseOr != null);

      this.RewriteChildren((BinaryOperation)bitwiseOr);
    }

    /// <summary>
    /// Rewrites the children of the given block expression.
    /// </summary>
    public virtual void RewriteChildren(BlockExpression blockExpression) {
      Contract.Requires(blockExpression != null);

      this.RewriteChildren((Expression)blockExpression);
      blockExpression.BlockStatement = this.Rewrite((BlockStatement)blockExpression.BlockStatement);
      blockExpression.Expression = this.Rewrite(blockExpression.Expression);
    }

    /// <summary>
    /// Rewrites the children of the given statement block.
    /// </summary>
    public virtual void RewriteChildren(BlockStatement block) {
      Contract.Requires(block != null);

      block.Statements = this.Rewrite(block.Statements);
    }

    /// <summary>
    /// Rewrites the children of the given bound expression.
    /// </summary>
    public virtual void RewriteChildren(BoundExpression boundExpression) {
      Contract.Requires(boundExpression != null);

      this.RewriteChildren((Expression)boundExpression);
      if (boundExpression.Instance != null)
        boundExpression.Instance = this.Rewrite(boundExpression.Instance);
      var local = boundExpression.Definition as ILocalDefinition;
      if (local != null)
        boundExpression.Definition = this.RewriteReference(local);
      else {
        var parameter = boundExpression.Definition as IParameterDefinition;
        if (parameter != null)
          boundExpression.Definition = this.RewriteReference(parameter);
        else {
          var fieldReference = (IFieldReference)boundExpression.Definition;
          boundExpression.Definition = this.Rewrite(fieldReference);
        }
      }
    }

    /// <summary>
    /// Rewrites the children of the given break statement.
    /// </summary>
    public virtual void RewriteChildren(BreakStatement breakStatement) {
      Contract.Requires(breakStatement != null);

      this.RewriteChildren((Statement)breakStatement);
    }

    /// <summary>
    /// Rewrites the children of the cast-if-possible expression.
    /// </summary>
    public virtual void RewriteChildren(CastIfPossible castIfPossible) {
      Contract.Requires(castIfPossible != null);

      this.RewriteChildren((Expression)castIfPossible);
      castIfPossible.ValueToCast = this.Rewrite(castIfPossible.ValueToCast);
      castIfPossible.TargetType = this.Rewrite(castIfPossible.TargetType);
    }

    /// <summary>
    /// Rewrites the children of the given catch clause.
    /// </summary>
    public virtual void RewriteChildren(CatchClause catchClause) {
      Contract.Requires(catchClause != null);

      catchClause.ExceptionType = this.Rewrite(catchClause.ExceptionType);
      if (!(catchClause.ExceptionContainer is Dummy))
        catchClause.ExceptionContainer = this.Rewrite(catchClause.ExceptionContainer);
      if (catchClause.FilterCondition != null)
        catchClause.FilterCondition = this.Rewrite(catchClause.FilterCondition);
      catchClause.Body = this.Rewrite((BlockStatement)catchClause.Body);
    }

    /// <summary>
    /// Rewrites the children of the given check-if-instance expression.
    /// </summary>
    public virtual void RewriteChildren(CheckIfInstance checkIfInstance) {
      Contract.Requires(checkIfInstance != null);

      this.RewriteChildren((Expression)checkIfInstance);
      checkIfInstance.Operand = this.Rewrite(checkIfInstance.Operand);
      checkIfInstance.TypeToCheck = this.Rewrite(checkIfInstance.TypeToCheck);
    }

    /// <summary>
    /// Rewrites the children of the given compile time constant.
    /// </summary>
    public virtual void RewriteChildren(CompileTimeConstant constant) {
      Contract.Requires(constant != null);

      this.RewriteChildren((Expression)constant);
    }

    /// <summary>
    /// Called from the type specific rewrite method to rewrite the common part of constructors and method calls.
    /// </summary>
    /// <param name="constructorOrMethodCall"></param>
    public virtual void RewriteChildren(ConstructorOrMethodCall constructorOrMethodCall) {
      Contract.Requires(constructorOrMethodCall != null);

      this.RewriteChildren((Expression)constructorOrMethodCall);
      constructorOrMethodCall.Arguments = this.Rewrite(constructorOrMethodCall.Arguments);
      constructorOrMethodCall.MethodToCall = this.Rewrite(constructorOrMethodCall.MethodToCall);
    }

    /// <summary>
    /// Rewrites the children of the given conditional expression.
    /// </summary>
    public virtual void RewriteChildren(Conditional conditional) {
      Contract.Requires(conditional != null);

      this.RewriteChildren((Expression)conditional);
      conditional.Condition = this.Rewrite(conditional.Condition);
      conditional.ResultIfTrue = this.Rewrite(conditional.ResultIfTrue);
      conditional.ResultIfFalse = this.Rewrite(conditional.ResultIfFalse);
    }

    /// <summary>
    /// Rewrites the children of the given conditional statement.
    /// </summary>
    public virtual void RewriteChildren(ConditionalStatement conditionalStatement) {
      Contract.Requires(conditionalStatement != null);

      this.RewriteChildren((Statement)conditionalStatement);
      conditionalStatement.Condition = this.Rewrite(conditionalStatement.Condition);
      conditionalStatement.TrueBranch = this.Rewrite(conditionalStatement.TrueBranch);
      conditionalStatement.FalseBranch = this.Rewrite(conditionalStatement.FalseBranch);
    }

    /// <summary>
    /// Rewrites the children of the given continue statement.
    /// </summary>
    public virtual void RewriteChildren(ContinueStatement continueStatement) {
      Contract.Requires(continueStatement != null);

      this.RewriteChildren((Statement)continueStatement);
    }

    /// <summary>
    /// Rewrites the children of the given conversion expression.
    /// </summary>
    public virtual void RewriteChildren(Conversion conversion) {
      Contract.Requires(conversion != null);

      this.RewriteChildren((Expression)conversion);
      conversion.ValueToConvert = this.Rewrite(conversion.ValueToConvert);
      conversion.TypeAfterConversion = this.Rewrite(conversion.TypeAfterConversion);
    }

    /// <summary>
    /// Rewrites the children of the given copy memory statement.
    /// </summary>
    public virtual void RewriteChildren(CopyMemoryStatement copyMemoryStatement) {
      Contract.Requires(copyMemoryStatement != null);

      this.RewriteChildren((Statement)copyMemoryStatement);
      copyMemoryStatement.TargetAddress = this.Rewrite(copyMemoryStatement.TargetAddress);
      copyMemoryStatement.SourceAddress = this.Rewrite(copyMemoryStatement.SourceAddress);
      copyMemoryStatement.NumberOfBytesToCopy = this.Rewrite(copyMemoryStatement.NumberOfBytesToCopy);
    }

    /// <summary>
    /// Rewrites the children of the given array creation expression.
    /// </summary>
    public virtual void RewriteChildren(CreateArray createArray) {
      Contract.Requires(createArray != null);

      this.RewriteChildren((Expression)createArray);
      createArray.ElementType = this.Rewrite(createArray.ElementType);
      createArray.Sizes = this.Rewrite(createArray.Sizes);
      createArray.Initializers = this.Rewrite(createArray.Initializers);
    }

    /// <summary>
    /// Rewrites the children of the anonymous object creation expression.
    /// </summary>
    public virtual void RewriteChildren(CreateDelegateInstance createDelegateInstance) {
      Contract.Requires(createDelegateInstance != null);

      this.RewriteChildren((Expression)createDelegateInstance);
      if (createDelegateInstance.Instance != null)
        createDelegateInstance.Instance = this.Rewrite(createDelegateInstance.Instance);
      createDelegateInstance.MethodToCallViaDelegate = this.Rewrite(createDelegateInstance.MethodToCallViaDelegate);
    }

    /// <summary>
    /// Rewrites the children of the given constructor call expression.
    /// </summary>
    public virtual void RewriteChildren(CreateObjectInstance createObjectInstance) {
      Contract.Requires(createObjectInstance != null);

      this.RewriteChildren((ConstructorOrMethodCall)createObjectInstance);
    }

    /// <summary>
    /// Rewrites the children of the given debugger break statement.
    /// </summary>
    public virtual void RewriteChildren(DebuggerBreakStatement debuggerBreakStatement) {
      Contract.Requires(debuggerBreakStatement != null);

      this.RewriteChildren((Statement)debuggerBreakStatement);
    }

    /// <summary>
    /// Rewrites the children of the given defalut value expression.
    /// </summary>
    public virtual void RewriteChildren(DefaultValue defaultValue) {
      Contract.Requires(defaultValue != null);

      this.RewriteChildren((Expression)defaultValue);
      defaultValue.DefaultValueType = this.Rewrite(defaultValue.DefaultValueType);
    }

    /// <summary>
    /// Rewrites the children of the given division expression.
    /// </summary>
    public virtual void RewriteChildren(Division division) {
      Contract.Requires(division != null);

      this.RewriteChildren((BinaryOperation)division);
    }

    /// <summary>
    /// Rewrites the children of the given do until statement.
    /// </summary>
    public virtual void RewriteChildren(DoUntilStatement doUntilStatement) {
      Contract.Requires(doUntilStatement != null);

      this.RewriteChildren((Statement)doUntilStatement);
      doUntilStatement.Body = this.Rewrite(doUntilStatement.Body);
      doUntilStatement.Condition = this.Rewrite(doUntilStatement.Condition);
    }

    /// <summary>
    /// Rewrites the children of the given dup value expression.
    /// </summary>
    public virtual void RewriteChildren(DupValue dupValue) {
      Contract.Requires(dupValue != null);

      this.RewriteChildren((Expression)dupValue);
    }

    /// <summary>
    /// Rewrites the children of the given empty statement.
    /// </summary>
    public virtual void RewriteChildren(EmptyStatement emptyStatement) {
      Contract.Requires(emptyStatement != null);

      this.RewriteChildren((Statement)emptyStatement);
    }

    /// <summary>
    /// Rewrites the children of the given equality expression.
    /// </summary>
    public virtual void RewriteChildren(Equality equality) {
      Contract.Requires(equality != null);

      this.RewriteChildren((BinaryOperation)equality);
    }

    /// <summary>
    /// Rewrites the children of the given exclusive or expression.
    /// </summary>
    public virtual void RewriteChildren(ExclusiveOr exclusiveOr) {
      Contract.Requires(exclusiveOr != null);

      this.RewriteChildren((BinaryOperation)exclusiveOr);
    }

    /// <summary>
    /// Called from the type specific rewrite method to rewrite the common part of all expressions.
    /// </summary>
    public virtual void RewriteChildren(Expression expression) {
      Contract.Requires(expression != null);

      expression.Type = this.Rewrite(expression.Type);
    }

    /// <summary>
    /// Rewrites the children of the given expression statement.
    /// </summary>
    public virtual void RewriteChildren(ExpressionStatement expressionStatement) {
      Contract.Requires(expressionStatement != null);

      this.RewriteChildren((Statement)expressionStatement);
      expressionStatement.Expression = this.Rewrite(expressionStatement.Expression);
    }

    /// <summary>
    /// Rewrites the children of the given fill memory statement.
    /// </summary>
    public virtual void RewriteChildren(FillMemoryStatement fillMemoryStatement) {
      Contract.Requires(fillMemoryStatement != null);

      this.RewriteChildren((Statement)fillMemoryStatement);
      fillMemoryStatement.TargetAddress = this.Rewrite(fillMemoryStatement.TargetAddress);
      fillMemoryStatement.FillValue = this.Rewrite(fillMemoryStatement.FillValue);
      fillMemoryStatement.NumberOfBytesToFill = this.Rewrite(fillMemoryStatement.NumberOfBytesToFill);
    }

    /// <summary>
    /// Rewrites the children of the given foreach statement.
    /// </summary>
    public virtual void RewriteChildren(ForEachStatement forEachStatement) {
      Contract.Requires(forEachStatement != null);

      this.RewriteChildren((Statement)forEachStatement);
      forEachStatement.Variable = this.Rewrite((LocalDefinition)forEachStatement.Variable);
      forEachStatement.Collection = this.Rewrite(forEachStatement.Collection);
      forEachStatement.Body = this.Rewrite(forEachStatement.Body);
    }

    /// <summary>
    /// Rewrites the children of the given for statement.
    /// </summary>
    public virtual void RewriteChildren(ForStatement forStatement) {
      Contract.Requires(forStatement != null);

      this.RewriteChildren((Statement)forStatement);
      forStatement.InitStatements = this.Rewrite(forStatement.InitStatements);
      forStatement.Condition = this.Rewrite(forStatement.Condition);
      forStatement.IncrementStatements = this.Rewrite(forStatement.IncrementStatements);
      forStatement.Body = this.Rewrite(forStatement.Body);
    }

    /// <summary>
    /// Rewrites the children of the given get type of typed reference expression.
    /// </summary>
    public virtual void RewriteChildren(GetTypeOfTypedReference getTypeOfTypedReference) {
      Contract.Requires(getTypeOfTypedReference != null);

      this.RewriteChildren((Expression)getTypeOfTypedReference);
      getTypeOfTypedReference.TypedReference = this.Rewrite(getTypeOfTypedReference.TypedReference);
    }

    /// <summary>
    /// Rewrites the children of the given get value of typed reference expression.
    /// </summary>
    public virtual void RewriteChildren(GetValueOfTypedReference getValueOfTypedReference) {
      Contract.Requires(getValueOfTypedReference != null);

      this.RewriteChildren((Expression)getValueOfTypedReference);
      getValueOfTypedReference.TypedReference = this.Rewrite(getValueOfTypedReference.TypedReference);
      getValueOfTypedReference.TargetType = this.Rewrite(getValueOfTypedReference.TargetType);
    }

    /// <summary>
    /// Rewrites the children of the given goto statement.
    /// </summary>
    public virtual void RewriteChildren(GotoStatement gotoStatement) {
      Contract.Requires(gotoStatement != null);

      this.RewriteChildren((Statement)gotoStatement);
    }

    /// <summary>
    /// Rewrites the children of the given goto switch case statement.
    /// </summary>
    public virtual void RewriteChildren(GotoSwitchCaseStatement gotoSwitchCaseStatement) {
      Contract.Requires(gotoSwitchCaseStatement != null);

      this.RewriteChildren((Statement)gotoSwitchCaseStatement);
    }

    /// <summary>
    /// Rewrites the children of the given greater-than expression.
    /// </summary>
    public virtual void RewriteChildren(GreaterThan greaterThan) {
      Contract.Requires(greaterThan != null);

      this.RewriteChildren((BinaryOperation)greaterThan);
    }

    /// <summary>
    /// Rewrites the children of the given greater-than-or-equal expression.
    /// </summary>
    public virtual void RewriteChildren(GreaterThanOrEqual greaterThanOrEqual) {
      Contract.Requires(greaterThanOrEqual != null);

      this.RewriteChildren((BinaryOperation)greaterThanOrEqual);
    }

    /// <summary>
    /// Rewrites the children of the given labeled statement.
    /// </summary>
    public virtual void RewriteChildren(LabeledStatement labeledStatement) {
      Contract.Requires(labeledStatement != null);

      this.RewriteChildren((Statement)labeledStatement);
      labeledStatement.Statement = this.Rewrite(labeledStatement.Statement);
    }

    /// <summary>
    /// Rewrites the children of the given left shift expression.
    /// </summary>
    public virtual void RewriteChildren(LeftShift leftShift) {
      Contract.Requires(leftShift != null);

      this.RewriteChildren((BinaryOperation)leftShift);
    }

    /// <summary>
    /// Rewrites the children of the given less-than expression.
    /// </summary>
    public virtual void RewriteChildren(LessThan lessThan) {
      Contract.Requires(lessThan != null);

      this.RewriteChildren((BinaryOperation)lessThan);
    }

    /// <summary>
    /// Rewrites the children of the given less-than-or-equal expression.
    /// </summary>
    public virtual void RewriteChildren(LessThanOrEqual lessThanOrEqual) {
      Contract.Requires(lessThanOrEqual != null);

      this.RewriteChildren((BinaryOperation)lessThanOrEqual);
    }

    /// <summary>
    /// Rewrites the children of the given local declaration statement.
    /// </summary>
    public virtual void RewriteChildren(LocalDeclarationStatement localDeclarationStatement) {
      Contract.Requires(localDeclarationStatement != null);

      this.RewriteChildren((Statement)localDeclarationStatement);
      localDeclarationStatement.LocalVariable = this.Rewrite(localDeclarationStatement.LocalVariable);
      if (localDeclarationStatement.InitialValue != null)
        localDeclarationStatement.InitialValue = this.Rewrite(localDeclarationStatement.InitialValue);
    }

    /// <summary>
    /// Rewrites the children of the given lock statement.
    /// </summary>
    public virtual void RewriteChildren(LockStatement lockStatement) {
      Contract.Requires(lockStatement != null);

      this.RewriteChildren((Statement)lockStatement);
      lockStatement.Guard = this.Rewrite(lockStatement.Guard);
      lockStatement.Body = this.Rewrite(lockStatement.Body);
    }

    /// <summary>
    /// Rewrites the children of the given logical not expression.
    /// </summary>
    public virtual void RewriteChildren(LogicalNot logicalNot) {
      Contract.Requires(logicalNot != null);

      this.RewriteChildren((UnaryOperation)logicalNot);
    }

    /// <summary>
    /// Rewrites the children of the given make typed reference expression.
    /// </summary>
    public virtual void RewriteChildren(MakeTypedReference makeTypedReference) {
      Contract.Requires(makeTypedReference != null);

      this.RewriteChildren((Expression)makeTypedReference);
      makeTypedReference.Operand = this.Rewrite(makeTypedReference.Operand);
    }

    /// <summary>
    /// Rewrites the children of the given method call.
    /// </summary>
    public virtual void RewriteChildren(MethodCall methodCall) {
      Contract.Requires(methodCall != null);

      if (!methodCall.IsStaticCall && !methodCall.IsJumpCall)
        methodCall.ThisArgument = this.Rewrite(methodCall.ThisArgument);
      this.RewriteChildren((ConstructorOrMethodCall)methodCall);
    }

    /// <summary>
    /// Rewrites the children of the given modulus expression.
    /// </summary>
    public virtual void RewriteChildren(Modulus modulus) {
      Contract.Requires(modulus != null);

      this.RewriteChildren((BinaryOperation)modulus);
    }

    /// <summary>
    /// Rewrites the children of the given multiplication expression.
    /// </summary>
    public virtual void RewriteChildren(Multiplication multiplication) {
      Contract.Requires(multiplication != null);

      this.RewriteChildren((BinaryOperation)multiplication);
    }

    /// <summary>
    /// Rewrites the children of the given named argument expression.
    /// </summary>
    public virtual void RewriteChildren(NamedArgument namedArgument) {
      Contract.Requires(namedArgument != null);

      this.RewriteChildren((Expression)namedArgument);
      namedArgument.ArgumentValue = this.Rewrite(namedArgument.ArgumentValue);
    }

    /// <summary>
    /// Rewrites the children of the given not equality expression.
    /// </summary>
    public virtual void RewriteChildren(NotEquality notEquality) {
      Contract.Requires(notEquality != null);

      this.RewriteChildren((BinaryOperation)notEquality);
    }

    /// <summary>
    /// Rewrites the children of the given old value expression.
    /// </summary>
    public virtual void RewriteChildren(OldValue oldValue) {
      Contract.Requires(oldValue != null);

      this.RewriteChildren((Expression)oldValue);
      oldValue.Expression = this.Rewrite(oldValue.Expression);
    }

    /// <summary>
    /// Rewrites the children of the given one's complement expression.
    /// </summary>
    public virtual void RewriteChildren(OnesComplement onesComplement) {
      Contract.Requires(onesComplement != null);

      this.RewriteChildren((UnaryOperation)onesComplement);
    }

    /// <summary>
    /// Rewrites the children of the given out argument expression.
    /// </summary>
    public virtual void RewriteChildren(OutArgument outArgument) {
      Contract.Requires(outArgument != null);

      this.RewriteChildren((Expression)outArgument);
      outArgument.Expression = (ITargetExpression)this.Rewrite((TargetExpression)outArgument.Expression);
    }

    /// <summary>
    /// Rewrites the children of the given pointer call.
    /// </summary>
    public virtual void RewriteChildren(PointerCall pointerCall) {
      Contract.Requires(pointerCall != null);

      this.RewriteChildren((Expression)pointerCall);
      pointerCall.Pointer = this.Rewrite(pointerCall.Pointer);
      pointerCall.Arguments = this.Rewrite(pointerCall.Arguments);
    }

    /// <summary>
    /// Rewrites the children of the given pop value expression.
    /// </summary>
    public virtual void RewriteChildren(PopValue popValue) {
      Contract.Requires(popValue != null);

      this.RewriteChildren((Expression)popValue);
    }

    /// <summary>
    /// Rewrites the children of the given push statement.
    /// </summary>
    public virtual void RewriteChildren(PushStatement pushStatement) {
      Contract.Requires(pushStatement != null);

      this.RewriteChildren((Statement)pushStatement);
      pushStatement.ValueToPush = this.Rewrite(pushStatement.ValueToPush);
    }

    /// <summary>
    /// Rewrites the children of the given ref argument expression.
    /// </summary>
    public virtual void RewriteChildren(RefArgument refArgument) {
      Contract.Requires(refArgument != null);

      this.RewriteChildren((Expression)refArgument);
      refArgument.Expression = this.Rewrite((AddressableExpression)refArgument.Expression);
    }

    /// <summary>
    /// Rewrites the children of the given resource usage statement.
    /// </summary>
    public virtual void RewriteChildren(ResourceUseStatement resourceUseStatement) {
      Contract.Requires(resourceUseStatement != null);

      this.RewriteChildren((Statement)resourceUseStatement);
      resourceUseStatement.ResourceAcquisitions = this.Rewrite(resourceUseStatement.ResourceAcquisitions);
      resourceUseStatement.Body = this.Rewrite(resourceUseStatement.Body);
    }

    /// <summary>
    /// Rewrites the children of the rethrow statement.
    /// </summary>
    public virtual void RewriteChildren(RethrowStatement rethrowStatement) {
      Contract.Requires(rethrowStatement != null);

      this.RewriteChildren((Statement)rethrowStatement);
    }

    /// <summary>
    /// Rewrites the children of the return statement.
    /// </summary>
    public virtual void RewriteChildren(ReturnStatement returnStatement) {
      Contract.Requires(returnStatement != null);

      this.RewriteChildren((Statement)returnStatement);
      if (returnStatement.Expression != null)
        returnStatement.Expression = this.Rewrite(returnStatement.Expression);
    }

    /// <summary>
    /// Rewrites the children of the given return value expression.
    /// </summary>
    public virtual void RewriteChildren(ReturnValue returnValue) {
      Contract.Requires(returnValue != null);

      this.RewriteChildren((Expression)returnValue);
    }

    /// <summary>
    /// Rewrites the children of the given right shift expression.
    /// </summary>
    public virtual void RewriteChildren(RightShift rightShift) {
      Contract.Requires(rightShift != null);

      this.RewriteChildren((BinaryOperation)rightShift);
    }

    /// <summary>
    /// Rewrites the children of the given runtime argument handle expression.
    /// </summary>
    public virtual void RewriteChildren(RuntimeArgumentHandleExpression runtimeArgumentHandleExpression) {
      Contract.Requires(runtimeArgumentHandleExpression != null);

      this.RewriteChildren((Expression)runtimeArgumentHandleExpression);
    }

    /// <summary>
    /// Rewrites the children of the given sizeof() expression.
    /// </summary>
    public virtual void RewriteChildren(SizeOf sizeOf) {
      Contract.Requires(sizeOf != null);

      this.RewriteChildren((Expression)sizeOf);
      sizeOf.TypeToSize = this.Rewrite(sizeOf.TypeToSize);
    }

    /// <summary>
    /// Rewrites the children of the the given source method body.
    /// </summary>
    public virtual void RewriteChildren(SourceMethodBody sourceMethodBody) {
      Contract.Requires(sourceMethodBody != null);

      sourceMethodBody.Block = this.Rewrite((BlockStatement)sourceMethodBody.Block);
    }

    /// <summary>
    /// Rewrites the children of the given stack array create expression.
    /// </summary>
    public virtual void RewriteChildren(StackArrayCreate stackArrayCreate) {
      Contract.Requires(stackArrayCreate != null);

      this.RewriteChildren((Expression)stackArrayCreate);
      stackArrayCreate.ElementType = this.Rewrite(stackArrayCreate.ElementType);
      stackArrayCreate.Size = this.Rewrite(stackArrayCreate.Size);
    }

    /// <summary>
    /// Called from the type specific rewrite method to rewrite the common part of all statements.
    /// </summary>
    public virtual void RewriteChildren(Statement statement) {
      //This is just an extension hook
    }

    /// <summary>
    /// Rewrites the children of the given subtraction expression.
    /// </summary>
    public virtual void RewriteChildren(Subtraction subtraction) {
      Contract.Requires(subtraction != null);

      this.RewriteChildren((BinaryOperation)subtraction);
    }

    /// <summary>
    /// Rewrites the children of the given switch case.
    /// </summary>
    public virtual void RewriteChildren(SwitchCase switchCase) {
      Contract.Requires(switchCase != null);

      if (!switchCase.IsDefault)
        switchCase.Expression = this.Rewrite((CompileTimeConstant)switchCase.Expression);
      switchCase.Body = this.Rewrite(switchCase.Body);
    }

    /// <summary>
    /// Rewrites the children of the given switch statement.
    /// </summary>
    public virtual void RewriteChildren(SwitchStatement switchStatement) {
      Contract.Requires(switchStatement != null);

      this.RewriteChildren((Statement)switchStatement);
      switchStatement.Expression = this.Rewrite(switchStatement.Expression);
      switchStatement.Cases = this.Rewrite(switchStatement.Cases);
    }

    /// <summary>
    /// Rewrites the children of the given target expression.
    /// </summary>
    public virtual void RewriteChildren(TargetExpression targetExpression) {
      Contract.Requires(targetExpression != null);

      this.RewriteChildren((Expression)targetExpression);
      var local = targetExpression.Definition as ILocalDefinition;
      if (local != null)
        targetExpression.Definition = this.RewriteReference(local);
      else {
        var parameter = targetExpression.Definition as IParameterDefinition;
        if (parameter != null)
          targetExpression.Definition = this.RewriteReference(parameter);
        else {
          var fieldReference = targetExpression.Definition as IFieldReference;
          if (fieldReference != null)
            targetExpression.Definition = this.Rewrite(fieldReference);
          else {
            var arrayIndexer = targetExpression.Definition as IArrayIndexer;
            if (arrayIndexer != null) {
              targetExpression.Definition = this.Rewrite(arrayIndexer);
              arrayIndexer = targetExpression.Definition as IArrayIndexer;
              if (arrayIndexer != null) {
                targetExpression.Instance = arrayIndexer.IndexedObject;
                return;
              }
            } else {
              var addressDereference = targetExpression.Definition as IAddressDereference;
              if (addressDereference != null)
                targetExpression.Definition = this.Rewrite(addressDereference);
              else {
                var propertyDefinition = targetExpression.Definition as IPropertyDefinition;
                if (propertyDefinition != null)
                  targetExpression.Definition = this.Rewrite(propertyDefinition);
                else
                  targetExpression.Definition = this.Rewrite((IThisReference)targetExpression.Definition);
              }
            }
          }
        }
      }
      if (targetExpression.Instance != null) {
        targetExpression.Instance = this.Rewrite(targetExpression.Instance);
      }
    }

    /// <summary>
    /// Rewrites the children of the given this reference expression.
    /// </summary>
    public virtual void RewriteChildren(ThisReference thisReference) {
      Contract.Requires(thisReference != null);

      this.RewriteChildren((Expression)thisReference);
    }

    /// <summary>
    /// Rewrites the children of the throw statement.
    /// </summary>
    public virtual void RewriteChildren(ThrowStatement throwStatement) {
      Contract.Requires(throwStatement != null);

      this.RewriteChildren((Statement)throwStatement);
      throwStatement.Exception = this.Rewrite(throwStatement.Exception);
    }

    /// <summary>
    /// Rewrites the children of the given tokenof() expression.
    /// </summary>
    public virtual void RewriteChildren(TokenOf tokenOf) {
      Contract.Requires(tokenOf != null);

      this.RewriteChildren((Expression)tokenOf);
      var fieldReference = tokenOf.Definition as IFieldReference;
      if (fieldReference != null)
        tokenOf.Definition = this.Rewrite(fieldReference);
      else {
        var methodReference = tokenOf.Definition as IMethodReference;
        if (methodReference != null)
          tokenOf.Definition = this.Rewrite(methodReference);
        else {
          var typeReference = (ITypeReference)tokenOf.Definition;
          tokenOf.Definition = this.Rewrite(typeReference);
        }
      }
    }

    /// <summary>
    /// Rewrites the children of the try-catch-filter-finally statement.
    /// </summary>
    public virtual void RewriteChildren(TryCatchFinallyStatement tryCatchFilterFinallyStatement) {
      Contract.Requires(tryCatchFilterFinallyStatement != null);

      this.RewriteChildren((Statement)tryCatchFilterFinallyStatement);
      tryCatchFilterFinallyStatement.TryBody = this.Rewrite((BlockStatement)tryCatchFilterFinallyStatement.TryBody);
      tryCatchFilterFinallyStatement.CatchClauses = this.Rewrite(tryCatchFilterFinallyStatement.CatchClauses);
      if (tryCatchFilterFinallyStatement.FaultBody != null)
        tryCatchFilterFinallyStatement.FaultBody = this.Rewrite((BlockStatement)tryCatchFilterFinallyStatement.FaultBody);
      if (tryCatchFilterFinallyStatement.FinallyBody != null)
        tryCatchFilterFinallyStatement.FinallyBody = this.Rewrite((BlockStatement)tryCatchFilterFinallyStatement.FinallyBody);
    }

    /// <summary>
    /// Rewrites the children of the given typeof() expression.
    /// </summary>
    public virtual void RewriteChildren(TypeOf typeOf) {
      Contract.Requires(typeOf != null);

      this.RewriteChildren((Expression)typeOf);
      typeOf.TypeToGet = this.Rewrite(typeOf.TypeToGet);
    }

    /// <summary>
    /// Rewrites the children of the given unary negation expression.
    /// </summary>
    public virtual void RewriteChildren(UnaryNegation unaryNegation) {
      Contract.Requires(unaryNegation != null);

      this.RewriteChildren((UnaryOperation)unaryNegation);
    }

    /// <summary>
    /// Called from the type specific rewrite method to rewrite the common part of all unary operation expressions.
    /// </summary>
    public virtual void RewriteChildren(UnaryOperation unaryOperation) {
      Contract.Requires(unaryOperation != null);

      this.RewriteChildren((Expression)unaryOperation);
      unaryOperation.Operand = this.Rewrite(unaryOperation.Operand);
    }

    /// <summary>
    /// Rewrites the children of the given unary plus expression.
    /// </summary>
    public virtual void RewriteChildren(UnaryPlus unaryPlus) {
      Contract.Requires(unaryPlus != null);

      this.RewriteChildren((UnaryOperation)unaryPlus);
    }

    /// <summary>
    /// Rewrites the children of the given vector length expression.
    /// </summary>
    public virtual void RewriteChildren(VectorLength vectorLength) {
      Contract.Requires(vectorLength != null);

      this.RewriteChildren((Expression)vectorLength);
      vectorLength.Vector = this.Rewrite(vectorLength.Vector);
    }

    /// <summary>
    /// Rewrites the children of the given while do statement.
    /// </summary>
    public virtual void RewriteChildren(WhileDoStatement whileDoStatement) {
      Contract.Requires(whileDoStatement != null);

      this.RewriteChildren((Statement)whileDoStatement);
      whileDoStatement.Condition = this.Rewrite(whileDoStatement.Condition);
      whileDoStatement.Body = this.Rewrite(whileDoStatement.Body);
    }

    /// <summary>
    /// Rewrites the children of the given yield break statement.
    /// </summary>
    public virtual void RewriteChildren(YieldBreakStatement yieldBreakStatement) {
      Contract.Requires(yieldBreakStatement != null);

      this.RewriteChildren((Statement)yieldBreakStatement);
    }

    /// <summary>
    /// Rewrites the children of the given yield return statement.
    /// </summary>
    public virtual void RewriteChildren(YieldReturnStatement yieldReturnStatement) {
      Contract.Requires(yieldReturnStatement != null);

      this.RewriteChildren((Statement)yieldReturnStatement);
      yieldReturnStatement.Expression = this.Rewrite(yieldReturnStatement.Expression);
    }

  }

}
