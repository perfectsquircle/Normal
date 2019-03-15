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

docker-test: docker	
	docker run --rm ${DOCKER_TAG} make test
