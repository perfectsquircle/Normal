pack:
	dotnet pack src/Toadstool -o ../../out --include-symbols --include-source -c Release

local:
	dotnet pack src/Toadstool/ -o ../../out --version-suffix=beta-`date +%s`
	nuget push out/*.nupkg -Source Local

.PHONY: test
test:
	dotnet test test/Toadstool.UnitTests/

clean:
	rm -vrf out
	find . -name bin | xargs rm -vrf
	find . -name obj | xargs rm -vrf

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

SQL_SERVER=toadstool_sqlserver_db
BACKUP_DIR=/var/opt/mssql/backup
BACKUP_NAME=wwi.bak

docker-databases:	
	docker-compose up -d 
	./wait-for-it.sh localhost:1433 -t 0
	docker exec -it ${SQL_SERVER} mkdir -p ${BACKUP_DIR}
	curl -L -o ${BACKUP_NAME} 'https://github.com/Microsoft/sql-server-samples/releases/download/wide-world-importers-v1.0/WideWorldImporters-Full.bak'
	docker cp ${BACKUP_NAME} ${SQL_SERVER}:${BACKUP_DIR}/${BACKUP_NAME}
	rm ${BACKUP_NAME}
	docker exec -it ${SQL_SERVER} /opt/mssql-tools/bin/sqlcmd \
		-S localhost -U SA -P 'Toadstool123' \
		-Q 'RESTORE DATABASE WideWorldImporters FROM DISK = "${BACKUP_DIR}/${BACKUP_NAME}" WITH MOVE "WWI_Primary" TO "/var/opt/mssql/data/WideWorldImporters.mdf", MOVE "WWI_UserData" TO "/var/opt/mssql/data/WideWorldImporters_userdata.ndf", MOVE "WWI_Log" TO "/var/opt/mssql/data/WideWorldImporters.ldf", MOVE "WWI_InMemory_Data_1" TO "/var/opt/mssql/data/WideWorldImporters_InMemory_Data_1"'
