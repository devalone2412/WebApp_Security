using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace WebApp_UnderTheHood;

public class HRManagerProbationRequirement : IAuthorizationRequirement
{
    public HRManagerProbationRequirement(int probationMonth)
    {
        ProbationMonth = probationMonth;
    }

    public int ProbationMonth { get; }
}

public class HRManagerProbationRequirementHandler : AuthorizationHandler<HRManagerProbationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HRManagerProbationRequirement requirement)
    {
        if (!context.User.HasClaim(x => x.Type == "EmploymentDate"))
        {
            return Task.CompletedTask;
        }

        if (DateTime.TryParse(context.User.FindFirst(x => x.Type == "EmploymentDate")?.Value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime employmentDate))
        {
            var period = DateTime.Now - employmentDate;
            if (period.Days > 30 * requirement.ProbationMonth)
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}
