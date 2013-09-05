namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using Microsoft.Cci;

    public class DebugCodeTraverser : CodeTraverser
    {
        Stack<object> objectsInTree = new Stack<object>();
        private readonly DebugOperatorCodeVisitor _visitor;
        public object CurrentObject
        {
            get;
            set;
        }
        public int LevelCount { get; set; }
        public DebugCodeTraverser(DebugOperatorCodeVisitor visitor)
        { 
       
            _visitor = visitor;
            _visitor.Traverser = this;
            PreorderVisitor = visitor;
        }

        private void MethodEnter(object obj)
        {
            CurrentObject = obj;
            objectsInTree.Push(obj);
            LevelCount++;
        }
        private void MethodExit()
        {
            if (objectsInTree.Count == 0)
            {
                CurrentObject = null;
            }
            else
            {
                CurrentObject = objectsInTree.Pop();
            }
            LevelCount--;
        }
        
        public override void TraverseChildren(IWin32Resource win32Resource)
{ MethodEnter(win32Resource);
            base.TraverseChildren(win32Resource);
     MethodExit();   }

        public override void TraverseChildren(IUnitReference unitReference)
{ MethodEnter(unitReference);
            base.TraverseChildren(unitReference);
     MethodExit();   }

        public override void TraverseChildren(IUnitNamespaceReference unitNamespaceReference)
{ MethodEnter(unitNamespaceReference);
            base.TraverseChildren(unitNamespaceReference);
     MethodExit();   }

        public override void TraverseChildren(IUnitSetNamespace unitSetNamespace)
{ MethodEnter(unitSetNamespace);
            base.TraverseChildren(unitSetNamespace);
     MethodExit();   }

        public override void TraverseChildren(IUnitNamespace namespaceDefinition)
{ MethodEnter(namespaceDefinition);
            base.TraverseChildren(namespaceDefinition);
     MethodExit();   }

        public override void TraverseChildren(ITypeReference typeReference)
{ MethodEnter(typeReference);
            base.TraverseChildren(typeReference);
     MethodExit();   }

        public override void TraverseChildren(ITypeDefinitionMember typeMember)
{ MethodEnter(typeMember);
            base.TraverseChildren(typeMember);
     MethodExit();   }

        public override void TraverseChildren(ITypeDefinition typeDefinition)
{ MethodEnter(typeDefinition);
            base.TraverseChildren(typeDefinition);
     MethodExit();   }

        public override void TraverseChildren(ISpecializedNestedTypeReference specializedNestedTypeReference)
{ MethodEnter(specializedNestedTypeReference);
            base.TraverseChildren(specializedNestedTypeReference);
     MethodExit();   }

        public override void TraverseChildren(ISpecializedMethodReference specializedMethodReference)
{ MethodEnter(specializedMethodReference);
            base.TraverseChildren(specializedMethodReference);
     MethodExit();   }

        public override void TraverseChildren(ISpecializedFieldReference specializedFieldReference)
{ MethodEnter(specializedFieldReference);
            base.TraverseChildren(specializedFieldReference);
     MethodExit();   }

        public override void TraverseChildren(ISecurityAttribute securityAttribute)
{ MethodEnter(securityAttribute);
            base.TraverseChildren(securityAttribute);
     MethodExit();   }

        public override void TraverseChildren(IRootUnitNamespaceReference rootUnitNamespaceReference)
{ MethodEnter(rootUnitNamespaceReference);
            base.TraverseChildren(rootUnitNamespaceReference);
     MethodExit();   }

        public override void TraverseChildren(IRootUnitSetNamespace rootUnitSetNamespace)
{ MethodEnter(rootUnitSetNamespace);
            base.TraverseChildren(rootUnitSetNamespace);
     MethodExit();   }

        public override void TraverseChildren(IRootUnitNamespace rootUnitNamespace)
{ MethodEnter(rootUnitNamespace);
            base.TraverseChildren(rootUnitNamespace);
     MethodExit();   }

        public override void TraverseChildren(IResourceReference resourceReference)
{ MethodEnter(resourceReference);
            base.TraverseChildren(resourceReference);
     MethodExit();   }

        public override void TraverseChildren(IPropertyDefinition propertyDefinition)
{ MethodEnter(propertyDefinition);
            base.TraverseChildren(propertyDefinition);
     MethodExit();   }

        public override void TraverseChildren(IPointerTypeReference pointerTypeReference)
{ MethodEnter(pointerTypeReference);
            base.TraverseChildren(pointerTypeReference);
     MethodExit();   }

        public override void TraverseChildren(IPlatformInvokeInformation platformInvokeInformation)
{ MethodEnter(platformInvokeInformation);
            base.TraverseChildren(platformInvokeInformation);
     MethodExit();   }

        public override void TraverseChildren(IPESection peSection)
{ MethodEnter(peSection);
            base.TraverseChildren(peSection);
     MethodExit();   }

        public override void TraverseChildren(IParameterTypeInformation parameterTypeInformation)
{ MethodEnter(parameterTypeInformation);
            base.TraverseChildren(parameterTypeInformation);
     MethodExit();   }

        public override void TraverseChildren(IParameterDefinition parameterDefinition)
{ MethodEnter(parameterDefinition);
            base.TraverseChildren(parameterDefinition);
     MethodExit();   }

        public override void TraverseChildren(IOperationExceptionInformation operationExceptionInformation)
{ MethodEnter(operationExceptionInformation);
            base.TraverseChildren(operationExceptionInformation);
     MethodExit();   }

        public override void TraverseChildren(IOperation operation)
{ MethodEnter(operation);
            base.TraverseChildren(operation);
     MethodExit();   }

        public override void TraverseChildren(INestedUnitSetNamespace nestedUnitSetNamespace)
{ MethodEnter(nestedUnitSetNamespace);
            base.TraverseChildren(nestedUnitSetNamespace);
     MethodExit();   }

        public override void TraverseChildren(INestedUnitNamespaceReference nestedUnitNamespaceReference)
{ MethodEnter(nestedUnitNamespaceReference);
            base.TraverseChildren(nestedUnitNamespaceReference);
     MethodExit();   }

        public override void TraverseChildren(INestedUnitNamespace nestedUnitNamespace)
{ MethodEnter(nestedUnitNamespace);
            base.TraverseChildren(nestedUnitNamespace);
     MethodExit();   }

        public override void TraverseChildren(INestedTypeReference nestedTypeReference)
{ MethodEnter(nestedTypeReference);
            base.TraverseChildren(nestedTypeReference);
     MethodExit();   }

        public override void TraverseChildren(INestedTypeDefinition nestedTypeDefinition)
{ MethodEnter(nestedTypeDefinition);
            base.TraverseChildren(nestedTypeDefinition);
     MethodExit();   }

        public override void TraverseChildren(INestedAliasForType nestedAliasForType)
{ MethodEnter(nestedAliasForType);
            base.TraverseChildren(nestedAliasForType);
     MethodExit();   }

        public override void TraverseChildren(INamespaceTypeReference namespaceTypeReference)
{ MethodEnter(namespaceTypeReference);
            base.TraverseChildren(namespaceTypeReference);
     MethodExit();   }

        public override void TraverseChildren(INamespaceTypeDefinition namespaceTypeDefinition)
{ MethodEnter(namespaceTypeDefinition);
            base.TraverseChildren(namespaceTypeDefinition);
     MethodExit();   }

        public override void TraverseChildren(INamespaceDefinition namespaceDefinition)
{ MethodEnter(namespaceDefinition);
            base.TraverseChildren(namespaceDefinition);
     MethodExit();   }

        public override void TraverseChildren(INamespaceAliasForType namespaceAliasForType)
{ MethodEnter(namespaceAliasForType);
            base.TraverseChildren(namespaceAliasForType);
     MethodExit();   }

        public override void TraverseChildren(INamedTypeDefinition namedTypeDefinition)
{ MethodEnter(namedTypeDefinition);
            base.TraverseChildren(namedTypeDefinition);
     MethodExit();   }

        public override void TraverseChildren(IModuleReference moduleReference)
{ MethodEnter(moduleReference);
            base.TraverseChildren(moduleReference);
     MethodExit();   }

        public override void TraverseChildren(IModule module)
{ MethodEnter(module);
            base.TraverseChildren(module);
     MethodExit();   }

        public override void TraverseChildren(IModifiedTypeReference modifiedTypeReference)
{ MethodEnter(modifiedTypeReference);
            base.TraverseChildren(modifiedTypeReference);
     MethodExit();   }

        public override void TraverseChildren(IMethodReference methodReference)
{ MethodEnter(methodReference);
            base.TraverseChildren(methodReference);
     MethodExit();   }

        public override void TraverseChildren(IMethodImplementation methodImplementation)
{ MethodEnter(methodImplementation);
            base.TraverseChildren(methodImplementation);
     MethodExit();   }

        public override void TraverseChildren(IMethodDefinition method)
{ MethodEnter(method);
            base.TraverseChildren(method);
     MethodExit();   }

        public override void TraverseChildren(IMethodBody methodBody)
{ MethodEnter(methodBody);
            base.TraverseChildren(methodBody);
     MethodExit();   }

        public override void TraverseChildren(IMetadataTypeOf typeOf)
{ MethodEnter(typeOf);
            base.TraverseChildren(typeOf);
     MethodExit();   }

        public override void TraverseChildren(IMetadataNamedArgument namedArgument)
{ MethodEnter(namedArgument);
            base.TraverseChildren(namedArgument);
     MethodExit();   }

        public override void TraverseChildren(IMetadataExpression expression)
{ MethodEnter(expression);
            base.TraverseChildren(expression);
     MethodExit();   }

        public override void TraverseChildren(IMetadataCreateArray createArray)
{ MethodEnter(createArray);
            base.TraverseChildren(createArray);
     MethodExit();   }

        public override void TraverseChildren(IMetadataConstant constant)
{ MethodEnter(constant);
            base.TraverseChildren(constant);
     MethodExit();   }

        public override void TraverseChildren(IMarshallingInformation marshallingInformation)
{ MethodEnter(marshallingInformation);
            base.TraverseChildren(marshallingInformation);
     MethodExit();   }

        public override void TraverseChildren(IManagedPointerTypeReference managedPointerTypeReference)
{ MethodEnter(managedPointerTypeReference);
            base.TraverseChildren(managedPointerTypeReference);
     MethodExit();   }

        public override void TraverseChildren(ILocalDefinition localDefinition)
{ MethodEnter(localDefinition);
            base.TraverseChildren(localDefinition);
     MethodExit();   }

        public override void TraverseChildren(IGlobalMethodDefinition globalMethodDefinition)
{ MethodEnter(globalMethodDefinition);
            base.TraverseChildren(globalMethodDefinition);
     MethodExit();   }

        public override void TraverseChildren(IGlobalFieldDefinition globalFieldDefinition)
{ MethodEnter(globalFieldDefinition);
            base.TraverseChildren(globalFieldDefinition);
     MethodExit();   }

        public override void TraverseChildren(IGenericTypeParameterReference genericTypeParameterReference)
{ MethodEnter(genericTypeParameterReference);
            base.TraverseChildren(genericTypeParameterReference);
     MethodExit();   }

        public override void TraverseChildren(IGenericTypeParameter genericTypeParameter)
{ MethodEnter(genericTypeParameter);
            base.TraverseChildren(genericTypeParameter);
     MethodExit();   }

        public override void TraverseChildren(IGenericTypeInstanceReference genericTypeInstanceReference)
{ MethodEnter(genericTypeInstanceReference);
            base.TraverseChildren(genericTypeInstanceReference);
     MethodExit();   }

        public override void TraverseChildren(IGenericParameter genericParameter)
{ MethodEnter(genericParameter);
            base.TraverseChildren(genericParameter);
     MethodExit();   }

        public override void TraverseChildren(IGenericParameterReference genericParameterReference)
{ MethodEnter(genericParameterReference);
            base.TraverseChildren(genericParameterReference);
     MethodExit();   }

        public override void TraverseChildren(IGenericMethodParameterReference genericMethodParameterReference)
{ MethodEnter(genericMethodParameterReference);
            base.TraverseChildren(genericMethodParameterReference);
     MethodExit();   }

        public override void TraverseChildren(IGenericMethodParameter genericMethodParameter)
{ MethodEnter(genericMethodParameter);
            base.TraverseChildren(genericMethodParameter);
     MethodExit();   }

        public override void TraverseChildren(IGenericMethodInstanceReference genericMethodInstanceReference)
{ MethodEnter(genericMethodInstanceReference);
            base.TraverseChildren(genericMethodInstanceReference);
     MethodExit();   }

        public override void TraverseChildren(IFunctionPointerTypeReference functionPointerTypeReference)
{ MethodEnter(functionPointerTypeReference);
            base.TraverseChildren(functionPointerTypeReference);
     MethodExit();   }

        public override void TraverseChildren(IFileReference fileReference)
{ MethodEnter(fileReference);
            base.TraverseChildren(fileReference);
     MethodExit();   }

        public override void TraverseChildren(IFieldReference fieldReference)
{ MethodEnter(fieldReference);
            base.TraverseChildren(fieldReference);
     MethodExit();   }

        public override void TraverseChildren(IFieldDefinition fieldDefinition)
{ MethodEnter(fieldDefinition);
            base.TraverseChildren(fieldDefinition);
     MethodExit();   }

        public override void TraverseChildren(IEventDefinition eventDefinition)
{ MethodEnter(eventDefinition);
            base.TraverseChildren(eventDefinition);
     MethodExit();   }

        public override void TraverseChildren(ICustomModifier customModifier)
{ MethodEnter(customModifier);
            base.TraverseChildren(customModifier);
     MethodExit();   }

        public override void TraverseChildren(ICustomAttribute customAttribute)
{ MethodEnter(customAttribute);
            base.TraverseChildren(customAttribute);
     MethodExit();   }

        public override void TraverseChildren(IAssemblyReference assemblyReference)
{ MethodEnter(assemblyReference);
            base.TraverseChildren(assemblyReference);
     MethodExit();   }

        public override void TraverseChildren(IAssembly assembly)
{ MethodEnter(assembly);
            base.TraverseChildren(assembly);
     MethodExit();   }

        public override void TraverseChildren(IArrayTypeReference arrayTypeReference)
{ MethodEnter(arrayTypeReference);
            base.TraverseChildren(arrayTypeReference);
     MethodExit();   }

        public override void TraverseChildren(IAliasForType aliasForType)
{ MethodEnter(aliasForType);
            base.TraverseChildren(aliasForType);
     MethodExit();   }

        public override void TraverseChildren(IYieldReturnStatement yieldReturnStatement)
{ MethodEnter(yieldReturnStatement);
            base.TraverseChildren(yieldReturnStatement);
     MethodExit();   }

        public override void TraverseChildren(IYieldBreakStatement yieldBreakStatement)
{ MethodEnter(yieldBreakStatement);
            base.TraverseChildren(yieldBreakStatement);
     MethodExit();   }

        public override void TraverseChildren(IWhileDoStatement whileDoStatement)
{ MethodEnter(whileDoStatement);
            base.TraverseChildren(whileDoStatement);
     MethodExit();   }

        public override void TraverseChildren(IVectorLength vectorLength)
{ MethodEnter(vectorLength);
            base.TraverseChildren(vectorLength);
     MethodExit();   }

        public override void TraverseChildren(IUnaryPlus unaryPlus)
{ MethodEnter(unaryPlus);
            base.TraverseChildren(unaryPlus);
     MethodExit();   }

        public override void TraverseChildren(IUnaryOperation unaryOperation)
{ MethodEnter(unaryOperation);
            base.TraverseChildren(unaryOperation);
     MethodExit();   }

        public override void TraverseChildren(IUnaryNegation unaryNegation)
{ MethodEnter(unaryNegation);
            base.TraverseChildren(unaryNegation);
     MethodExit();   }

        public override void TraverseChildren(ITypeOf typeOf)
{ MethodEnter(typeOf);
            base.TraverseChildren(typeOf);
     MethodExit();   }

        public override void TraverseChildren(ITokenOf tokenOf)
{ MethodEnter(tokenOf);
            base.TraverseChildren(tokenOf);
     MethodExit();   }

        public override void TraverseChildren(ITryCatchFinallyStatement tryCatchFilterFinallyStatement)
{ MethodEnter(tryCatchFilterFinallyStatement);
            base.TraverseChildren(tryCatchFilterFinallyStatement);
     MethodExit();   }

        public override void TraverseChildren(IThrowStatement throwStatement)
{ MethodEnter(throwStatement);
            base.TraverseChildren(throwStatement);
     MethodExit();   }

        public override void TraverseChildren(IThisReference thisReference)
{ MethodEnter(thisReference);
            base.TraverseChildren(thisReference);
     MethodExit();   }

        public override void TraverseChildren(ITargetExpression targetExpression)
{ MethodEnter(targetExpression);
            base.TraverseChildren(targetExpression);
     MethodExit();   }

        public override void TraverseChildren(ISwitchStatement switchStatement)
{ MethodEnter(switchStatement);
            base.TraverseChildren(switchStatement);
     MethodExit();   }

        public override void TraverseChildren(ISwitchCase switchCase)
{ MethodEnter(switchCase);
            base.TraverseChildren(switchCase);
     MethodExit();   }

        public override void TraverseChildren(ISubtraction subtraction)
{ MethodEnter(subtraction);
            base.TraverseChildren(subtraction);
     MethodExit();   }

        public override void TraverseChildren(IStatement statement)
{ MethodEnter(statement);
            base.TraverseChildren(statement);
     MethodExit();   }

        public override void TraverseChildren(IStackArrayCreate stackArrayCreate)
{ MethodEnter(stackArrayCreate);
            base.TraverseChildren(stackArrayCreate);
     MethodExit();   }

        public override void TraverseChildren(ISourceMethodBody sourceMethodBody)
{ MethodEnter(sourceMethodBody);
            base.TraverseChildren(sourceMethodBody);
     MethodExit();   }

        public override void TraverseChildren(ISizeOf sizeOf)
{ MethodEnter(sizeOf);
            base.TraverseChildren(sizeOf);
     MethodExit();   }

        public override void TraverseChildren(IRuntimeArgumentHandleExpression runtimeArgumentHandleExpression)
{ MethodEnter(runtimeArgumentHandleExpression);
            base.TraverseChildren(runtimeArgumentHandleExpression);
     MethodExit();   }

        public override void TraverseChildren(IRightShift rightShift)
{ MethodEnter(rightShift);
            base.TraverseChildren(rightShift);
     MethodExit();   }

        public override void TraverseChildren(IReturnValue returnValue)
{ MethodEnter(returnValue);
            base.TraverseChildren(returnValue);
     MethodExit();   }

        public override void TraverseChildren(IReturnStatement returnStatement)
{ MethodEnter(returnStatement);
            base.TraverseChildren(returnStatement);
     MethodExit();   }

        public override void TraverseChildren(IRethrowStatement rethrowStatement)
{ MethodEnter(rethrowStatement);
            base.TraverseChildren(rethrowStatement);
     MethodExit();   }

        public override void TraverseChildren(IResourceUseStatement resourceUseStatement)
{ MethodEnter(resourceUseStatement);
            base.TraverseChildren(resourceUseStatement);
     MethodExit();   }

        public override void TraverseChildren(IRefArgument refArgument)
{ MethodEnter(refArgument);
            base.TraverseChildren(refArgument);
     MethodExit();   }

        public override void TraverseChildren(IPushStatement pushStatement)
{ MethodEnter(pushStatement);
            base.TraverseChildren(pushStatement);
     MethodExit();   }

        public override void TraverseChildren(IPopValue popValue)
{ MethodEnter(popValue);
            base.TraverseChildren(popValue);
     MethodExit();   }

        public override void TraverseChildren(IPointerCall pointerCall)
{ MethodEnter(pointerCall);
            base.TraverseChildren(pointerCall);
     MethodExit();   }

        public override void TraverseChildren(IOutArgument outArgument)
{ MethodEnter(outArgument);
            base.TraverseChildren(outArgument);
     MethodExit();   }

        public override void TraverseChildren(IOnesComplement onesComplement)
{ MethodEnter(onesComplement);
            base.TraverseChildren(onesComplement);
     MethodExit();   }

        public override void TraverseChildren(IOldValue oldValue)
{ MethodEnter(oldValue);
            base.TraverseChildren(oldValue);
     MethodExit();   }

        public override void TraverseChildren(INotEquality notEquality)
{ MethodEnter(notEquality);
            base.TraverseChildren(notEquality);
     MethodExit();   }

        public override void TraverseChildren(INamedArgument namedArgument)
{ MethodEnter(namedArgument);
            base.TraverseChildren(namedArgument);
     MethodExit();   }

        public override void TraverseChildren(IMultiplication multiplication)
{ MethodEnter(multiplication);
            base.TraverseChildren(multiplication);
     MethodExit();   }

        public override void TraverseChildren(IModulus modulus)
{ MethodEnter(modulus);
            base.TraverseChildren(modulus);
     MethodExit();   }

        public override void TraverseChildren(IMethodCall methodCall)
{ MethodEnter(methodCall);
            base.TraverseChildren(methodCall);
     MethodExit();   }

        public override void TraverseChildren(IMakeTypedReference makeTypedReference)
{ MethodEnter(makeTypedReference);
            base.TraverseChildren(makeTypedReference);
     MethodExit();   }

        public override void TraverseChildren(ILogicalNot logicalNot)
{ MethodEnter(logicalNot);
            base.TraverseChildren(logicalNot);
     MethodExit();   }

        public override void TraverseChildren(ILockStatement lockStatement)
{ MethodEnter(lockStatement);
            base.TraverseChildren(lockStatement);
     MethodExit();   }

        public override void TraverseChildren(ILocalDeclarationStatement localDeclarationStatement)
{ MethodEnter(localDeclarationStatement);
            base.TraverseChildren(localDeclarationStatement);
     MethodExit();   }

        public override void TraverseChildren(ILessThanOrEqual lessThanOrEqual)
{ MethodEnter(lessThanOrEqual);
            base.TraverseChildren(lessThanOrEqual);
     MethodExit();   }

        public override void TraverseChildren(ILessThan lessThan)
{ MethodEnter(lessThan);
            base.TraverseChildren(lessThan);
     MethodExit();   }

        public override void TraverseChildren(ILeftShift leftShift)
{ MethodEnter(leftShift);
            base.TraverseChildren(leftShift);
     MethodExit();   }

        public override void TraverseChildren(ILabeledStatement labeledStatement)
{ MethodEnter(labeledStatement);
            base.TraverseChildren(labeledStatement);
     MethodExit();   }

        public override void TraverseChildren(IGreaterThanOrEqual greaterThanOrEqual)
{ MethodEnter(greaterThanOrEqual);
            base.TraverseChildren(greaterThanOrEqual);
     MethodExit();   }

        public override void TraverseChildren(IGreaterThan greaterThan)
{ MethodEnter(greaterThan);
            base.TraverseChildren(greaterThan);
     MethodExit();   }

        public override void TraverseChildren(IGotoSwitchCaseStatement gotoSwitchCaseStatement)
{ MethodEnter(gotoSwitchCaseStatement);
            base.TraverseChildren(gotoSwitchCaseStatement);
     MethodExit();   }

        public override void TraverseChildren(IGotoStatement gotoStatement)
{ MethodEnter(gotoStatement);
            base.TraverseChildren(gotoStatement);
     MethodExit();   }

        public override void TraverseChildren(IGetValueOfTypedReference getValueOfTypedReference)
{ MethodEnter(getValueOfTypedReference);
            base.TraverseChildren(getValueOfTypedReference);
     MethodExit();   }

        public override void TraverseChildren(IGetTypeOfTypedReference getTypeOfTypedReference)
{ MethodEnter(getTypeOfTypedReference);
            base.TraverseChildren(getTypeOfTypedReference);
     MethodExit();   }

        public override void TraverseChildren(IForStatement forStatement)
{ MethodEnter(forStatement);
            base.TraverseChildren(forStatement);
     MethodExit();   }

        public override void TraverseChildren(IForEachStatement forEachStatement)
{ MethodEnter(forEachStatement);
            base.TraverseChildren(forEachStatement);
     MethodExit();   }

        public override void TraverseChildren(IFillMemoryStatement fillMemoryStatement)
{ MethodEnter(fillMemoryStatement);
            base.TraverseChildren(fillMemoryStatement);
     MethodExit();   }

        public override void TraverseChildren(IExpressionStatement expressionStatement)
{ MethodEnter(expressionStatement);
            base.TraverseChildren(expressionStatement);
     MethodExit();   }

        public override void TraverseChildren(IExpression expression)
{ MethodEnter(expression);
            base.TraverseChildren(expression);
     MethodExit();   }

        public override void TraverseChildren(IExclusiveOr exclusiveOr)
{ MethodEnter(exclusiveOr);
            base.TraverseChildren(exclusiveOr);
     MethodExit();   }

        public override void TraverseChildren(IEquality equality)
{ MethodEnter(equality);
            base.TraverseChildren(equality);
     MethodExit();   }

        public override void TraverseChildren(IEmptyStatement emptyStatement)
{ MethodEnter(emptyStatement);
            base.TraverseChildren(emptyStatement);
     MethodExit();   }

        public override void TraverseChildren(IDupValue dupValue)
{ MethodEnter(dupValue);
            base.TraverseChildren(dupValue);
     MethodExit();   }

        public override void TraverseChildren(IDoUntilStatement doUntilStatement)
{ MethodEnter(doUntilStatement);
            base.TraverseChildren(doUntilStatement);
     MethodExit();   }

        public override void TraverseChildren(IDivision division)
{ MethodEnter(division);
            base.TraverseChildren(division);
     MethodExit();   }

        public override void TraverseChildren(IDefaultValue defaultValue)
{ MethodEnter(defaultValue);
            base.TraverseChildren(defaultValue);
     MethodExit();   }

        public override void TraverseChildren(IDebuggerBreakStatement debuggerBreakStatement)
{ MethodEnter(debuggerBreakStatement);
            base.TraverseChildren(debuggerBreakStatement);
     MethodExit();   }

        public override void TraverseChildren(ICreateObjectInstance createObjectInstance)
{ MethodEnter(createObjectInstance);
            base.TraverseChildren(createObjectInstance);
     MethodExit();   }

        public override void TraverseChildren(ICreateDelegateInstance createDelegateInstance)
{ MethodEnter(createDelegateInstance);
            base.TraverseChildren(createDelegateInstance);
     MethodExit();   }

        public override void TraverseChildren(ICreateArray createArray)
{ MethodEnter(createArray);
            base.TraverseChildren(createArray);
     MethodExit();   }

        public override void TraverseChildren(IConversion conversion)
{ MethodEnter(conversion);
            base.TraverseChildren(conversion);
     MethodExit();   }

        public override void TraverseChildren(ICopyMemoryStatement copyMemoryStatement)
{ MethodEnter(copyMemoryStatement);
            base.TraverseChildren(copyMemoryStatement);
     MethodExit();   }

        public override void TraverseChildren(IContinueStatement continueStatement)
{ MethodEnter(continueStatement);
            base.TraverseChildren(continueStatement);
     MethodExit();   }

        public override void TraverseChildren(IConditionalStatement conditionalStatement)
{ MethodEnter(conditionalStatement);
            base.TraverseChildren(conditionalStatement);
     MethodExit();   }

        public override void TraverseChildren(IConditional conditional)
{ MethodEnter(conditional);
            base.TraverseChildren(conditional);
     MethodExit();   }

        public override void TraverseChildren(ICompileTimeConstant constant)
{ MethodEnter(constant);
            base.TraverseChildren(constant);
     MethodExit();   }

        public override void TraverseChildren(ICheckIfInstance checkIfInstance)
{ MethodEnter(checkIfInstance);
            base.TraverseChildren(checkIfInstance);
     MethodExit();   }

        public override void TraverseChildren(ICatchClause catchClause)
{ MethodEnter(catchClause);
            base.TraverseChildren(catchClause);
     MethodExit();   }

        public override void TraverseChildren(ICastIfPossible castIfPossible)
{ MethodEnter(castIfPossible);
            base.TraverseChildren(castIfPossible);
     MethodExit();   }

        public override void TraverseChildren(IBreakStatement breakStatement)
{ MethodEnter(breakStatement);
            base.TraverseChildren(breakStatement);
     MethodExit();   }

        public override void TraverseChildren(IBoundExpression boundExpression)
{ MethodEnter(boundExpression);
            base.TraverseChildren(boundExpression);
     MethodExit();   }

        public override void TraverseChildren(IBlockStatement block)
{ MethodEnter(block);
            base.TraverseChildren(block);
     MethodExit();   }

        public override void TraverseChildren(IBlockExpression blockExpression)
{ MethodEnter(blockExpression);
            base.TraverseChildren(blockExpression);
     MethodExit();   }

        public override void TraverseChildren(IBitwiseOr bitwiseOr)
{ MethodEnter(bitwiseOr);
            base.TraverseChildren(bitwiseOr);
     MethodExit();   }

        public override void TraverseChildren(IBitwiseAnd bitwiseAnd)
{ MethodEnter(bitwiseAnd);
            base.TraverseChildren(bitwiseAnd);
     MethodExit();   }

        public override void TraverseChildren(IBinaryOperation binaryOperation)
{ MethodEnter(binaryOperation);
            base.TraverseChildren(binaryOperation);
     MethodExit();   }

        public override void TraverseChildren(IAssumeStatement assumeStatement)
{ MethodEnter(assumeStatement);
            base.TraverseChildren(assumeStatement);
     MethodExit();   }

        public override void TraverseChildren(IAssignment assignment)
{ MethodEnter(assignment);
            base.TraverseChildren(assignment);
     MethodExit();   }

        public override void TraverseChildren(IAssertStatement assertStatement)
{ MethodEnter(assertStatement);
            base.TraverseChildren(assertStatement);
     MethodExit();   }

        public override void TraverseChildren(IArrayIndexer arrayIndexer)
{ MethodEnter(arrayIndexer);
            base.TraverseChildren(arrayIndexer);
     MethodExit();   }

        public override void TraverseChildren(IAnonymousDelegate anonymousDelegate)
{ MethodEnter(anonymousDelegate);
            base.TraverseChildren(anonymousDelegate);
     MethodExit();   }

        public override void TraverseChildren(IAddressOf addressOf)
{ MethodEnter(addressOf);
            base.TraverseChildren(addressOf);
     MethodExit();   }

        public override void TraverseChildren(IAddressDereference addressDereference)
{ MethodEnter(addressDereference);
            base.TraverseChildren(addressDereference);
     MethodExit();   }

        public override void TraverseChildren(IAddressableExpression addressableExpression)
{ MethodEnter(addressableExpression);
            base.TraverseChildren(addressableExpression);
     MethodExit();   }

        public override void TraverseChildren(IAddition addition)
{ MethodEnter(addition);
            base.TraverseChildren(addition);
     MethodExit();   }
    }
}