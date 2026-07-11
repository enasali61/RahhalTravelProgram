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
<img width="1384" height="637" alt="1" src="https://github.com/user-attachments/assets/e07e0321-c327-489d-8952-aad9652dc2c5" />

-AI Integration
Image Recognition – Identify landmarks from photos
Itinerary Generator – AI-powered trip plans based on budget, days, interests, and traveler count
AI Chatbot – Conversational assistant with session memory and Arabic/English support

-Places Management
Full CRUD for tourist places with multilingual names, category filtering, image uploads, and price differentiation based on user type (Egyptian/Foreigner, Student/Adult).

-Trip Planning
Create, edit, and delete trips. Add or remove places, generate daily schedules, and calculate total costs.
<img width="1382" height="771" alt="2" src="https://github.com/user-attachments/assets/8842b865-3f30-445c-9c5d-d320722281b2" />
<img width="1723" height="687" alt="3" src="https://github.com/user-attachments/assets/a5c2f20e-0083-4137-935e-f58567cef257" />

-Basket (Redis)
Temporary storage for places before confirming a trip. Redis ensures fast performance and enrichment fetches full place details from SQL.

-Subscriptions (Stripe)
Weekly, Monthly, and Yearly subscription plans with Stripe Checkout and webhook handling.

-Wishlist
Save favorite places, toggle favorites, and retrieve saved list.

-Admin Dashboard
Dashboard statistics (users, places, trips, scans) and full place/user management.
<img width="1661" height="603" alt="4" src="https://github.com/user-attachments/assets/dcf13d98-9dcd-4bce-bf93-e7d706a6b935" />

-Backend Demo
Swagger UI: https://localhost:7022/swagger
<img width="907" height="864" alt="Screenshot (337)" src="https://github.com/user-attachments/assets/678def80-bd37-4c5d-8e9f-b21c55915449" />
<img width="894" height="879" alt="Screenshot (336)" src="https://github.com/user-attachments/assets/3e56b027-fc75-45ea-a3f0-d6fc6d05a944" />
<img width="900" height="868" alt="Screenshot (335)" src="https://github.com/user-attachments/assets/24efc5aa-3cc3-4bc6-9a02-f67de7718dc1" />
