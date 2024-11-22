# 使用PostgreSQL 16.1官方镜像
FROM postgres:17.1

# 设置环境变量
ENV POSTGRES_DB=api_database
ENV POSTGRES_USER=postgres
ENV POSTGRES_PASSWORD=pwd123456

# 拷贝备份文件到容器中
COPY initdb.sh /docker-entrypoint-initdb.d/
EXPOSE 5432