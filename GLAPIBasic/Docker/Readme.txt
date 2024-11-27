docker build -t azure-data-image .
docker run -d -p 8080:8080 --name azure-data-container azure-data-image



docker build -t azure-data-image .
docker run -d -p 8080:8080 --name azure-data-container azure-data-image

编译pg数据库镜像
docker build -t pg-api-image .
docker run -d --name pg-api_container -p 5432:5432 pg-api-image	

容器管理工具
docker run -d -p 5433:80 --name pgAdmin4 -e "PGADMIN_DEFAULT_EMAIL=gudianjun@hotmail.com" -e "PGADMIN_DEFAULT_PASSWORD=111111" dpage/pgadmin4



dotnet ef dbcontext scaffold "Host=140.83.84.36;Port=5432;Username=postgres;Password=pwd123456;Database=postgres;Search Path=public" Npgsql.EntityFrameworkCore.PostgreSQL -o ./Models --context-dir ./Data -c PgDbContext --force  --schema public
// 使用配置文件中的连接字符串
dotnet ef dbcontext scaffold Name=DefaultConnection Npgsql.EntityFrameworkCore.PostgreSQL -o ./Models --context-dir ./Data -c PgDbContext --force --schema public


dotnet ef migrations remove
// 初始化数据库复原
dotnet ef migrations add InitialCreate
// 更新数据库
dotnet ef database update
// 删除数据库
dotnet ef database drop

自增字段需要手动添加以下标记
[Key]
[DatabaseGenerated(DatabaseGeneratedOption.Identity)] // 标记为自增字段