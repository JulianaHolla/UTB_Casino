# UTB Casino

Casino simulation (virtual credits only) built with ASP.NET Core MVC, following the UTB project structure.

Run:

    dotnet run --project UTB.Casino.Web

Then open https://localhost:7100. Admin area: /Admin/Game/Select.

Notes:
- Data lives in memory (`DatabaseFake`) and resets on restart.
- There is no login: the first seeded player ("demo") is the current player.
- Requires the .NET 10 SDK. For an older SDK change `TargetFramework` in the .csproj.
- yo yo yo
