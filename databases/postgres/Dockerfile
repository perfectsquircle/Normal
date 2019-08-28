FROM postgres:10

COPY docker-entrypoint-initdb.d/* /docker-entrypoint-initdb.d/

ADD 'https://github.com/Azure/azure-postgresql/raw/master/samples/databases/wide-world-importers/wide_world_importers_pg.dump' /tmp/wide_world_importers_pg.dump

RUN chmod 777 /tmp/wide_world_importers_pg.dump
