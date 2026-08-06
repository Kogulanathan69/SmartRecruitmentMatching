# Project reference and package requirements

The Team Lead should confirm these existing project references:

- `NexHire.Application` → `NexHire.Domain`
- `NexHire.Infrastructure` → `NexHire.Application`, `NexHire.Domain`
- `NexHire.API` → `NexHire.Application`, `NexHire.Infrastructure`

Infrastructure packages required for this implementation:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.*" />
<PackageReference Include="Microsoft.Extensions.Configuration.Binder" Version="9.0.*" />
```

Unit-test packages required in the team's existing test project:

```xml
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.*" />
<PackageReference Include="xunit" Version="2.*" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.*" />
```

Use the exact versions already selected by the Team Lead rather than introducing a conflicting version.
