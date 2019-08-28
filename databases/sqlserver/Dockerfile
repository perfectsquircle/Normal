FROM mcr.microsoft.com/mssql/server:latest

WORKDIR /var/work
COPY . .

ADD https://github.com/Microsoft/sql-server-samples/releases/download/wide-world-importers-v1.0/WideWorldImporters-Full.bak /var/opt/mssql/backup/wwi.bak

CMD /opt/mssql/bin/sqlservr