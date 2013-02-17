namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;

    public class OperatorCodeRewriter
    {
        public MutationTarget MutationTarget { get; set; }

        public INameTable NameTable { get; set; }

        public MetadataReaderHost Host { get; set; }

        public Module Module { get; set; }

        public OperatorCodeVisitor OperatorCodeVisitor { get; set; }

        /// <summary>
        /// Rewrites the given addition.
        /// </summary>
        /// <param name="addition"></param>
        public virtual IExpression Rewrite(IAddition addition)
        {
            return addition;
        }


        /// <summary>
        /// Rewrites the given addressable expression.
        /// </summary>
        /// <param name="addressableExpression"></param>
        public virtual IAddressableExpression Rewrite(IAddressableExpression addressableExpression)
        {
            return addressableExpression;
        }


        /// <summary>
        /// Rewrites the given address dereference expression.
        /// </summary>
        /// <param name="addressDereference"></param>
        public virtual IExpression Rewrite(IAddressDereference addressDereference)
        {
            return addressDereference;
        }


        /// <summary>
        /// Rewrites the given AddressOf expression.
        /// </summary>
        /// <param name="addressOf"></param>
        public virtual IExpression Rewrite(IAddressOf addressOf)
        {
            return addressOf;
        }


        /// <summary>
        /// Rewrites the given anonymous delegate expression.
        /// </summary>
        /// <param name="anonymousDelegate"></param>
        public virtual IExpression Rewrite(IAnonymousDelegate anonymousDelegate)
        {
            return anonymousDelegate;
        }


        /// <summary>
        /// Rewrites the given array indexer expression.
        /// </summary>
        /// <param name="arrayIndexer"></param>
        public virtual IExpression Rewrite(IArrayIndexer arrayIndexer)
        {
            return arrayIndexer;
        }


        /// <summary>
        /// Rewrites the given assert statement.
        /// </summary>
        /// <param name="assertStatement"></param>
        public virtual IStatement Rewrite(IAssertStatement assertStatement)
        {
            return assertStatement;
        }


        /// <summary>
        /// Rewrites the given assignment expression.
        /// </summary>
        /// <param name="assignment"></param>
        public virtual IExpression Rewrite(IAssignment assignment)
        {
            return assignment;
        }


        /// <summary>
        /// Rewrites the given assume statement.
        /// </summary>
        /// <param name="assumeStatement"></param>
        public virtual IStatement Rewrite(IAssumeStatement assumeStatement)
        {
            return assumeStatement;
        }


        /// <summary>
        /// Rewrites the given bitwise and expression.
        /// </summary>
        /// <param name="binaryOperation"></param>
        public virtual IExpression Rewrite(IBinaryOperation binaryOperation)
        {
            return binaryOperation;
        }


        /// <summary>
        /// Rewrites the given bitwise and expression.
        /// </summary>
        /// <param name="bitwiseAnd"></param>
        public virtual IExpression Rewrite(IBitwiseAnd bitwiseAnd)
        {
            return bitwiseAnd;
        }


        /// <summary>
        /// Rewrites the given bitwise or expression.
        /// </summary>
        /// <param name="bitwiseOr"></param>
        public virtual IExpression Rewrite(IBitwiseOr bitwiseOr)
        {
            return bitwiseOr;
        }


        /// <summary>
        /// Rewrites the given block expression.
        /// </summary>
        /// <param name="blockExpression"></param>
        public virtual IExpression Rewrite(IBlockExpression blockExpression)
        {
            return blockExpression;
        }


        /// <summary>
        /// Rewrites the given statement block.
        /// </summary>
        /// <param name="block"></param>
        public virtual IBlockStatement Rewrite(IBlockStatement block)
        {
            return block;
        }


        /// <summary>
        /// Rewrites the given bound expression.
        /// </summary>
        /// <param name="boundExpression"></param>
        public virtual IExpression Rewrite(IBoundExpression boundExpression)
        {
            return boundExpression;
        }


        /// <summary>
        /// Rewrites the given break statement.
        /// </summary>
        /// <param name="breakStatement"></param>
        public virtual IStatement Rewrite(IBreakStatement breakStatement)
        {
            return breakStatement;
        }


        /// <summary>
        /// Rewrites the cast-if-possible expression.
        /// </summary>
        /// <param name="castIfPossible"></param>
        public virtual IExpression Rewrite(ICastIfPossible castIfPossible)
        {
            return castIfPossible;
        }


        /// <summary>
        /// Rewrites the given catch clause.
        /// </summary>
        /// <param name="catchClause"></param>
        public virtual ICatchClause Rewrite(ICatchClause catchClause)
        {
            return catchClause;
        }


        /// <summary>
        /// Rewrites the given check-if-instance expression.
        /// </summary>
        /// <param name="checkIfInstance"></param>
        public virtual IExpression Rewrite(ICheckIfInstance checkIfInstance)
        {
            return checkIfInstance;
        }


        /// <summary>
        /// Rewrites the given compile time constant.
        /// </summary>
        /// <param name="constant"></param>
        public virtual ICompileTimeConstant Rewrite(ICompileTimeConstant constant)
        {
            return constant;
        }


        /// <summary>
        /// Rewrites the given conditional expression.
        /// </summary>
        /// <param name="conditional"></param>
        public virtual IExpression Rewrite(IConditional conditional)
        {
            return conditional;
        }


        /// <summary>
        /// Rewrites the given conditional statement.
        /// </summary>
        /// <param name="conditionalStatement"></param>
        public virtual IStatement Rewrite(IConditionalStatement conditionalStatement)
        {
            return conditionalStatement;
        }


        /// <summary>
        /// Rewrites the given continue statement.
        /// </summary>
        /// <param name="continueStatement"></param>
        public virtual IStatement Rewrite(IContinueStatement continueStatement)
        {
            return continueStatement;
        }


        /// <summary>
        /// Rewrites the given conversion expression.
        /// </summary>
        /// <param name="conversion"></param>
        public virtual IExpression Rewrite(IConversion conversion)
        {
            return conversion;
        }


        /// <summary>
        /// Rewrites the given copy memory statement.
        /// </summary>
        /// <param name="copyMemoryStatement"></param>
        public virtual IStatement Rewrite(ICopyMemoryStatement copyMemoryStatement)
        {
            return copyMemoryStatement;
        }


        /// <summary>
        /// Rewrites the given array creation expression.
        /// </summary>
        /// <param name="createArray"></param>
        public virtual IExpression Rewrite(ICreateArray createArray)
        {
            return createArray;
        }


        /// <summary>
        /// Rewrites the anonymous object creation expression.
        /// </summary>
        /// <param name="createDelegateInstance"></param>
        public virtual IExpression Rewrite(ICreateDelegateInstance createDelegateInstance)
        {
            return createDelegateInstance;
        }


        /// <summary>
        /// Rewrites the given constructor call expression.
        /// </summary>
        /// <param name="createObjectInstance"></param>
        public virtual IExpression Rewrite(ICreateObjectInstance createObjectInstance)
        {
            return createObjectInstance;
        }


        /// <summary>
        /// Rewrites the given debugger break statement.
        /// </summary>
        /// <param name="debuggerBreakStatement"></param>
        public virtual IStatement Rewrite(IDebuggerBreakStatement debuggerBreakStatement)
        {
            return debuggerBreakStatement;
        }


        /// <summary>
        /// Rewrites the given defalut value expression.
        /// </summary>
        /// <param name="defaultValue"></param>
        public virtual IExpression Rewrite(IDefaultValue defaultValue)
        {
            return defaultValue;
        }


        /// <summary>
        /// Rewrites the given division expression.
        /// </summary>
        /// <param name="division"></param>
        public virtual IExpression Rewrite(IDivision division)
        {
            return division;
        }


        /// <summary>
        /// Rewrites the given do until statement.
        /// </summary>
        /// <param name="doUntilStatement"></param>
        public virtual IStatement Rewrite(IDoUntilStatement doUntilStatement)
        {
            return doUntilStatement;
        }


        /// <summary>
        /// Rewrites the given dup value expression.
        /// </summary>
        /// <param name="dupValue"></param>
        public virtual IExpression Rewrite(IDupValue dupValue)
        {
            return dupValue;
        }


        /// <summary>
        /// Rewrites the given empty statement.
        /// </summary>
        /// <param name="emptyStatement"></param>
        public virtual IStatement Rewrite(IEmptyStatement emptyStatement)
        {
            return emptyStatement;
        }


        /// <summary>
        /// Rewrites the given equality expression.
        /// </summary>
        /// <param name="equality"></param>
        public virtual IExpression Rewrite(IEquality equality)
        {
            return equality;
        }


        /// <summary>
        /// Rewrites the given exclusive or expression.
        /// </summary>
        /// <param name="exclusiveOr"></param>
        public virtual IExpression Rewrite(IExclusiveOr exclusiveOr)
        {
            return exclusiveOr;
        }


        /// <summary>
        /// Rewrites the given expression.
        /// </summary>
        /// <param name="expression"></param>
        public virtual IExpression Rewrite(IExpression expression)
        {
            return expression;
        }


        /// <summary>
        /// Rewrites the given expression statement.
        /// </summary>
        /// <param name="expressionStatement"></param>
        public virtual IStatement Rewrite(IExpressionStatement expressionStatement)
        {
            return expressionStatement;
        }


        /// <summary>
        /// Rewrites the given fill memory statement.
        /// </summary>
        /// <param name="fillMemoryStatement"></param>
        public virtual IStatement Rewrite(IFillMemoryStatement fillMemoryStatement)
        {
            return fillMemoryStatement;
        }


        /// <summary>
        /// Rewrites the given foreach statement.
        /// </summary>
        /// <param name="forEachStatement"></param>
        public virtual IStatement Rewrite(IForEachStatement forEachStatement)
        {
            return forEachStatement;
        }


        /// <summary>
        /// Rewrites the given for statement.
        /// </summary>
        /// <param name="forStatement"></param>
        public virtual IStatement Rewrite(IForStatement forStatement)
        {
            return forStatement;
        }


        /// <summary>
        /// Rewrites the given get type of typed reference expression.
        /// </summary>
        /// <param name="getTypeOfTypedReference"></param>
        public virtual IExpression Rewrite(IGetTypeOfTypedReference getTypeOfTypedReference)
        {
            return getTypeOfTypedReference;
        }


        /// <summary>
        /// Rewrites the given get value of typed reference expression.
        /// </summary>
        /// <param name="getValueOfTypedReference"></param>
        public virtual IExpression Rewrite(IGetValueOfTypedReference getValueOfTypedReference)
        {
            return getValueOfTypedReference;
        }


        /// <summary>
        /// Rewrites the given goto statement.
        /// </summary>
        /// <param name="gotoStatement"></param>
        public virtual IStatement Rewrite(IGotoStatement gotoStatement)
        {
            return gotoStatement;
        }


        /// <summary>
        /// Rewrites the given goto switch case statement.
        /// </summary>
        /// <param name="gotoSwitchCaseStatement"></param>
        public virtual IStatement Rewrite(IGotoSwitchCaseStatement gotoSwitchCaseStatement)
        {
            return gotoSwitchCaseStatement;
        }


        /// <summary>
        /// Rewrites the given greater-than expression.
        /// </summary>
        /// <param name="greaterThan"></param>
        public virtual IExpression Rewrite(IGreaterThan greaterThan)
        {
            return greaterThan;
        }


        /// <summary>
        /// Rewrites the given greater-than-or-equal expression.
        /// </summary>
        /// <param name="greaterThanOrEqual"></param>
        public virtual IExpression Rewrite(IGreaterThanOrEqual greaterThanOrEqual)
        {
            return greaterThanOrEqual;
        }


        /// <summary>
        /// Rewrites the given labeled statement.
        /// </summary>
        /// <param name="labeledStatement"></param>
        public virtual IStatement Rewrite(ILabeledStatement labeledStatement)
        {
            return labeledStatement;
        }


        /// <summary>
        /// Rewrites the given left shift expression.
        /// </summary>
        /// <param name="leftShift"></param>
        public virtual IExpression Rewrite(ILeftShift leftShift)
        {
            return leftShift;
        }


        /// <summary>
        /// Rewrites the given less-than expression.
        /// </summary>
        /// <param name="lessThan"></param>
        public virtual IExpression Rewrite(ILessThan lessThan)
        {
            return lessThan;
        }


        /// <summary>
        /// Rewrites the given less-than-or-equal expression.
        /// </summary>
        /// <param name="lessThanOrEqual"></param>
        public virtual IExpression Rewrite(ILessThanOrEqual lessThanOrEqual)
        {
            return lessThanOrEqual;
        }


        /// <summary>
        /// Rewrites the given local declaration statement.
        /// </summary>
        /// <param name="localDeclarationStatement"></param>
        public virtual IStatement Rewrite(ILocalDeclarationStatement localDeclarationStatement)
        {
            return localDeclarationStatement;
        }


        /// <summary>
        /// Rewrites the given lock statement.
        /// </summary>
        /// <param name="lockStatement"></param>
        public virtual IStatement Rewrite(ILockStatement lockStatement)
        {
            return lockStatement;
        }


        /// <summary>
        /// Rewrites the given logical not expression.
        /// </summary>
        /// <param name="logicalNot"></param>
        public virtual IExpression Rewrite(ILogicalNot logicalNot)
        {
            return logicalNot;
        }


        /// <summary>
        /// Rewrites the given make typed reference expression.
        /// </summary>
        /// <param name="makeTypedReference"></param>
        public virtual IExpression Rewrite(IMakeTypedReference makeTypedReference)
        {
            return makeTypedReference;
        }


        /// <summary>
        /// Rewrites the the given method body.
        /// </summary>
        /// <param name="methodBody"></param>
        public virtual IMethodBody Rewrite(IMethodBody methodBody)
        {
            return methodBody;
        }


        /// <summary>
        /// Rewrites the given method call.
        /// </summary>
        /// <param name="methodCall"></param>
        public virtual IExpression Rewrite(IMethodCall methodCall)
        {
            return methodCall;
        }


        /// <summary>
        /// Rewrites the given modulus expression.
        /// </summary>
        /// <param name="modulus"></param>
        public virtual IExpression Rewrite(IModulus modulus)
        {
            return modulus;
        }


        /// <summary>
        /// Rewrites the given multiplication expression.
        /// </summary>
        /// <param name="multiplication"></param>
        public virtual IExpression Rewrite(IMultiplication multiplication)
        {
            return multiplication;
        }


        /// <summary>
        /// Rewrites the given named argument expression.
        /// </summary>
        /// <param name="namedArgument"></param>
        public virtual IExpression Rewrite(INamedArgument namedArgument)
        {
            return namedArgument;
        }


        /// <summary>
        /// Rewrites the given not equality expression.
        /// </summary>
        /// <param name="notEquality"></param>
        public virtual IExpression Rewrite(INotEquality notEquality)
        {
            return notEquality;
        }


        /// <summary>
        /// Rewrites the given old value expression.
        /// </summary>
        /// <param name="oldValue"></param>
        public virtual IExpression Rewrite(IOldValue oldValue)
        {
            return oldValue;
        }


        /// <summary>
        /// Rewrites the given one's complement expression.
        /// </summary>
        /// <param name="onesComplement"></param>
        public virtual IExpression Rewrite(IOnesComplement onesComplement)
        {
            return onesComplement;
        }


        /// <summary>
        /// Rewrites the given out argument expression.
        /// </summary>
        /// <param name="outArgument"></param>
        public virtual IExpression Rewrite(IOutArgument outArgument)
        {
            return outArgument;
        }


        /// <summary>
        /// Rewrites the given pointer call.
        /// </summary>
        /// <param name="pointerCall"></param>
        public virtual IExpression Rewrite(IPointerCall pointerCall)
        {
            return pointerCall;
        }


        /// <summary>
        /// Rewrites the given pop value expression.
        /// </summary>
        /// <param name="popValue"></param>
        public virtual IExpression Rewrite(IPopValue popValue)
        {
            return popValue;
        }


        /// <summary>
        /// Rewrites the given push statement.
        /// </summary>
        /// <param name="pushStatement"></param>
        public virtual IStatement Rewrite(IPushStatement pushStatement)
        {
            return pushStatement;
        }


        /// <summary>
        /// Rewrites the given ref argument expression.
        /// </summary>
        /// <param name="refArgument"></param>
        public virtual IExpression Rewrite(IRefArgument refArgument)
        {
            return refArgument;
        }


        /// <summary>
        /// Rewrites the given resource usage statement.
        /// </summary>
        /// <param name="resourceUseStatement"></param>
        public virtual IStatement Rewrite(IResourceUseStatement resourceUseStatement)
        {
            return resourceUseStatement;
        }


        /// <summary>
        /// Rewrites the rethrow statement.
        /// </summary>
        /// <param name="rethrowStatement"></param>
        public virtual IStatement Rewrite(IRethrowStatement rethrowStatement)
        {
            return rethrowStatement;
        }


        /// <summary>
        /// Rewrites the return statement.
        /// </summary>
        /// <param name="returnStatement"></param>
        public virtual IStatement Rewrite(IReturnStatement returnStatement)
        {
            return returnStatement;
        }


        /// <summary>
        /// Rewrites the given return value expression.
        /// </summary>
        /// <param name="returnValue"></param>
        public virtual IExpression Rewrite(IReturnValue returnValue)
        {
            return returnValue;
        }


        /// <summary>
        /// Rewrites the given right shift expression.
        /// </summary>
        /// <param name="rightShift"></param>
        public virtual IExpression Rewrite(IRightShift rightShift)
        {
            return rightShift;
        }


        /// <summary>
        /// Rewrites the given runtime argument handle expression.
        /// </summary>
        /// <param name="runtimeArgumentHandleExpression"></param>
        public virtual IExpression Rewrite(IRuntimeArgumentHandleExpression runtimeArgumentHandleExpression)
        {
            return runtimeArgumentHandleExpression;
        }


        /// <summary>
        /// Rewrites the given sizeof(); expression.
        /// </summary>
        /// <param name="sizeOf"></param>
        public virtual IExpression Rewrite(ISizeOf sizeOf)
        {
            return sizeOf;
        }


        /// <summary>
        /// Rewrites the given stack array create expression.
        /// </summary>
        /// <param name="stackArrayCreate"></param>
        public virtual IExpression Rewrite(IStackArrayCreate stackArrayCreate)
        {
            return stackArrayCreate;
        }


        /// <summary>
        /// Rewrites the the given source method body.
        /// </summary>
        /// <param name="sourceMethodBody"></param>
        public virtual ISourceMethodBody Rewrite(ISourceMethodBody sourceMethodBody)
        {
            return sourceMethodBody;
        }


        /// <summary>
        /// Rewrites the specified statement.
        /// </summary>
        /// <param name="statement">The statement.</param>
        public virtual IStatement Rewrite(IStatement statement)
        {
            return statement;
        }


        /// <summary>
        /// Rewrites the given subtraction expression.
        /// </summary>
        /// <param name="subtraction"></param>
        public virtual IExpression Rewrite(ISubtraction subtraction)
        {
            return subtraction;
        }


        /// <summary>
        /// Rewrites the given switch case.
        /// </summary>
        /// <param name="switchCase"></param>
        public virtual ISwitchCase Rewrite(ISwitchCase switchCase)
        {
            return switchCase;
        }


        /// <summary>
        /// Rewrites the given switch statement.
        /// </summary>
        /// <param name="switchStatement"></param>
        public virtual IStatement Rewrite(ISwitchStatement switchStatement)
        {
            return switchStatement;
        }


        /// <summary>
        /// Rewrites the given target expression.
        /// </summary>
        /// <param name="targetExpression"></param>
        public virtual ITargetExpression Rewrite(ITargetExpression targetExpression)
        {
            return targetExpression;
        }


        /// <summary>
        /// Rewrites the given this reference expression.
        /// </summary>
        /// <param name="thisReference"></param>
        public virtual IExpression Rewrite(IThisReference thisReference)
        {
            return thisReference;
        }


        /// <summary>
        /// Rewrites the throw statement.
        /// </summary>
        /// <param name="throwStatement"></param>
        public virtual IStatement Rewrite(IThrowStatement throwStatement)
        {
            return throwStatement;
        }


        /// <summary>
        /// Rewrites the given tokenof(); expression.
        /// </summary>
        /// <param name="tokenOf"></param>
        public virtual IExpression Rewrite(ITokenOf tokenOf)
        {
            return tokenOf;
        }


        /// <summary>
        /// Rewrites the try-catch-filter-finally statement.
        /// </summary>
        /// <param name="tryCatchFilterFinallyStatement"></param>
        public virtual IStatement Rewrite(ITryCatchFinallyStatement tryCatchFilterFinallyStatement)
        {
            return tryCatchFilterFinallyStatement;
        }


        /// <summary>
        /// Rewrites the given typeof(); expression.
        /// </summary>
        /// <param name="typeOf"></param>
        public virtual IExpression Rewrite(ITypeOf typeOf)
        {
            return typeOf;
        }


        /// <summary>
        /// Rewrites the given unary negation expression.
        /// </summary>
        /// <param name="unaryNegation"></param>
        public virtual IExpression Rewrite(IUnaryNegation unaryNegation)
        {
            return unaryNegation;
        }


        /// <summary>
        /// Rewrites the given unary plus expression.
        /// </summary>
        /// <param name="unaryPlus"></param>
        public virtual IExpression Rewrite(IUnaryPlus unaryPlus)
        {
            return unaryPlus;
        }


        /// <summary>
        /// Rewrites the given vector length expression.
        /// </summary>
        /// <param name="vectorLength"></param>
        public virtual IExpression Rewrite(IVectorLength vectorLength)
        {
            return vectorLength;
        }


        /// <summary>
        /// Rewrites the given while do statement.
        /// </summary>
        /// <param name="whileDoStatement"></param>
        public virtual IStatement Rewrite(IWhileDoStatement whileDoStatement)
        {
            return whileDoStatement;
        }


        /// <summary>
        /// Rewrites the given yield break statement.
        /// </summary>
        /// <param name="yieldBreakStatement"></param>
        public virtual IStatement Rewrite(IYieldBreakStatement yieldBreakStatement)
        {
            return yieldBreakStatement;
        }


        /// <summary>
        /// Rewrites the given yield return statement.
        /// </summary>
        /// <param name="yieldReturnStatement"></param>
        public virtual IStatement Rewrite(IYieldReturnStatement yieldReturnStatement)
        {
            return yieldReturnStatement;
        }


        /// <summary>
        /// Rewrites the given list of catch clauses.
        /// </summary>
        /// <param name="catchClauses"></param>
        public virtual List<ICatchClause> Rewrite(List<ICatchClause> catchClauses)
        {
            return catchClauses;
        }


        /// <summary>
        /// Rewrites the given list of expressions.
        /// </summary>
        /// <param name="expressions"></param>
        public virtual List<IExpression> Rewrite(List<IExpression> expressions)
        {
            return expressions;
        }


        /// <summary>
        /// Rewrites the given list of switch cases.
        /// </summary>
        /// <param name="switchCases"></param>
        public virtual List<ISwitchCase> Rewrite(List<ISwitchCase> switchCases)
        {
            return switchCases;
        }


        /// <summary>
        /// Rewrites the given list of statements.
        /// </summary>
        /// <param name="statements"></param>
        public virtual List<IStatement> Rewrite(List<IStatement> statements)
        {
            return statements;
        }


        /// <summary>
        /// Rewrites the alias for type
        /// </summary>
        public virtual IAliasForType Rewrite(IAliasForType aliasForType)
        {
            return aliasForType;
        }


        /// <summary>
        /// Rewrites the alias for type member.
        /// </summary>
        public virtual IAliasMember Rewrite(IAliasMember aliasMember)
        {
            return aliasMember;
        }


        /// <summary>
        /// Rewrites the array type reference.
        /// </summary>
        public virtual IArrayTypeReference Rewrite(IArrayTypeReference arrayTypeReference)
        {
            return arrayTypeReference;
        }


        /// <summary>
        /// Rewrites the given assembly.
        /// </summary>
        public virtual IAssembly Rewrite(IAssembly assembly)
        {
            return assembly;
        }


        /// <summary>
        /// Rewrites the given assembly reference.
        /// </summary>
        public virtual IAssemblyReference Rewrite(IAssemblyReference assemblyReference)
        {
            return assemblyReference;
        }


        /// <summary>
        /// Rewrites the given custom attribute.
        /// </summary>
        public virtual ICustomAttribute Rewrite(ICustomAttribute customAttribute)
        {
            return customAttribute;
        }


        /// <summary>
        /// Rewrites the given custom modifier.
        /// </summary>
        public virtual ICustomModifier Rewrite(ICustomModifier customModifier)
        {
            return customModifier;
        }


        /// <summary>
        /// Rewrites the given event definition.
        /// </summary>
        public virtual IEventDefinition Rewrite(IEventDefinition eventDefinition)
        {
            return eventDefinition;
        }


        /// <summary>
        /// Rewrites the given field definition.
        /// </summary>
        public virtual IFieldDefinition Rewrite(IFieldDefinition fieldDefinition)
        {
            return fieldDefinition;
        }


        /// <summary>
        /// Rewrites the given field reference.
        /// </summary>
        public virtual IFieldReference Rewrite(IFieldReference fieldReference)
        {
            return fieldReference;
        }


        /// <summary>
        /// Rewrites the reference to a local definition.
        /// </summary>
        public virtual object RewriteReference(ILocalDefinition localDefinition)
        {
            return localDefinition;
        }


        /// <summary>
        /// Rewrites the reference to a parameter.
        /// </summary>
        public virtual object RewriteReference(IParameterDefinition parameterDefinition)
        {
            return parameterDefinition;
        }


        /// <summary>
        /// Rewrites the given field reference.
        /// </summary>
        public virtual IFieldReference RewriteUnspecialized(IFieldReference fieldReference)
        {
            return fieldReference;
        }


        /// <summary>
        /// Rewrites the given file reference.
        /// </summary>
        public virtual IFileReference Rewrite(IFileReference fileReference)
        {
            return fileReference;
        }


        /// <summary>
        /// Rewrites the given function pointer type reference.
        /// </summary>
        public virtual IFunctionPointerTypeReference Rewrite(IFunctionPointerTypeReference functionPointerTypeReference)
        {
            return functionPointerTypeReference;
        }


        /// <summary>
        /// Rewrites the given generic method instance reference.
        /// </summary>
        public virtual IGenericMethodInstanceReference Rewrite(
            IGenericMethodInstanceReference genericMethodInstanceReference)
        {
            return genericMethodInstanceReference;
        }


        /// <summary>
        /// Rewrites the given generic method parameter reference.
        /// </summary>
        public virtual IGenericMethodParameter Rewrite(IGenericMethodParameter genericMethodParameter)
        {
            return genericMethodParameter;
        }


        /// <summary>
        /// Rewrites the given generic method parameter reference.
        /// </summary>
        public virtual ITypeReference Rewrite(IGenericMethodParameterReference genericMethodParameterReference)
        {
            return genericMethodParameterReference;
        }


        /// <summary>
        /// Rewrites the given generic type instance reference.
        /// </summary>
        public virtual ITypeReference Rewrite(IGenericTypeInstanceReference genericTypeInstanceReference)
        {
            return genericTypeInstanceReference;
        }


        /// <summary>
        /// Rewrites the given generic type parameter reference.
        /// </summary>
        public virtual IGenericTypeParameter Rewrite(IGenericTypeParameter genericTypeParameter)
        {
            return genericTypeParameter;
        }


        /// <summary>
        /// Rewrites the given generic type parameter reference.
        /// </summary>
        public virtual ITypeReference Rewrite(IGenericTypeParameterReference genericTypeParameterReference)
        {
            return genericTypeParameterReference;
        }


        /// <summary>
        /// Rewrites the specified global field definition.
        /// </summary>
        public virtual IGlobalFieldDefinition Rewrite(IGlobalFieldDefinition globalFieldDefinition)
        {
            return globalFieldDefinition;
        }


        /// <summary>
        /// Rewrites the specified global method definition.
        /// </summary>
        public virtual IGlobalMethodDefinition Rewrite(IGlobalMethodDefinition globalMethodDefinition)
        {
            return globalMethodDefinition;
        }


        /// <summary>
        /// Rewrites the specified local definition.
        /// </summary>
        public virtual ILocalDefinition Rewrite(ILocalDefinition localDefinition)
        {
            return localDefinition;
        }


        /// <summary>
        /// Rewrites the given managed pointer type reference.
        /// </summary>
        public virtual IManagedPointerTypeReference Rewrite(IManagedPointerTypeReference managedPointerTypeReference)
        {
            return managedPointerTypeReference;
        }


        /// <summary>
        /// Rewrites the given marshalling information.
        /// </summary>
        public virtual IMarshallingInformation Rewrite(IMarshallingInformation marshallingInformation)
        {
            return marshallingInformation;
        }


        /// <summary>
        /// Rewrites the given metadata constant.
        /// </summary>
        public virtual IMetadataConstant Rewrite(IMetadataConstant constant)
        {
            return constant;
        }


        /// <summary>
        /// Rewrites the given metadata array creation expression.
        /// </summary>
        public virtual IMetadataCreateArray Rewrite(IMetadataCreateArray metadataCreateArray)
        {
            return metadataCreateArray;
        }


        /// <summary>
        /// Rewrites the given metadata expression.
        /// </summary>
        public virtual IMetadataExpression Rewrite(IMetadataExpression metadataExpression)
        {
            return metadataExpression;
        }


        /// <summary>
        /// Rewrites the given metadata named argument expression.
        /// </summary>
        public virtual IMetadataNamedArgument Rewrite(IMetadataNamedArgument namedArgument)
        {
            return namedArgument;
        }


        /// <summary>
        /// Rewrites the given metadata typeof expression.
        /// </summary>
        public virtual IMetadataTypeOf Rewrite(IMetadataTypeOf metadataTypeOf)
        {
            return metadataTypeOf;
        }


        /// <summary>
        /// Rewrites the given method definition.
        /// </summary>
        public virtual IMethodDefinition Rewrite(IMethodDefinition method)
        {
            return method;
        }


        /// <summary>
        /// Rewrites the given method implementation.
        /// </summary>
        public virtual IMethodImplementation Rewrite(IMethodImplementation methodImplementation)
        {
            return methodImplementation;
        }


        /// <summary>
        /// Rewrites the given method reference.
        /// </summary>
        public virtual IMethodReference Rewrite(IMethodReference methodReference)
        {
            return methodReference;
        }


        /// <summary>
        /// Rewrites the given method reference.
        /// </summary>
        public virtual IMethodReference RewriteUnspecialized(IMethodReference methodReference)
        {
            return methodReference;
        }


        /// <summary>
        /// Rewrites the given modified type reference.
        /// </summary>
        public virtual IModifiedTypeReference Rewrite(IModifiedTypeReference modifiedTypeReference)
        {
            return modifiedTypeReference;
        }


        /// <summary>
        /// Rewrites the given module.
        /// </summary>
        public virtual IModule Rewrite(IModule module)
        {
            return module;
        }


        /// <summary>
        /// Rewrites the given module reference.
        /// </summary>
        public virtual IModuleReference Rewrite(IModuleReference moduleReference)
        {
            return moduleReference;
        }


        /// <summary>
        /// Rewrites the named specified type reference.
        /// </summary>
        public virtual INamedTypeDefinition Rewrite(INamedTypeDefinition namedTypeDefinition)
        {
            return namedTypeDefinition;
        }


        /// <summary>
        /// Rewrites the named specified type reference.
        /// </summary>
        public virtual INamedTypeReference Rewrite(INamedTypeReference typeReference)
        {
            return typeReference;
        }


        /// <summary>
        /// Rewrites the namespace alias for type.
        /// </summary>
        public virtual INamespaceAliasForType Rewrite(INamespaceAliasForType namespaceAliasForType)
        {
            return namespaceAliasForType;
        }


        /// <summary>
        /// Rewrites the namespace definition.
        /// </summary>
        public virtual INamespaceDefinition Rewrite(INamespaceDefinition namespaceDefinition)
        {
            return namespaceDefinition;
        }


        /// <summary>
        /// Rewrites the specified namespace member.
        /// </summary>
        public virtual INamespaceMember Rewrite(INamespaceMember namespaceMember)
        {
            return namespaceMember;
        }


        /// <summary>
        /// Rewrites the given namespace type definition.
        /// </summary>
        public virtual INamespaceTypeDefinition Rewrite(INamespaceTypeDefinition namespaceTypeDefinition)
        {
            return namespaceTypeDefinition;
        }


        /// <summary>
        /// Rewrites the given namespace type reference.
        /// </summary>
        public virtual INamespaceTypeReference Rewrite(INamespaceTypeReference namespaceTypeReference)
        {
            return namespaceTypeReference;
        }


        /// <summary>
        /// Rewrites the nested alias for type
        /// </summary>
        public virtual INestedAliasForType Rewrite(INestedAliasForType nestedAliasForType)
        {
            return nestedAliasForType;
        }


        /// <summary>
        /// Rewrites the given nested type definition.
        /// </summary>
        public virtual INestedTypeDefinition Rewrite(INestedTypeDefinition namespaceTypeDefinition)
        {
            return namespaceTypeDefinition;
        }


        /// <summary>
        /// Rewrites the given namespace type reference.
        /// </summary>
        public virtual INestedTypeReference Rewrite(INestedTypeReference nestedTypeReference)
        {
            return nestedTypeReference;
        }


        /// <summary>
        /// Rewrites the given namespace type reference.
        /// </summary>
        public virtual INestedTypeReference RewriteUnspecialized(INestedTypeReference nestedTypeReference)
        {
            return nestedTypeReference;
        }


        /// <summary>
        /// Rewrites the specified nested unit namespace.
        /// </summary>
        public virtual INestedUnitNamespace Rewrite(INestedUnitNamespace nestedUnitNamespace)
        {
            return nestedUnitNamespace;
        }


        /// <summary>
        /// Rewrites the specified reference to a nested unit namespace.
        /// </summary>
        public virtual INestedUnitNamespaceReference Rewrite(INestedUnitNamespaceReference nestedUnitNamespaceReference)
        {
            return nestedUnitNamespaceReference;
        }


        /// <summary>
        /// Rewrites the specified operation.
        /// </summary>
        public virtual IOperation Rewrite(IOperation operation)
        {
            return operation;
        }


        /// <summary>
        /// Rewrites the specified operation exception information.
        /// </summary>
        public virtual IOperationExceptionInformation Rewrite(
            IOperationExceptionInformation operationExceptionInformation)
        {
            return operationExceptionInformation;
        }


        /// <summary>
        /// Rewrites the given parameter definition.
        /// </summary>
        public virtual IParameterDefinition Rewrite(IParameterDefinition parameterDefinition)
        {
            return parameterDefinition;
        }


        /// <summary>
        /// Rewrites the given parameter type information.
        /// </summary>
        public virtual IParameterTypeInformation Rewrite(IParameterTypeInformation parameterTypeInformation)
        {
            return parameterTypeInformation;
        }


        /// <summary>
        /// Rewrites the given PE section.
        /// </summary>
        public virtual IPESection Rewrite(IPESection peSection)
        {
            return peSection;
        }


        /// <summary>
        /// Rewrites the specified platform invoke information.
        /// </summary>
        public virtual IPlatformInvokeInformation Rewrite(IPlatformInvokeInformation platformInvokeInformation)
        {
            return platformInvokeInformation;
        }


        /// <summary>
        /// Rewrites the given pointer type reference.
        /// </summary>
        public virtual IPointerTypeReference Rewrite(IPointerTypeReference pointerTypeReference)
        {
            return pointerTypeReference;
        }


        /// <summary>
        /// Rewrites the given property definition.
        /// </summary>
        public virtual IPropertyDefinition Rewrite(IPropertyDefinition propertyDefinition)
        {
            return propertyDefinition;
        }


        /// <summary>
        /// Rewrites the given reference to a manifest resource.
        /// </summary>
        public virtual IResourceReference Rewrite(IResourceReference resourceReference)
        {
            return resourceReference;
        }


        /// <summary>
        /// Rewrites the given root unit namespace.
        /// </summary>
        public virtual IRootUnitNamespace Rewrite(IRootUnitNamespace rootUnitNamespace)
        {
            return rootUnitNamespace;
        }


        /// <summary>
        /// Rewrites the given reference to a root unit namespace.
        /// </summary>
        public virtual IRootUnitNamespaceReference Rewrite(IRootUnitNamespaceReference rootUnitNamespaceReference)
        {
            return rootUnitNamespaceReference;
        }


        /// <summary>
        /// Rewrites the given security attribute.
        /// </summary>
        public virtual ISecurityAttribute Rewrite(ISecurityAttribute securityAttribute)
        {
            return securityAttribute;
        }


        /// <summary>
        /// Rewrites the given specialized field reference.
        /// </summary>
        public virtual ISpecializedFieldReference Rewrite(ISpecializedFieldReference specializedFieldReference)
        {
            return specializedFieldReference;
        }


        /// <summary>
        /// Rewrites the given specialized method reference.
        /// </summary>
        public virtual IMethodReference Rewrite(ISpecializedMethodReference specializedMethodReference)
        {
            return specializedMethodReference;
        }


        /// <summary>
        /// Rewrites the given specialized nested type reference.
        /// </summary>
        public virtual INestedTypeReference Rewrite(ISpecializedNestedTypeReference specializedNestedTypeReference)
        {
            return specializedNestedTypeReference;
        }


        /// <summary>
        /// Rewrites the given type definition.
        /// </summary>
        public virtual ITypeDefinition Rewrite(ITypeDefinition typeDefinition)
        {
            return typeDefinition;
        }


        /// <summary>
        /// Rewrites the specified type member.
        /// </summary>
        public virtual ITypeDefinitionMember Rewrite(ITypeDefinitionMember typeMember)
        {
            return typeMember;
        }


        /// <summary>
        /// Rewrites the specified type reference.
        /// </summary>
        public virtual ITypeReference Rewrite(ITypeReference typeReference)
        {
            return typeReference;
        }


        /// <summary>
        /// Rewrites the specified unit.
        /// </summary>
        public virtual IUnit Rewrite(IUnit unit)
        {
            return unit;
        }


        /// <summary>
        /// Rewrites the specified unit namespace.
        /// </summary>
        public virtual IUnitNamespace Rewrite(IUnitNamespace unitNamespace)
        {
            return unitNamespace;
        }


        /// <summary>
        /// Rewrites the specified reference to a unit namespace.
        /// </summary>
        public virtual IUnitNamespaceReference Rewrite(IUnitNamespaceReference unitNamespaceReference)
        {
            return unitNamespaceReference;
        }


        /// <summary>
        /// Rewrites the specified unit reference.
        /// </summary>
        public virtual IUnitReference Rewrite(IUnitReference unitReference)
        {
            return unitReference;
        }


        /// <summary>
        /// Rewrites the given Win32 resource.
        /// </summary>
        public virtual IWin32Resource Rewrite(IWin32Resource win32Resource)
        {
            return win32Resource;
        }


        /// <summary>
        /// Rewrites the list of aliases for types.
        /// </summary>
        public virtual List<IAliasForType> /*?*/ Rewrite(List<IAliasForType> /*?*/ aliasesForTypes)
        {
            return aliasesForTypes;
        }


        /// <summary>
        /// Rewrites the list of members of a type alias.
        /// </summary>
        public virtual List<IAliasMember> /*?*/ Rewrite(List<IAliasMember> /*?*/ aliasMembers)
        {
            return aliasMembers;
        }


        /// <summary>
        /// Rewrites the specified assembly references.
        /// </summary>
        public virtual List<IAssemblyReference> /*?*/ Rewrite(List<IAssemblyReference> /*?*/ assemblyReferences)
        {
            return assemblyReferences;
        }


        /// <summary>
        /// Rewrites the specified custom attributes.
        /// </summary>
        public virtual List<ICustomAttribute> /*?*/ Rewrite(List<ICustomAttribute> /*?*/ customAttributes)
        {
            return customAttributes;
        }


        /// <summary>
        /// Rewrites the specified custom modifiers.
        /// </summary>
        public virtual List<ICustomModifier> /*?*/ Rewrite(List<ICustomModifier> /*?*/ customModifiers)
        {
            return customModifiers;
        }


        /// <summary>
        /// Rewrites the specified events.
        /// </summary>
        public virtual List<IEventDefinition> /*?*/ Rewrite(List<IEventDefinition> /*?*/ events)
        {
            return events;
        }


        /// <summary>
        /// Rewrites the specified fields.
        /// </summary>
        public virtual List<IFieldDefinition> /*?*/ Rewrite(List<IFieldDefinition> /*?*/ fields)
        {
            return fields;
        }


        /// <summary>
        /// Rewrites the specified file references.
        /// </summary>
        public virtual List<IFileReference> /*?*/ Rewrite(List<IFileReference> /*?*/ fileReferences)
        {
            return fileReferences;
        }


        /// <summary>
        /// Rewrites the specified generic parameters.
        /// </summary>
        public virtual List<IGenericMethodParameter> /*?*/ Rewrite(List<IGenericMethodParameter> /*?*/ genericParameters)
        {
            return genericParameters;
        }


        /// <summary>
        /// Rewrites the specified generic parameters.
        /// </summary>
        public virtual List<IGenericTypeParameter> /*?*/ Rewrite(List<IGenericTypeParameter> /*?*/ genericParameters)
        {
            return genericParameters;
        }


        /// <summary>
        /// Rewrites the specified local definitions.
        /// </summary>
        public virtual List<ILocalDefinition> /*?*/ Rewrite(List<ILocalDefinition> /*?*/ localDefinitions)
        {
            return localDefinitions;
        }


        /// <summary>
        /// Rewrites the specified expressions.
        /// </summary>
        public virtual List<IMetadataExpression> /*?*/ Rewrite(List<IMetadataExpression> /*?*/ expressions)
        {
            return expressions;
        }


        /// <summary>
        /// Rewrites the specified named arguments.
        /// </summary>
        public virtual List<IMetadataNamedArgument> /*?*/ Rewrite(List<IMetadataNamedArgument> /*?*/ namedArguments)
        {
            return namedArguments;
        }


        /// <summary>
        /// Rewrites the specified methods.
        /// </summary>
        public virtual List<IMethodDefinition> /*?*/ Rewrite(List<IMethodDefinition> /*?*/ methods)
        {
            return methods;
        }


        /// <summary>
        /// Rewrites the specified method implementations.
        /// </summary>
        public virtual List<IMethodImplementation> /*?*/ Rewrite(List<IMethodImplementation> /*?*/ methodImplementations)
        {
            return methodImplementations;
        }


        /// <summary>
        /// Rewrites the specified method references.
        /// </summary>
        public virtual List<IMethodReference> /*?*/ Rewrite(List<IMethodReference> /*?*/ methodReferences)
        {
            return methodReferences;
        }


        /// <summary>
        /// Rewrites the specified modules.
        /// </summary>
        public virtual List<IModule> /*?*/ Rewrite(List<IModule> /*?*/ modules)
        {
            return modules;
        }


        /// <summary>
        /// Rewrites the specified module references.
        /// </summary>
        public virtual List<IModuleReference> /*?*/ Rewrite(List<IModuleReference> /*?*/ moduleReferences)
        {
            return moduleReferences;
        }


        /// <summary>
        /// Rewrites the specified types.
        /// </summary>
        public virtual List<INamedTypeDefinition> /*?*/ Rewrite(List<INamedTypeDefinition> /*?*/ types)
        {
            return types;
        }


        /// <summary>
        /// Rewrites the specified namespace members.
        /// </summary>
        public virtual List<INamespaceMember> /*?*/ Rewrite(List<INamespaceMember> /*?*/ namespaceMembers)
        {
            return namespaceMembers;
        }


        /// <summary>
        /// Rewrites the specified nested types.
        /// </summary>
        public virtual List<INestedTypeDefinition> /*?*/ Rewrite(List<INestedTypeDefinition> /*?*/ nestedTypes)
        {
            return nestedTypes;
        }


        /// <summary>
        /// Rewrites the specified operations.
        /// </summary>
        public virtual List<IOperation> /*?*/ Rewrite(List<IOperation> /*?*/ operations)
        {
            return operations;
        }


        /// <summary>
        /// Rewrites the specified operation exception informations.
        /// </summary>
        public virtual List<IOperationExceptionInformation> /*?*/ Rewrite(
            List<IOperationExceptionInformation> /*?*/ operationExceptionInformations)
        {
            return operationExceptionInformations;
        }


        /// <summary>
        /// Rewrites the specified parameters.
        /// </summary>
        public virtual List<IParameterDefinition> /*?*/ Rewrite(List<IParameterDefinition> /*?*/ parameters)
        {
            return parameters;
        }


        /// <summary>
        /// Rewrites the specified parameter type informations.
        /// </summary>
        public virtual List<IParameterTypeInformation> /*?*/ Rewrite(
            List<IParameterTypeInformation> /*?*/ parameterTypeInformations)
        {
            return parameterTypeInformations;
        }


        /// <summary>
        /// Rewrites the specified PE sections.
        /// </summary>
        public virtual List<IPESection> /*?*/ Rewrite(List<IPESection> /*?*/ peSections)
        {
            return peSections;
        }


        /// <summary>
        /// Rewrites the specified properties.
        /// </summary>
        public virtual List<IPropertyDefinition> /*?*/ Rewrite(List<IPropertyDefinition> /*?*/ properties)
        {
            return properties;
        }


        /// <summary>
        /// Rewrites the specified resource references.
        /// </summary>
        public virtual List<IResourceReference> /*?*/ Rewrite(List<IResourceReference> /*?*/ resourceReferences)
        {
            return resourceReferences;
        }


        /// <summary>
        /// Rewrites the specified security attributes.
        /// </summary>
        public virtual List<ISecurityAttribute> /*?*/ Rewrite(List<ISecurityAttribute> /*?*/ securityAttributes)
        {
            return securityAttributes;
        }


        /// <summary>
        /// Rewrites the specified type members.
        /// </summary>
        /// <remarks>Not used by the rewriter itself.</remarks>
        public virtual List<ITypeDefinitionMember> /*?*/ Rewrite(List<ITypeDefinitionMember> /*?*/ typeMembers)
        {
            return typeMembers;
        }


        /// <summary>
        /// Rewrites the specified type references.
        /// </summary>
        public virtual List<ITypeReference> /*?*/ Rewrite(List<ITypeReference> /*?*/ typeReferences)
        {
            return typeReferences;
        }


        /// <summary>
        /// Rewrites the specified type references.
        /// </summary>
        public virtual List<IWin32Resource> /*?*/ Rewrite(List<IWin32Resource> /*?*/ win32Resources)
        {
            return win32Resources;
        }
    }
}