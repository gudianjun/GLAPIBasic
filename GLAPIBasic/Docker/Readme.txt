docker build -t azure-data-image .
docker run -d -p 8080:8080 --name azure-data-container azure-data-image



docker build -t azure-data-image .
docker run -d -p 8080:8080 --name azure-data-container azure-data-image

编译pg数据库镜像
docker build -t pg-api-image .
docker run -d --name pg-api_container -p 5432:5432 pg-api-image	

容器管理工具
docker run -d -p 5433:80 --name pgAdmin4 -e "PGADMIN_DEFAULT_EMAIL=gudianjun@hotmail.com" -e "PGADMIN_DEFAULT_PASSWORD=111111" dpage/pgadmin4



dotnet ef dbcontext scaffold "Host=192.168.166.195;Port=5432;Username=postgres;Password=pwd123456;Database=api_database;Search Path=api_data" Npgsql.EntityFrameworkCore.PostgreSQL -o ./Models --context-dir ./Data -c PgDbContext --force  --schema api_data
