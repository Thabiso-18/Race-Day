
---

##  Getting Started

### Prerequisites
- SQL Server Management Studio (SSMS)
- GitHub account
- (Optional) Node.js/React for frontend development

### Database Setup
1. Open SQL Server Management Studio
2. Run the script from `/docs/schema.sql` on a clean database
3. Verify all tables are created with sample data

### CI/CD Pipeline
This repository includes a GitHub Actions workflow that validates:
- `/docs` folder exists
- ERD diagram is present (PNG or PDF)
- Endpoint plan document exists
- SQL script is present

![CI/CD Build Status](docs/build-status.png)

---

##  Planning Documentation

All planning documents are stored in the `/docs` folder:

1. **Entity Relationship Diagram** (`erd.png`)
   - Full database model with 6 entities
   - Primary keys, foreign keys, and cardinalities
   - All attributes and constraints

2. **API Endpoint Plan** (`endpoints.md`)
   - Complete endpoint specifications
   - HTTP methods, routes, descriptions
   - Role requirements and request/response details

3. **Database Schema** (`schema.sql`)
   - CREATE TABLE statements for all entities
   - Primary keys, foreign keys, constraints
   - Sample data with 2 organisers, 2 participants, 3 events

---

## 🎥 Video Walkthrough

Watch the complete planning walkthrough on YouTube:

**[RaceDay System Planning Walkthrough](https://youtu.be/your-video-link)**

*The video covers:*
- System overview and role definitions
- ERD decisions and database design choices
- API endpoint plan rationale
- Live SQL script execution in SSMS
- GitHub repository structure and CI/CD validation

---

##  Sample Data

The SQL script seeds the database with realistic sample data:

### Users
- **2 Organisers**: Alice Organiser, Bob Organiser
- **2 Participants**: Charlie Runner, Diana Athlete

### Events
1. **Spring 10K Run** (Open) - Central Park, May 15, 2026
2. **Summer Marathon** (Open) - Lakeside Stadium, July 20, 2026
3. **Autumn Trail Race** (Draft) - Central Park, October 10, 2026

### Categories
- Event 1: 10K Men, 10K Women, 10K Masters
- Event 2: Full Marathon, Half Marathon
- Event 3: Trail Run (to be defined)

### Enrolments & Results
- Charlie enrolled in 10K Men (confirmed, paid) - Result: 5th place, 3600 seconds
- Diana enrolled in 10K Women (pending, unpaid) - No result yet
- Diana enrolled in Full Marathon (confirmed, paid) - Result: 3rd place, 7200 seconds

---

##  Technology Stack

| Component | Technology |
|-----------|------------|
| Database | Microsoft SQL Server |
| API | RESTful (to be implemented in Part 2) |
| Authentication | JWT (to be implemented in Part 2) |
| Version Control | Git / GitHub |
| CI/CD | GitHub Actions |
| Documentation | Markdown, PNG/PDF |

---

##  Learning Outcomes

This project demonstrates:

- ✅ Designing a relational database using ERD
- ✅ Planning a RESTful API with structured endpoint specifications
- ✅ Writing SQL scripts to create and populate a database
- ✅ Role-based system design and access control
- ✅ Version control with meaningful commits
- ✅ CI/CD pipeline configuration

---

## 📧 Contact

For any questions regarding this project, please refer to the assignment brief or contact your course instructor.

---

## 📄 License

© The Independent Institute of Education (Pty) Ltd 2026

---

https://youtu.be/d9Wws4GR9PE?si=mN6iqXxI06lcwpRy