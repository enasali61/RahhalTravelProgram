Rahhal helps users explore Egypt, plan personalized trips, manage budgets, save favorite places, 
and get AI-driven recommendations—all through a secure and scalable backend system.

-Tech Stack
.NET 8 – Backend framework
ASP.NET Core Web API – RESTful APIs
Entity Framework Core – ORM for SQL Server
SQL Server – Relational database
Redis – Temporary basket storage
JWT – Authentication & Authorization
Stripe – Subscription payments
SendGrid – Email service (password reset)
AutoMapper – Object-to-object mapping
Swagger / OpenAPI – API documentation
ngrok – Local tunnel for testing

-Architecture
>The project follows Clean Architecture with four main layers:
>Domain – Entities, Enums, BaseEntity
>Application (Services) – Business logic and use cases
>Infrastructure – Database, Redis, Stripe, repositories
>Presentation – Controllers, DTOs, Swagger
This structure ensures separation of concerns, maintainability, and testability.

-Authentication
JWT-based login, registration, role-based access (Admin/User), and password reset via email.
<img width="1384" height="533" alt="1" src="https://github.com/user-attachments/assets/d6b53b44-e739-4cee-a06f-8b638a922412" />

-AI Integration
Image Recognition – Identify landmarks from photos
Itinerary Generator – AI-powered trip plans based on budget, days, interests, and traveler count
AI Chatbot – Conversational assistant with session memory and Arabic/English support

-Places Management
Full CRUD for tourist places with multilingual names, category filtering, image uploads, and price differentiation based on user type (Egyptian/Foreigner, Student/Adult).

-Trip Planning
Create, edit, and delete trips. Add or remove places, generate daily schedules, and calculate total costs.
<img width="255" height="811" alt="c" src="https://github.com/user-attachments/assets/daa6b56f-cbfa-4e4a-8169-381c0a337316" />
<img width="1262" height="549" alt="E" src="https://github.com/user-attachments/assets/a6ba8a1b-510b-4ba4-9a21-b9c7f64ce93c" />
<img width="1258" height="623" alt="d" src="https://github.com/user-attachments/assets/0481ed97-6d90-4c06-967f-2c7f2599d448" />


-Basket (Redis)
Temporary storage for places before confirming a trip. Redis ensures fast performance and enrichment fetches full place details from SQL.

-Subscriptions (Stripe)
Weekly, Monthly, and Yearly subscription plans with Stripe Checkout and webhook handling.

-Wishlist
Save favorite places, toggle favorites, and retrieve saved list.

-Admin Dashboard
Dashboard statistics (users, places, trips, scans) and full place/user management.
<img width="1661" height="550" alt="4" src="https://github.com/user-attachments/assets/ce50fa9f-e520-4835-bbe1-e44d6c9f0d6a" />
-Backend Demo
Swagger UI: https://localhost:7022/swagger
-Project Screenshots
<img width="900" height="868" alt="Screenshot (335)" src="https://github.com/user-attachments/assets/24efc5aa-3cc3-4bc6-9a02-f67de7718dc1" />
<img width="894" height="879" alt="Screenshot (336)" src="https://github.com/user-attachments/assets/3e56b027-fc75-45ea-a3f0-d6fc6d05a944" />
<img width="907" height="864" alt="Screenshot (337)" src="https://github.com/user-attachments/assets/678def80-bd37-4c5d-8e9f-b21c55915449" />

Educational project – Graduation Project
