pack:
	dotnet pack src/Normal.Abstractions -o ./out -c Release
	dotnet pack src/Normal -o ./out -c Release
	dotnet pack src/Normal.AspNetCore -o ./out -c Release

local:
	dotnet pack src/Normal/ -o ./out --version-suffix=beta-`date +%s`
	nuget push out/*.nupkg -Source Local

build:
	dotnet build src/Normal

.PHONY: test
test:
	dotnet test test/Normal.UnitTests

benchmarks:
	dotnet run --project test/Normal.PerformanceTests -c Release

clean:
	rm -vrf out
	find . -name bin | xargs rm -vrf
	find . -name obj | xargs rm -vrf

restore:
	dotnet restore . 

DOCKER_TAG?=normal
DOCKER_NAME?=${DOCKER_TAG}-$(shell date +%s)

docker:
	docker build -q -t ${DOCKER_TAG} .

docker-pack: docker
	docker run --name ${DOCKER_NAME} ${DOCKER_TAG} make
	-docker cp ${DOCKER_NAME}:/app/out out
	-docker rm ${DOCKER_NAME}

docker-compose-down: 
	docker-compose down -v

wait-for=docker-compose run --rm wait-for

docker-compose-up:
	docker-compose up --no-start

postgres: docker-compose-up
	docker-compose start postgres
	$(wait-for) normal_postgres_db:5432 

sqlserver: docker-compose-up
	docker-compose start sqlserver
	$(wait-for) normal_sqlserver_db:1433
	docker exec -it normal_sqlserver_db bash ./import-data.sh

databases: sqlserver postgres