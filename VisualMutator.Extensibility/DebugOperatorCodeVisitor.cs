namespace VisualMutator.Extensibility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Cci;

    public class DebugOperatorCodeVisitor : CodeVisitor, IOperatorCodeVisitor
    {
        private readonly List<InvokInfo> allElements = new List<InvokInfo>();
        public DebugCodeTraverser Traverser { get; set; }
        public List<InvokInfo> AllElements
        {
            get { return allElements; }
        }

        public override void Visit(IAddition addition)
        {
            allElements.Add(new InvokInfo(Traverser, "IAddition", addition));
        }

        public override void Visit(IAddressableExpression addressableExpression)
        {
            allElements.Add(new InvokInfo(Traverser, "IAddressableExpression", addressableExpression));
        }

        public override void Visit(IAddressDereference addressDereference)
        {
            allElements.Add(new InvokInfo(Traverser, "IAddressDereference", addressDereference));
        }

        public override void Visit(IAddressOf addressOf)
        {
            allElements.Add(new InvokInfo(Traverser, "IAddressOf", addressOf));
        }

        public override void Visit(IAnonymousDelegate anonymousDelegate)
        {
            allElements.Add(new InvokInfo(Traverser, "IAnonymousDelegate", anonymousDelegate));
        }

        public override void Visit(IArrayIndexer arrayIndexer)
        {
            allElements.Add(new InvokInfo(Traverser, "IArrayIndexer", arrayIndexer));
        }

        public override void Visit(IAssertStatement assertStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "IAssertStatement", assertStatement));
        }

        public override void Visit(IAssignment assignment)
        {
            allElements.Add(new InvokInfo(Traverser, "IAssignment", assignment));
        }

        public override void Visit(IAssumeStatement assumeStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "IAssumeStatement", assumeStatement));
        }

        public override void Visit(IBitwiseAnd bitwiseAnd)
        {
            allElements.Add(new InvokInfo(Traverser, "IBitwiseAnd", bitwiseAnd));
        }

        public override void Visit(IBinaryOperation binaryOperation)
        {
            allElements.Add(new InvokInfo(Traverser, "IBinaryOperation", binaryOperation));
        }

        public override void Visit(IBitwiseOr bitwiseOr)
        {
            allElements.Add(new InvokInfo(Traverser, "IBitwiseOr", bitwiseOr));
        }

        public override void Visit(IBlockExpression blockExpression)
        {
            allElements.Add(new InvokInfo(Traverser, "IBlockExpression", blockExpression));
        }

        public override void Visit(IBlockStatement block)
        {
            allElements.Add(new InvokInfo(Traverser, "IBlockStatement", block));
        }

        public override void Visit(IBreakStatement breakStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "IBreakStatement", breakStatement));
        }

        public override void Visit(ICastIfPossible castIfPossible)
        {
            allElements.Add(new InvokInfo(Traverser, "ICastIfPossible", castIfPossible));
        }

        public override void Visit(ICatchClause catchClause)
        {
            allElements.Add(new InvokInfo(Traverser, "ICatchClause", catchClause));
        }

        public override void Visit(ICheckIfInstance checkIfInstance)
        {
            allElements.Add(new InvokInfo(Traverser, "ICheckIfInstance", checkIfInstance));
        }

        public override void Visit(ICompileTimeConstant constant)
        {
            allElements.Add(new InvokInfo(Traverser, "ICompileTimeConstant", constant));
        }

        public override void Visit(IConversion conversion)
        {
            allElements.Add(new InvokInfo(Traverser, "IConversion", conversion));
        }

        public override void Visit(IConditional conditional)
        {
            allElements.Add(new InvokInfo(Traverser, "IConditional", conditional));
        }

        public override void Visit(IConditionalStatement conditionalStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "IConditionalStatement", conditionalStatement));
        }

        public override void Visit(IContinueStatement continueStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "IContinueStatement", continueStatement));
        }

        public override void Visit(ICopyMemoryStatement copyMemoryStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "ICopyMemoryStatement", copyMemoryStatement));
        }

        public override void Visit(ICreateArray createArray)
        {
            allElements.Add(new InvokInfo(Traverser, "ICreateArray", createArray));
        }

        public override void Visit(ICreateObjectInstance createObjectInstance)
        {
            allElements.Add(new InvokInfo(Traverser, "ICreateObjectInstance", createObjectInstance));
        }

        public override void Visit(ICreateDelegateInstance createDelegateInstance)
        {
            allElements.Add(new InvokInfo(Traverser, "ICreateDelegateInstance", createDelegateInstance));
        }

        public override void Visit(IDefaultValue defaultValue)
        {
            allElements.Add(new InvokInfo(Traverser, "IDefaultValue", defaultValue));
        }

        public override void Visit(IDivision division)
        {
            allElements.Add(new InvokInfo(Traverser, "IDivision", division));
        }

        public override void Visit(IDoUntilStatement doUntilStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "IDoUntilStatement", doUntilStatement));
        }

        public override void Visit(IDupValue dupValue)
        {
            allElements.Add(new InvokInfo(Traverser, "IDupValue", dupValue));
        }

        public override void Visit(IEmptyStatement emptyStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "IEmptyStatement", emptyStatement));
        }

        public override void Visit(IEquality equality)
        {
            allElements.Add(new InvokInfo(Traverser, "IEquality", equality));
        }

        public override void Visit(IExclusiveOr exclusiveOr)
        {
            allElements.Add(new InvokInfo(Traverser, "IExclusiveOr", exclusiveOr));
        }

        public override void Visit(IBoundExpression boundExpression)
        {
            allElements.Add(new InvokInfo(Traverser, "IBoundExpression", boundExpression));
        }

        public override void Visit(IDebuggerBreakStatement debuggerBreakStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "IDebuggerBreakStatement", debuggerBreakStatement));
        }

        public override void Visit(IExpression expression)
        {
            allElements.Add(new InvokInfo(Traverser, "IExpression", expression));
        }

        public override void Visit(IExpressionStatement expressionStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "IExpressionStatement", expressionStatement));
        }

        public override void Visit(IFillMemoryStatement fillMemoryStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "IFillMemoryStatement", fillMemoryStatement));
        }

        public override void Visit(IForEachStatement forEachStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "IForEachStatement", forEachStatement));
        }

        public override void Visit(IForStatement forStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "IForStatement", forStatement));
        }

        public override void Visit(IGetTypeOfTypedReference getTypeOfTypedReference)
        {
            allElements.Add(new InvokInfo(Traverser, "IGetTypeOfTypedReference", getTypeOfTypedReference));
        }

        public override void Visit(IGetValueOfTypedReference getValueOfTypedReference)
        {
            allElements.Add(new InvokInfo(Traverser, "IGetValueOfTypedReference", getValueOfTypedReference));
        }

        public override void Visit(IGotoStatement gotoStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "IGotoStatement", gotoStatement));
        }

        public override void Visit(IGotoSwitchCaseStatement gotoSwitchCaseStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "IGotoSwitchCaseStatement", gotoSwitchCaseStatement));
        }

        public override void Visit(IGreaterThan greaterThan)
        {
            allElements.Add(new InvokInfo(Traverser, "IGreaterThan", greaterThan));
        }

        public override void Visit(IGreaterThanOrEqual greaterThanOrEqual)
        {
            allElements.Add(new InvokInfo(Traverser, "IGreaterThanOrEqual", greaterThanOrEqual));
        }

        public override void Visit(ILabeledStatement labeledStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "ILabeledStatement", labeledStatement));
        }

        public override void Visit(ILeftShift leftShift)
        {
            allElements.Add(new InvokInfo(Traverser, "ILeftShift", leftShift));
        }

        public override void Visit(ILessThan lessThan)
        {
            allElements.Add(new InvokInfo(Traverser, "ILessThan", lessThan));
        }

        public override void Visit(ILessThanOrEqual lessThanOrEqual)
        {
            allElements.Add(new InvokInfo(Traverser, "ILessThanOrEqual", lessThanOrEqual));
        }

        public override void Visit(ILocalDeclarationStatement localDeclarationStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "ILocalDeclarationStatement", localDeclarationStatement));
        }

        public override void Visit(ILockStatement lockStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "ILockStatement", lockStatement));
        }

        public override void Visit(ILogicalNot logicalNot)
        {
            allElements.Add(new InvokInfo(Traverser, "ILogicalNot", logicalNot));
        }

        public override void Visit(IMakeTypedReference makeTypedReference)
        {
            allElements.Add(new InvokInfo(Traverser, "IMakeTypedReference", makeTypedReference));
        }

        public override void Visit(IMethodCall methodCall)
        {
            allElements.Add(new InvokInfo(Traverser, "IMethodCall", methodCall));
        }

        public override void Visit(IModulus modulus)
        {
            allElements.Add(new InvokInfo(Traverser, "IModulus", modulus));
        }

        public override void Visit(IMultiplication multiplication)
        {
            allElements.Add(new InvokInfo(Traverser, "IMultiplication", multiplication));
        }

        public override void Visit(INamedArgument namedArgument)
        {
            allElements.Add(new InvokInfo(Traverser, "INamedArgument", namedArgument));
        }

        public override void Visit(INotEquality notEquality)
        {
            allElements.Add(new InvokInfo(Traverser, "INotEquality", notEquality));
        }

        public override void Visit(IOldValue oldValue)
        {
            allElements.Add(new InvokInfo(Traverser, "IOldValue", oldValue));
        }

        public override void Visit(IOnesComplement onesComplement)
        {
            allElements.Add(new InvokInfo(Traverser, "IOnesComplement", onesComplement));
        }

        public override void Visit(IOutArgument outArgument)
        {
            allElements.Add(new InvokInfo(Traverser, "IOutArgument", outArgument));
        }

        public override void Visit(IPointerCall pointerCall)
        {
            allElements.Add(new InvokInfo(Traverser, "IPointerCall", pointerCall));
        }

        public override void Visit(IPopValue popValue)
        {
            allElements.Add(new InvokInfo(Traverser, "IPopValue", popValue));
        }

        public override void Visit(IPushStatement pushStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "IPushStatement", pushStatement));
        }

        public override void Visit(IRefArgument refArgument)
        {
            allElements.Add(new InvokInfo(Traverser, "IRefArgument", refArgument));
        }

        public override void Visit(IResourceUseStatement resourceUseStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "IResourceUseStatement", resourceUseStatement));
        }

        public override void Visit(IRethrowStatement rethrowStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "IRethrowStatement", rethrowStatement));
        }

        public override void Visit(IReturnStatement returnStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "IReturnStatement", returnStatement));
        }

        public override void Visit(IReturnValue returnValue)
        {
            allElements.Add(new InvokInfo(Traverser, "IReturnValue", returnValue));
        }

        public override void Visit(IRightShift rightShift)
        {
            allElements.Add(new InvokInfo(Traverser, "IRightShift", rightShift));
        }

        public override void Visit(IStackArrayCreate stackArrayCreate)
        {
            allElements.Add(new InvokInfo(Traverser, "IStackArrayCreate", stackArrayCreate));
        }

        public override void Visit(IRuntimeArgumentHandleExpression runtimeArgumentHandleExpression)
        {
            allElements.Add(new InvokInfo(Traverser, "IRuntimeArgumentHandleExpression", runtimeArgumentHandleExpression));
        }

        public override void Visit(ISizeOf sizeOf)
        {
            allElements.Add(new InvokInfo(Traverser, "ISizeOf", sizeOf));
        }

        public override void Visit(IStatement statement)
        {
            allElements.Add(new InvokInfo(Traverser, "IStatement", statement));
        }

        public override void Visit(ISubtraction subtraction)
        {
            allElements.Add(new InvokInfo(Traverser, "ISubtraction", subtraction));
        }

        public override void Visit(ISwitchCase switchCase)
        {
            allElements.Add(new InvokInfo(Traverser, "ISwitchCase", switchCase));
        }

        public override void Visit(ISwitchStatement switchStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "ISwitchStatement", switchStatement));
        }

        public override void Visit(ITargetExpression targetExpression)
        {
            allElements.Add(new InvokInfo(Traverser, "ITargetExpression", targetExpression));
        }

        public override void Visit(IThisReference thisReference)
        {
            allElements.Add(new InvokInfo(Traverser, "IThisReference", thisReference));
        }

        public override void Visit(IThrowStatement throwStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "IThrowStatement", throwStatement));
        }

        public override void Visit(ITryCatchFinallyStatement tryCatchFilterFinallyStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "ITryCatchFinallyStatement", tryCatchFilterFinallyStatement));
        }

        public override void Visit(ITokenOf tokenOf)
        {
            allElements.Add(new InvokInfo(Traverser, "ITokenOf", tokenOf));
        }

        public override void Visit(ITypeOf typeOf)
        {
            allElements.Add(new InvokInfo(Traverser, "ITypeOf", typeOf));
        }

        public override void Visit(IUnaryNegation unaryNegation)
        {
            allElements.Add(new InvokInfo(Traverser, "IUnaryNegation", unaryNegation));
        }

        public override void Visit(IUnaryOperation unaryOperation)
        {
            allElements.Add(new InvokInfo(Traverser, "IUnaryOperation", unaryOperation));
        }

        public override void Visit(IUnaryPlus unaryPlus)
        {
            allElements.Add(new InvokInfo(Traverser, "IUnaryPlus", unaryPlus));
        }

        public override void Visit(IVectorLength vectorLength)
        {
            allElements.Add(new InvokInfo(Traverser, "IVectorLength", vectorLength));
        }

        public override void Visit(IWhileDoStatement whileDoStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "IWhileDoStatement", whileDoStatement));
        }

        public override void Visit(IYieldBreakStatement yieldBreakStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "IYieldBreakStatement", yieldBreakStatement));
        }

        public override void Visit(IYieldReturnStatement yieldReturnStatement)
        {
            allElements.Add(new InvokInfo(Traverser, "IYieldReturnStatement", yieldReturnStatement));
        }

        public override void Visit(IAliasForType aliasForType)
        {
            allElements.Add(new InvokInfo(Traverser, "IAliasForType", aliasForType));
        }

        public override void Visit(IArrayTypeReference arrayTypeReference)
        {
            allElements.Add(new InvokInfo(Traverser, "IArrayTypeReference", arrayTypeReference));
        }

        public override void Visit(IAssembly assembly)
        {
            allElements.Add(new InvokInfo(Traverser, "IAssembly", assembly));
        }

        public override void Visit(IAssemblyReference assemblyReference)
        {
            allElements.Add(new InvokInfo(Traverser, "IAssemblyReference", assemblyReference));
        }

        public override void Visit(ICustomAttribute customAttribute)
        {
            allElements.Add(new InvokInfo(Traverser, "ICustomAttribute", customAttribute));
        }

        public override void Visit(ICustomModifier customModifier)
        {
            allElements.Add(new InvokInfo(Traverser, "ICustomModifier", customModifier));
        }

        public override void Visit(IEventDefinition eventDefinition)
        {
            allElements.Add(new InvokInfo(Traverser, "IEventDefinition", eventDefinition));
        }

        public override void Visit(IFieldDefinition fieldDefinition)
        {
            allElements.Add(new InvokInfo(Traverser, "IFieldDefinition", fieldDefinition));
        }

        public override void Visit(IFieldReference fieldReference)
        {
            allElements.Add(new InvokInfo(Traverser, "IFieldReference", fieldReference));
        }

        public override void Visit(IFileReference fileReference)
        {
            allElements.Add(new InvokInfo(Traverser, "IFileReference", fileReference));
        }

        public override void Visit(IFunctionPointerTypeReference functionPointerTypeReference)
        {
            allElements.Add(new InvokInfo(Traverser, "IFunctionPointerTypeReference", functionPointerTypeReference));
        }

        public override void Visit(IGenericMethodInstanceReference genericMethodInstanceReference)
        {
            allElements.Add(new InvokInfo(Traverser, "IGenericMethodInstanceReference", genericMethodInstanceReference));
        }

        public override void Visit(IGenericMethodParameter genericMethodParameter)
        {
            allElements.Add(new InvokInfo(Traverser, "IGenericMethodParameter", genericMethodParameter));
        }

        public override void Visit(IGenericMethodParameterReference genericMethodParameterReference)
        {
            allElements.Add(new InvokInfo(Traverser, "IGenericMethodParameterReference", genericMethodParameterReference));
        }

        public override void Visit(IGenericParameter genericParameter)
        {
            allElements.Add(new InvokInfo(Traverser, "IGenericParameter", genericParameter));
        }

        public override void Visit(IGenericParameterReference genericParameterReference)
        {
            allElements.Add(new InvokInfo(Traverser, "IGenericParameterReference", genericParameterReference));
        }

        public override void Visit(IGenericTypeInstanceReference genericTypeInstanceReference)
        {
            allElements.Add(new InvokInfo(Traverser, "IGenericTypeInstanceReference", genericTypeInstanceReference));
        }

        public override void Visit(IGenericTypeParameter genericTypeParameter)
        {
            allElements.Add(new InvokInfo(Traverser, "IGenericTypeParameter", genericTypeParameter));
        }

        public override void Visit(IGenericTypeParameterReference genericTypeParameterReference)
        {
            allElements.Add(new InvokInfo(Traverser, "IGenericTypeParameterReference", genericTypeParameterReference));
        }

        public override void Visit(IGlobalFieldDefinition globalFieldDefinition)
        {
            allElements.Add(new InvokInfo(Traverser, "IGlobalFieldDefinition", globalFieldDefinition));
        }

        public override void Visit(IGlobalMethodDefinition globalMethodDefinition)
        {
            allElements.Add(new InvokInfo(Traverser, "IGlobalMethodDefinition", globalMethodDefinition));
        }

        public override void Visit(ILocalDefinition localDefinition)
        {
            allElements.Add(new InvokInfo(Traverser, "ILocalDefinition", localDefinition));
        }

        public override void VisitReference(ILocalDefinition localDefinition)
        {
            allElements.Add(new InvokInfo(Traverser, "ILocalDefinition", localDefinition));
        }

        public override void Visit(IManagedPointerTypeReference managedPointerTypeReference)
        {
            allElements.Add(new InvokInfo(Traverser, "IManagedPointerTypeReference", managedPointerTypeReference));
        }

        public override void Visit(IMarshallingInformation marshallingInformation)
        {
            allElements.Add(new InvokInfo(Traverser, "IMarshallingInformation", marshallingInformation));
        }

        public override void Visit(IMetadataConstant constant)
        {
            allElements.Add(new InvokInfo(Traverser, "IMetadataConstant", constant));
        }

        public override void Visit(IMetadataCreateArray createArray)
        {
            allElements.Add(new InvokInfo(Traverser, "IMetadataCreateArray", createArray));
        }

        public override void Visit(IMetadataExpression expression)
        {
            allElements.Add(new InvokInfo(Traverser, "IMetadataExpression", expression));
        }

        public override void Visit(IMetadataNamedArgument namedArgument)
        {
            allElements.Add(new InvokInfo(Traverser, "IMetadataNamedArgument", namedArgument));
        }

        public override void Visit(IMetadataTypeOf typeOf)
        {
            allElements.Add(new InvokInfo(Traverser, "IMetadataTypeOf", typeOf));
        }

        public override void Visit(IMethodBody methodBody)
        {
            allElements.Add(new InvokInfo(Traverser, "IMethodBody", methodBody));
        }

        public override void Visit(IMethodDefinition method)
        {
            allElements.Add(new InvokInfo(Traverser, "IMethodDefinition", method));
        }

        public override void Visit(IMethodImplementation methodImplementation)
        {
            allElements.Add(new InvokInfo(Traverser, "IMethodImplementation", methodImplementation));
        }

        public override void Visit(IMethodReference methodReference)
        {
            allElements.Add(new InvokInfo(Traverser, "IMethodReference", methodReference));
        }

        public override void Visit(IModifiedTypeReference modifiedTypeReference)
        {
            allElements.Add(new InvokInfo(Traverser, "IModifiedTypeReference", modifiedTypeReference));
        }

        public override void Visit(IModule module)
        {
            allElements.Add(new InvokInfo(Traverser, "IModule", module));
        }

        public override void Visit(IModuleReference moduleReference)
        {
            allElements.Add(new InvokInfo(Traverser, "IModuleReference", moduleReference));
        }

        public override void Visit(INamedTypeDefinition namedTypeDefinition)
        {
            allElements.Add(new InvokInfo(Traverser, "INamedTypeDefinition", namedTypeDefinition));
        }

        public override void Visit(INamedTypeReference namedTypeReference)
        {
            allElements.Add(new InvokInfo(Traverser, "INamedTypeReference", namedTypeReference));
        }

        public override void Visit(INamespaceAliasForType namespaceAliasForType)
        {
            allElements.Add(new InvokInfo(Traverser, "INamespaceAliasForType", namespaceAliasForType));
        }

        public override void Visit(INamespaceDefinition namespaceDefinition)
        {
            allElements.Add(new InvokInfo(Traverser, "INamespaceDefinition", namespaceDefinition));
        }

        public override void Visit(INamespaceMember namespaceMember)
        {
            allElements.Add(new InvokInfo(Traverser, "INamespaceMember", namespaceMember));
        }

        public override void Visit(INamespaceTypeDefinition namespaceTypeDefinition)
        {
            allElements.Add(new InvokInfo(Traverser, "INamespaceTypeDefinition", namespaceTypeDefinition));
        }

        public override void Visit(INamespaceTypeReference namespaceTypeReference)
        {
            allElements.Add(new InvokInfo(Traverser, "INamespaceTypeReference", namespaceTypeReference));
        }

        public override void Visit(INestedAliasForType nestedAliasForType)
        {
            allElements.Add(new InvokInfo(Traverser, "INestedAliasForType", nestedAliasForType));
        }

        public override void Visit(INestedTypeDefinition nestedTypeDefinition)
        {
            allElements.Add(new InvokInfo(Traverser, "INestedTypeDefinition", nestedTypeDefinition));
        }

        public override void Visit(INestedTypeReference nestedTypeReference)
        {
            allElements.Add(new InvokInfo(Traverser, "INestedTypeReference", nestedTypeReference));
        }

        public override void Visit(INestedUnitNamespace nestedUnitNamespace)
        {
            allElements.Add(new InvokInfo(Traverser, "INestedUnitNamespace", nestedUnitNamespace));
        }

        public override void Visit(INestedUnitNamespaceReference nestedUnitNamespaceReference)
        {
            allElements.Add(new InvokInfo(Traverser, "INestedUnitNamespaceReference", nestedUnitNamespaceReference));
        }

        public override void Visit(INestedUnitSetNamespace nestedUnitSetNamespace)
        {
            allElements.Add(new InvokInfo(Traverser, "INestedUnitSetNamespace", nestedUnitSetNamespace));
        }

        public override void Visit(IOperation operation)
        {
            allElements.Add(new InvokInfo(Traverser, "IOperation", operation));
        }

        public override void Visit(IOperationExceptionInformation operationExceptionInformation)
        {
            allElements.Add(new InvokInfo(Traverser, "IOperationExceptionInformation", operationExceptionInformation));
        }

        public override void Visit(IParameterDefinition parameterDefinition)
        {
            allElements.Add(new InvokInfo(Traverser, "IParameterDefinition", parameterDefinition));
        }

        public override void VisitReference(IParameterDefinition parameterDefinition)
        {
            allElements.Add(new InvokInfo(Traverser, "IParameterDefinition", parameterDefinition));
        }

        public override void Visit(IPropertyDefinition propertyDefinition)
        {
            allElements.Add(new InvokInfo(Traverser, "IPropertyDefinition", propertyDefinition));
        }

        public override void Visit(IParameterTypeInformation parameterTypeInformation)
        {
            allElements.Add(new InvokInfo(Traverser, "IParameterTypeInformation", parameterTypeInformation));
        }

        public override void Visit(IPESection peSection)
        {
            allElements.Add(new InvokInfo(Traverser, "IPESection", peSection));
        }

        public override void Visit(IPlatformInvokeInformation platformInvokeInformation)
        {
            allElements.Add(new InvokInfo(Traverser, "IPlatformInvokeInformation", platformInvokeInformation));
        }

        public override void Visit(IPointerTypeReference pointerTypeReference)
        {
            allElements.Add(new InvokInfo(Traverser, "IPointerTypeReference", pointerTypeReference));
        }

        public override void Visit(IResourceReference resourceReference)
        {
            allElements.Add(new InvokInfo(Traverser, "IResourceReference", resourceReference));
        }

        public override void Visit(IRootUnitNamespace rootUnitNamespace)
        {
            allElements.Add(new InvokInfo(Traverser, "IRootUnitNamespace", rootUnitNamespace));
        }

        public override void Visit(IRootUnitNamespaceReference rootUnitNamespaceReference)
        {
            allElements.Add(new InvokInfo(Traverser, "IRootUnitNamespaceReference", rootUnitNamespaceReference));
        }

        public override void Visit(IRootUnitSetNamespace rootUnitSetNamespace)
        {
            allElements.Add(new InvokInfo(Traverser, "IRootUnitSetNamespace", rootUnitSetNamespace));
        }

        public override void Visit(ISecurityAttribute securityAttribute)
        {
            allElements.Add(new InvokInfo(Traverser, "ISecurityAttribute", securityAttribute));
        }

        public override void Visit(ISpecializedEventDefinition specializedEventDefinition)
        {
            allElements.Add(new InvokInfo(Traverser, "ISpecializedEventDefinition", specializedEventDefinition));
        }

        public override void Visit(ISpecializedFieldDefinition specializedFieldDefinition)
        {
            allElements.Add(new InvokInfo(Traverser, "ISpecializedFieldDefinition", specializedFieldDefinition));
        }

        public override void Visit(ISpecializedFieldReference specializedFieldReference)
        {
            allElements.Add(new InvokInfo(Traverser, "ISpecializedFieldReference", specializedFieldReference));
        }

        public override void Visit(ISpecializedMethodDefinition specializedMethodDefinition)
        {
            allElements.Add(new InvokInfo(Traverser, "ISpecializedMethodDefinition", specializedMethodDefinition));
        }

        public override void Visit(ISpecializedMethodReference specializedMethodReference)
        {
            allElements.Add(new InvokInfo(Traverser, "ISpecializedMethodReference", specializedMethodReference));
        }

        public override void Visit(ISpecializedPropertyDefinition specializedPropertyDefinition)
        {
            allElements.Add(new InvokInfo(Traverser, "ISpecializedPropertyDefinition", specializedPropertyDefinition));
        }

        public override void Visit(ISpecializedNestedTypeDefinition specializedNestedTypeDefinition)
        {
            allElements.Add(new InvokInfo(Traverser, "ISpecializedNestedTypeDefinition", specializedNestedTypeDefinition));
        }

        public override void Visit(ISpecializedNestedTypeReference specializedNestedTypeReference)
        {
            allElements.Add(new InvokInfo(Traverser, "ISpecializedNestedTypeReference", specializedNestedTypeReference));
        }

        public override void Visit(ITypeDefinition typeDefinition)
        {
            allElements.Add(new InvokInfo(Traverser, "ITypeDefinition", typeDefinition));
        }

        public override void Visit(ITypeDefinitionMember typeMember)
        {
            allElements.Add(new InvokInfo(Traverser, "ITypeDefinitionMember", typeMember));
        }

        public override void Visit(ITypeMemberReference typeMember)
        {
            allElements.Add(new InvokInfo(Traverser, "ITypeMemberReference", typeMember));
        }

        public override void Visit(ITypeReference typeReference)
        {
            allElements.Add(new InvokInfo(Traverser, "ITypeReference", typeReference));
        }

        public override void Visit(IUnit unit)
        {
            allElements.Add(new InvokInfo(Traverser, "IUnit", unit));
        }

        public override void Visit(IUnitReference unitReference)
        {
            allElements.Add(new InvokInfo(Traverser, "IUnitReference", unitReference));
        }

        public override void Visit(IUnitNamespace unitNamespace)
        {
            allElements.Add(new InvokInfo(Traverser, "IUnitNamespace", unitNamespace));
        }

        public override void Visit(IUnitNamespaceReference unitNamespaceReference)
        {
            allElements.Add(new InvokInfo(Traverser, "IUnitNamespaceReference", unitNamespaceReference));
        }

        public override void Visit(IUnitSet unitSet)
        {
            allElements.Add(new InvokInfo(Traverser, "IUnitSet", unitSet));
        }

        public override void Visit(IUnitSetNamespace unitSetNamespace)
        {
            allElements.Add(new InvokInfo(Traverser, "IUnitSetNamespace", unitSetNamespace));
        }

        public override void Visit(IWin32Resource win32Resource)
        {
            allElements.Add(new InvokInfo(Traverser, "IWin32Resource", win32Resource));
        }

      

        public IVisualCodeVisitor Parent { get; set; }
        public MetadataReaderHost Host { get; set; }
        public IOperatorUtils OperatorUtils { get; set; }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            int c = 0;
            foreach (var element in AllElements)
            {//c + "/" + AllElements.Count + 
                string text = element.ToString();
                b.AppendLine(text);
              //  Console.WriteLine(text);
                c++;
            }
            return b.ToString();
            //return AllElements.Select((i, c) => c+"/"+AllElements.Count + i.ToString())
            //  .Aggregate((s,s2)=>s+Environment.NewLine+s2);
        }
        public void Initialize()
        {
            
        }

        public void VisitAny(object o)
        {
            
        }
    }
}