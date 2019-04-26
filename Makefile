pack:
	dotnet pack src/Toadstool -o ../../out --include-symbols --include-source -c Release

local:
	dotnet pack src/Toadstool/ -o ../../out --version-suffix=beta-`date +%s`
	nuget push out/*.nupkg -Source Local

.PHONY: test
test:
	dotnet test 

clean: 
	rm -vrf out
	find . -name bin | xargs rm -vrf
	find . -name obj | xargs rm -vrf
	rm wait-for
	rm $(POSTGRES_BACKUP_NAME)
	rm $(SQL_SERVER_BACKUP_NAME)

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

compose-down: 
	docker-compose down -v

compose-up:
	# docker-compose down -v
	docker-compose up -d

POSTGRES_CONTAINER=toadstool_postgres_db
POSTGRES_BACKUP_DIR=/tmp
POSTGRES_BACKUP_NAME=wide_world_importers_pg.dump

wide_world_importers_pg.dump:
	curl -L -o ${POSTGRES_BACKUP_NAME} 'https://github.com/Azure/azure-postgresql/raw/master/samples/databases/wide-world-importers/wide_world_importers_pg.dump'

wait-for:
	curl -L -o wait-for https://raw.githubusercontent.com/eficode/wait-for/master/wait-for
	chmod +x wait-for

# Stolen from https://github.com/Azure/azure-postgresql/tree/master/samples/databases/wide-world-importers
docker-database-postgres: compose-up wide_world_importers_pg.dump wait-for
	./wait-for localhost:5432
	docker exec -it ${POSTGRES_CONTAINER} mkdir -p ${POSTGRES_BACKUP_DIR}
	docker cp ${POSTGRES_BACKUP_NAME} ${POSTGRES_CONTAINER}:${POSTGRES_BACKUP_DIR}/${POSTGRES_BACKUP_NAME}
	sleep 3
	docker exec -it ${POSTGRES_CONTAINER} pg_restore -U postgres -w -v -Fc -1 -d wide_world_importers_pg ${POSTGRES_BACKUP_DIR}/${POSTGRES_BACKUP_NAME}

SQL_SERVER_CONTAINER=toadstool_sqlserver_db
SQL_SERVER_BACKUP_DIR=/var/opt/mssql/backup
SQL_SERVER_BACKUP_NAME=wwi.bak

wwi.bak:
	curl -L -o ${SQL_SERVER_BACKUP_NAME} 'https://github.com/Microsoft/sql-server-samples/releases/download/wide-world-importers-v1.0/WideWorldImporters-Full.bak'

# Stolen from https://docs.microsoft.com/en-us/sql/linux/tutorial-restore-backup-in-sql-server-container?view=sql-server-2017#copy-a-backup-file-into-the-container
docker-database-sqlserver: compose-up wait-for wwi.bak
	./wait-for localhost:1433
	docker exec -it ${SQL_SERVER_CONTAINER} mkdir -p ${SQL_SERVER_BACKUP_DIR}
	docker cp ${SQL_SERVER_BACKUP_NAME} ${SQL_SERVER_CONTAINER}:${SQL_SERVER_BACKUP_DIR}/${SQL_SERVER_BACKUP_NAME}
	docker exec -it ${SQL_SERVER_CONTAINER} /opt/mssql-tools/bin/sqlcmd \
		-S localhost -U SA -P 'Toadstool123' \
		-Q 'RESTORE DATABASE WideWorldImporters FROM DISK = "${SQL_SERVER_BACKUP_DIR}/${SQL_SERVER_BACKUP_NAME}" WITH MOVE "WWI_Primary" TO "/var/opt/mssql/data/WideWorldImporters.mdf", MOVE "WWI_UserData" TO "/var/opt/mssql/data/WideWorldImporters_userdata.ndf", MOVE "WWI_Log" TO "/var/opt/mssql/data/WideWorldImporters.ldf", MOVE "WWI_InMemory_Data_1" TO "/var/opt/mssql/data/WideWorldImporters_InMemory_Data_1"'

docker-databases: docker-database-sqlserver docker-database-postgres