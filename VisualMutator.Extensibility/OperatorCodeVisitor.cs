namespace VisualMutator.Extensibility
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Microsoft.Cci;

    public class OperatorCodeVisitor : IOperatorCodeVisitor
    {

        public VisualCodeVisitor Parent
        {
            get;
            set;
        }
        public delegate void DelMarkMutationTarget(object obj, List<string> passesInfo  = null);
        public DelMarkMutationTarget MarkMutationTarget;
        public virtual void Initialize()
        {
            
        }
        /// <summary>
        /// Performs some computation with the given addition.
        /// </summary>
        /// <param name="addition"></param>
        public virtual void Visit(IAddition addition)
        {
            this.Visit((IBinaryOperation)addition);
        }

        /// <summary>
        /// Performs some computation with the given addressable expression.
        /// </summary>
        /// <param name="addressableExpression"></param>
        public virtual void Visit(IAddressableExpression addressableExpression)
        {
            this.Visit((IExpression)addressableExpression);
        }

        /// <summary>
        /// Performs some computation with the given address dereference expression.
        /// </summary>
        /// <param name="addressDereference"></param>
        public virtual void Visit(IAddressDereference addressDereference)
        {
            this.Visit((IExpression)addressDereference);
        }

        /// <summary>
        /// Performs some computation with the given AddressOf expression.
        /// </summary>
        /// <param name="addressOf"></param>
        public virtual void Visit(IAddressOf addressOf)
        {
            this.Visit((IExpression)addressOf);
        }

        /// <summary>
        /// Performs some computation with the given anonymous delegate expression.
        /// </summary>
        /// <param name="anonymousDelegate"></param>
        public virtual void Visit(IAnonymousDelegate anonymousDelegate)
        {
            this.Visit((IExpression)anonymousDelegate);
        }

        /// <summary>
        /// Performs some computation with the given array indexer expression.
        /// </summary>
        /// <param name="arrayIndexer"></param>
        public virtual void Visit(IArrayIndexer arrayIndexer)
        {
            this.Visit((IExpression)arrayIndexer);
        }

        /// <summary>
        /// Performs some computation with the given assert statement.
        /// </summary>
        /// <param name="assertStatement"></param>
        public virtual void Visit(IAssertStatement assertStatement)
        {
            this.Visit((IStatement)assertStatement);
        }

        /// <summary>
        /// Performs some computation with the given assignment expression.
        /// </summary>
        /// <param name="assignment"></param>
        public virtual void Visit(IAssignment assignment)
        {
            this.Visit((IExpression)assignment);
        }

        /// <summary>
        /// Performs some computation with the given assume statement.
        /// </summary>
        /// <param name="assumeStatement"></param>
        public virtual void Visit(IAssumeStatement assumeStatement)
        {
            this.Visit((IStatement)assumeStatement);
        }

        /// <summary>
        /// Performs some computation with the given bitwise and expression.
        /// </summary>
        /// <param name="bitwiseAnd"></param>
        public virtual void Visit(IBitwiseAnd bitwiseAnd)
        {
            this.Visit((IBinaryOperation)bitwiseAnd);
        }

        /// <summary>
        /// Performs some computation with the given bitwise and expression.
        /// </summary>
        /// <param name="binaryOperation"></param>
        public virtual void Visit(IBinaryOperation binaryOperation)
        {
            this.Visit((IExpression)binaryOperation);
        }

        /// <summary>
        /// Performs some computation with the given bitwise or expression.
        /// </summary>
        /// <param name="bitwiseOr"></param>
        public virtual void Visit(IBitwiseOr bitwiseOr)
        {
            this.Visit((IBinaryOperation)bitwiseOr);
        }

        /// <summary>
        /// Performs some computation with the given block expression.
        /// </summary>
        /// <param name="blockExpression"></param>
        public virtual void Visit(IBlockExpression blockExpression)
        {
            this.Visit((IExpression)blockExpression);
        }

        /// <summary>
        /// Performs some computation with the given statement block.
        /// </summary>
        /// <param name="block"></param>
        public virtual void Visit(IBlockStatement block)
        {
            this.Visit((IStatement)block);
        }

        /// <summary>
        /// Performs some computation with the given break statement.
        /// </summary>
        /// <param name="breakStatement"></param>
        public virtual void Visit(IBreakStatement breakStatement)
        {
            this.Visit((IStatement)breakStatement);
        }

        /// <summary>
        /// Performs some computation with the cast-if-possible expression.
        /// </summary>
        /// <param name="castIfPossible"></param>
        public virtual void Visit(ICastIfPossible castIfPossible)
        {
            this.Visit((IExpression)castIfPossible);
        }

        /// <summary>
        /// Performs some computation with the given catch clause.
        /// </summary>
        /// <param name="catchClause"></param>
        public virtual void Visit(ICatchClause catchClause)
        {
        }

        /// <summary>
        /// Performs some computation with the given check-if-instance expression.
        /// </summary>
        /// <param name="checkIfInstance"></param>
        public virtual void Visit(ICheckIfInstance checkIfInstance)
        {
            this.Visit((IExpression)checkIfInstance);
        }

        /// <summary>
        /// Performs some computation with the given compile time constant.
        /// </summary>
        /// <param name="constant"></param>
        public virtual void Visit(ICompileTimeConstant constant)
        {
            this.Visit((IExpression)constant);
        }

        /// <summary>
        /// Performs some computation with the given conversion expression.
        /// </summary>
        /// <param name="conversion"></param>
        public virtual void Visit(IConversion conversion)
        {
            this.Visit((IExpression)conversion);
        }

        /// <summary>
        /// Performs some computation with the given conditional expression.
        /// </summary>
        /// <param name="conditional"></param>
        public virtual void Visit(IConditional conditional)
        {
            this.Visit((IExpression)conditional);
        }

        /// <summary>
        /// Performs some computation with the given conditional statement.
        /// </summary>
        /// <param name="conditionalStatement"></param>
        public virtual void Visit(IConditionalStatement conditionalStatement)
        {
            this.Visit((IStatement)conditionalStatement);
        }

        /// <summary>
        /// Performs some computation with the given continue statement.
        /// </summary>
        /// <param name="continueStatement"></param>
        public virtual void Visit(IContinueStatement continueStatement)
        {
            this.Visit((IStatement)continueStatement);
        }

        /// <summary>
        /// Performs some computation with the given copy memory statement.
        /// </summary>
        /// <param name="copyMemoryStatement"></param>
        public virtual void Visit(ICopyMemoryStatement copyMemoryStatement)
        {
            this.Visit((IStatement)copyMemoryStatement);
        }

        /// <summary>
        /// Performs some computation with the given array creation expression.
        /// </summary>
        /// <param name="createArray"></param>
        public virtual void Visit(ICreateArray createArray)
        {
            this.Visit((IExpression)createArray);
        }

        /// <summary>
        /// Performs some computation with the given constructor call expression.
        /// </summary>
        /// <param name="createObjectInstance"></param>
        public virtual void Visit(ICreateObjectInstance createObjectInstance)
        {
            this.Visit((IExpression)createObjectInstance);
        }

        /// <summary>
        /// Performs some computation with the anonymous object creation expression.
        /// </summary>
        /// <param name="createDelegateInstance"></param>
        public virtual void Visit(ICreateDelegateInstance createDelegateInstance)
        {
            this.Visit((IExpression)createDelegateInstance);
        }

        /// <summary>
        /// Performs some computation with the given defalut value expression.
        /// </summary>
        /// <param name="defaultValue"></param>
        public virtual void Visit(IDefaultValue defaultValue)
        {
            this.Visit((IExpression)defaultValue);
        }

        /// <summary>
        /// Performs some computation with the given division expression.
        /// </summary>
        /// <param name="division"></param>
        public virtual void Visit(IDivision division)
        {
            this.Visit((IBinaryOperation)division);
        }

        /// <summary>
        /// Performs some computation with the given do until statement.
        /// </summary>
        /// <param name="doUntilStatement"></param>
        public virtual void Visit(IDoUntilStatement doUntilStatement)
        {
            this.Visit((IStatement)doUntilStatement);
        }

        /// <summary>
        /// Performs some computation with the given dup value expression.
        /// </summary>
        /// <param name="dupValue"></param>
        public virtual void Visit(IDupValue dupValue)
        {
            this.Visit((IExpression)dupValue);
        }

        /// <summary>
        /// Performs some computation with the given empty statement.
        /// </summary>
        /// <param name="emptyStatement"></param>
        public virtual void Visit(IEmptyStatement emptyStatement)
        {
            this.Visit((IStatement)emptyStatement);
        }

        /// <summary>
        /// Performs some computation with the given equality expression.
        /// </summary>
        /// <param name="equality"></param>
        public virtual void Visit(IEquality equality)
        {
            this.Visit((IBinaryOperation)equality);
        }

        /// <summary>
        /// Performs some computation with the given exclusive or expression.
        /// </summary>
        /// <param name="exclusiveOr"></param>
        public virtual void Visit(IExclusiveOr exclusiveOr)
        {
            this.Visit((IBinaryOperation)exclusiveOr);
        }

        /// <summary>
        /// Performs some computation with the given bound expression.
        /// </summary>
        /// <param name="boundExpression"></param>
        public virtual void Visit(IBoundExpression boundExpression)
        {
            this.Visit((IExpression)boundExpression);
        }

        /// <summary>
        /// Performs some computation with the given debugger break statement.
        /// </summary>
        /// <param name="debuggerBreakStatement"></param>
        public virtual void Visit(IDebuggerBreakStatement debuggerBreakStatement)
        {
            this.Visit((IStatement)debuggerBreakStatement);
        }

        /// <summary>
        /// Performs some computation with the given expression.
        /// </summary>
        /// <param name="expression"></param>
        public virtual void Visit(IExpression expression)
        {
        }

        /// <summary>
        /// Performs some computation with the given expression statement.
        /// </summary>
        /// <param name="expressionStatement"></param>
        public virtual void Visit(IExpressionStatement expressionStatement)
        {
            this.Visit((IStatement)expressionStatement);
        }

        /// <summary>
        /// Performs some computation with the given fill memory statement.
        /// </summary>
        /// <param name="fillMemoryStatement"></param>
        public virtual void Visit(IFillMemoryStatement fillMemoryStatement)
        {
            this.Visit((IStatement)fillMemoryStatement);
        }

        /// <summary>
        /// Performs some computation with the given foreach statement.
        /// </summary>
        /// <param name="forEachStatement"></param>
        public virtual void Visit(IForEachStatement forEachStatement)
        {
            this.Visit((IStatement)forEachStatement);
        }

        /// <summary>
        /// Performs some computation with the given for statement.
        /// </summary>
        /// <param name="forStatement"></param>
        public virtual void Visit(IForStatement forStatement)
        {
            this.Visit((IStatement)forStatement);
        }

        /// <summary>
        /// Performs some computation with the given get type of typed reference expression.
        /// </summary>
        /// <param name="getTypeOfTypedReference"></param>
        public virtual void Visit(IGetTypeOfTypedReference getTypeOfTypedReference)
        {
            this.Visit((IExpression)getTypeOfTypedReference);
        }

        /// <summary>
        /// Performs some computation with the given get value of typed reference expression.
        /// </summary>
        /// <param name="getValueOfTypedReference"></param>
        public virtual void Visit(IGetValueOfTypedReference getValueOfTypedReference)
        {
            this.Visit((IExpression)getValueOfTypedReference);
        }

        /// <summary>
        /// Performs some computation with the given goto statement.
        /// </summary>
        /// <param name="gotoStatement"></param>
        public virtual void Visit(IGotoStatement gotoStatement)
        {
            this.Visit((IStatement)gotoStatement);
        }

        /// <summary>
        /// Performs some computation with the given goto switch case statement.
        /// </summary>
        /// <param name="gotoSwitchCaseStatement"></param>
        public virtual void Visit(IGotoSwitchCaseStatement gotoSwitchCaseStatement)
        {
            this.Visit((IStatement)gotoSwitchCaseStatement);
        }

        /// <summary>
        /// Performs some computation with the given greater-than expression.
        /// </summary>
        /// <param name="greaterThan"></param>
        public virtual void Visit(IGreaterThan greaterThan)
        {
            this.Visit((IBinaryOperation)greaterThan);
        }

        /// <summary>
        /// Performs some computation with the given greater-than-or-equal expression.
        /// </summary>
        /// <param name="greaterThanOrEqual"></param>
        public virtual void Visit(IGreaterThanOrEqual greaterThanOrEqual)
        {
            this.Visit((IBinaryOperation)greaterThanOrEqual);
        }

        /// <summary>
        /// Performs some computation with the given labeled statement.
        /// </summary>
        /// <param name="labeledStatement"></param>
        public virtual void Visit(ILabeledStatement labeledStatement)
        {
            this.Visit((IStatement)labeledStatement);
        }

        /// <summary>
        /// Performs some computation with the given left shift expression.
        /// </summary>
        /// <param name="leftShift"></param>
        public virtual void Visit(ILeftShift leftShift)
        {
            this.Visit((IBinaryOperation)leftShift);
        }

        /// <summary>
        /// Performs some computation with the given less-than expression.
        /// </summary>
        /// <param name="lessThan"></param>
        public virtual void Visit(ILessThan lessThan)
        {
            this.Visit((IBinaryOperation)lessThan);
        }

        /// <summary>
        /// Performs some computation with the given less-than-or-equal expression.
        /// </summary>
        /// <param name="lessThanOrEqual"></param>
        public virtual void Visit(ILessThanOrEqual lessThanOrEqual)
        {
            this.Visit((IBinaryOperation)lessThanOrEqual);
        }

        /// <summary>
        /// Performs some computation with the given local declaration statement.
        /// </summary>
        /// <param name="localDeclarationStatement"></param>
        public virtual void Visit(ILocalDeclarationStatement localDeclarationStatement)
        {
            this.Visit((IStatement)localDeclarationStatement);
        }

        /// <summary>
        /// Performs some computation with the given lock statement.
        /// </summary>
        /// <param name="lockStatement"></param>
        public virtual void Visit(ILockStatement lockStatement)
        {
            this.Visit((IStatement)lockStatement);
        }

        /// <summary>
        /// Performs some computation with the given logical not expression.
        /// </summary>
        /// <param name="logicalNot"></param>
        public virtual void Visit(ILogicalNot logicalNot)
        {
            this.Visit((IUnaryOperation)logicalNot);
        }

        /// <summary>
        /// Performs some computation with the given make typed reference expression.
        /// </summary>
        /// <param name="makeTypedReference"></param>
        public virtual void Visit(IMakeTypedReference makeTypedReference)
        {
            this.Visit((IExpression)makeTypedReference);
        }

        /// <summary>
        /// Performs some computation with the given method call.
        /// </summary>
        /// <param name="methodCall"></param>
        public virtual void Visit(IMethodCall methodCall)
        {
            this.Visit((IExpression)methodCall);
        }

        /// <summary>
        /// Performs some computation with the given modulus expression.
        /// </summary>
        /// <param name="modulus"></param>
        public virtual void Visit(IModulus modulus)
        {
            this.Visit((IBinaryOperation)modulus);
        }

        /// <summary>
        /// Performs some computation with the given multiplication expression.
        /// </summary>
        /// <param name="multiplication"></param>
        public virtual void Visit(IMultiplication multiplication)
        {
            this.Visit((IBinaryOperation)multiplication);
        }

        /// <summary>
        /// Performs some computation with the given named argument expression.
        /// </summary>
        /// <param name="namedArgument"></param>
        public virtual void Visit(INamedArgument namedArgument)
        {
            this.Visit((IExpression)namedArgument);
        }

        /// <summary>
        /// Performs some computation with the given not equality expression.
        /// </summary>
        /// <param name="notEquality"></param>
        public virtual void Visit(INotEquality notEquality)
        {
            this.Visit((IBinaryOperation)notEquality);
        }

        /// <summary>
        /// Performs some computation with the given old value expression.
        /// </summary>
        /// <param name="oldValue"></param>
        public virtual void Visit(IOldValue oldValue)
        {
            this.Visit((IExpression)oldValue);
        }

        /// <summary>
        /// Performs some computation with the given one's complement expression.
        /// </summary>
        /// <param name="onesComplement"></param>
        public virtual void Visit(IOnesComplement onesComplement)
        {
            this.Visit((IUnaryOperation)onesComplement);
        }

        /// <summary>
        /// Performs some computation with the given out argument expression.
        /// </summary>
        /// <param name="outArgument"></param>
        public virtual void Visit(IOutArgument outArgument)
        {
            this.Visit((IExpression)outArgument);
        }

        /// <summary>
        /// Performs some computation with the given pointer call.
        /// </summary>
        /// <param name="pointerCall"></param>
        public virtual void Visit(IPointerCall pointerCall)
        {
            this.Visit((IExpression)pointerCall);
        }

        /// <summary>
        /// Performs some computation with the given pop value expression.
        /// </summary>
        /// <param name="popValue"></param>
        public virtual void Visit(IPopValue popValue)
        {
            this.Visit((IExpression)popValue);
        }

        /// <summary>
        /// Performs some computation with the given push statement.
        /// </summary>
        /// <param name="pushStatement"></param>
        public virtual void Visit(IPushStatement pushStatement)
        {
            this.Visit((IStatement)pushStatement);
        }

        /// <summary>
        /// Performs some computation with the given ref argument expression.
        /// </summary>
        /// <param name="refArgument"></param>
        public virtual void Visit(IRefArgument refArgument)
        {
            this.Visit((IExpression)refArgument);
        }

        /// <summary>
        /// Performs some computation with the given resource usage statement.
        /// </summary>
        /// <param name="resourceUseStatement"></param>
        public virtual void Visit(IResourceUseStatement resourceUseStatement)
        {
            this.Visit((IStatement)resourceUseStatement);
        }

        /// <summary>
        /// Performs some computation with the rethrow statement.
        /// </summary>
        /// <param name="rethrowStatement"></param>
        public virtual void Visit(IRethrowStatement rethrowStatement)
        {
            this.Visit((IStatement)rethrowStatement);
        }

        /// <summary>
        /// Performs some computation with the return statement.
        /// </summary>
        /// <param name="returnStatement"></param>
        public virtual void Visit(IReturnStatement returnStatement)
        {
            this.Visit((IStatement)returnStatement);
        }

        /// <summary>
        /// Performs some computation with the given return value expression.
        /// </summary>
        /// <param name="returnValue"></param>
        public virtual void Visit(IReturnValue returnValue)
        {
            this.Visit((IExpression)returnValue);
        }

        /// <summary>
        /// Performs some computation with the given right shift expression.
        /// </summary>
        /// <param name="rightShift"></param>
        public virtual void Visit(IRightShift rightShift)
        {
            this.Visit((IBinaryOperation)rightShift);
        }

        /// <summary>
        /// Performs some computation with the given stack array create expression.
        /// </summary>
        /// <param name="stackArrayCreate"></param>
        public virtual void Visit(IStackArrayCreate stackArrayCreate)
        {
            this.Visit((IExpression)stackArrayCreate);
        }

        /// <summary>
        /// Performs some computation with the given runtime argument handle expression.
        /// </summary>
        /// <param name="runtimeArgumentHandleExpression"></param>
        public virtual void Visit(IRuntimeArgumentHandleExpression runtimeArgumentHandleExpression)
        {
            this.Visit((IExpression)runtimeArgumentHandleExpression);
        }

        /// <summary>
        /// Performs some computation with the given sizeof() expression.
        /// </summary>
        /// <param name="sizeOf"></param>
        public virtual void Visit(ISizeOf sizeOf)
        {
            this.Visit((IExpression)sizeOf);
        }

        /// <summary>
        /// Visits the specified statement.
        /// </summary>
        /// <param name="statement">The statement.</param>
        public virtual void Visit(IStatement statement)
        {
        }

        /// <summary>
        /// Performs some computation with the given subtraction expression.
        /// </summary>
        /// <param name="subtraction"></param>
        public virtual void Visit(ISubtraction subtraction)
        {
            this.Visit((IBinaryOperation)subtraction);
        }

        /// <summary>
        /// Performs some computation with the given switch case.
        /// </summary>
        /// <param name="switchCase"></param>
        public virtual void Visit(ISwitchCase switchCase)
        {
        }

        /// <summary>
        /// Performs some computation with the given switch statement.
        /// </summary>
        /// <param name="switchStatement"></param>
        public virtual void Visit(ISwitchStatement switchStatement)
        {
            this.Visit((IStatement)switchStatement);
        }

        /// <summary>
        /// Performs some computation with the given target expression.
        /// </summary>
        /// <param name="targetExpression"></param>
        public virtual void Visit(ITargetExpression targetExpression)
        {
            this.Visit((IExpression)targetExpression);
        }

        /// <summary>
        /// Performs some computation with the given this reference expression.
        /// </summary>
        /// <param name="thisReference"></param>
        public virtual void Visit(IThisReference thisReference)
        {
            this.Visit((IExpression)thisReference);
        }

        /// <summary>
        /// Performs some computation with the throw statement.
        /// </summary>
        /// <param name="throwStatement"></param>
        public virtual void Visit(IThrowStatement throwStatement)
        {
            this.Visit((IStatement)throwStatement);
        }

        /// <summary>
        /// Performs some computation with the try-catch-filter-finally statement.
        /// </summary>
        /// <param name="tryCatchFilterFinallyStatement"></param>
        public virtual void Visit(ITryCatchFinallyStatement tryCatchFilterFinallyStatement)
        {
            this.Visit((IStatement)tryCatchFilterFinallyStatement);
        }

        /// <summary>
        /// Performs some computation with the given tokenof() expression.
        /// </summary>
        /// <param name="tokenOf"></param>
        public virtual void Visit(ITokenOf tokenOf)
        {
            this.Visit((IExpression)tokenOf);
        }

        /// <summary>
        /// Performs some computation with the given typeof() expression.
        /// </summary>
        /// <param name="typeOf"></param>
        public virtual void Visit(ITypeOf typeOf)
        {
            this.Visit((IExpression)typeOf);
        }

        /// <summary>
        /// Performs some computation with the given unary negation expression.
        /// </summary>
        /// <param name="unaryNegation"></param>
        public virtual void Visit(IUnaryNegation unaryNegation)
        {
            this.Visit((IUnaryOperation)unaryNegation);
        }

        /// <summary>
        /// Performs some computation with the given unary operation expression.
        /// </summary>
        /// <param name="unaryOperation"></param>
        public virtual void Visit(IUnaryOperation unaryOperation)
        {
            this.Visit((IExpression)unaryOperation);
        }

        /// <summary>
        /// Performs some computation with the given unary plus expression.
        /// </summary>
        /// <param name="unaryPlus"></param>
        public virtual void Visit(IUnaryPlus unaryPlus)
        {
            this.Visit((IUnaryOperation)unaryPlus);
        }

        /// <summary>
        /// Performs some computation with the given vector length expression.
        /// </summary>
        /// <param name="vectorLength"></param>
        public virtual void Visit(IVectorLength vectorLength)
        {
            this.Visit((IExpression)vectorLength);
        }

        /// <summary>
        /// Performs some computation with the given while do statement.
        /// </summary>
        /// <param name="whileDoStatement"></param>
        public virtual void Visit(IWhileDoStatement whileDoStatement)
        {
            this.Visit((IStatement)whileDoStatement);
        }

        /// <summary>
        /// Performs some computation with the given yield break statement.
        /// </summary>
        /// <param name="yieldBreakStatement"></param>
        public virtual void Visit(IYieldBreakStatement yieldBreakStatement)
        {
            this.Visit((IStatement)yieldBreakStatement);
        }

        /// <summary>
        /// Performs some computation with the given yield return statement.
        /// </summary>
        /// <param name="yieldReturnStatement"></param>
        public virtual void Visit(IYieldReturnStatement yieldReturnStatement)
        {
            this.Visit((IStatement)yieldReturnStatement);
        }

        /// <summary>
        /// Visits the specified alias for type.
        /// </summary>
        public virtual void Visit(IAliasForType aliasForType)
        {
        }

        /// <summary>
        /// Performs some computation with the given array type reference.
        /// </summary>
        public virtual void Visit(IArrayTypeReference arrayTypeReference)
        {
            this.Visit((ITypeReference)arrayTypeReference);
        }

        /// <summary>
        /// Performs some computation with the given assembly.
        /// </summary>
        public virtual void Visit(IAssembly assembly)
        {
            this.Visit((IModule)assembly);
        }

        /// <summary>
        /// Performs some computation with the given assembly reference.
        /// </summary>
        public virtual void Visit(IAssemblyReference assemblyReference)
        {
            this.Visit((IModuleReference)assemblyReference);
        }

        /// <summary>
        /// Performs some computation with the given custom attribute.
        /// </summary>
        public virtual void Visit(ICustomAttribute customAttribute)
        {
        }

        /// <summary>
        /// Performs some computation with the given custom modifier.
        /// </summary>
        public virtual void Visit(ICustomModifier customModifier)
        {
        }

        /// <summary>
        /// Performs some computation with the given event definition.
        /// </summary>
        public virtual void Visit(IEventDefinition eventDefinition)
        {
            this.Visit((ITypeDefinitionMember)eventDefinition);
        }

        /// <summary>
        /// Performs some computation with the given field definition.
        /// </summary>
        public virtual void Visit(IFieldDefinition fieldDefinition)
        {
            this.Visit((ITypeDefinitionMember)fieldDefinition);
        }

        /// <summary>
        /// Performs some computation with the given field reference.
        /// </summary>
        public virtual void Visit(IFieldReference fieldReference)
        {
            this.Visit((ITypeMemberReference)fieldReference);
        }

        /// <summary>
        /// Performs some computation with the given file reference.
        /// </summary>
        public virtual void Visit(IFileReference fileReference)
        {
        }

        /// <summary>
        /// Performs some computation with the given function pointer type reference.
        /// </summary>
        public virtual void Visit(IFunctionPointerTypeReference functionPointerTypeReference)
        {
            this.Visit((ITypeReference)functionPointerTypeReference);
        }

        /// <summary>
        /// Performs some computation with the given generic method instance reference.
        /// </summary>
        public virtual void Visit(IGenericMethodInstanceReference genericMethodInstanceReference)
        {
            this.Visit((IMethodReference)genericMethodInstanceReference);
        }

        /// <summary>
        /// Performs some computation with the given generic method parameter.
        /// </summary>
        public virtual void Visit(IGenericMethodParameter genericMethodParameter)
        {
            this.Visit((IGenericParameter)genericMethodParameter);
        }

        /// <summary>
        /// Performs some computation with the given generic method parameter reference.
        /// </summary>
        public virtual void Visit(IGenericMethodParameterReference genericMethodParameterReference)
        {
            this.Visit((IGenericParameterReference)genericMethodParameterReference);
        }

        /// <summary>
        /// Performs some computation with the given generic parameter.
        /// </summary>
        public virtual void Visit(IGenericParameter genericParameter)
        {
            this.Visit((INamedTypeDefinition)genericParameter);
        }

        /// <summary>
        /// Performs some computation with the given generic parameter.
        /// </summary>
        public virtual void Visit(IGenericParameterReference genericParameterReference)
        {
            this.Visit((ITypeReference)genericParameterReference);
        }

        /// <summary>
        /// Performs some computation with the given generic type instance reference.
        /// </summary>
        public virtual void Visit(IGenericTypeInstanceReference genericTypeInstanceReference)
        {
            this.Visit((ITypeReference)genericTypeInstanceReference);
        }

        /// <summary>
        /// Performs some computation with the given generic parameter.
        /// </summary>
        public virtual void Visit(IGenericTypeParameter genericTypeParameter)
        {
            this.Visit((IGenericParameter)genericTypeParameter);
        }

        /// <summary>
        /// Performs some computation with the given generic type parameter reference.
        /// </summary>
        public virtual void Visit(IGenericTypeParameterReference genericTypeParameterReference)
        {
            this.Visit((IGenericParameterReference)genericTypeParameterReference);
        }

        /// <summary>
        /// Performs some computation with the given global field definition.
        /// </summary>
        public virtual void Visit(IGlobalFieldDefinition globalFieldDefinition)
        {
            this.Visit((IFieldDefinition)globalFieldDefinition);
        }

        /// <summary>
        /// Performs some computation with the given global method definition.
        /// </summary>
        public virtual void Visit(IGlobalMethodDefinition globalMethodDefinition)
        {
            this.Visit((IMethodDefinition)globalMethodDefinition);
        }

        /// <summary>
        /// Performs some computation with the given local definition.
        /// </summary>
        public virtual void Visit(ILocalDefinition localDefinition)
        {
        }

        /// <summary>
        /// Performs some computation with the given local definition.
        /// </summary>
        public virtual void VisitReference(ILocalDefinition localDefinition)
        {
        }

        /// <summary>
        /// Performs some computation with the given managed pointer type reference.
        /// </summary>
        public virtual void Visit(IManagedPointerTypeReference managedPointerTypeReference)
        {
            this.Visit((ITypeReference)managedPointerTypeReference);
        }

        /// <summary>
        /// Performs some computation with the given marshalling information.
        /// </summary>
        public virtual void Visit(IMarshallingInformation marshallingInformation)
        {
        }

        /// <summary>
        /// Performs some computation with the given metadata constant.
        /// </summary>
        public virtual void Visit(IMetadataConstant constant)
        {
            this.Visit((IMetadataExpression)constant);
        }

        /// <summary>
        /// Performs some computation with the given metadata array creation expression.
        /// </summary>
        public virtual void Visit(IMetadataCreateArray createArray)
        {
            this.Visit((IMetadataExpression)createArray);
        }

        /// <summary>
        /// Performs some computation with the given metadata expression.
        /// </summary>
        public virtual void Visit(IMetadataExpression expression)
        {
        }

        /// <summary>
        /// Performs some computation with the given metadata named argument expression.
        /// </summary>
        public virtual void Visit(IMetadataNamedArgument namedArgument)
        {
            this.Visit((IMetadataExpression)namedArgument);
        }

        /// <summary>
        /// Performs some computation with the given metadata typeof expression.
        /// </summary>
        public virtual void Visit(IMetadataTypeOf typeOf)
        {
            this.Visit((IMetadataExpression)typeOf);
        }

        /// <summary>
        /// Performs some computation with the given method body.
        /// </summary>
        public virtual void Visit(IMethodBody methodBody)
        {
        }

        /// <summary>
        /// Performs some computation with the given method definition.
        /// </summary>
        public virtual void Visit(IMethodDefinition method)
        {
            this.Visit((ITypeDefinitionMember)method);
        }

        /// <summary>
        /// Performs some computation with the given method implementation.
        /// </summary>
        public virtual void Visit(IMethodImplementation methodImplementation)
        {
        }

        /// <summary>
        /// Performs some computation with the given method reference.
        /// </summary>
        public virtual void Visit(IMethodReference methodReference)
        {
            this.Visit((ITypeMemberReference)methodReference);
        }

        /// <summary>
        /// Performs some computation with the given modified type reference.
        /// </summary>
        public virtual void Visit(IModifiedTypeReference modifiedTypeReference)
        {
            this.Visit((ITypeReference)modifiedTypeReference);
        }

        /// <summary>
        /// Performs some computation with the given module.
        /// </summary>
        public virtual void Visit(IModule module)
        {
            this.Visit((IUnit)module);
        }

        /// <summary>
        /// Performs some computation with the given module reference.
        /// </summary>
        public virtual void Visit(IModuleReference moduleReference)
        {
            this.Visit((IUnitReference)moduleReference);
        }

        /// <summary>
        /// Performs some computation with the given named type definition.
        /// </summary>
        public virtual void Visit(INamedTypeDefinition namedTypeDefinition)
        {
            this.Visit((ITypeDefinition)namedTypeDefinition);
        }

        /// <summary>
        /// Performs some computation with the given named type reference.
        /// </summary>
        public virtual void Visit(INamedTypeReference namedTypeReference)
        {
            this.Visit((ITypeReference)namedTypeReference);
        }

        /// <summary>
        /// Performs some computation with the given alias for a namespace type definition.
        /// </summary>
        public virtual void Visit(INamespaceAliasForType namespaceAliasForType)
        {
            this.Visit((IAliasForType)namespaceAliasForType);
        }

        /// <summary>
        /// Visits the specified namespace definition.
        /// </summary>
        public virtual void Visit(INamespaceDefinition namespaceDefinition)
        {
        }

        /// <summary>
        /// Visits the specified namespace member.
        /// </summary>
        public virtual void Visit(INamespaceMember namespaceMember)
        {
        }

        /// <summary>
        /// Performs some computation with the given namespace type definition.
        /// </summary>
        public virtual void Visit(INamespaceTypeDefinition namespaceTypeDefinition)
        {
            this.Visit((INamedTypeDefinition)namespaceTypeDefinition);
        }

        /// <summary>
        /// Performs some computation with the given namespace type reference.
        /// </summary>
        public virtual void Visit(INamespaceTypeReference namespaceTypeReference)
        {
            this.Visit((INamedTypeReference)namespaceTypeReference);
        }

        /// <summary>
        /// Performs some computation with the given alias to a nested type definition.
        /// </summary>
        public virtual void Visit(INestedAliasForType nestedAliasForType)
        {
            this.Visit((IAliasForType)nestedAliasForType);
        }

        /// <summary>
        /// Performs some computation with the given nested type definition.
        /// </summary>
        public virtual void Visit(INestedTypeDefinition nestedTypeDefinition)
        {
            this.Visit((INamedTypeDefinition)nestedTypeDefinition);
        }

        /// <summary>
        /// Performs some computation with the given nested type reference.
        /// </summary>
        public virtual void Visit(INestedTypeReference nestedTypeReference)
        {
            this.Visit((INamedTypeReference)nestedTypeReference);
            this.Visit((ITypeMemberReference)nestedTypeReference);
        }

        /// <summary>
        /// Performs some computation with the given nested unit namespace.
        /// </summary>
        public virtual void Visit(INestedUnitNamespace nestedUnitNamespace)
        {
            this.Visit((IUnitNamespace)nestedUnitNamespace);
        }

        /// <summary>
        /// Performs some computation with the given nested unit namespace reference.
        /// </summary>
        public virtual void Visit(INestedUnitNamespaceReference nestedUnitNamespaceReference)
        {
            this.Visit((IUnitNamespaceReference)nestedUnitNamespaceReference);
        }

        /// <summary>
        /// Performs some computation with the given nested unit set namespace.
        /// </summary>
        public virtual void Visit(INestedUnitSetNamespace nestedUnitSetNamespace)
        {
            this.Visit((IUnitSetNamespace)nestedUnitSetNamespace);
        }

        /// <summary>
        /// Performs some computation with the given IL operation.
        /// </summary>
        public virtual void Visit(IOperation operation)
        {
        }

        /// <summary>
        /// Performs some computation with the given IL operation exception information instance.
        /// </summary>
        public virtual void Visit(IOperationExceptionInformation operationExceptionInformation)
        {
        }

        /// <summary>
        /// Performs some computation with the given parameter definition.
        /// </summary>
        public virtual void Visit(IParameterDefinition parameterDefinition)
        {
        }

        /// <summary>
        /// Performs some computation with the given parameter definition.
        /// </summary>
        public virtual void VisitReference(IParameterDefinition parameterDefinition)
        {
        }

        /// <summary>
        /// Performs some computation with the given property definition.
        /// </summary>
        public virtual void Visit(IPropertyDefinition propertyDefinition)
        {
            this.Visit((ITypeDefinitionMember)propertyDefinition);
        }

        /// <summary>
        /// Performs some computation with the given parameter type information.
        /// </summary>
        public virtual void Visit(IParameterTypeInformation parameterTypeInformation)
        {
        }

        /// <summary>
        /// Performs some compuation with the given PE section.
        /// </summary>
        public virtual void Visit(IPESection peSection)
        {
        }

        /// <summary>
        /// Performs some compuation with the given platoform invoke information.
        /// </summary>
        public virtual void Visit(IPlatformInvokeInformation platformInvokeInformation)
        {
        }

        /// <summary>
        /// Performs some computation with the given pointer type reference.
        /// </summary>
        public virtual void Visit(IPointerTypeReference pointerTypeReference)
        {
            this.Visit((ITypeReference)pointerTypeReference);
        }

        /// <summary>
        /// Performs some computation with the given reference to a manifest resource.
        /// </summary>
        public virtual void Visit(IResourceReference resourceReference)
        {
        }

        /// <summary>
        /// Performs some computation with the given root unit namespace.
        /// </summary>
        public virtual void Visit(IRootUnitNamespace rootUnitNamespace)
        {
            this.Visit((IUnitNamespace)rootUnitNamespace);
        }

        /// <summary>
        /// Performs some computation with the given root unit namespace reference.
        /// </summary>
        public virtual void Visit(IRootUnitNamespaceReference rootUnitNamespaceReference)
        {
            this.Visit((IUnitNamespaceReference)rootUnitNamespaceReference);
        }

        /// <summary>
        /// Performs some computation with the given root unit set namespace.
        /// </summary>
        public virtual void Visit(IRootUnitSetNamespace rootUnitSetNamespace)
        {
            this.Visit((IUnitSetNamespace)rootUnitSetNamespace);
        }

        /// <summary>
        /// Performs some computation with the given security attribute.
        /// </summary>
        public virtual void Visit(ISecurityAttribute securityAttribute)
        {
        }

        /// <summary>
        /// Performs some computation with the given specialized event definition.
        /// </summary>
        public virtual void Visit(ISpecializedEventDefinition specializedEventDefinition)
        {
            this.Visit((IEventDefinition)specializedEventDefinition);
        }

        /// <summary>
        /// Performs some computation with the given specialized field definition.
        /// </summary>
        public virtual void Visit(ISpecializedFieldDefinition specializedFieldDefinition)
        {
            this.Visit((IFieldDefinition)specializedFieldDefinition);
        }

        /// <summary>
        /// Performs some computation with the given specialized field reference.
        /// </summary>
        public virtual void Visit(ISpecializedFieldReference specializedFieldReference)
        {
            this.Visit((IFieldReference)specializedFieldReference);
        }

        /// <summary>
        /// Performs some computation with the given specialized method definition.
        /// </summary>
        public virtual void Visit(ISpecializedMethodDefinition specializedMethodDefinition)
        {
            this.Visit((IMethodDefinition)specializedMethodDefinition);
        }

        /// <summary>
        /// Performs some computation with the given specialized method reference.
        /// </summary>
        public virtual void Visit(ISpecializedMethodReference specializedMethodReference)
        {
            this.Visit((IMethodReference)specializedMethodReference);
        }

        /// <summary>
        /// Performs some computation with the given specialized propperty definition.
        /// </summary>
        public virtual void Visit(ISpecializedPropertyDefinition specializedPropertyDefinition)
        {
            this.Visit((IPropertyDefinition)specializedPropertyDefinition);
        }

        /// <summary>
        /// Performs some computation with the given specialized nested type definition.
        /// </summary>
        public virtual void Visit(ISpecializedNestedTypeDefinition specializedNestedTypeDefinition)
        {
            this.Visit((INestedTypeDefinition)specializedNestedTypeDefinition);
        }

        /// <summary>
        /// Performs some computation with the given specialized nested type reference.
        /// </summary>
        public virtual void Visit(ISpecializedNestedTypeReference specializedNestedTypeReference)
        {
            this.Visit((INestedTypeReference)specializedNestedTypeReference);
        }

        /// <summary>
        /// Visits the specified type definition.
        /// </summary>
        public virtual void Visit(ITypeDefinition typeDefinition)
        {
            Contract.Requires(typeDefinition != null);
        }

        /// <summary>
        /// Visits the specified type member.
        /// </summary>
        public virtual void Visit(ITypeDefinitionMember typeMember)
        {
            Contract.Requires(typeMember != null);
        }

        /// <summary>
        /// Visits the specified type member reference.
        /// </summary>
        public virtual void Visit(ITypeMemberReference typeMember)
        {
            Contract.Requires(typeMember != null);
        }

        /// <summary>
        /// Visits the specified type reference.
        /// </summary>
        public virtual void Visit(ITypeReference typeReference)
        {
            Contract.Requires(typeReference != null);
        }

        /// <summary>
        /// Visits the specified unit.
        /// </summary>
        public virtual void Visit(IUnit unit)
        {
            Contract.Requires(unit != null);
        }

        /// <summary>
        /// Visits the specified unit reference.
        /// </summary>
        public virtual void Visit(IUnitReference unitReference)
        {
            Contract.Requires(unitReference != null);
        }

        /// <summary>
        /// Visits the specified unit namespace.
        /// </summary>
        public virtual void Visit(IUnitNamespace unitNamespace)
        {
            Contract.Requires(unitNamespace != null);
            this.Visit((INamespaceDefinition)unitNamespace);
        }

        /// <summary>
        /// Visits the specified unit namespace reference.
        /// </summary>
        public virtual void Visit(IUnitNamespaceReference unitNamespaceReference)
        {
            Contract.Requires(unitNamespaceReference != null);
        }

        /// <summary>
        /// Performs some computation with the given unit set.
        /// </summary>
        public virtual void Visit(IUnitSet unitSet)
        {
        }

        /// <summary>
        /// Visits the specified unit set namespace.
        /// </summary>
        public virtual void Visit(IUnitSetNamespace unitSetNamespace)
        {
            Contract.Requires(unitSetNamespace != null);
        }

        /// <summary>
        /// Performs some computation with the given Win32 resource.
        /// </summary>
        public virtual void Visit(IWin32Resource win32Resource)
        {
        }

    }
}