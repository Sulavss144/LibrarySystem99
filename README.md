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

