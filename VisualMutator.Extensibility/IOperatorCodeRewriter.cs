namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;


    public interface IOperatorCodeRewriter
    {
        UserMutationTarget MutationTarget { get; set; }
        INameTable NameTable { get; set; }
        MetadataReaderHost Host { get; set; }
        IModule Module { get; set; }
        IOperatorUtils OperatorUtils { get; set; }

        void MethodEnter(MethodDefinition method);
        void MethodExit(MethodDefinition method);
        /// <summary>
        /// Rewrites the given addition.
        /// </summary>
        /// <param name="addition"></param>
        IExpression Rewrite(IAddition addition);

        /// <summary>
        /// Rewrites the given addressable expression.
        /// </summary>
        /// <param name="addressableExpression"></param>
        IAddressableExpression Rewrite(IAddressableExpression addressableExpression);

        /// <summary>
        /// Rewrites the given address dereference expression.
        /// </summary>
        /// <param name="addressDereference"></param>
        IExpression Rewrite(IAddressDereference addressDereference);

        /// <summary>
        /// Rewrites the given AddressOf expression.
        /// </summary>
        /// <param name="addressOf"></param>
        IExpression Rewrite(IAddressOf addressOf);

        /// <summary>
        /// Rewrites the given anonymous delegate expression.
        /// </summary>
        /// <param name="anonymousDelegate"></param>
        IExpression Rewrite(IAnonymousDelegate anonymousDelegate);

        /// <summary>
        /// Rewrites the given array indexer expression.
        /// </summary>
        /// <param name="arrayIndexer"></param>
        IExpression Rewrite(IArrayIndexer arrayIndexer);

        /// <summary>
        /// Rewrites the given assert statement.
        /// </summary>
        /// <param name="assertStatement"></param>
        IStatement Rewrite(IAssertStatement assertStatement);

        /// <summary>
        /// Rewrites the given assignment expression.
        /// </summary>
        /// <param name="assignment"></param>
        IExpression Rewrite(IAssignment assignment);

        /// <summary>
        /// Rewrites the given assume statement.
        /// </summary>
        /// <param name="assumeStatement"></param>
        IStatement Rewrite(IAssumeStatement assumeStatement);

        /// <summary>
        /// Rewrites the given bitwise and expression.
        /// </summary>
        /// <param name="binaryOperation"></param>
        IExpression Rewrite(IBinaryOperation binaryOperation);

        /// <summary>
        /// Rewrites the given bitwise and expression.
        /// </summary>
        /// <param name="bitwiseAnd"></param>
        IExpression Rewrite(IBitwiseAnd bitwiseAnd);

        /// <summary>
        /// Rewrites the given bitwise or expression.
        /// </summary>
        /// <param name="bitwiseOr"></param>
        IExpression Rewrite(IBitwiseOr bitwiseOr);

        /// <summary>
        /// Rewrites the given block expression.
        /// </summary>
        /// <param name="blockExpression"></param>
        IExpression Rewrite(IBlockExpression blockExpression);

        /// <summary>
        /// Rewrites the given statement block.
        /// </summary>
        /// <param name="block"></param>
        IBlockStatement Rewrite(IBlockStatement block);

        /// <summary>
        /// Rewrites the given bound expression.
        /// </summary>
        /// <param name="boundExpression"></param>
        IExpression Rewrite(IBoundExpression boundExpression);

        /// <summary>
        /// Rewrites the given break statement.
        /// </summary>
        /// <param name="breakStatement"></param>
        IStatement Rewrite(IBreakStatement breakStatement);

        /// <summary>
        /// Rewrites the cast-if-possible expression.
        /// </summary>
        /// <param name="castIfPossible"></param>
        IExpression Rewrite(ICastIfPossible castIfPossible);

        /// <summary>
        /// Rewrites the given catch clause.
        /// </summary>
        /// <param name="catchClause"></param>
        ICatchClause Rewrite(ICatchClause catchClause);

        /// <summary>
        /// Rewrites the given check-if-instance expression.
        /// </summary>
        /// <param name="checkIfInstance"></param>
        IExpression Rewrite(ICheckIfInstance checkIfInstance);

        /// <summary>
        /// Rewrites the given compile time constant.
        /// </summary>
        /// <param name="constant"></param>
        ICompileTimeConstant Rewrite(ICompileTimeConstant constant);

        /// <summary>
        /// Rewrites the given conditional expression.
        /// </summary>
        /// <param name="conditional"></param>
        IExpression Rewrite(IConditional conditional);

        /// <summary>
        /// Rewrites the given conditional statement.
        /// </summary>
        /// <param name="conditionalStatement"></param>
        IStatement Rewrite(IConditionalStatement conditionalStatement);

        /// <summary>
        /// Rewrites the given continue statement.
        /// </summary>
        /// <param name="continueStatement"></param>
        IStatement Rewrite(IContinueStatement continueStatement);

        /// <summary>
        /// Rewrites the given conversion expression.
        /// </summary>
        /// <param name="conversion"></param>
        IExpression Rewrite(IConversion conversion);

        /// <summary>
        /// Rewrites the given copy memory statement.
        /// </summary>
        /// <param name="copyMemoryStatement"></param>
        IStatement Rewrite(ICopyMemoryStatement copyMemoryStatement);

        /// <summary>
        /// Rewrites the given array creation expression.
        /// </summary>
        /// <param name="createArray"></param>
        IExpression Rewrite(ICreateArray createArray);

        /// <summary>
        /// Rewrites the anonymous object creation expression.
        /// </summary>
        /// <param name="createDelegateInstance"></param>
        IExpression Rewrite(ICreateDelegateInstance createDelegateInstance);

        /// <summary>
        /// Rewrites the given constructor call expression.
        /// </summary>
        /// <param name="createObjectInstance"></param>
        IExpression Rewrite(ICreateObjectInstance createObjectInstance);

        /// <summary>
        /// Rewrites the given debugger break statement.
        /// </summary>
        /// <param name="debuggerBreakStatement"></param>
        IStatement Rewrite(IDebuggerBreakStatement debuggerBreakStatement);

        /// <summary>
        /// Rewrites the given defalut value expression.
        /// </summary>
        /// <param name="defaultValue"></param>
        IExpression Rewrite(IDefaultValue defaultValue);

        /// <summary>
        /// Rewrites the given division expression.
        /// </summary>
        /// <param name="division"></param>
        IExpression Rewrite(IDivision division);

        /// <summary>
        /// Rewrites the given do until statement.
        /// </summary>
        /// <param name="doUntilStatement"></param>
        IStatement Rewrite(IDoUntilStatement doUntilStatement);

        /// <summary>
        /// Rewrites the given dup value expression.
        /// </summary>
        /// <param name="dupValue"></param>
        IExpression Rewrite(IDupValue dupValue);

        /// <summary>
        /// Rewrites the given empty statement.
        /// </summary>
        /// <param name="emptyStatement"></param>
        IStatement Rewrite(IEmptyStatement emptyStatement);

        /// <summary>
        /// Rewrites the given equality expression.
        /// </summary>
        /// <param name="equality"></param>
        IExpression Rewrite(IEquality equality);

        /// <summary>
        /// Rewrites the given exclusive or expression.
        /// </summary>
        /// <param name="exclusiveOr"></param>
        IExpression Rewrite(IExclusiveOr exclusiveOr);

        /// <summary>
        /// Rewrites the given expression.
        /// </summary>
        /// <param name="expression"></param>
        IExpression Rewrite(IExpression expression);

        /// <summary>
        /// Rewrites the given expression statement.
        /// </summary>
        /// <param name="expressionStatement"></param>
        IStatement Rewrite(IExpressionStatement expressionStatement);

        /// <summary>
        /// Rewrites the given fill memory statement.
        /// </summary>
        /// <param name="fillMemoryStatement"></param>
        IStatement Rewrite(IFillMemoryStatement fillMemoryStatement);

        /// <summary>
        /// Rewrites the given foreach statement.
        /// </summary>
        /// <param name="forEachStatement"></param>
        IStatement Rewrite(IForEachStatement forEachStatement);

        /// <summary>
        /// Rewrites the given for statement.
        /// </summary>
        /// <param name="forStatement"></param>
        IStatement Rewrite(IForStatement forStatement);

        /// <summary>
        /// Rewrites the given get type of typed reference expression.
        /// </summary>
        /// <param name="getTypeOfTypedReference"></param>
        IExpression Rewrite(IGetTypeOfTypedReference getTypeOfTypedReference);

        /// <summary>
        /// Rewrites the given get value of typed reference expression.
        /// </summary>
        /// <param name="getValueOfTypedReference"></param>
        IExpression Rewrite(IGetValueOfTypedReference getValueOfTypedReference);

        /// <summary>
        /// Rewrites the given goto statement.
        /// </summary>
        /// <param name="gotoStatement"></param>
        IStatement Rewrite(IGotoStatement gotoStatement);

        /// <summary>
        /// Rewrites the given goto switch case statement.
        /// </summary>
        /// <param name="gotoSwitchCaseStatement"></param>
        IStatement Rewrite(IGotoSwitchCaseStatement gotoSwitchCaseStatement);

        /// <summary>
        /// Rewrites the given greater-than expression.
        /// </summary>
        /// <param name="greaterThan"></param>
        IExpression Rewrite(IGreaterThan greaterThan);

        /// <summary>
        /// Rewrites the given greater-than-or-equal expression.
        /// </summary>
        /// <param name="greaterThanOrEqual"></param>
        IExpression Rewrite(IGreaterThanOrEqual greaterThanOrEqual);

        /// <summary>
        /// Rewrites the given labeled statement.
        /// </summary>
        /// <param name="labeledStatement"></param>
        IStatement Rewrite(ILabeledStatement labeledStatement);

        /// <summary>
        /// Rewrites the given left shift expression.
        /// </summary>
        /// <param name="leftShift"></param>
        IExpression Rewrite(ILeftShift leftShift);

        /// <summary>
        /// Rewrites the given less-than expression.
        /// </summary>
        /// <param name="lessThan"></param>
        IExpression Rewrite(ILessThan lessThan);

        /// <summary>
        /// Rewrites the given less-than-or-equal expression.
        /// </summary>
        /// <param name="lessThanOrEqual"></param>
        IExpression Rewrite(ILessThanOrEqual lessThanOrEqual);

        /// <summary>
        /// Rewrites the given local declaration statement.
        /// </summary>
        /// <param name="localDeclarationStatement"></param>
        IStatement Rewrite(ILocalDeclarationStatement localDeclarationStatement);

        /// <summary>
        /// Rewrites the given lock statement.
        /// </summary>
        /// <param name="lockStatement"></param>
        IStatement Rewrite(ILockStatement lockStatement);

        /// <summary>
        /// Rewrites the given logical not expression.
        /// </summary>
        /// <param name="logicalNot"></param>
        IExpression Rewrite(ILogicalNot logicalNot);

        /// <summary>
        /// Rewrites the given make typed reference expression.
        /// </summary>
        /// <param name="makeTypedReference"></param>
        IExpression Rewrite(IMakeTypedReference makeTypedReference);

        /// <summary>
        /// Rewrites the the given method body.
        /// </summary>
        /// <param name="methodBody"></param>
        IMethodBody Rewrite(IMethodBody methodBody);

        /// <summary>
        /// Rewrites the given method call.
        /// </summary>
        /// <param name="methodCall"></param>
        IExpression Rewrite(IMethodCall methodCall);

        /// <summary>
        /// Rewrites the given modulus expression.
        /// </summary>
        /// <param name="modulus"></param>
        IExpression Rewrite(IModulus modulus);

        /// <summary>
        /// Rewrites the given multiplication expression.
        /// </summary>
        /// <param name="multiplication"></param>
        IExpression Rewrite(IMultiplication multiplication);

        /// <summary>
        /// Rewrites the given named argument expression.
        /// </summary>
        /// <param name="namedArgument"></param>
        IExpression Rewrite(INamedArgument namedArgument);

        /// <summary>
        /// Rewrites the given not equality expression.
        /// </summary>
        /// <param name="notEquality"></param>
        IExpression Rewrite(INotEquality notEquality);

        /// <summary>
        /// Rewrites the given old value expression.
        /// </summary>
        /// <param name="oldValue"></param>
        IExpression Rewrite(IOldValue oldValue);

        /// <summary>
        /// Rewrites the given one's complement expression.
        /// </summary>
        /// <param name="onesComplement"></param>
        IExpression Rewrite(IOnesComplement onesComplement);

        /// <summary>
        /// Rewrites the given out argument expression.
        /// </summary>
        /// <param name="outArgument"></param>
        IExpression Rewrite(IOutArgument outArgument);

        /// <summary>
        /// Rewrites the given pointer call.
        /// </summary>
        /// <param name="pointerCall"></param>
        IExpression Rewrite(IPointerCall pointerCall);

        /// <summary>
        /// Rewrites the given pop value expression.
        /// </summary>
        /// <param name="popValue"></param>
        IExpression Rewrite(IPopValue popValue);

        /// <summary>
        /// Rewrites the given push statement.
        /// </summary>
        /// <param name="pushStatement"></param>
        IStatement Rewrite(IPushStatement pushStatement);

        /// <summary>
        /// Rewrites the given ref argument expression.
        /// </summary>
        /// <param name="refArgument"></param>
        IExpression Rewrite(IRefArgument refArgument);

        /// <summary>
        /// Rewrites the given resource usage statement.
        /// </summary>
        /// <param name="resourceUseStatement"></param>
        IStatement Rewrite(IResourceUseStatement resourceUseStatement);

        /// <summary>
        /// Rewrites the rethrow statement.
        /// </summary>
        /// <param name="rethrowStatement"></param>
        IStatement Rewrite(IRethrowStatement rethrowStatement);

        /// <summary>
        /// Rewrites the return statement.
        /// </summary>
        /// <param name="returnStatement"></param>
        IStatement Rewrite(IReturnStatement returnStatement);

        /// <summary>
        /// Rewrites the given return value expression.
        /// </summary>
        /// <param name="returnValue"></param>
        IExpression Rewrite(IReturnValue returnValue);

        /// <summary>
        /// Rewrites the given right shift expression.
        /// </summary>
        /// <param name="rightShift"></param>
        IExpression Rewrite(IRightShift rightShift);

        /// <summary>
        /// Rewrites the given runtime argument handle expression.
        /// </summary>
        /// <param name="runtimeArgumentHandleExpression"></param>
        IExpression Rewrite(IRuntimeArgumentHandleExpression runtimeArgumentHandleExpression);

        /// <summary>
        /// Rewrites the given sizeof(); expression.
        /// </summary>
        /// <param name="sizeOf"></param>
        IExpression Rewrite(ISizeOf sizeOf);

        /// <summary>
        /// Rewrites the given stack array create expression.
        /// </summary>
        /// <param name="stackArrayCreate"></param>
        IExpression Rewrite(IStackArrayCreate stackArrayCreate);

        /// <summary>
        /// Rewrites the the given source method body.
        /// </summary>
        /// <param name="sourceMethodBody"></param>
        ISourceMethodBody Rewrite(ISourceMethodBody sourceMethodBody);

        /// <summary>
        /// Rewrites the specified statement.
        /// </summary>
        /// <param name="statement">The statement.</param>
        IStatement Rewrite(IStatement statement);

        /// <summary>
        /// Rewrites the given subtraction expression.
        /// </summary>
        /// <param name="subtraction"></param>
        IExpression Rewrite(ISubtraction subtraction);

        /// <summary>
        /// Rewrites the given switch case.
        /// </summary>
        /// <param name="switchCase"></param>
        ISwitchCase Rewrite(ISwitchCase switchCase);

        /// <summary>
        /// Rewrites the given switch statement.
        /// </summary>
        /// <param name="switchStatement"></param>
        IStatement Rewrite(ISwitchStatement switchStatement);

        /// <summary>
        /// Rewrites the given target expression.
        /// </summary>
        /// <param name="targetExpression"></param>
        ITargetExpression Rewrite(ITargetExpression targetExpression);

        /// <summary>
        /// Rewrites the given this reference expression.
        /// </summary>
        /// <param name="thisReference"></param>
        IExpression Rewrite(IThisReference thisReference);

        /// <summary>
        /// Rewrites the throw statement.
        /// </summary>
        /// <param name="throwStatement"></param>
        IStatement Rewrite(IThrowStatement throwStatement);

        /// <summary>
        /// Rewrites the given tokenof(); expression.
        /// </summary>
        /// <param name="tokenOf"></param>
        IExpression Rewrite(ITokenOf tokenOf);

        /// <summary>
        /// Rewrites the try-catch-filter-finally statement.
        /// </summary>
        /// <param name="tryCatchFilterFinallyStatement"></param>
        IStatement Rewrite(ITryCatchFinallyStatement tryCatchFilterFinallyStatement);

        /// <summary>
        /// Rewrites the given typeof(); expression.
        /// </summary>
        /// <param name="typeOf"></param>
        IExpression Rewrite(ITypeOf typeOf);

        /// <summary>
        /// Rewrites the given unary negation expression.
        /// </summary>
        /// <param name="unaryNegation"></param>
        IExpression Rewrite(IUnaryNegation unaryNegation);

        /// <summary>
        /// Rewrites the given unary plus expression.
        /// </summary>
        /// <param name="unaryPlus"></param>
        IExpression Rewrite(IUnaryPlus unaryPlus);

        /// <summary>
        /// Rewrites the given vector length expression.
        /// </summary>
        /// <param name="vectorLength"></param>
        IExpression Rewrite(IVectorLength vectorLength);

        /// <summary>
        /// Rewrites the given while do statement.
        /// </summary>
        /// <param name="whileDoStatement"></param>
        IStatement Rewrite(IWhileDoStatement whileDoStatement);

        /// <summary>
        /// Rewrites the given yield break statement.
        /// </summary>
        /// <param name="yieldBreakStatement"></param>
        IStatement Rewrite(IYieldBreakStatement yieldBreakStatement);

        /// <summary>
        /// Rewrites the given yield return statement.
        /// </summary>
        /// <param name="yieldReturnStatement"></param>
        IStatement Rewrite(IYieldReturnStatement yieldReturnStatement);

        /// <summary>
        /// Rewrites the given list of catch clauses.
        /// </summary>
        /// <param name="catchClauses"></param>
        List<ICatchClause> Rewrite(List<ICatchClause> catchClauses);

        /// <summary>
        /// Rewrites the given list of expressions.
        /// </summary>
        /// <param name="expressions"></param>
        List<IExpression> Rewrite(List<IExpression> expressions);

        /// <summary>
        /// Rewrites the given list of switch cases.
        /// </summary>
        /// <param name="switchCases"></param>
        List<ISwitchCase> Rewrite(List<ISwitchCase> switchCases);

        /// <summary>
        /// Rewrites the given list of statements.
        /// </summary>
        /// <param name="statements"></param>
        List<IStatement> Rewrite(List<IStatement> statements);

        /// <summary>
        /// Rewrites the alias for type
        /// </summary>
        IAliasForType Rewrite(IAliasForType aliasForType);

        /// <summary>
        /// Rewrites the alias for type member.
        /// </summary>
        IAliasMember Rewrite(IAliasMember aliasMember);

        /// <summary>
        /// Rewrites the array type reference.
        /// </summary>
        IArrayTypeReference Rewrite(IArrayTypeReference arrayTypeReference);

        /// <summary>
        /// Rewrites the given assembly.
        /// </summary>
        IAssembly Rewrite(IAssembly assembly);

        /// <summary>
        /// Rewrites the given assembly reference.
        /// </summary>
        IAssemblyReference Rewrite(IAssemblyReference assemblyReference);

        /// <summary>
        /// Rewrites the given custom attribute.
        /// </summary>
        ICustomAttribute Rewrite(ICustomAttribute customAttribute);

        /// <summary>
        /// Rewrites the given custom modifier.
        /// </summary>
        ICustomModifier Rewrite(ICustomModifier customModifier);

        /// <summary>
        /// Rewrites the given event definition.
        /// </summary>
        IEventDefinition Rewrite(IEventDefinition eventDefinition);

        /// <summary>
        /// Rewrites the given field definition.
        /// </summary>
        IFieldDefinition Rewrite(IFieldDefinition fieldDefinition);

        /// <summary>
        /// Rewrites the given field reference.
        /// </summary>
        IFieldReference Rewrite(IFieldReference fieldReference);

        /// <summary>
        /// Rewrites the reference to a local definition.
        /// </summary>
        object RewriteReference(ILocalDefinition localDefinition);

        /// <summary>
        /// Rewrites the reference to a parameter.
        /// </summary>
        object RewriteReference(IParameterDefinition parameterDefinition);

        /// <summary>
        /// Rewrites the given field reference.
        /// </summary>
        IFieldReference RewriteUnspecialized(IFieldReference fieldReference);

        /// <summary>
        /// Rewrites the given file reference.
        /// </summary>
        IFileReference Rewrite(IFileReference fileReference);

        /// <summary>
        /// Rewrites the given function pointer type reference.
        /// </summary>
        IFunctionPointerTypeReference Rewrite(IFunctionPointerTypeReference functionPointerTypeReference);

        /// <summary>
        /// Rewrites the given generic method instance reference.
        /// </summary>
        IGenericMethodInstanceReference Rewrite(
            IGenericMethodInstanceReference genericMethodInstanceReference);

        /// <summary>
        /// Rewrites the given generic method parameter reference.
        /// </summary>
        IGenericMethodParameter Rewrite(IGenericMethodParameter genericMethodParameter);

        /// <summary>
        /// Rewrites the given generic method parameter reference.
        /// </summary>
        ITypeReference Rewrite(IGenericMethodParameterReference genericMethodParameterReference);

        /// <summary>
        /// Rewrites the given generic type instance reference.
        /// </summary>
        ITypeReference Rewrite(IGenericTypeInstanceReference genericTypeInstanceReference);

        /// <summary>
        /// Rewrites the given generic type parameter reference.
        /// </summary>
        IGenericTypeParameter Rewrite(IGenericTypeParameter genericTypeParameter);

        /// <summary>
        /// Rewrites the given generic type parameter reference.
        /// </summary>
        ITypeReference Rewrite(IGenericTypeParameterReference genericTypeParameterReference);

        /// <summary>
        /// Rewrites the specified global field definition.
        /// </summary>
        IGlobalFieldDefinition Rewrite(IGlobalFieldDefinition globalFieldDefinition);

        /// <summary>
        /// Rewrites the specified global method definition.
        /// </summary>
        IGlobalMethodDefinition Rewrite(IGlobalMethodDefinition globalMethodDefinition);

        /// <summary>
        /// Rewrites the specified local definition.
        /// </summary>
        ILocalDefinition Rewrite(ILocalDefinition localDefinition);

        /// <summary>
        /// Rewrites the given managed pointer type reference.
        /// </summary>
        IManagedPointerTypeReference Rewrite(IManagedPointerTypeReference managedPointerTypeReference);

        /// <summary>
        /// Rewrites the given marshalling information.
        /// </summary>
        IMarshallingInformation Rewrite(IMarshallingInformation marshallingInformation);

        /// <summary>
        /// Rewrites the given metadata constant.
        /// </summary>
        IMetadataConstant Rewrite(IMetadataConstant constant);

        /// <summary>
        /// Rewrites the given metadata array creation expression.
        /// </summary>
        IMetadataCreateArray Rewrite(IMetadataCreateArray metadataCreateArray);

        /// <summary>
        /// Rewrites the given metadata expression.
        /// </summary>
        IMetadataExpression Rewrite(IMetadataExpression metadataExpression);

        /// <summary>
        /// Rewrites the given metadata named argument expression.
        /// </summary>
        IMetadataNamedArgument Rewrite(IMetadataNamedArgument namedArgument);

        /// <summary>
        /// Rewrites the given metadata typeof expression.
        /// </summary>
        IMetadataTypeOf Rewrite(IMetadataTypeOf metadataTypeOf);

        /// <summary>
        /// Rewrites the given method definition.
        /// </summary>
        IMethodDefinition Rewrite(IMethodDefinition method);

        /// <summary>
        /// Rewrites the given method implementation.
        /// </summary>
        IMethodImplementation Rewrite(IMethodImplementation methodImplementation);

        /// <summary>
        /// Rewrites the given method reference.
        /// </summary>
        IMethodReference Rewrite(IMethodReference methodReference);

        /// <summary>
        /// Rewrites the given method reference.
        /// </summary>
        IMethodReference RewriteUnspecialized(IMethodReference methodReference);

        /// <summary>
        /// Rewrites the given modified type reference.
        /// </summary>
        IModifiedTypeReference Rewrite(IModifiedTypeReference modifiedTypeReference);

        /// <summary>
        /// Rewrites the given module.
        /// </summary>
        IModule Rewrite(IModule module);

        /// <summary>
        /// Rewrites the given module reference.
        /// </summary>
        IModuleReference Rewrite(IModuleReference moduleReference);

        /// <summary>
        /// Rewrites the named specified type reference.
        /// </summary>
        INamedTypeDefinition Rewrite(INamedTypeDefinition namedTypeDefinition);

        /// <summary>
        /// Rewrites the named specified type reference.
        /// </summary>
        INamedTypeReference Rewrite(INamedTypeReference typeReference);

        /// <summary>
        /// Rewrites the namespace alias for type.
        /// </summary>
        INamespaceAliasForType Rewrite(INamespaceAliasForType namespaceAliasForType);

        /// <summary>
        /// Rewrites the namespace definition.
        /// </summary>
        INamespaceDefinition Rewrite(INamespaceDefinition namespaceDefinition);

        /// <summary>
        /// Rewrites the specified namespace member.
        /// </summary>
        INamespaceMember Rewrite(INamespaceMember namespaceMember);

        /// <summary>
        /// Rewrites the given namespace type definition.
        /// </summary>
        INamespaceTypeDefinition Rewrite(INamespaceTypeDefinition namespaceTypeDefinition);

        /// <summary>
        /// Rewrites the given namespace type reference.
        /// </summary>
        INamespaceTypeReference Rewrite(INamespaceTypeReference namespaceTypeReference);

        /// <summary>
        /// Rewrites the nested alias for type
        /// </summary>
        INestedAliasForType Rewrite(INestedAliasForType nestedAliasForType);

        /// <summary>
        /// Rewrites the given nested type definition.
        /// </summary>
        INestedTypeDefinition Rewrite(INestedTypeDefinition namespaceTypeDefinition);

        /// <summary>
        /// Rewrites the given namespace type reference.
        /// </summary>
        INestedTypeReference Rewrite(INestedTypeReference nestedTypeReference);

        /// <summary>
        /// Rewrites the given namespace type reference.
        /// </summary>
        INestedTypeReference RewriteUnspecialized(INestedTypeReference nestedTypeReference);

        /// <summary>
        /// Rewrites the specified nested unit namespace.
        /// </summary>
        INestedUnitNamespace Rewrite(INestedUnitNamespace nestedUnitNamespace);

        /// <summary>
        /// Rewrites the specified reference to a nested unit namespace.
        /// </summary>
        INestedUnitNamespaceReference Rewrite(INestedUnitNamespaceReference nestedUnitNamespaceReference);

        /// <summary>
        /// Rewrites the specified operation.
        /// </summary>
        IOperation Rewrite(IOperation operation);

        /// <summary>
        /// Rewrites the specified operation exception information.
        /// </summary>
        IOperationExceptionInformation Rewrite(
            IOperationExceptionInformation operationExceptionInformation);

        /// <summary>
        /// Rewrites the given parameter definition.
        /// </summary>
        IParameterDefinition Rewrite(IParameterDefinition parameterDefinition);

        /// <summary>
        /// Rewrites the given parameter type information.
        /// </summary>
        IParameterTypeInformation Rewrite(IParameterTypeInformation parameterTypeInformation);

        /// <summary>
        /// Rewrites the given PE section.
        /// </summary>
        IPESection Rewrite(IPESection peSection);

        /// <summary>
        /// Rewrites the specified platform invoke information.
        /// </summary>
        IPlatformInvokeInformation Rewrite(IPlatformInvokeInformation platformInvokeInformation);

        /// <summary>
        /// Rewrites the given pointer type reference.
        /// </summary>
        IPointerTypeReference Rewrite(IPointerTypeReference pointerTypeReference);

        /// <summary>
        /// Rewrites the given property definition.
        /// </summary>
        IPropertyDefinition Rewrite(IPropertyDefinition propertyDefinition);

        /// <summary>
        /// Rewrites the given reference to a manifest resource.
        /// </summary>
        IResourceReference Rewrite(IResourceReference resourceReference);

        /// <summary>
        /// Rewrites the given root unit namespace.
        /// </summary>
        IRootUnitNamespace Rewrite(IRootUnitNamespace rootUnitNamespace);

        /// <summary>
        /// Rewrites the given reference to a root unit namespace.
        /// </summary>
        IRootUnitNamespaceReference Rewrite(IRootUnitNamespaceReference rootUnitNamespaceReference);

        /// <summary>
        /// Rewrites the given security attribute.
        /// </summary>
        ISecurityAttribute Rewrite(ISecurityAttribute securityAttribute);

        /// <summary>
        /// Rewrites the given specialized field reference.
        /// </summary>
        ISpecializedFieldReference Rewrite(ISpecializedFieldReference specializedFieldReference);

        /// <summary>
        /// Rewrites the given specialized method reference.
        /// </summary>
        IMethodReference Rewrite(ISpecializedMethodReference specializedMethodReference);

        /// <summary>
        /// Rewrites the given specialized nested type reference.
        /// </summary>
        INestedTypeReference Rewrite(ISpecializedNestedTypeReference specializedNestedTypeReference);

        /// <summary>
        /// Rewrites the given type definition.
        /// </summary>
        ITypeDefinition Rewrite(ITypeDefinition typeDefinition);

        /// <summary>
        /// Rewrites the specified type member.
        /// </summary>
        ITypeDefinitionMember Rewrite(ITypeDefinitionMember typeMember);

        /// <summary>
        /// Rewrites the specified type reference.
        /// </summary>
        ITypeReference Rewrite(ITypeReference typeReference);

        /// <summary>
        /// Rewrites the specified unit.
        /// </summary>
        IUnit Rewrite(IUnit unit);

        /// <summary>
        /// Rewrites the specified unit namespace.
        /// </summary>
        IUnitNamespace Rewrite(IUnitNamespace unitNamespace);

        /// <summary>
        /// Rewrites the specified reference to a unit namespace.
        /// </summary>
        IUnitNamespaceReference Rewrite(IUnitNamespaceReference unitNamespaceReference);

        /// <summary>
        /// Rewrites the specified unit reference.
        /// </summary>
        IUnitReference Rewrite(IUnitReference unitReference);

        /// <summary>
        /// Rewrites the given Win32 resource.
        /// </summary>
        IWin32Resource Rewrite(IWin32Resource win32Resource);

        /// <summary>
        /// Rewrites the list of aliases for types.
        /// </summary>
        List<IAliasForType> /*?*/ Rewrite(List<IAliasForType> /*?*/ aliasesForTypes);

        /// <summary>
        /// Rewrites the list of members of a type alias.
        /// </summary>
        List<IAliasMember> /*?*/ Rewrite(List<IAliasMember> /*?*/ aliasMembers);

        /// <summary>
        /// Rewrites the specified assembly references.
        /// </summary>
        List<IAssemblyReference> /*?*/ Rewrite(List<IAssemblyReference> /*?*/ assemblyReferences);

        /// <summary>
        /// Rewrites the specified custom attributes.
        /// </summary>
        List<ICustomAttribute> /*?*/ Rewrite(List<ICustomAttribute> /*?*/ customAttributes);

        /// <summary>
        /// Rewrites the specified custom modifiers.
        /// </summary>
        List<ICustomModifier> /*?*/ Rewrite(List<ICustomModifier> /*?*/ customModifiers);

        /// <summary>
        /// Rewrites the specified events.
        /// </summary>
        List<IEventDefinition> /*?*/ Rewrite(List<IEventDefinition> /*?*/ events);

        /// <summary>
        /// Rewrites the specified fields.
        /// </summary>
        List<IFieldDefinition> /*?*/ Rewrite(List<IFieldDefinition> /*?*/ fields);

        /// <summary>
        /// Rewrites the specified file references.
        /// </summary>
        List<IFileReference> /*?*/ Rewrite(List<IFileReference> /*?*/ fileReferences);

        /// <summary>
        /// Rewrites the specified generic parameters.
        /// </summary>
        List<IGenericMethodParameter> /*?*/ Rewrite(List<IGenericMethodParameter> /*?*/ genericParameters);

        /// <summary>
        /// Rewrites the specified generic parameters.
        /// </summary>
        List<IGenericTypeParameter> /*?*/ Rewrite(List<IGenericTypeParameter> /*?*/ genericParameters);

        /// <summary>
        /// Rewrites the specified local definitions.
        /// </summary>
        List<ILocalDefinition> /*?*/ Rewrite(List<ILocalDefinition> /*?*/ localDefinitions);

        /// <summary>
        /// Rewrites the specified expressions.
        /// </summary>
        List<IMetadataExpression> /*?*/ Rewrite(List<IMetadataExpression> /*?*/ expressions);

        /// <summary>
        /// Rewrites the specified named arguments.
        /// </summary>
        List<IMetadataNamedArgument> /*?*/ Rewrite(List<IMetadataNamedArgument> /*?*/ namedArguments);

        /// <summary>
        /// Rewrites the specified methods.
        /// </summary>
        List<IMethodDefinition> /*?*/ Rewrite(List<IMethodDefinition> /*?*/ methods);

        /// <summary>
        /// Rewrites the specified method implementations.
        /// </summary>
        List<IMethodImplementation> /*?*/ Rewrite(List<IMethodImplementation> /*?*/ methodImplementations);

        /// <summary>
        /// Rewrites the specified method references.
        /// </summary>
        List<IMethodReference> /*?*/ Rewrite(List<IMethodReference> /*?*/ methodReferences);

        /// <summary>
        /// Rewrites the specified modules.
        /// </summary>
        List<IModule> /*?*/ Rewrite(List<IModule> /*?*/ modules);

        /// <summary>
        /// Rewrites the specified module references.
        /// </summary>
        List<IModuleReference> /*?*/ Rewrite(List<IModuleReference> /*?*/ moduleReferences);

        /// <summary>
        /// Rewrites the specified types.
        /// </summary>
        List<INamedTypeDefinition> /*?*/ Rewrite(List<INamedTypeDefinition> /*?*/ types);

        /// <summary>
        /// Rewrites the specified namespace members.
        /// </summary>
        List<INamespaceMember> /*?*/ Rewrite(List<INamespaceMember> /*?*/ namespaceMembers);

        /// <summary>
        /// Rewrites the specified nested types.
        /// </summary>
        List<INestedTypeDefinition> /*?*/ Rewrite(List<INestedTypeDefinition> /*?*/ nestedTypes);

        /// <summary>
        /// Rewrites the specified operations.
        /// </summary>
        List<IOperation> /*?*/ Rewrite(List<IOperation> /*?*/ operations);

        /// <summary>
        /// Rewrites the specified operation exception informations.
        /// </summary>
        List<IOperationExceptionInformation> /*?*/ Rewrite(
            List<IOperationExceptionInformation> /*?*/ operationExceptionInformations);

        /// <summary>
        /// Rewrites the specified parameters.
        /// </summary>
        List<IParameterDefinition> /*?*/ Rewrite(List<IParameterDefinition> /*?*/ parameters);

        /// <summary>
        /// Rewrites the specified parameter type informations.
        /// </summary>
        List<IParameterTypeInformation> /*?*/ Rewrite(
            List<IParameterTypeInformation> /*?*/ parameterTypeInformations);

        /// <summary>
        /// Rewrites the specified PE sections.
        /// </summary>
        List<IPESection> /*?*/ Rewrite(List<IPESection> /*?*/ peSections);

        /// <summary>
        /// Rewrites the specified properties.
        /// </summary>
        List<IPropertyDefinition> /*?*/ Rewrite(List<IPropertyDefinition> /*?*/ properties);

        /// <summary>
        /// Rewrites the specified resource references.
        /// </summary>
        List<IResourceReference> /*?*/ Rewrite(List<IResourceReference> /*?*/ resourceReferences);

        /// <summary>
        /// Rewrites the specified security attributes.
        /// </summary>
        List<ISecurityAttribute> /*?*/ Rewrite(List<ISecurityAttribute> /*?*/ securityAttributes);

        /// <summary>
        /// Rewrites the specified type members.
        /// </summary>
        /// <remarks>Not used by the rewriter itself.</remarks>
        List<ITypeDefinitionMember> /*?*/ Rewrite(List<ITypeDefinitionMember> /*?*/ typeMembers);

        /// <summary>
        /// Rewrites the specified type references.
        /// </summary>
        List<ITypeReference> /*?*/ Rewrite(List<ITypeReference> /*?*/ typeReferences);

        /// <summary>
        /// Rewrites the specified type references.
        /// </summary>
        List<IWin32Resource> /*?*/ Rewrite(List<IWin32Resource> /*?*/ win32Resources);

        void Initialize();
    }
}