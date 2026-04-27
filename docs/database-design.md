# Database Design — Person 3

## ER Diagram (ASCII)

```
┌──────────────────┐
│    Department     │
├──────────────────┤
│ PK Id (uuid)     │
│    Name (varchar) │
└───────┬──────────┘
        │ 1
        │
        │ N
┌───────▼──────────────┐
│      Employee         │
├──────────────────────┤
│ PK Id (uuid)         │
│    FirstName (100)   │
│    LastName (100)    │
│    Email (255) UQ    │
│    IsActive (bool)   │
│ FK DepartmentId      │
└───┬──────────────────┘
    │ 1
    │
    ├─── N ──────────────────────┐
    │                            │
    │ 1                          │ 1
┌───▼───────────────────┐  ┌────▼──────────────────┐
│   PayrollStructure    │  │       Payroll          │
├───────────────────────┤  ├────────────────────────┤
│ PK Id (uuid)          │  │ PK Id (uuid)           │
│    BaseSalary (18,2)  │  │    Month (int)         │
│    Bonus (18,2)       │  │    Year (int)          │
│    Deductions (18,2)  │  │    BaseSalary (18,2)   │
│    IsActive (bool)    │  │    Bonus (18,2)        │
│ FK EmployeeId         │  │    Deductions (18,2)   │
└───────────────────────┘  │    TotalAmount (18,2)  │
                           │    Status (enum)       │
                           │ FK EmployeeId          │
                           └───┬────────────────────┘
                               │ 1
                               │
                   ┌───────────┤───────────┐
                   │ N                     │ N
           ┌───────▼────────┐   ┌──────────▼───────┐
           │    Approval     │   │     Payslip       │
           ├─────────────────┤   ├──────────────────┤
           │ PK Id (uuid)    │   │ PK Id (uuid)     │
           │    Status (enum)│   │    GeneratedAt   │
           │    ReviewedAt   │   │ FK PayrollId     │
           │ FK PayrollId    │   └──────────────────┘
           └─────────────────┘
```

## ER Diagram (Mermaid)

```mermaid
erDiagram
    Department {
        uuid Id PK
        varchar(200) Name
    }

    Employee {
        uuid Id PK
        varchar(100) FirstName
        varchar(100) LastName
        varchar(255) Email UK
        boolean IsActive
        uuid DepartmentId FK
    }

    PayrollStructure {
        uuid Id PK
        uuid EmployeeId FK
        numeric_18_2 BaseSalary
        numeric_18_2 Bonus
        numeric_18_2 Deductions
        boolean IsActive
    }

    Payroll {
        uuid Id PK
        uuid EmployeeId FK
        integer Month
        integer Year
        numeric_18_2 BaseSalary
        numeric_18_2 Bonus
        numeric_18_2 Deductions
        numeric_18_2 TotalAmount
        integer Status
    }

    Approval {
        uuid Id PK
        uuid PayrollId FK
        integer Status
        timestamptz ReviewedAt
    }

    Payslip {
        uuid Id PK
        uuid PayrollId FK
        timestamptz GeneratedAt
    }

    Department ||--o{ Employee : "has"
    Employee ||--o{ PayrollStructure : "has"
    Employee ||--o{ Payroll : "has"
    Payroll ||--o| Approval : "has"
    Payroll ||--o| Payslip : "generates"
```

## Relationships

| From | To | Type | FK | On Delete |
|---|---|---|---|---|
| Employee | Department | N:1 | DepartmentId | Restrict |
| PayrollStructure | Employee | N:1 | EmployeeId | Cascade |
| Payroll | Employee | N:1 | EmployeeId | Cascade |
| Approval | Payroll | N:1 | PayrollId | Cascade |
| Payslip | Payroll | N:1 | PayrollId | Cascade |

## Indexes

| Table | Column(s) | Type |
|---|---|---|
| Employee | Email | Unique |
| Payroll | EmployeeId, Month, Year | Unique (prevents duplicate payroll per month) |

## Column Types

- Monetary values: `numeric(18,2)`
- Primary keys: `uuid`
- Timestamps: `timestamp with time zone`
- Enums stored as: `integer`

## Enums

**PayrollStatus**: Draft (1), Approved (2)

**ApprovalStatus**: Pending (1), Approved (2), Rejected (3)
