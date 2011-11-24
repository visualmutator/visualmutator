namespace VisualMutator.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using CommonUtilityInfrastructure;

    public static class UserMessages
    {
       

public static string ErrorPretest_TestsFailed(string firstTestThatFailedFullName, string message)
{
    return @"One or more tests failed or were inconclusive on unmodified source assemblies. 
All tests must pass before starting mutation testing process. 
Mutation session cannot continue.

First test that failed: {0}
Message: {1}"
        .Formatted(firstTestThatFailedFullName, message);


}
public static string ErrorPretest_UnknownError(string exceptionMessage)
{
    return 
@"Error occurred while pre-testing source assemblies. Mutation session cannot continue.

Exception:
{0}"
        .Formatted(exceptionMessage);


}

public static string ErrorPretest_VerificationFailure(string exceptionMessage)
{
    return
@"Some of source project assemblies failed verification.
Mutant verification will be disabled, also errors may occur during testing and while decompiling code for preview.

Details:

{0}"
        .Formatted(exceptionMessage);


}

     
    }
}