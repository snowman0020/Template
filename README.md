The structure of the Web Api Template Project is as follows

1. Template => Controller part is use for connect to frontend (Main Project)

2. Template.Domain => Model part is use mapping data

3. Template.Helper => Utility code part for use project

4. Template.Infrastructure => Database part is use SQL SERVER (Main Database)

5. Template.Infrastructure.MariaDB => Database part is MariaDB

6. Template.Infrastructure.MySQL => Database part is MySQL

7. Template.Infrastructure.Oracle => Database part is Oracle

8. Template.Infrastructure.PostgreSQL => Database part is PostgreSQL

9. Template.Infrastructure.SQLite => Database part is SQLite

10. Template.Infrastructure.MongoDB => Database part is MongoDB

11. Template.Service => Code Crud part is use for Controller call from this

12. Template.UnitTest => Unit Test part

13. Template.Infrastructure.MongoDB => Unit Test crud part is MongoDB

14. Template.Infrastructure.MySQL => Unit Test crud part is MySQL

15. Template.Infrastructure.Oracle => Unit Test crud part is Oracle

16. Template.Infrastructure.PostgreSQL => Unit Test crud part is PostgreSQL

17. Template.Infrastructure.SQLite => Unit Test crud part is SQLite

-----------------------------------------------------------------------------------
1. Create Database On SQL Server Name=> TemplateDB 
2. Goto=> Template.Infrastructure (Set main project before)
3. Then type dotnet run=> enter
-----------------------------------------------------------------------------------

(Have many database can switch for use database interest and change som code for suport it)

-----------------------------------------------------------------------------------
This project use

1. Web Api

2. Entity Framework Core

3. SQL Server

4. Swagger UI 

5. xUnit Test

6. Mapping data from appsettings.json to Class

7. Clean code (Maybe)

8. Filter, Paging, Sort By

9. Loging with Seril Log

10. JWT authentication

11. Custom Exception

12. Password Hash (Argon2)

13. Database code first

14. Protect sensitive information with Microsoft DataProtection

15. MessageQueue using RabbitMQ (Masstransit)

16. Http Client (PostAsync)

17. Caching with Redis with Microsoft StackExchangeRedis

18. Support many database (SQLServer, MariaDBServer, MySQLServer, OracleServer, SQLiteServer, PostgreSQLServer, MongoDBServer)

19. Send email with graph api or smtp (support body type Text and Html code)

20. Line notification using line message api

21. Example crud data for support many database (SQLServer, MariaDBServer, MySQLServer, OracleServer, SQLiteServer, PostgreSQLServer, MongoDBServer) (Unit test) 

22. Background processing  using Hangfire (SQLite database)
-----------------------------------------------------------------------------------
Note:

Protect sensitive information

This is not used. It is in Helper=> DataProtected Folder to protect important information. If this section is enabled (actually, the Template is enabled but not called)

The system will generate key-.xml in dataProtectedInformation Folder. Keep it safe because this file is required to encrypt or decrypt messages.

-----------------------------------------------------------------------------------
MessageQueue using RabbitMQ (Masstransit)

Helper=> MessagePublish Folder and MessageConsume Folder

(You can set it up for use between projects. For example, if there are 2 Web Api, change QueueName and add QueueName in Program.cs to AddConsumer and ConfigureConsumer)

This section will be included in UserService=> DeleteUserAsync When someone uses this function, the message will be published and the message value will be consumed. Then, the Api will be pinged with Http Client (PostAsync) MessageService to insert data to the Message Table.

-----------------------------------------------------------------------------------
Caching with Redis with Microsoft StackExchangeRedis

Helper=> DataCache Folder, this section will be used when LoginAsync.

If LoginAsync is called, the system will first check if there is a value of the person who logged in to the Cache. If there is or it is not expired, the token will be thrown to send the original value. But if not, the system will save the key token.

-----------------------------------------------------------------------------------
Hangfire is used to check if there are any messages that have not been sent to the Line message.

----------------------------------------------------------------------------------
Thank you very much, Enjoy code!
