namespace VisualMutator.Extensibility
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Cci;

    public interface IOperatorCodeVisitor
    {

        /// <summary>
        /// Performs some computation with the given addition.
        /// </summary>
        /// <param name="addition"></param>
        void Visit(IAddition addition);

        /// <summary>
        /// Performs some computation with the given addressable expression.
        /// </summary>
        /// <param name="addressableExpression"></param>
        void Visit(IAddressableExpression addressableExpression);

        /// <summary>
        /// Performs some computation with the given address dereference expression.
        /// </summary>
        /// <param name="addressDereference"></param>
        void Visit(IAddressDereference addressDereference);

        /// <summary>
        /// Performs some computation with the given AddressOf expression.
        /// </summary>
        /// <param name="addressOf"></param>
        void Visit(IAddressOf addressOf);

        /// <summary>
        /// Performs some computation with the given anonymous delegate expression.
        /// </summary>
        /// <param name="anonymousDelegate"></param>
        void Visit(IAnonymousDelegate anonymousDelegate);

        /// <summary>
        /// Performs some computation with the given array indexer expression.
        /// </summary>
        /// <param name="arrayIndexer"></param>
        void Visit(IArrayIndexer arrayIndexer);

        /// <summary>
        /// Performs some computation with the given assert statement.
        /// </summary>
        /// <param name="assertStatement"></param>
        void Visit(IAssertStatement assertStatement);

        /// <summary>
        /// Performs some computation with the given assignment expression.
        /// </summary>
        /// <param name="assignment"></param>
        void Visit(IAssignment assignment);

        /// <summary>
        /// Performs some computation with the given assume statement.
        /// </summary>
        /// <param name="assumeStatement"></param>
        void Visit(IAssumeStatement assumeStatement);

        /// <summary>
        /// Performs some computation with the given bitwise and expression.
        /// </summary>
        /// <param name="bitwiseAnd"></param>
        void Visit(IBitwiseAnd bitwiseAnd);

        /// <summary>
        /// Performs some computation with the given bitwise and expression.
        /// </summary>
        /// <param name="binaryOperation"></param>
        void Visit(IBinaryOperation binaryOperation);

        /// <summary>
        /// Performs some computation with the given bitwise or expression.
        /// </summary>
        /// <param name="bitwiseOr"></param>
        void Visit(IBitwiseOr bitwiseOr);

        /// <summary>
        /// Performs some computation with the given block expression.
        /// </summary>
        /// <param name="blockExpression"></param>
        void Visit(IBlockExpression blockExpression);

        /// <summary>
        /// Performs some computation with the given statement block.
        /// </summary>
        /// <param name="block"></param>
        void Visit(IBlockStatement block);

        /// <summary>
        /// Performs some computation with the given break statement.
        /// </summary>
        /// <param name="breakStatement"></param>
        void Visit(IBreakStatement breakStatement);

        /// <summary>
        /// Performs some computation with the cast-if-possible expression.
        /// </summary>
        /// <param name="castIfPossible"></param>
        void Visit(ICastIfPossible castIfPossible);

        /// <summary>
        /// Performs some computation with the given catch clause.
        /// </summary>
        /// <param name="catchClause"></param>
        void Visit(ICatchClause catchClause);

        /// <summary>
        /// Performs some computation with the given check-if-instance expression.
        /// </summary>
        /// <param name="checkIfInstance"></param>
        void Visit(ICheckIfInstance checkIfInstance);

        /// <summary>
        /// Performs some computation with the given compile time constant.
        /// </summary>
        /// <param name="constant"></param>
        void Visit(ICompileTimeConstant constant);

        /// <summary>
        /// Performs some computation with the given conversion expression.
        /// </summary>
        /// <param name="conversion"></param>
        void Visit(IConversion conversion);

        /// <summary>
        /// Performs some computation with the given conditional expression.
        /// </summary>
        /// <param name="conditional"></param>
        void Visit(IConditional conditional);

        /// <summary>
        /// Performs some computation with the given conditional statement.
        /// </summary>
        /// <param name="conditionalStatement"></param>
        void Visit(IConditionalStatement conditionalStatement);

        /// <summary>
        /// Performs some computation with the given continue statement.
        /// </summary>
        /// <param name="continueStatement"></param>
        void Visit(IContinueStatement continueStatement);

        /// <summary>
        /// Performs some computation with the given copy memory statement.
        /// </summary>
        /// <param name="copyMemoryStatement"></param>
        void Visit(ICopyMemoryStatement copyMemoryStatement);

        /// <summary>
        /// Performs some computation with the given array creation expression.
        /// </summary>
        /// <param name="createArray"></param>
        void Visit(ICreateArray createArray);

        /// <summary>
        /// Performs some computation with the given constructor call expression.
        /// </summary>
        /// <param name="createObjectInstance"></param>
        void Visit(ICreateObjectInstance createObjectInstance);

        /// <summary>
        /// Performs some computation with the anonymous object creation expression.
        /// </summary>
        /// <param name="createDelegateInstance"></param>
        void Visit(ICreateDelegateInstance createDelegateInstance);

        /// <summary>
        /// Performs some computation with the given defalut value expression.
        /// </summary>
        /// <param name="defaultValue"></param>
        void Visit(IDefaultValue defaultValue);

        /// <summary>
        /// Performs some computation with the given division expression.
        /// </summary>
        /// <param name="division"></param>
        void Visit(IDivision division);

        /// <summary>
        /// Performs some computation with the given do until statement.
        /// </summary>
        /// <param name="doUntilStatement"></param>
        void Visit(IDoUntilStatement doUntilStatement);

        /// <summary>
        /// Performs some computation with the given dup value expression.
        /// </summary>
        /// <param name="dupValue"></param>
        void Visit(IDupValue dupValue);

        /// <summary>
        /// Performs some computation with the given empty statement.
        /// </summary>
        /// <param name="emptyStatement"></param>
        void Visit(IEmptyStatement emptyStatement);

        /// <summary>
        /// Performs some computation with the given equality expression.
        /// </summary>
        /// <param name="equality"></param>
        void Visit(IEquality equality);

        /// <summary>
        /// Performs some computation with the given exclusive or expression.
        /// </summary>
        /// <param name="exclusiveOr"></param>
        void Visit(IExclusiveOr exclusiveOr);

        /// <summary>
        /// Performs some computation with the given bound expression.
        /// </summary>
        /// <param name="boundExpression"></param>
        void Visit(IBoundExpression boundExpression);

        /// <summary>
        /// Performs some computation with the given debugger break statement.
        /// </summary>
        /// <param name="debuggerBreakStatement"></param>
        void Visit(IDebuggerBreakStatement debuggerBreakStatement);

        /// <summary>
        /// Performs some computation with the given expression.
        /// </summary>
        /// <param name="expression"></param>
        void Visit(IExpression expression);

        /// <summary>
        /// Performs some computation with the given expression statement.
        /// </summary>
        /// <param name="expressionStatement"></param>
        void Visit(IExpressionStatement expressionStatement);

        /// <summary>
        /// Performs some computation with the given fill memory statement.
        /// </summary>
        /// <param name="fillMemoryStatement"></param>
        void Visit(IFillMemoryStatement fillMemoryStatement);

        /// <summary>
        /// Performs some computation with the given foreach statement.
        /// </summary>
        /// <param name="forEachStatement"></param>
        void Visit(IForEachStatement forEachStatement);

        /// <summary>
        /// Performs some computation with the given for statement.
        /// </summary>
        /// <param name="forStatement"></param>
        void Visit(IForStatement forStatement);

        /// <summary>
        /// Performs some computation with the given get type of typed reference expression.
        /// </summary>
        /// <param name="getTypeOfTypedReference"></param>
        void Visit(IGetTypeOfTypedReference getTypeOfTypedReference);

        /// <summary>
        /// Performs some computation with the given get value of typed reference expression.
        /// </summary>
        /// <param name="getValueOfTypedReference"></param>
        void Visit(IGetValueOfTypedReference getValueOfTypedReference);

        /// <summary>
        /// Performs some computation with the given goto statement.
        /// </summary>
        /// <param name="gotoStatement"></param>
        void Visit(IGotoStatement gotoStatement);

        /// <summary>
        /// Performs some computation with the given goto switch case statement.
        /// </summary>
        /// <param name="gotoSwitchCaseStatement"></param>
        void Visit(IGotoSwitchCaseStatement gotoSwitchCaseStatement);

        /// <summary>
        /// Performs some computation with the given greater-than expression.
        /// </summary>
        /// <param name="greaterThan"></param>
        void Visit(IGreaterThan greaterThan);

        /// <summary>
        /// Performs some computation with the given greater-than-or-equal expression.
        /// </summary>
        /// <param name="greaterThanOrEqual"></param>
        void Visit(IGreaterThanOrEqual greaterThanOrEqual);

        /// <summary>
        /// Performs some computation with the given labeled statement.
        /// </summary>
        /// <param name="labeledStatement"></param>
        void Visit(ILabeledStatement labeledStatement);

        /// <summary>
        /// Performs some computation with the given left shift expression.
        /// </summary>
        /// <param name="leftShift"></param>
        void Visit(ILeftShift leftShift);

        /// <summary>
        /// Performs some computation with the given less-than expression.
        /// </summary>
        /// <param name="lessThan"></param>
        void Visit(ILessThan lessThan);

        /// <summary>
        /// Performs some computation with the given less-than-or-equal expression.
        /// </summary>
        /// <param name="lessThanOrEqual"></param>
        void Visit(ILessThanOrEqual lessThanOrEqual);

        /// <summary>
        /// Performs some computation with the given local declaration statement.
        /// </summary>
        /// <param name="localDeclarationStatement"></param>
        void Visit(ILocalDeclarationStatement localDeclarationStatement);

        /// <summary>
        /// Performs some computation with the given lock statement.
        /// </summary>
        /// <param name="lockStatement"></param>
        void Visit(ILockStatement lockStatement);

        /// <summary>
        /// Performs some computation with the given logical not expression.
        /// </summary>
        /// <param name="logicalNot"></param>
        void Visit(ILogicalNot logicalNot);

        /// <summary>
        /// Performs some computation with the given make typed reference expression.
        /// </summary>
        /// <param name="makeTypedReference"></param>
        void Visit(IMakeTypedReference makeTypedReference);

        /// <summary>
        /// Performs some computation with the given method call.
        /// </summary>
        /// <param name="methodCall"></param>
        void Visit(IMethodCall methodCall);

        /// <summary>
        /// Performs some computation with the given modulus expression.
        /// </summary>
        /// <param name="modulus"></param>
        void Visit(IModulus modulus);

        /// <summary>
        /// Performs some computation with the given multiplication expression.
        /// </summary>
        /// <param name="multiplication"></param>
        void Visit(IMultiplication multiplication);

        /// <summary>
        /// Performs some computation with the given named argument expression.
        /// </summary>
        /// <param name="namedArgument"></param>
        void Visit(INamedArgument namedArgument);

        /// <summary>
        /// Performs some computation with the given not equality expression.
        /// </summary>
        /// <param name="notEquality"></param>
        void Visit(INotEquality notEquality);

        /// <summary>
        /// Performs some computation with the given old value expression.
        /// </summary>
        /// <param name="oldValue"></param>
        void Visit(IOldValue oldValue);

        /// <summary>
        /// Performs some computation with the given one's complement expression.
        /// </summary>
        /// <param name="onesComplement"></param>
        void Visit(IOnesComplement onesComplement);

        /// <summary>
        /// Performs some computation with the given out argument expression.
        /// </summary>
        /// <param name="outArgument"></param>
        void Visit(IOutArgument outArgument);

        /// <summary>
        /// Performs some computation with the given pointer call.
        /// </summary>
        /// <param name="pointerCall"></param>
        void Visit(IPointerCall pointerCall);

        /// <summary>
        /// Performs some computation with the given pop value expression.
        /// </summary>
        /// <param name="popValue"></param>
        void Visit(IPopValue popValue);

        /// <summary>
        /// Performs some computation with the given push statement.
        /// </summary>
        /// <param name="pushStatement"></param>
        void Visit(IPushStatement pushStatement);

        /// <summary>
        /// Performs some computation with the given ref argument expression.
        /// </summary>
        /// <param name="refArgument"></param>
        void Visit(IRefArgument refArgument);

        /// <summary>
        /// Performs some computation with the given resource usage statement.
        /// </summary>
        /// <param name="resourceUseStatement"></param>
        void Visit(IResourceUseStatement resourceUseStatement);

        /// <summary>
        /// Performs some computation with the rethrow statement.
        /// </summary>
        /// <param name="rethrowStatement"></param>
        void Visit(IRethrowStatement rethrowStatement);

        /// <summary>
        /// Performs some computation with the return statement.
        /// </summary>
        /// <param name="returnStatement"></param>
        void Visit(IReturnStatement returnStatement);

        /// <summary>
        /// Performs some computation with the given return value expression.
        /// </summary>
        /// <param name="returnValue"></param>
        void Visit(IReturnValue returnValue);

        /// <summary>
        /// Performs some computation with the given right shift expression.
        /// </summary>
        /// <param name="rightShift"></param>
        void Visit(IRightShift rightShift);

        /// <summary>
        /// Performs some computation with the given stack array create expression.
        /// </summary>
        /// <param name="stackArrayCreate"></param>
        void Visit(IStackArrayCreate stackArrayCreate);

        /// <summary>
        /// Performs some computation with the given runtime argument handle expression.
        /// </summary>
        /// <param name="runtimeArgumentHandleExpression"></param>
        void Visit(IRuntimeArgumentHandleExpression runtimeArgumentHandleExpression);

        /// <summary>
        /// Performs some computation with the given sizeof() expression.
        /// </summary>
        /// <param name="sizeOf"></param>
        void Visit(ISizeOf sizeOf);

        /// <summary>
        /// Visits the specified statement.
        /// </summary>
        /// <param name="statement">The statement.</param>
        void Visit(IStatement statement);

        /// <summary>
        /// Performs some computation with the given subtraction expression.
        /// </summary>
        /// <param name="subtraction"></param>
        void Visit(ISubtraction subtraction);

        /// <summary>
        /// Performs some computation with the given switch case.
        /// </summary>
        /// <param name="switchCase"></param>
        void Visit(ISwitchCase switchCase);

        /// <summary>
        /// Performs some computation with the given switch statement.
        /// </summary>
        /// <param name="switchStatement"></param>
        void Visit(ISwitchStatement switchStatement);

        /// <summary>
        /// Performs some computation with the given target expression.
        /// </summary>
        /// <param name="targetExpression"></param>
        void Visit(ITargetExpression targetExpression);

        /// <summary>
        /// Performs some computation with the given this reference expression.
        /// </summary>
        /// <param name="thisReference"></param>
        void Visit(IThisReference thisReference);

        /// <summary>
        /// Performs some computation with the throw statement.
        /// </summary>
        /// <param name="throwStatement"></param>
        void Visit(IThrowStatement throwStatement);

        /// <summary>
        /// Performs some computation with the try-catch-filter-finally statement.
        /// </summary>
        /// <param name="tryCatchFilterFinallyStatement"></param>
        void Visit(ITryCatchFinallyStatement tryCatchFilterFinallyStatement);

        /// <summary>
        /// Performs some computation with the given tokenof() expression.
        /// </summary>
        /// <param name="tokenOf"></param>
        void Visit(ITokenOf tokenOf);

        /// <summary>
        /// Performs some computation with the given typeof() expression.
        /// </summary>
        /// <param name="typeOf"></param>
        void Visit(ITypeOf typeOf);

        /// <summary>
        /// Performs some computation with the given unary negation expression.
        /// </summary>
        /// <param name="unaryNegation"></param>
        void Visit(IUnaryNegation unaryNegation);

        /// <summary>
        /// Performs some computation with the given unary operation expression.
        /// </summary>
        /// <param name="unaryOperation"></param>
        void Visit(IUnaryOperation unaryOperation);

        /// <summary>
        /// Performs some computation with the given unary plus expression.
        /// </summary>
        /// <param name="unaryPlus"></param>
        void Visit(IUnaryPlus unaryPlus);

        /// <summary>
        /// Performs some computation with the given vector length expression.
        /// </summary>
        /// <param name="vectorLength"></param>
        void Visit(IVectorLength vectorLength);

        /// <summary>
        /// Performs some computation with the given while do statement.
        /// </summary>
        /// <param name="whileDoStatement"></param>
        void Visit(IWhileDoStatement whileDoStatement);

        /// <summary>
        /// Performs some computation with the given yield break statement.
        /// </summary>
        /// <param name="yieldBreakStatement"></param>
        void Visit(IYieldBreakStatement yieldBreakStatement);

        /// <summary>
        /// Performs some computation with the given yield return statement.
        /// </summary>
        /// <param name="yieldReturnStatement"></param>
        void Visit(IYieldReturnStatement yieldReturnStatement);

        /// <summary>
        /// Visits the specified alias for type.
        /// </summary>
        void Visit(IAliasForType aliasForType);

        /// <summary>
        /// Performs some computation with the given array type reference.
        /// </summary>
        void Visit(IArrayTypeReference arrayTypeReference);

        /// <summary>
        /// Performs some computation with the given assembly.
        /// </summary>
        void Visit(IAssembly assembly);

        /// <summary>
        /// Performs some computation with the given assembly reference.
        /// </summary>
        void Visit(IAssemblyReference assemblyReference);

        /// <summary>
        /// Performs some computation with the given custom attribute.
        /// </summary>
        void Visit(ICustomAttribute customAttribute);

        /// <summary>
        /// Performs some computation with the given custom modifier.
        /// </summary>
        void Visit(ICustomModifier customModifier);

        /// <summary>
        /// Performs some computation with the given event definition.
        /// </summary>
        void Visit(IEventDefinition eventDefinition);

        /// <summary>
        /// Performs some computation with the given field definition.
        /// </summary>
        void Visit(IFieldDefinition fieldDefinition);

        /// <summary>
        /// Performs some computation with the given field reference.
        /// </summary>
        void Visit(IFieldReference fieldReference);

        /// <summary>
        /// Performs some computation with the given file reference.
        /// </summary>
        void Visit(IFileReference fileReference);

        /// <summary>
        /// Performs some computation with the given function pointer type reference.
        /// </summary>
        void Visit(IFunctionPointerTypeReference functionPointerTypeReference);

        /// <summary>
        /// Performs some computation with the given generic method instance reference.
        /// </summary>
        void Visit(IGenericMethodInstanceReference genericMethodInstanceReference);

        /// <summary>
        /// Performs some computation with the given generic method parameter.
        /// </summary>
        void Visit(IGenericMethodParameter genericMethodParameter);

        /// <summary>
        /// Performs some computation with the given generic method parameter reference.
        /// </summary>
        void Visit(IGenericMethodParameterReference genericMethodParameterReference);

        /// <summary>
        /// Performs some computation with the given generic parameter.
        /// </summary>
        void Visit(IGenericParameter genericParameter);

        /// <summary>
        /// Performs some computation with the given generic parameter.
        /// </summary>
        void Visit(IGenericParameterReference genericParameterReference);

        /// <summary>
        /// Performs some computation with the given generic type instance reference.
        /// </summary>
        void Visit(IGenericTypeInstanceReference genericTypeInstanceReference);

        /// <summary>
        /// Performs some computation with the given generic parameter.
        /// </summary>
        void Visit(IGenericTypeParameter genericTypeParameter);

        /// <summary>
        /// Performs some computation with the given generic type parameter reference.
        /// </summary>
        void Visit(IGenericTypeParameterReference genericTypeParameterReference);

        /// <summary>
        /// Performs some computation with the given global field definition.
        /// </summary>
        void Visit(IGlobalFieldDefinition globalFieldDefinition);

        /// <summary>
        /// Performs some computation with the given global method definition.
        /// </summary>
        void Visit(IGlobalMethodDefinition globalMethodDefinition);

        /// <summary>
        /// Performs some computation with the given local definition.
        /// </summary>
        void Visit(ILocalDefinition localDefinition);

        /// <summary>
        /// Performs some computation with the given local definition.
        /// </summary>
        void VisitReference(ILocalDefinition localDefinition);

        /// <summary>
        /// Performs some computation with the given managed pointer type reference.
        /// </summary>
        void Visit(IManagedPointerTypeReference managedPointerTypeReference);

        /// <summary>
        /// Performs some computation with the given marshalling information.
        /// </summary>
        void Visit(IMarshallingInformation marshallingInformation);

        /// <summary>
        /// Performs some computation with the given metadata constant.
        /// </summary>
        void Visit(IMetadataConstant constant);

        /// <summary>
        /// Performs some computation with the given metadata array creation expression.
        /// </summary>
        void Visit(IMetadataCreateArray createArray);

        /// <summary>
        /// Performs some computation with the given metadata expression.
        /// </summary>
        void Visit(IMetadataExpression expression);

        /// <summary>
        /// Performs some computation with the given metadata named argument expression.
        /// </summary>
        void Visit(IMetadataNamedArgument namedArgument);

        /// <summary>
        /// Performs some computation with the given metadata typeof expression.
        /// </summary>
        void Visit(IMetadataTypeOf typeOf);

        /// <summary>
        /// Performs some computation with the given method body.
        /// </summary>
        void Visit(IMethodBody methodBody);

        /// <summary>
        /// Performs some computation with the given method definition.
        /// </summary>
        void Visit(IMethodDefinition method);

        /// <summary>
        /// Performs some computation with the given method implementation.
        /// </summary>
        void Visit(IMethodImplementation methodImplementation);

        /// <summary>
        /// Performs some computation with the given method reference.
        /// </summary>
        void Visit(IMethodReference methodReference);

        /// <summary>
        /// Performs some computation with the given modified type reference.
        /// </summary>
        void Visit(IModifiedTypeReference modifiedTypeReference);

        /// <summary>
        /// Performs some computation with the given module.
        /// </summary>
        void Visit(IModule module);

        /// <summary>
        /// Performs some computation with the given module reference.
        /// </summary>
        void Visit(IModuleReference moduleReference);

        /// <summary>
        /// Performs some computation with the given named type definition.
        /// </summary>
        void Visit(INamedTypeDefinition namedTypeDefinition);

        /// <summary>
        /// Performs some computation with the given named type reference.
        /// </summary>
        void Visit(INamedTypeReference namedTypeReference);

        /// <summary>
        /// Performs some computation with the given alias for a namespace type definition.
        /// </summary>
        void Visit(INamespaceAliasForType namespaceAliasForType);

        /// <summary>
        /// Visits the specified namespace definition.
        /// </summary>
        void Visit(INamespaceDefinition namespaceDefinition);

        /// <summary>
        /// Visits the specified namespace member.
        /// </summary>
        void Visit(INamespaceMember namespaceMember);

        /// <summary>
        /// Performs some computation with the given namespace type definition.
        /// </summary>
        void Visit(INamespaceTypeDefinition namespaceTypeDefinition);

        /// <summary>
        /// Performs some computation with the given namespace type reference.
        /// </summary>
        void Visit(INamespaceTypeReference namespaceTypeReference);

        /// <summary>
        /// Performs some computation with the given alias to a nested type definition.
        /// </summary>
        void Visit(INestedAliasForType nestedAliasForType);

        /// <summary>
        /// Performs some computation with the given nested type definition.
        /// </summary>
        void Visit(INestedTypeDefinition nestedTypeDefinition);

        /// <summary>
        /// Performs some computation with the given nested type reference.
        /// </summary>
        void Visit(INestedTypeReference nestedTypeReference);

        /// <summary>
        /// Performs some computation with the given nested unit namespace.
        /// </summary>
        void Visit(INestedUnitNamespace nestedUnitNamespace);

        /// <summary>
        /// Performs some computation with the given nested unit namespace reference.
        /// </summary>
        void Visit(INestedUnitNamespaceReference nestedUnitNamespaceReference);

        /// <summary>
        /// Performs some computation with the given nested unit set namespace.
        /// </summary>
        void Visit(INestedUnitSetNamespace nestedUnitSetNamespace);

        /// <summary>
        /// Performs some computation with the given IL operation.
        /// </summary>
        void Visit(IOperation operation);

        /// <summary>
        /// Performs some computation with the given IL operation exception information instance.
        /// </summary>
        void Visit(IOperationExceptionInformation operationExceptionInformation);

        /// <summary>
        /// Performs some computation with the given parameter definition.
        /// </summary>
        void Visit(IParameterDefinition parameterDefinition);

        /// <summary>
        /// Performs some computation with the given parameter definition.
        /// </summary>
        void VisitReference(IParameterDefinition parameterDefinition);

        /// <summary>
        /// Performs some computation with the given property definition.
        /// </summary>
        void Visit(IPropertyDefinition propertyDefinition);

        /// <summary>
        /// Performs some computation with the given parameter type information.
        /// </summary>
        void Visit(IParameterTypeInformation parameterTypeInformation);

        /// <summary>
        /// Performs some compuation with the given PE section.
        /// </summary>
        void Visit(IPESection peSection);

        /// <summary>
        /// Performs some compuation with the given platoform invoke information.
        /// </summary>
        void Visit(IPlatformInvokeInformation platformInvokeInformation);

        /// <summary>
        /// Performs some computation with the given pointer type reference.
        /// </summary>
        void Visit(IPointerTypeReference pointerTypeReference);

        /// <summary>
        /// Performs some computation with the given reference to a manifest resource.
        /// </summary>
        void Visit(IResourceReference resourceReference);

        /// <summary>
        /// Performs some computation with the given root unit namespace.
        /// </summary>
        void Visit(IRootUnitNamespace rootUnitNamespace);

        /// <summary>
        /// Performs some computation with the given root unit namespace reference.
        /// </summary>
        void Visit(IRootUnitNamespaceReference rootUnitNamespaceReference);

        /// <summary>
        /// Performs some computation with the given root unit set namespace.
        /// </summary>
        void Visit(IRootUnitSetNamespace rootUnitSetNamespace);

        /// <summary>
        /// Performs some computation with the given security attribute.
        /// </summary>
        void Visit(ISecurityAttribute securityAttribute);

        /// <summary>
        /// Performs some computation with the given specialized event definition.
        /// </summary>
        void Visit(ISpecializedEventDefinition specializedEventDefinition);

        /// <summary>
        /// Performs some computation with the given specialized field definition.
        /// </summary>
        void Visit(ISpecializedFieldDefinition specializedFieldDefinition);

        /// <summary>
        /// Performs some computation with the given specialized field reference.
        /// </summary>
        void Visit(ISpecializedFieldReference specializedFieldReference);

        /// <summary>
        /// Performs some computation with the given specialized method definition.
        /// </summary>
        void Visit(ISpecializedMethodDefinition specializedMethodDefinition);

        /// <summary>
        /// Performs some computation with the given specialized method reference.
        /// </summary>
        void Visit(ISpecializedMethodReference specializedMethodReference);

        /// <summary>
        /// Performs some computation with the given specialized propperty definition.
        /// </summary>
        void Visit(ISpecializedPropertyDefinition specializedPropertyDefinition);

        /// <summary>
        /// Performs some computation with the given specialized nested type definition.
        /// </summary>
        void Visit(ISpecializedNestedTypeDefinition specializedNestedTypeDefinition);

        /// <summary>
        /// Performs some computation with the given specialized nested type reference.
        /// </summary>
        void Visit(ISpecializedNestedTypeReference specializedNestedTypeReference);

        /// <summary>
        /// Visits the specified type definition.
        /// </summary>
        void Visit(ITypeDefinition typeDefinition);

        /// <summary>
        /// Visits the specified type member.
        /// </summary>
        void Visit(ITypeDefinitionMember typeMember);

        /// <summary>
        /// Visits the specified type member reference.
        /// </summary>
        void Visit(ITypeMemberReference typeMember);

        /// <summary>
        /// Visits the specified type reference.
        /// </summary>
        void Visit(ITypeReference typeReference);

        /// <summary>
        /// Visits the specified unit.
        /// </summary>
        void Visit(IUnit unit);

        /// <summary>
        /// Visits the specified unit reference.
        /// </summary>
        void Visit(IUnitReference unitReference);

        /// <summary>
        /// Visits the specified unit namespace.
        /// </summary>
        void Visit(IUnitNamespace unitNamespace);

        /// <summary>
        /// Visits the specified unit namespace reference.
        /// </summary>
        void Visit(IUnitNamespaceReference unitNamespaceReference);

        /// <summary>
        /// Performs some computation with the given unit set.
        /// </summary>
        void Visit(IUnitSet unitSet);

        /// <summary>
        /// Visits the specified unit set namespace.
        /// </summary>
        void Visit(IUnitSetNamespace unitSetNamespace);

        /// <summary>
        /// Performs some computation with the given Win32 resource.
        /// </summary>
        void Visit(IWin32Resource win32Resource);

        IVisualCodeVisitor Parent
        {
            get;
            set;
        }
        MetadataReaderHost Host { get; set; }
        

        IOperatorUtils OperatorUtils
        {
            get;
            set;
        }
        void Initialize();

        void VisitAny(object o);
    }
}