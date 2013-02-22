namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using Microsoft.Cci;

    public class VisualCodeVisitor : CodeVisitor
    {
        private IOperatorCodeVisitor visitor;
        protected int elementCounter;

        private List<MutationTarget> mutationTargets;
        private List<MutationTarget> commonTargets;

        private IMethodDefinition _currentMethod;

        public List<MutationTarget> MutationTargets
        {
            get { return mutationTargets; }
        }
        public List<MutationTarget> CommonTargets
        {
            get
            {
                return commonTargets;
            }
        }
        public VisualCodeVisitor(IOperatorCodeVisitor visitor)
        {
            this.visitor = visitor;

            mutationTargets = new List<MutationTarget>();
            commonTargets = new List<MutationTarget>();
        }


        protected virtual bool Process(object obj)
        {
            elementCounter++;
            return true;
        }

        public void MarkMutationTarget(object o, List<string> passesInfo)
        {
            if (passesInfo == null)
            {
                passesInfo = new List<string>
                {
                    ""
                };
            }
            for (int i = 0; i < passesInfo.Count; i++)
            {
                var mutationTarget = new MutationTarget(o.GetType().Name, elementCounter, i, passesInfo[i]);
                if (_currentMethod != null)
                {
                    mutationTarget.Method = new MethodIdentifier(_currentMethod);
                }
                mutationTargets.Add(mutationTarget);
            }
           
        }

        public void MarkCommon(object o)
        {

            var mutationTarget = new MutationTarget(o.GetType().Name, elementCounter, 0, "");

            commonTargets.Add(mutationTarget);


        }

        public void MethodEnter(IMethodDefinition method)
        {
            _currentMethod = method;
        }

        public void MethodExit(IMethodDefinition method)
        {
            _currentMethod = null;
        }





        public override void Visit(IAddition addition)
        {if(Process(addition)){visitor.Visit(addition);}
            base.Visit(addition);
        }

        public override void Visit(IAddressableExpression addressableExpression)
        {if(Process(addressableExpression)){visitor.Visit(addressableExpression);}
            base.Visit(addressableExpression);
        }

        public override void Visit(IAddressDereference addressDereference)
        {if(Process(addressDereference)){visitor.Visit(addressDereference);}
            base.Visit(addressDereference);
        }

        public override void Visit(IAnonymousDelegate anonymousDelegate)
        {if(Process(anonymousDelegate)){visitor.Visit(anonymousDelegate);}
            base.Visit(anonymousDelegate);
        }

        public override void Visit(IAddressOf addressOf)
        {if(Process(addressOf)){visitor.Visit(addressOf);}
            base.Visit(addressOf);
        }

        public override void Visit(IArrayIndexer arrayIndexer)
        {if(Process(arrayIndexer)){visitor.Visit(arrayIndexer);}
            base.Visit(arrayIndexer);
        }

        public override void Visit(IAssertStatement assertStatement)
        {if(Process(assertStatement)){visitor.Visit(assertStatement);}
            base.Visit(assertStatement);
        }

        public override void Visit(IAssignment assignment)
        {if(Process(assignment)){visitor.Visit(assignment);}
            base.Visit(assignment);
        }

        public override void Visit(IBitwiseAnd bitwiseAnd)
        {if(Process(bitwiseAnd)){visitor.Visit(bitwiseAnd);}
            base.Visit(bitwiseAnd);
        }

        public override void Visit(IAssumeStatement assumeStatement)
        {if(Process(assumeStatement)){visitor.Visit(assumeStatement);}
            base.Visit(assumeStatement);
        }

        public override void Visit(IBitwiseOr bitwiseOr)
        {if(Process(bitwiseOr)){visitor.Visit(bitwiseOr);}
            base.Visit(bitwiseOr);
        }

        public override void Visit(IBinaryOperation binaryOperation)
        {if(Process(binaryOperation)){visitor.Visit(binaryOperation);}
            base.Visit(binaryOperation);
        }

        public override void Visit(IBlockExpression blockExpression)
        {if(Process(blockExpression)){visitor.Visit(blockExpression);}
            base.Visit(blockExpression);
        }

        public override void Visit(IBreakStatement breakStatement)
        {if(Process(breakStatement)){visitor.Visit(breakStatement);}
            base.Visit(breakStatement);
        }

        public override void Visit(IBlockStatement block)
        {if(Process(block)){visitor.Visit(block);}
            base.Visit(block);
        }

        public override void Visit(ICastIfPossible castIfPossible)
        {if(Process(castIfPossible)){visitor.Visit(castIfPossible);}
            base.Visit(castIfPossible);
        }

        public override void Visit(ICatchClause catchClause)
        {if(Process(catchClause)){visitor.Visit(catchClause);}
            base.Visit(catchClause);
        }

        public override void Visit(ICheckIfInstance checkIfInstance)
        {if(Process(checkIfInstance)){visitor.Visit(checkIfInstance);}
            base.Visit(checkIfInstance);
        }

        public override void Visit(ICompileTimeConstant constant)
        {if(Process(constant)){visitor.Visit(constant);}
            base.Visit(constant);
        }

        public override void Visit(IConversion conversion)
        {if(Process(conversion)){visitor.Visit(conversion);}
            base.Visit(conversion);
        }

        public override void Visit(IConditionalStatement conditionalStatement)
        {if(Process(conditionalStatement)){visitor.Visit(conditionalStatement);}
            base.Visit(conditionalStatement);
        }

        public override void Visit(IConditional conditional)
        {if(Process(conditional)){visitor.Visit(conditional);}
            base.Visit(conditional);
        }

        public override void Visit(IContinueStatement continueStatement)
        {if(Process(continueStatement)){visitor.Visit(continueStatement);}
            base.Visit(continueStatement);
        }

        public override void Visit(ICopyMemoryStatement copyMemoryStatement)
        {if(Process(copyMemoryStatement)){visitor.Visit(copyMemoryStatement);}
            base.Visit(copyMemoryStatement);
        }

        public override void Visit(ICreateArray createArray)
        {if(Process(createArray)){visitor.Visit(createArray);}
            base.Visit(createArray);
        }

        public override void Visit(ICreateDelegateInstance createDelegateInstance)
        {if(Process(createDelegateInstance)){visitor.Visit(createDelegateInstance);}
            base.Visit(createDelegateInstance);
        }

        public override void Visit(IDefaultValue defaultValue)
        {if(Process(defaultValue)){visitor.Visit(defaultValue);}
            base.Visit(defaultValue);
        }

        public override void Visit(ICreateObjectInstance createObjectInstance)
        {if(Process(createObjectInstance)){visitor.Visit(createObjectInstance);}
            base.Visit(createObjectInstance);
        }

        public override void Visit(IDivision division)
        {if(Process(division)){visitor.Visit(division);}
            base.Visit(division);
        }

        public override void Visit(IDoUntilStatement doUntilStatement)
        {if(Process(doUntilStatement)){visitor.Visit(doUntilStatement);}
            base.Visit(doUntilStatement);
        }

        public override void Visit(IDupValue dupValue)
        {if(Process(dupValue)){visitor.Visit(dupValue);}
            base.Visit(dupValue);
        }

        public override void Visit(IEquality equality)
        {if(Process(equality)){visitor.Visit(equality);}
            base.Visit(equality);
        }

        public override void Visit(IExclusiveOr exclusiveOr)
        {if(Process(exclusiveOr)){visitor.Visit(exclusiveOr);}
            base.Visit(exclusiveOr);
        }

        public override void Visit(IEmptyStatement emptyStatement)
        {if(Process(emptyStatement)){visitor.Visit(emptyStatement);}
            base.Visit(emptyStatement);
        }

        public override void Visit(IDebuggerBreakStatement debuggerBreakStatement)
        {if(Process(debuggerBreakStatement)){visitor.Visit(debuggerBreakStatement);}
            base.Visit(debuggerBreakStatement);
        }

        public override void Visit(IBoundExpression boundExpression)
        {if(Process(boundExpression)){visitor.Visit(boundExpression);}
            base.Visit(boundExpression);
        }

        public override void Visit(IExpression expression)
        {if(Process(expression)){visitor.Visit(expression);}
            base.Visit(expression);
        }

        public override void Visit(IExpressionStatement expressionStatement)
        {if(Process(expressionStatement)){visitor.Visit(expressionStatement);}
            base.Visit(expressionStatement);
        }

        public override void Visit(IFillMemoryStatement fillMemoryStatement)
        {if(Process(fillMemoryStatement)){visitor.Visit(fillMemoryStatement);}
            base.Visit(fillMemoryStatement);
        }

        public override void Visit(IForEachStatement forEachStatement)
        {if(Process(forEachStatement)){visitor.Visit(forEachStatement);}
            base.Visit(forEachStatement);
        }

        public override void Visit(IForStatement forStatement)
        {if(Process(forStatement)){visitor.Visit(forStatement);}
            base.Visit(forStatement);
        }

        public override void Visit(IGetTypeOfTypedReference getTypeOfTypedReference)
        {if(Process(getTypeOfTypedReference)){visitor.Visit(getTypeOfTypedReference);}
            base.Visit(getTypeOfTypedReference);
        }

        public override void Visit(IGotoStatement gotoStatement)
        {if(Process(gotoStatement)){visitor.Visit(gotoStatement);}
            base.Visit(gotoStatement);
        }

        public override void Visit(IGetValueOfTypedReference getValueOfTypedReference)
        {if(Process(getValueOfTypedReference)){visitor.Visit(getValueOfTypedReference);}
            base.Visit(getValueOfTypedReference);
        }

        public override void Visit(IGreaterThan greaterThan)
        {if(Process(greaterThan)){visitor.Visit(greaterThan);}
            base.Visit(greaterThan);
        }

        public override void Visit(IGotoSwitchCaseStatement gotoSwitchCaseStatement)
        {if(Process(gotoSwitchCaseStatement)){visitor.Visit(gotoSwitchCaseStatement);}
            base.Visit(gotoSwitchCaseStatement);
        }

        public override void Visit(ILabeledStatement labeledStatement)
        {if(Process(labeledStatement)){visitor.Visit(labeledStatement);}
            base.Visit(labeledStatement);
        }

        public override void Visit(IGreaterThanOrEqual greaterThanOrEqual)
        {if(Process(greaterThanOrEqual)){visitor.Visit(greaterThanOrEqual);}
            base.Visit(greaterThanOrEqual);
        }

        public override void Visit(ILessThan lessThan)
        {if(Process(lessThan)){visitor.Visit(lessThan);}
            base.Visit(lessThan);
        }

        public override void Visit(ILeftShift leftShift)
        {if(Process(leftShift)){visitor.Visit(leftShift);}
            base.Visit(leftShift);
        }

        public override void Visit(ILessThanOrEqual lessThanOrEqual)
        {if(Process(lessThanOrEqual)){visitor.Visit(lessThanOrEqual);}
            base.Visit(lessThanOrEqual);
        }

        public override void Visit(ILocalDeclarationStatement localDeclarationStatement)
        {if(Process(localDeclarationStatement)){visitor.Visit(localDeclarationStatement);}
            base.Visit(localDeclarationStatement);
        }

        public override void Visit(ILogicalNot logicalNot)
        {if(Process(logicalNot)){visitor.Visit(logicalNot);}
            base.Visit(logicalNot);
        }

        public override void Visit(ILockStatement lockStatement)
        {if(Process(lockStatement)){visitor.Visit(lockStatement);}
            base.Visit(lockStatement);
        }

        public override void Visit(IModulus modulus)
        {if(Process(modulus)){visitor.Visit(modulus);}
            base.Visit(modulus);
        }

        public override void Visit(IMethodCall methodCall)
        {if(Process(methodCall)){visitor.Visit(methodCall);}
            base.Visit(methodCall);
        }

        public override void Visit(IMakeTypedReference makeTypedReference)
        {if(Process(makeTypedReference)){visitor.Visit(makeTypedReference);}
            base.Visit(makeTypedReference);
        }

        public override void Visit(IMultiplication multiplication)
        {if(Process(multiplication)){visitor.Visit(multiplication);}
            base.Visit(multiplication);
        }

        public override void Visit(INamedArgument namedArgument)
        {if(Process(namedArgument)){visitor.Visit(namedArgument);}
            base.Visit(namedArgument);
        }

        public override void Visit(INotEquality notEquality)
        {if(Process(notEquality)){visitor.Visit(notEquality);}
            base.Visit(notEquality);
        }

        public override void Visit(IOldValue oldValue)
        {if(Process(oldValue)){visitor.Visit(oldValue);}
            base.Visit(oldValue);
        }

        public override void Visit(IOutArgument outArgument)
        {if(Process(outArgument)){visitor.Visit(outArgument);}
            base.Visit(outArgument);
        }

        public override void Visit(IOnesComplement onesComplement)
        {if(Process(onesComplement)){visitor.Visit(onesComplement);}
            base.Visit(onesComplement);
        }

        public override void Visit(IPopValue popValue)
        {if(Process(popValue)){visitor.Visit(popValue);}
            base.Visit(popValue);
        }

        public override void Visit(IPointerCall pointerCall)
        {if(Process(pointerCall)){visitor.Visit(pointerCall);}
            base.Visit(pointerCall);
        }

        public override void Visit(IPushStatement pushStatement)
        {if(Process(pushStatement)){visitor.Visit(pushStatement);}
            base.Visit(pushStatement);
        }

        public override void Visit(IResourceUseStatement resourceUseStatement)
        {if(Process(resourceUseStatement)){visitor.Visit(resourceUseStatement);}
            base.Visit(resourceUseStatement);
        }

        public override void Visit(IRefArgument refArgument)
        {if(Process(refArgument)){visitor.Visit(refArgument);}
            base.Visit(refArgument);
        }

        public override void Visit(IRethrowStatement rethrowStatement)
        {if(Process(rethrowStatement)){visitor.Visit(rethrowStatement);}
            base.Visit(rethrowStatement);
        }

        public override void Visit(IReturnStatement returnStatement)
        {if(Process(returnStatement)){visitor.Visit(returnStatement);}
            base.Visit(returnStatement);
        }

        public override void Visit(IReturnValue returnValue)
        {if(Process(returnValue)){visitor.Visit(returnValue);}
            base.Visit(returnValue);
        }

        public override void Visit(IStackArrayCreate stackArrayCreate)
        {if(Process(stackArrayCreate)){visitor.Visit(stackArrayCreate);}
            base.Visit(stackArrayCreate);
        }

        public override void Visit(IRightShift rightShift)
        {if(Process(rightShift)){visitor.Visit(rightShift);}
            base.Visit(rightShift);
        }

        public override void Visit(IRuntimeArgumentHandleExpression runtimeArgumentHandleExpression)
        {if(Process(runtimeArgumentHandleExpression)){visitor.Visit(runtimeArgumentHandleExpression);}
            base.Visit(runtimeArgumentHandleExpression);
        }

        public override void Visit(ISwitchCase switchCase)
        {if(Process(switchCase)){visitor.Visit(switchCase);}
            base.Visit(switchCase);
        }

        public override void Visit(ISwitchStatement switchStatement)
        {if(Process(switchStatement)){visitor.Visit(switchStatement);}
            base.Visit(switchStatement);
        }

        public override void Visit(ISizeOf sizeOf)
        {if(Process(sizeOf)){visitor.Visit(sizeOf);}
            base.Visit(sizeOf);
        }

        public override void Visit(IStatement statement)
        {if(Process(statement)){visitor.Visit(statement);}
            base.Visit(statement);
        }

        public override void Visit(ISubtraction subtraction)
        {if(Process(subtraction)){visitor.Visit(subtraction);}
            base.Visit(subtraction);
        }

        public override void Visit(ITargetExpression targetExpression)
        {if(Process(targetExpression)){visitor.Visit(targetExpression);}
            base.Visit(targetExpression);
        }

        public override void Visit(IThisReference thisReference)
        {if(Process(thisReference)){visitor.Visit(thisReference);}
            base.Visit(thisReference);
        }

        public override void Visit(IThrowStatement throwStatement)
        {if(Process(throwStatement)){visitor.Visit(throwStatement);}
            base.Visit(throwStatement);
        }

        public override void Visit(ITryCatchFinallyStatement tryCatchFilterFinallyStatement)
        {if(Process(tryCatchFilterFinallyStatement)){visitor.Visit(tryCatchFilterFinallyStatement);}
            base.Visit(tryCatchFilterFinallyStatement);
        }

        public override void Visit(ITokenOf tokenOf)
        {if(Process(tokenOf)){visitor.Visit(tokenOf);}
            base.Visit(tokenOf);
        }

        public override void Visit(ITypeOf typeOf)
        {if(Process(typeOf)){visitor.Visit(typeOf);}
            base.Visit(typeOf);
        }

        public override void Visit(IUnaryNegation unaryNegation)
        {if(Process(unaryNegation)){visitor.Visit(unaryNegation);}
            base.Visit(unaryNegation);
        }

        public override void Visit(IUnaryOperation unaryOperation)
        {if(Process(unaryOperation)){visitor.Visit(unaryOperation);}
            base.Visit(unaryOperation);
        }

        public override void Visit(IUnaryPlus unaryPlus)
        {if(Process(unaryPlus)){visitor.Visit(unaryPlus);}
            base.Visit(unaryPlus);
        }

        public override void Visit(IVectorLength vectorLength)
        {if(Process(vectorLength)){visitor.Visit(vectorLength);}
            base.Visit(vectorLength);
        }

        public override void Visit(IWhileDoStatement whileDoStatement)
        {if(Process(whileDoStatement)){visitor.Visit(whileDoStatement);}
            base.Visit(whileDoStatement);
        }

        public override void Visit(IYieldBreakStatement yieldBreakStatement)
        {if(Process(yieldBreakStatement)){visitor.Visit(yieldBreakStatement);}
            base.Visit(yieldBreakStatement);
        }

        public override void Visit(IYieldReturnStatement yieldReturnStatement)
        {if(Process(yieldReturnStatement)){visitor.Visit(yieldReturnStatement);}
            base.Visit(yieldReturnStatement);
        }

        public override void Visit(IAliasForType aliasForType)
        {if(Process(aliasForType)){visitor.Visit(aliasForType);}
            base.Visit(aliasForType);
        }

        public override void Visit(IArrayTypeReference arrayTypeReference)
        {if(Process(arrayTypeReference)){visitor.Visit(arrayTypeReference);}
            base.Visit(arrayTypeReference);
        }

        public override void Visit(IAssembly assembly)
        {if(Process(assembly)){visitor.Visit(assembly);}
            base.Visit(assembly);
        }

        public override void Visit(IAssemblyReference assemblyReference)
        {if(Process(assemblyReference)){visitor.Visit(assemblyReference);}
            base.Visit(assemblyReference);
        }

        public override void Visit(ICustomAttribute customAttribute)
        {if(Process(customAttribute)){visitor.Visit(customAttribute);}
            base.Visit(customAttribute);
        }

        public override void Visit(ICustomModifier customModifier)
        {if(Process(customModifier)){visitor.Visit(customModifier);}
            base.Visit(customModifier);
        }

        public override void Visit(IEventDefinition eventDefinition)
        {if(Process(eventDefinition)){visitor.Visit(eventDefinition);}
            base.Visit(eventDefinition);
        }

        public override void Visit(IFieldDefinition fieldDefinition)
        {if(Process(fieldDefinition)){visitor.Visit(fieldDefinition);}
            base.Visit(fieldDefinition);
        }

        public override void Visit(IFieldReference fieldReference)
        {if(Process(fieldReference)){visitor.Visit(fieldReference);}
            base.Visit(fieldReference);
        }

        public override void Visit(IFileReference fileReference)
        {if(Process(fileReference)){visitor.Visit(fileReference);}
            base.Visit(fileReference);
        }

        public override void Visit(IFunctionPointerTypeReference functionPointerTypeReference)
        {if(Process(functionPointerTypeReference)){visitor.Visit(functionPointerTypeReference);}
            base.Visit(functionPointerTypeReference);
        }

        public override void Visit(IGenericMethodInstanceReference genericMethodInstanceReference)
        {if(Process(genericMethodInstanceReference)){visitor.Visit(genericMethodInstanceReference);}
            base.Visit(genericMethodInstanceReference);
        }

        public override void Visit(IGenericMethodParameter genericMethodParameter)
        {if(Process(genericMethodParameter)){visitor.Visit(genericMethodParameter);}
            base.Visit(genericMethodParameter);
        }

        public override void Visit(IGenericMethodParameterReference genericMethodParameterReference)
        {if(Process(genericMethodParameterReference)){visitor.Visit(genericMethodParameterReference);}
            base.Visit(genericMethodParameterReference);
        }

        public override void Visit(IGenericParameter genericParameter)
        {if(Process(genericParameter)){visitor.Visit(genericParameter);}
            base.Visit(genericParameter);
        }

        public override void Visit(IGenericParameterReference genericParameterReference)
        {if(Process(genericParameterReference)){visitor.Visit(genericParameterReference);}
            base.Visit(genericParameterReference);
        }

        public override void Visit(IGenericTypeInstanceReference genericTypeInstanceReference)
        {if(Process(genericTypeInstanceReference)){visitor.Visit(genericTypeInstanceReference);}
            base.Visit(genericTypeInstanceReference);
        }

        public override void Visit(IGenericTypeParameter genericTypeParameter)
        {if(Process(genericTypeParameter)){visitor.Visit(genericTypeParameter);}
            base.Visit(genericTypeParameter);
        }

        public override void Visit(IGenericTypeParameterReference genericTypeParameterReference)
        {if(Process(genericTypeParameterReference)){visitor.Visit(genericTypeParameterReference);}
            base.Visit(genericTypeParameterReference);
        }

        public override void Visit(IGlobalFieldDefinition globalFieldDefinition)
        {if(Process(globalFieldDefinition)){visitor.Visit(globalFieldDefinition);}
            base.Visit(globalFieldDefinition);
        }

        public override void Visit(IGlobalMethodDefinition globalMethodDefinition)
        {if(Process(globalMethodDefinition)){visitor.Visit(globalMethodDefinition);}
            base.Visit(globalMethodDefinition);
        }

        public override void Visit(ILocalDefinition localDefinition)
        {if(Process(localDefinition)){visitor.Visit(localDefinition);}
            base.Visit(localDefinition);
        }

        public override void VisitReference(ILocalDefinition localDefinition)
        {if(Process(localDefinition)){visitor.Visit(localDefinition);}
            base.VisitReference(localDefinition);
        }

        public override void Visit(IManagedPointerTypeReference managedPointerTypeReference)
        {if(Process(managedPointerTypeReference)){visitor.Visit(managedPointerTypeReference);}
            base.Visit(managedPointerTypeReference);
        }

        public override void Visit(IMarshallingInformation marshallingInformation)
        {if(Process(marshallingInformation)){visitor.Visit(marshallingInformation);}
            base.Visit(marshallingInformation);
        }

        public override void Visit(IMetadataConstant constant)
        {if(Process(constant)){visitor.Visit(constant);}
            base.Visit(constant);
        }

        public override void Visit(IMetadataCreateArray createArray)
        {if(Process(createArray)){visitor.Visit(createArray);}
            base.Visit(createArray);
        }

        public override void Visit(IMetadataExpression expression)
        {if(Process(expression)){visitor.Visit(expression);}
            base.Visit(expression);
        }

        public override void Visit(IMetadataNamedArgument namedArgument)
        {if(Process(namedArgument)){visitor.Visit(namedArgument);}
            base.Visit(namedArgument);
        }

        public override void Visit(IMetadataTypeOf typeOf)
        {if(Process(typeOf)){visitor.Visit(typeOf);}
            base.Visit(typeOf);
        }

        public override void Visit(IMethodBody methodBody)
        {if(Process(methodBody)){visitor.Visit(methodBody);}
            base.Visit(methodBody);
        }

        public override void Visit(IMethodDefinition method)
        {if(Process(method)){visitor.Visit(method);}
            base.Visit(method);
        }

        public override void Visit(IMethodImplementation methodImplementation)
        {if(Process(methodImplementation)){visitor.Visit(methodImplementation);}
            base.Visit(methodImplementation);
        }

        public override void Visit(IMethodReference methodReference)
        {if(Process(methodReference)){visitor.Visit(methodReference);}
            base.Visit(methodReference);
        }

        public override void Visit(IModifiedTypeReference modifiedTypeReference)
        {if(Process(modifiedTypeReference)){visitor.Visit(modifiedTypeReference);}
            base.Visit(modifiedTypeReference);
        }

        public override void Visit(IModule module)
        {if(Process(module)){visitor.Visit(module);}
            base.Visit(module);
        }

        public override void Visit(IModuleReference moduleReference)
        {if(Process(moduleReference)){visitor.Visit(moduleReference);}
            base.Visit(moduleReference);
        }

        public override void Visit(INamedTypeDefinition namedTypeDefinition)
        {if(Process(namedTypeDefinition)){visitor.Visit(namedTypeDefinition);}
            base.Visit(namedTypeDefinition);
        }

        public override void Visit(INamedTypeReference namedTypeReference)
        {if(Process(namedTypeReference)){visitor.Visit(namedTypeReference);}
            base.Visit(namedTypeReference);
        }

        public override void Visit(INamespaceAliasForType namespaceAliasForType)
        {if(Process(namespaceAliasForType)){visitor.Visit(namespaceAliasForType);}
            base.Visit(namespaceAliasForType);
        }

        public override void Visit(INamespaceDefinition namespaceDefinition)
        {if(Process(namespaceDefinition)){visitor.Visit(namespaceDefinition);}
            base.Visit(namespaceDefinition);
        }

        public override void Visit(INamespaceMember namespaceMember)
        {if(Process(namespaceMember)){visitor.Visit(namespaceMember);}
            base.Visit(namespaceMember);
        }

        public override void Visit(INamespaceTypeDefinition namespaceTypeDefinition)
        {if(Process(namespaceTypeDefinition)){visitor.Visit(namespaceTypeDefinition);}
            base.Visit(namespaceTypeDefinition);
        }

        public override void Visit(INamespaceTypeReference namespaceTypeReference)
        {if(Process(namespaceTypeReference)){visitor.Visit(namespaceTypeReference);}
            base.Visit(namespaceTypeReference);
        }

        public override void Visit(INestedAliasForType nestedAliasForType)
        {if(Process(nestedAliasForType)){visitor.Visit(nestedAliasForType);}
            base.Visit(nestedAliasForType);
        }

        public override void Visit(INestedTypeDefinition nestedTypeDefinition)
        {if(Process(nestedTypeDefinition)){visitor.Visit(nestedTypeDefinition);}
            base.Visit(nestedTypeDefinition);
        }

        public override void Visit(INestedTypeReference nestedTypeReference)
        {if(Process(nestedTypeReference)){visitor.Visit(nestedTypeReference);}
            base.Visit(nestedTypeReference);
        }

        public override void Visit(INestedUnitNamespace nestedUnitNamespace)
        {if(Process(nestedUnitNamespace)){visitor.Visit(nestedUnitNamespace);}
            base.Visit(nestedUnitNamespace);
        }

        public override void Visit(INestedUnitNamespaceReference nestedUnitNamespaceReference)
        {if(Process(nestedUnitNamespaceReference)){visitor.Visit(nestedUnitNamespaceReference);}
            base.Visit(nestedUnitNamespaceReference);
        }

        public override void Visit(INestedUnitSetNamespace nestedUnitSetNamespace)
        {if(Process(nestedUnitSetNamespace)){visitor.Visit(nestedUnitSetNamespace);}
            base.Visit(nestedUnitSetNamespace);
        }

        public override void Visit(IOperation operation)
        {if(Process(operation)){visitor.Visit(operation);}
            base.Visit(operation);
        }

        public override void Visit(IOperationExceptionInformation operationExceptionInformation)
        {if(Process(operationExceptionInformation)){visitor.Visit(operationExceptionInformation);}
            base.Visit(operationExceptionInformation);
        }

        public override void Visit(IParameterDefinition parameterDefinition)
        {if(Process(parameterDefinition)){visitor.Visit(parameterDefinition);}
            base.Visit(parameterDefinition);
        }

        public override void VisitReference(IParameterDefinition parameterDefinition)
        {if(Process(parameterDefinition)){visitor.Visit(parameterDefinition);}
            base.VisitReference(parameterDefinition);
        }

        public override void Visit(IPropertyDefinition propertyDefinition)
        {if(Process(propertyDefinition)){visitor.Visit(propertyDefinition);}
            base.Visit(propertyDefinition);
        }

        public override void Visit(IParameterTypeInformation parameterTypeInformation)
        {if(Process(parameterTypeInformation)){visitor.Visit(parameterTypeInformation);}
            base.Visit(parameterTypeInformation);
        }

        public override void Visit(IPESection peSection)
        {if(Process(peSection)){visitor.Visit(peSection);}
            base.Visit(peSection);
        }

        public override void Visit(IPlatformInvokeInformation platformInvokeInformation)
        {if(Process(platformInvokeInformation)){visitor.Visit(platformInvokeInformation);}
            base.Visit(platformInvokeInformation);
        }

        public override void Visit(IPointerTypeReference pointerTypeReference)
        {if(Process(pointerTypeReference)){visitor.Visit(pointerTypeReference);}
            base.Visit(pointerTypeReference);
        }

        public override void Visit(IResourceReference resourceReference)
        {if(Process(resourceReference)){visitor.Visit(resourceReference);}
            base.Visit(resourceReference);
        }

        public override void Visit(IRootUnitNamespace rootUnitNamespace)
        {if(Process(rootUnitNamespace)){visitor.Visit(rootUnitNamespace);}
            base.Visit(rootUnitNamespace);
        }

        public override void Visit(IRootUnitNamespaceReference rootUnitNamespaceReference)
        {if(Process(rootUnitNamespaceReference)){visitor.Visit(rootUnitNamespaceReference);}
            base.Visit(rootUnitNamespaceReference);
        }

        public override void Visit(IRootUnitSetNamespace rootUnitSetNamespace)
        {if(Process(rootUnitSetNamespace)){visitor.Visit(rootUnitSetNamespace);}
            base.Visit(rootUnitSetNamespace);
        }

        public override void Visit(ISecurityAttribute securityAttribute)
        {if(Process(securityAttribute)){visitor.Visit(securityAttribute);}
            base.Visit(securityAttribute);
        }

        public override void Visit(ISpecializedEventDefinition specializedEventDefinition)
        {if(Process(specializedEventDefinition)){visitor.Visit(specializedEventDefinition);}
            base.Visit(specializedEventDefinition);
        }

        public override void Visit(ISpecializedFieldDefinition specializedFieldDefinition)
        {if(Process(specializedFieldDefinition)){visitor.Visit(specializedFieldDefinition);}
            base.Visit(specializedFieldDefinition);
        }

        public override void Visit(ISpecializedFieldReference specializedFieldReference)
        {if(Process(specializedFieldReference)){visitor.Visit(specializedFieldReference);}
            base.Visit(specializedFieldReference);
        }

        public override void Visit(ISpecializedMethodDefinition specializedMethodDefinition)
        {if(Process(specializedMethodDefinition)){visitor.Visit(specializedMethodDefinition);}
            base.Visit(specializedMethodDefinition);
        }

        public override void Visit(ISpecializedMethodReference specializedMethodReference)
        {if(Process(specializedMethodReference)){visitor.Visit(specializedMethodReference);}
            base.Visit(specializedMethodReference);
        }

        public override void Visit(ISpecializedPropertyDefinition specializedPropertyDefinition)
        {if(Process(specializedPropertyDefinition)){visitor.Visit(specializedPropertyDefinition);}
            base.Visit(specializedPropertyDefinition);
        }

        public override void Visit(ISpecializedNestedTypeDefinition specializedNestedTypeDefinition)
        {if(Process(specializedNestedTypeDefinition)){visitor.Visit(specializedNestedTypeDefinition);}
            base.Visit(specializedNestedTypeDefinition);
        }

        public override void Visit(ISpecializedNestedTypeReference specializedNestedTypeReference)
        {if(Process(specializedNestedTypeReference)){visitor.Visit(specializedNestedTypeReference);}
            base.Visit(specializedNestedTypeReference);
        }

        public override void Visit(ITypeDefinition typeDefinition)
        {if(Process(typeDefinition)){visitor.Visit(typeDefinition);}
            base.Visit(typeDefinition);
        }

        public override void Visit(ITypeDefinitionMember typeMember)
        {if(Process(typeMember)){visitor.Visit(typeMember);}
            base.Visit(typeMember);
        }

        public override void Visit(ITypeMemberReference typeMember)
        {if(Process(typeMember)){visitor.Visit(typeMember);}
            base.Visit(typeMember);
        }

        public override void Visit(ITypeReference typeReference)
        {if(Process(typeReference)){visitor.Visit(typeReference);}
            base.Visit(typeReference);
        }

        public override void Visit(IUnit unit)
        {if(Process(unit)){visitor.Visit(unit);}
            base.Visit(unit);
        }

        public override void Visit(IUnitReference unitReference)
        {if(Process(unitReference)){visitor.Visit(unitReference);}
            base.Visit(unitReference);
        }

        public override void Visit(IUnitNamespace unitNamespace)
        {if(Process(unitNamespace)){visitor.Visit(unitNamespace);}
            base.Visit(unitNamespace);
        }

        public override void Visit(IUnitNamespaceReference unitNamespaceReference)
        {if(Process(unitNamespaceReference)){visitor.Visit(unitNamespaceReference);}
            base.Visit(unitNamespaceReference);
        }

        public override void Visit(IUnitSet unitSet)
        {if(Process(unitSet)){visitor.Visit(unitSet);}
            base.Visit(unitSet);
        }

        public override void Visit(IUnitSetNamespace unitSetNamespace)
        {if(Process(unitSetNamespace)){visitor.Visit(unitSetNamespace);}
            base.Visit(unitSetNamespace);
        }

        public override void Visit(IWin32Resource win32Resource)
        {if(Process(win32Resource)){visitor.Visit(win32Resource);}
            base.Visit(win32Resource);
        }


       
    }
}