# Database Migration Troubleshooting Guide

## Problem Statement

**Error**: Database migration generation fails with DI resolution error when executing:

```bash
dotnet ef migrations add "Phase3AdvancedFeatures"
```

**Full Error Message**:

```
Build succeeded.
An error occurred while accessing the Microsoft.Extensions.Hosting services.
Continuing without the application service provider...
Unable to create a 'DbContext' of type 'Chatbot.API.Data.ChatbotDbContext'.
The exception... was thrown while attempting to create an instance.
```

---

## 🔍 Root Cause Analysis

### Why It Fails

1. **Program.cs DI Setup**: All Phase 3 repositories are registered during initialization
2. **Repository Dependencies**: Repositories require `ChatbotDbContext` (IServiceProvider)
3. **Design-Time Context**: EF Core migrations need DbContext without full DI bootstrap
4. **Factory Ignored**: `ChatbotDbContextFactory.cs` created but not recognized by EF Core

### Why It's Blocking

- Cannot create migration file without resolving DbContext
- Cannot apply schema to database
- Cannot test repository layer functionality
- Production deployment requires migrations

---

## ✅ Solution Approaches (In Order of Preference)

### **APPROACH 1: Modify Program.cs DI (RECOMMENDED)**

**Concept**: Skip repository registration during design-time

**Implementation**:

```csharp
// In Program.cs, wrap Phase 3 registrations:
if (!System.Diagnostics.Process.GetCurrentProcess().ProcessName.Contains("dotnet"))
{
    // Only register in runtime, not during design-time
    // Register Phase 3 repositories
    builder.Services.AddScoped<IConversationAnalyticsRepository, ConversationAnalyticsRepository>();
    builder.Services.AddScoped<IMLInsightRepository, MLInsightRepository>();
    builder.Services.AddScoped<IWorkflowRepository, WorkflowRepository>();
    builder.Services.AddScoped<IUserSegmentRepository, UserSegmentRepository>();
    builder.Services.AddScoped<ISearchIndexRepository, SearchIndexRepository>();

    // Register Phase 3 services
    builder.Services.AddScoped<IConversationAnalyticsService, ConversationAnalyticsService>();
    builder.Services.AddScoped<IMLInsightService, MLInsightService>();
    builder.Services.AddScoped<IWorkflowService, WorkflowService>();
    builder.Services.AddScoped<IUserSegmentationService, UserSegmentationService>();
    builder.Services.AddScoped<ISearchService, SearchService>();
    builder.Services.AddScoped<IAnalyticsExportService, AnalyticsExportService>();
}
```

**Try This**:

```bash
cd Chatbot.API
dotnet ef migrations add "Phase3AdvancedFeatures"
```

---

### **APPROACH 2: Explicit DbContextFactory Specification**

**Concept**: Tell EF Core exactly where to find design-time factory

**Verification** (First, verify ChatbotDbContextFactory exists):

```bash
# Check for factory file
Get-ChildItem -Path "Chatbot.API\Data" -Recurse -Filter "*Factory*"
```

**Try This**:

```bash
cd Chatbot.API

# Explicitly specify factory assembly
dotnet ef migrations add "Phase3AdvancedFeatures" `
    --context "Chatbot.API.Data.ChatbotDbContext" `
    --project "." `
    --startup-project "."
```

---

### **APPROACH 3: Use Minimal DbContext (Temporary Workaround)**

**Concept**: Create a minimal DbContext without dependencies

**Create file**: `Chatbot.API/Data/MigrationDbContext.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Chatbot.API.Data;

public class MigrationDbContext : ChatbotDbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=chatbot.db");
        }
    }
}
```

**Try This**:

```bash
dotnet ef migrations add "Phase3AdvancedFeatures" --context "Chatbot.API.Data.MigrationDbContext"
```

---

### **APPROACH 4: Manual Migration Creation (If All Else Fails)**

**Concept**: Create migration file manually without EF Core generation

**Steps**:

1. **Create file**: `Chatbot.API/Migrations/20260213000000_Phase3AdvancedFeatures.cs`

```csharp
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chatbot.API.Migrations
{
    /// <inheritdoc />
    public partial class Phase3AdvancedFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create ConversationAnalytics table
            migrationBuilder.CreateTable(
                name: "ConversationAnalytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ConversationId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    AverageSentiment = table.Column<double>(type: "REAL", nullable: false),
                    EngagementScore = table.Column<double>(type: "REAL", nullable: false),
                    ResponseCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationAnalytics", x => x.Id);
                });

            // Create MLInsight table
            migrationBuilder.CreateTable(
                name: "MLInsights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    ConversationId = table.Column<int>(type: "INTEGER", nullable: false),
                    InsightValue = table.Column<string>(type: "TEXT", nullable: false),
                    Confidence = table.Column<double>(type: "REAL", nullable: false),
                    InsightType = table.Column<int>(type: "INTEGER", nullable: false),
                    IsReviewed = table.Column<bool>(type: "INTEGER", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MLInsights", x => x.Id);
                });

            // Create Workflow, UserSegment, SearchIndex tables similarly...
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ConversationAnalytics");
            migrationBuilder.DropTable(name: "MLInsights");
            // Drop other tables...
        }
    }
}
```

2. **Create designer file**: `Chatbot.API/Migrations/20260213000000_Phase3AdvancedFeatures.Designer.cs`

3. **Apply migration**:

```bash
dotnet ef database update
```

---

## 🧪 Testing the Solution

### After Successfully Creating Migration

```bash
# 1. Verify migration file exists
Get-ChildItem "Chatbot.API\Migrations" -Filter "*Phase3*"

# 2. Build project
dotnet build

# 3. Apply to database
dotnet ef database update

# 4. Verify database schema
# Open chatbot.db with SQLite viewer and check for Phase 3 tables
```

### Verification Queries (in SQLite)

```sql
-- Verify tables created
SELECT name FROM sqlite_master WHERE type='table' AND name LIKE '%Analytics%';
SELECT name FROM sqlite_master WHERE type='table' AND name LIKE '%MLInsight%';
SELECT name FROM sqlite_master WHERE type='table' AND name LIKE '%Workflow%';
SELECT name FROM sqlite_master WHERE type='table' AND name LIKE '%UserSegment%';
SELECT name FROM sqlite_master WHERE type='table' AND name LIKE '%SearchIndex%';

-- Check migration history
SELECT name FROM sqlite_master WHERE type='table' AND name='__EFMigrationsHistory';
SELECT MigrationId FROM __EFMigrationsHistory ORDER BY MigrationId DESC;
```

---

## 📋 Checklist When Migration Succeeds

- [ ] Migration file created in `Chatbot.API/Migrations/`
- [ ] Migration file name follows pattern: `YYYYMMDDHHMMSS_Phase3AdvancedFeatures.cs`
- [ ] Designer file created: `YYYYMMDDHHMMSS_Phase3AdvancedFeatures.Designer.cs`
- [ ] `DbContextModelSnapshot.cs` updated with Phase 3 entities
- [ ] `dotnet build` returns 0 errors
- [ ] `dotnet ef database update` completes successfully
- [ ] `chatbot.db` file updated with new tables
- [ ] Service layer can now access repositories with real data

---

## 🔄 Next Steps After Migration Success

1. **Replace Service TODOs** with actual repository calls
2. **Implement userId Extraction** in API controllers (25+ locations)
3. **Update Frontend** to use real HTTP calls instead of mock data
4. **Run Integration Tests** against real database
5. **Verify Data Flow** end-to-end from API → Database → Frontend

---

## ⚠️ Common Errors & Fixes

### Error: "No DbContext found"

**Solution**: Ensure `ChatbotDbContext` is registered in DI or use `--context` parameter

### Error: "Migration already exists"

**Solution**: Use different migration name (add timestamp, e.g., `Phase3AdvancedFeatures_v2`)

### Error: "Schema invalid"

**Solution**: Check entity property names match in Phase3Entities.cs and DbContext

### Error: "Connection string not found"

**Solution**: Verify `appsettings.json` has connection string or factory provides hardcoded value

---

## 📞 Quick Command Reference

```bash
# Check for design-time factory
Get-ChildItem -Recurse -Filter "*Factory*" Chatbot.API/Data/

# List existing migrations
dotnet ef migrations list

# Create new migration
dotnet ef migrations add "Phase3AdvancedFeatures"

# Apply migration to database
dotnet ef database update

# Rollback migration
dotnet ef database update "20260212173801_AddUserPreferencesAndNewFeatures"

# Remove last migration (if not yet applied)
dotnet ef migrations remove

# View SQL that migration generates
dotnet ef migrations script
```

---

_Last Updated: February 13, 2026 - Blocking Issue Resolution Guide_
