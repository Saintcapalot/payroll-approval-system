FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY PayrollApprovalSystem.sln .
COPY src/PayrollApprovalSystem.Domain/PayrollApprovalSystem.Domain.csproj src/PayrollApprovalSystem.Domain/
COPY src/PayrollApprovalSystem.Application/PayrollApprovalSystem.Application.csproj src/PayrollApprovalSystem.Application/
COPY src/PayrollApprovalSystem.Infrastructure/PayrollApprovalSystem.Infrastructure.csproj src/PayrollApprovalSystem.Infrastructure/
COPY src/PayrollApprovalSystem.Api/PayrollApprovalSystem.Api.csproj src/PayrollApprovalSystem.Api/
RUN dotnet restore

COPY . .
RUN dotnet publish src/PayrollApprovalSystem.Api/PayrollApprovalSystem.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "PayrollApprovalSystem.Api.dll"]
