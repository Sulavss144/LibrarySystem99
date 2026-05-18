# LibraryKOI — Library Management System

A web-based library management system built with ASP.NET MVC 5, featuring role-based access for librarians and members, full borrowing workflow with automated fines, book reservations, reviews, and reporting.

Developed as a group project at KOI Institute, Sydney.

---

## Table of Contents

- [Tech Stack](#tech-stack)
- [Features](#features)
- [Screenshots](#screenshots)
- [Getting Started](#getting-started)
- [Test Credentials](#test-credentials)
- [Project Structure](#project-structure)
- [Team](#team)

---

## Tech Stack

- **Framework:** ASP.NET MVC 5 (.NET Framework 4.7.2)
- **Language:** C#
- **ORM:** Entity Framework 6 (Code-First with Migrations)
- **Authentication:** ASP.NET Identity (Individual User Accounts)
- **Database:** SQL Server Express LocalDB
- **Frontend:** Razor views, Bootstrap, Chart.js
- **IDE:** Visual Studio 2026

---

## Features

### Authentication & Authorization
- User registration with role selection (Member / Librarian)
- Login / Logout with "Remember Me" persistent cookies
- In-app password reset flow (no email server required)
- Role-based authorization on every protected page
- Auto-redirect to role-specific dashboard after login

### Books Management
- Full CRUD for librarians (create, read, update, delete)
- Browse catalogue with search by title or author
- Color-coded availability indicators
- Book details page with reviews and average ratings

### Borrowing Workflow
- Members borrow available books with one click
- Configurable loan period (default 14 days)
- Automatic policy enforcement: max books per user, unpaid-fine blocking
- Return flow with automatic late-fine calculation
- "My Borrowings" page with active and historical loans

### Reservations
- Members can reserve unavailable books and join a queue
- Queue auto-promotion: when a book is returned, the first member in queue is marked "Ready for Pickup"
- One-click conversion of a "Ready" reservation into a borrow
- Members and librarians can cancel reservations

### Fines
- Auto-generated when a book is returned after due date
- Configurable rate per day overdue
- Members view and pay their fines online
- Librarians manage all fines (create, edit, delete, mark paid)

### Borrowing Policy
- Single configurable global policy: max books per user, loan days, max renewals, fine per day
- Default policy seeded automatically on first run
- Changes apply to all new borrows immediately

### Member Management (Librarian)
- View all members with profile summary
- Drill into each member: borrowing history, fines, KPIs
- Edit member email and phone
- Delete members (blocked if they have active borrowings)

### User Profile
- Full profile with photo (URL), full name, date of birth, address, bio
- Edit own profile
- Change password
- Same profile page works for both Members and Librarians

### Reports & Analytics (Librarian)
Six reports on one page with Chart.js visualizations:
1. Overdue books (with member info and days overdue)
2. Most popular books (top 10 by borrow count)
3. Member activity (top borrowers + inactive members)
4. Fines summary (collected vs unpaid, by member)
5. Borrowing trends (last 14 days line chart + last 6 months bar chart)
6. Inventory status (low stock and fully borrowed books)

### Reviews
- Members leave 5-star reviews on any book
- Inline star widget with hover preview
- One review per user per book; edit / delete own reviews
- Average rating computed across all reviews
- Librarian moderation page (view all, delete inappropriate)

### Website Feedback
- Open feedback form on About page (anyone can submit, no login required)
- Public display of approved feedback on the same page
- Librarian moderation: hide/show, reply, permanently delete
- Replies appear publicly below the original feedback

### Admin Tools
- Reset library data (auto-return all borrows, optionally clear fines) — useful for demos
- Role seeding on first visit

---

## Screenshots



<!-- Example markdown for embedding images:
![Landing Page](docs/screenshots/landing.png)
![Librarian Dashboard](docs/screenshots/librarian-dashboard.png)
-->
---

## Getting Started

### Prerequisites

- Visual Studio 2022 or later (2026 used in development)
- .NET Framework 4.7.2 SDK
- SQL Server Express LocalDB (included with Visual Studio)

### Installation

1. **Clone the repository**
```bash
   git clone https://github.com/Sulavss144/LibrarySystem99.git
   cd LibrarySystem99
```

2. **Open the solution**
   - Open `LibrarySystem99.slnx` in Visual Studio.

3. **Restore NuGet packages**
   - Right-click the solution → "Restore NuGet Packages"
   - Or simply build the project (Ctrl+Shift+B) and packages will restore automatically.

4. **Apply database migrations**
   - Open **Tools → NuGet Package Manager → Package Manager Console**
   - Make sure the Default project is set to `LibrarySystem99`
   - Run:
```powershell
     Update-Database
```
   - This creates the LocalDB database and applies all migrations.

5. **Run the project**
   - Press **F5** (or click the Run button).
   - The app opens at `https://localhost:44331/` (or similar).

6. **First-time setup**
   - On first visit, the system automatically seeds the Librarian and Member roles, plus a default borrowing policy.
   - Register a new account using the **Register** button. Pick your role (Librarian or Member).

---

## Test Credentials

If you want pre-seeded users for testing, register these accounts after the first run:

| Email | Password | Role |
|---|---|---|
| `librarian@test.com` | `Pass@word1` | Librarian |
| `test@test.com` | `Pass@word1` | Member |
| `mem@test.com` | `Pass@word1` | Member |

> Note: Identity password policy requires at least one uppercase, one digit, and one special character.

---

## Project Structure

LibrarySystem99/
├── Controllers/
│   ├── AccountController.cs           # Login, register, password reset
│   ├── BooksController.cs             # Books CRUD + borrow/return
│   ├── BookReviewsController.cs       # Star ratings & reviews
│   ├── BorrowingPoliciesController.cs # Single global policy editor
│   ├── BorrowingTransactionsController.cs
│   ├── FeedbackController.cs          # Website feedback moderation
│   ├── FinesController.cs             # Fines management
│   ├── HomeController.cs              # Landing page + role redirect
│   ├── LibrarianController.cs         # Librarian dashboard
│   ├── ManageController.cs            # Profile + account settings
│   ├── MemberController.cs            # Member dashboard
│   ├── MembersController.cs           # Librarian member management
│   ├── ReportsController.cs           # Analytics & reports
│   └── ReservationsController.cs      # Reservation queue
├── Models/
│   ├── ApplicationUser (IdentityModels.cs)
│   ├── Book.cs
│   ├── BookReview.cs
│   ├── BorrowingPolicy.cs
│   ├── BorrowingTransaction.cs
│   ├── Fine.cs
│   ├── Reservation.cs
│   ├── WebsiteFeedback.cs
│   ├── ReportsViewModel.cs
│   └── ViewModels (Account, Manage, ...)
├── Views/
│   ├── Books/                         # Books CRUD + MyBooks
│   ├── BookReviews/
│   ├── BorrowingPolicies/
│   ├── Feedback/
│   ├── Home/                          # Landing, About, Contact
│   ├── Librarian/                     # Librarian dashboard
│   ├── Manage/                        # Profile + account settings
│   ├── Member/                        # Member dashboard
│   ├── Members/                       # Librarian member management
│   ├── Reports/
│   ├── Reservations/
│   └── Shared/                        # _Layout, _BookReviews, _LoginPartial
├── Migrations/                        # EF Code-First migrations
├── App_Data/                          # LocalDB .mdf files
├── Content/                           # CSS, images
└── Scripts/                           # JavaScript libraries


---

## Team

| Name | Student ID |
|---|---|
| Sulav | 20032639 |
| Ashif | 20032206 |
| Ehsan | 20032627 |
| Manish | 20030726 |

---

## License

This project was created for academic purposes at KOI Institute. Not licensed for commercial use.

---

## Acknowledgements

- ASP.NET Identity scaffolding for the authentication foundation
- Bootstrap for responsive UI components
- Chart.js for the analytics visualizations on the Reports page
