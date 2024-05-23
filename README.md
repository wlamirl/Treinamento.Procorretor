dotnet new fluentblazor -au Individual -int Auto -o Treinamento.Procorretor

links: https://youtu.be/tNzSuwV62Lw?si=CEhPN8fuAlyY_3oU
Links: https://youtu.be/tNzSuwV62Lw?si=9IhiQOxgNGsloJ4P

dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore -v 8.05
dotnet remove package <PACKAGE_NAME>
dotnet list package

dotnet tool update --global dotnet-ef

dotnet ef migrations add Init -o Data/Migrations/

dotnet ef database update