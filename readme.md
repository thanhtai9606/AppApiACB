Tạo database từ Model + DbContext

dotnet ef migrations add AddAuthDB
dotnet ef database update

1. export PATH="$PATH:$HOME/.dotnet/tools/"
2. dotnet ef dbcontext scaffold   "Server=172.17.0.3;Database=ACB-System;user=root;pwd=admin;" "Pomelo.EntityFrameworkCore.MySql" -o ./Models -f