pack:
	dotnet pack src/Toadstool -o ../../out --include-symbols --include-source -c Release

local:
	dotnet pack src/Toadstool/ -o ../../out --version-suffix=beta-`date +%s`
	nuget push out/*.nupkg -Source Local

.PHONY: test
test: databases
	dotnet test 

clean: down
	rm -vrf out
	find . -name bin | xargs rm -vrf
	find . -name obj | xargs rm -vrf
	rm -f db/wait-for
	rm -f db/$(POSTGRES_BACKUP_NAME)
	rm -f db/$(SQL_SERVER_BACKUP_NAME)
	rm -f db/postgres
	rm -f db/sqlserver
	rm -f db/up

restore:
	dotnet restore . -v q

DOCKER_TAG?=toadstool
DOCKER_NAME?=${DOCKER_TAG}-$(shell date +%s)

docker:
	docker build -q -t ${DOCKER_TAG} .

docker-pack: docker
	docker run --name ${DOCKER_NAME} ${DOCKER_TAG} make \
	&& docker cp ${DOCKER_NAME}:/app/out out \
	|| docker rm ${DOCKER_NAME}
	docker rm ${DOCKER_NAME}

down: 
	docker-compose down -v
	rm -f db/postgres
	rm -f db/sqlserver
	rm -f db/up

db/up:
	docker-compose up -d
	echo date > db/up

POSTGRES_CONTAINER=toadstool_postgres_db
POSTGRES_BACKUP_DIR=/tmp
POSTGRES_BACKUP_NAME=wide_world_importers_pg.dump

db/wide_world_importers_pg.dump:
	curl -L -o db/${POSTGRES_BACKUP_NAME} 'https://github.com/Azure/azure-postgresql/raw/master/samples/databases/wide-world-importers/wide_world_importers_pg.dump'

db/wait-for:
	curl -L -o db/wait-for https://raw.githubusercontent.com/eficode/wait-for/master/wait-for
	chmod +x db/wait-for

# Stolen from https://github.com/Azure/azure-postgresql/tree/master/samples/databases/wide-world-importers
db/postgres: db/up db/wide_world_importers_pg.dump db/wait-for
	./db/wait-for localhost:5432
	docker exec -it ${POSTGRES_CONTAINER} mkdir -p ${POSTGRES_BACKUP_DIR}
	docker cp db/${POSTGRES_BACKUP_NAME} ${POSTGRES_CONTAINER}:${POSTGRES_BACKUP_DIR}/${POSTGRES_BACKUP_NAME}
	sleep 3
	docker exec -it ${POSTGRES_CONTAINER} pg_restore -U postgres -w -v -Fc -1 -d wide_world_importers_pg ${POSTGRES_BACKUP_DIR}/${POSTGRES_BACKUP_NAME}
	echo $(POSTGRES_CONTAINER) > db/postgres

SQL_SERVER_CONTAINER=toadstool_sqlserver_db
SQL_SERVER_BACKUP_DIR=/var/opt/mssql/backup
SQL_SERVER_BACKUP_NAME=wwi.bak

db/wwi.bak:
	curl -L -o db/${SQL_SERVER_BACKUP_NAME} 'https://github.com/Microsoft/sql-server-samples/releases/download/wide-world-importers-v1.0/WideWorldImporters-Full.bak'

# Stolen from https://docs.microsoft.com/en-us/sql/linux/tutorial-restore-backup-in-sql-server-container?view=sql-server-2017#copy-a-backup-file-into-the-container
db/sqlserver: db/up db/wait-for db/wwi.bak
	./db/wait-for localhost:1433
	docker exec -it ${SQL_SERVER_CONTAINER} mkdir -p ${SQL_SERVER_BACKUP_DIR}
	docker cp db/${SQL_SERVER_BACKUP_NAME} ${SQL_SERVER_CONTAINER}:${SQL_SERVER_BACKUP_DIR}/${SQL_SERVER_BACKUP_NAME}
	docker exec -it ${SQL_SERVER_CONTAINER} /opt/mssql-tools/bin/sqlcmd \
		-S localhost -U SA -P 'Toadstool123' \
		-Q 'RESTORE DATABASE WideWorldImporters FROM DISK = "${SQL_SERVER_BACKUP_DIR}/${SQL_SERVER_BACKUP_NAME}" WITH MOVE "WWI_Primary" TO "/var/opt/mssql/data/WideWorldImporters.mdf", MOVE "WWI_UserData" TO "/var/opt/mssql/data/WideWorldImporters_userdata.ndf", MOVE "WWI_Log" TO "/var/opt/mssql/data/WideWorldImporters.ldf", MOVE "WWI_InMemory_Data_1" TO "/var/opt/mssql/data/WideWorldImporters_InMemory_Data_1"'
	echo $(SQL_SERVER_CONTAINER) > db/sqlserver

databases: db/sqlserver db/postgres