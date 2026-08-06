# NexHire Member 5 — Interview, Evaluation & Offer Management

Branch: `feature/interview`

This package is an **overlay** for the existing NexHire folder structure. It contains only Member 5-owned code plus reviewed integration instructions. It does not overwrite Authentication, Applications, Matching, Company, Candidate, Admin, `Program.cs`, or the shared `ApplicationDbContext`.

## Implemented scope

- Schedule, list, view, reschedule, cancel, and complete interviews.
- Record interview score and evaluator feedback.
- Create, send, list, view, accept, reject, expire, and withdraw offers.
- Company and candidate ownership checks.
- Configurable application-status eligibility and role names.
- Stable Problem Details errors.
- EF Core repositories through a module-isolated `Member5DbContext`.
- Company and job-seeker HTML/JavaScript pages.
- SQL review script and unit-test source files.

## Important integration dependency

Member 4 must implement `IApplicationAccessReader`. The contract returns the application owner company, candidate profile, current status, job title, company name, and candidate name. A safe `NotConfiguredApplicationAccessReader` is included so the host starts, but Interview/Offer creation returns a dependency error until the real adapter is registered.

Member 1 / Team Lead must confirm JWT claim names used by `ClaimsCurrentActor`. Claim names are configurable through `Member5Claims`.

## Team Lead Program.cs additions

```csharp
builder.Services.AddMember5Api(builder.Configuration);
builder.Services.AddMember5InterviewOfferModule(builder.Configuration);
```

Required namespaces:

```csharp
using NexHire.API.Extensions;
using NexHire.Infrastructure;
```

## appsettings.json example

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=NexHire;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Member5Claims": {
    "UserIdClaim": "sub",
    "RoleClaim": "role",
    "CompanyIdClaim": "company_id",
    "CandidateProfileIdClaim": "candidate_profile_id"
  },
  "Member5Rules": {
    "MinimumInterviewScore": 0,
    "MaximumInterviewScore": 100,
    "MaximumPageSize": 100,
    "AllowedInterviewApplicationStatuses": [ "Shortlisted" ],
    "AllowedOfferApplicationStatuses": [ "InterviewCompleted", "Selected", "Shortlisted" ],
    "CompanyRoles": [ "Company", "Employer" ],
    "CandidateRoles": [ "JobSeeker", "Candidate" ]
  }
}
```

## EF Core

This package uses `Member5DbContext` to avoid editing the shared `ApplicationDbContext` without approval. The Team Lead may either:

1. Register `Member5DbContext` with the same database connection and create a dedicated migration, or
2. Move the three entity configurations and DbSets into the central `ApplicationDbContext` after architecture review.

The SQL review script is at `deployment/sql/member5-interview-offer.sql`.

## Copy and commit

Copy the contents inside this overlay directly into `Z:\Kogulanathan69\SmartRecruitmentMatching` while on `feature/interview`, then run:

```cmd
git status
git add .
git commit -m "feat(interview): implement interview and offer workflow"
git push origin feature/interview
```

Do not merge your own pull request. Base the pull request on `develop`.
